using AskalePortal.DAL.Education;
using AskalePortal.Data.Models;
using AskalePortal.Data.RequestModels.Education;
using AskalePortal.Data.RequestParams;
using AskalePortal.Data.ResponseModels;
using AskalePortal.Data.ResponseModels.Education;
using AskalePortal.Data.ResponseParams;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.Reflection;

namespace AskalePortal.BLL.Education;

public abstract class EducationCrudService<T>(IEducationCrudRepository<T> repository) : IEducationCrudService<T>
    where T : class
{
    protected readonly IEducationCrudRepository<T> Repository = repository;

    public virtual async Task<T> SaveAsync(T entity, int userId, CancellationToken ct)
    {
        var id = (int?)typeof(T).GetProperty("Id")?.GetValue(entity) ?? 0;
        if (id == 0)
        {
            Set(entity, "createdDate", DateTime.Now);
            Set(entity, "createdUserId", userId);
            Set(entity, "enabled", true);
        }
        else
        {
            Set(entity, "updatedDate", DateTime.Now);
            Set(entity, "updatedUserId", userId);
        }

        return await Repository.SaveAsync(entity, ct);
    }

    public Task<int> DeleteAsync(int id, CancellationToken ct) => Repository.SoftDeleteAsync(id, ct);
    public Task<T?> GetByIdAsync(int id, CancellationToken ct) => Repository.GetByIdAsync(id, ct);
    public Task<List<T>> GetAllAsync(CancellationToken ct) => Repository.GetAllEnabledAsync(ct);

    public async Task<PageReturn<T>> FilterPageableAsync(FilterPageParam<object> request, CancellationToken ct)
    {
        var rows = await Repository.GetAllEnabledAsync(ct);
        var page = Math.Max(request.page ?? 0, 0);
        var size = Math.Clamp(request.size ?? 20, 1, 500);
        var content = rows.Skip(page * size).Take(size).ToList();
        return CreatePage(content, rows.Count, page, size);
    }

    public Task<List<T>> GetAllFilterAsync(object? request, CancellationToken ct) => Repository.GetAllEnabledAsync(ct);

    protected static PageReturn<TDto> CreatePage<TDto>(List<TDto> content, int total, int page, int size) => new()
    {
        content = content,
        totalElements = total,
        totalPages = (int)Math.Ceiling(total / (double)size),
        number = page,
        size = size,
        numberOfElements = content.Count,
        first = page == 0,
        last = (page + 1) * size >= total,
        empty = content.Count == 0,
        pageable = new Pageable { pageNumber = page, pageSize = size, offset = page * size, paged = true, unpaged = false },
        sort = new Sort { sorted = false, unsorted = true, empty = true }
    };

    protected static async Task<PageReturn<TDto>> PageAsync<TDto>(IQueryable<TDto> query, int page, int size, CancellationToken ct)
    {
        page = Math.Max(page, 0);
        size = Math.Clamp(size, 1, 500);
        var total = await query.CountAsync(ct);
        var content = await query.Skip(page * size).Take(size).ToListAsync(ct);
        return CreatePage(content, total, page, size);
    }

    private static void Set(object entity, string propertyName, object value)
    {
        var property = entity.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
        if (property is null || !property.CanWrite) return;
        var target = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
        if (target == value.GetType() || target.IsAssignableFrom(value.GetType())) property.SetValue(entity, value);
    }
}

public sealed class EducationService(
    IEducationRepository repository,
    IEducationVideoService videos,
    DBDataContext db) : EducationCrudService<EgitimTable>(repository), IEducationService
{
    public async Task<PageReturn<EducationDto>> FilterAsync(FilterPageParam<EducationSearchRequest> request, CancellationToken ct)
    {
        var filter = request.liste ?? new EducationSearchRequest();
        var userId = filter.userId ?? request.userId;
        var user = userId.HasValue
            ? await db.AdminUser.AsNoTracking().FirstOrDefaultAsync(x => x.Id == userId.Value, ct)
            : null;
        var result = await PageAsync(repository.Search(user?.roleId == 1, userId, filter.sectionId, filter.courseName), request.page ?? 0, request.size ?? 20, ct);
        foreach (var row in result.content ?? []) row.listEducationVideoDto = await videos.ListAsync(row.id, ct);
        return result;
    }

    public Task<List<EgitimTable>> ListBySectionAsync(int id, CancellationToken ct) => repository.ListBySectionAsync(id, ct);
}

public sealed class EducationSectionService(IEducationSectionRepository repository)
    : EducationCrudService<EgitimBolumTable>(repository), IEducationSectionService
{
    public Task<PageReturn<EgitimBolumTable>> FilterAsync(FilterPageParam<EducationSectionSearchRequest> request, CancellationToken ct) =>
        PageAsync(repository.Search(request.liste?.egitimBolumu), request.page ?? 0, request.size ?? 20, ct);
}

public sealed class EgitimSorulariService(IEgitimSorulariRepository repository)
    : EducationCrudService<EgitimSorulariTable>(repository), IEgitimSorulariService
{
    public Task<List<EgitimSorulariTable>> ListByVideoAsync(int id, CancellationToken ct) => repository.ListByVideoAsync(id, ct);
    public Task<List<EgitimSorulariDto>> ListDtosAsync(int id, CancellationToken ct) => repository.ListDtosAsync(id, ct);
}

public sealed class EducationVideoService(
    IEducationVideoRepository repository,
    IEgitimSorulariService questions,
    IConfiguration configuration,
    IWebHostEnvironment environment,
    DBDataContext db) : EducationCrudService<EgitimVideoTable>(repository), IEducationVideoService
{
    private const int EducationModuleId = 56;

    public async Task<List<EducationVideoDto>> ListAsync(int id, CancellationToken ct)
    {
        var rows = await repository.ListDtosAsync(id, ct);
        foreach (var row in rows) row.listEducationQuestionDto = await questions.ListDtosAsync(row.id, ct);
        return rows;
    }

    private string Root
    {
        get
        {
            var mode = Environment.GetEnvironmentVariable("ASKALE_ENVIRONMENT")?.ToLowerInvariant()
                       ?? (environment.IsProduction() ? "server" : environment.IsDevelopment() ? "local" : "test");
            var basePath = configuration[$"FilePath:{mode}"] ?? configuration[$"FilePath:{mode.ToLowerInvariant()}"];
            if (string.IsNullOrWhiteSpace(basePath)) throw new InvalidOperationException($"FilePath:{mode} configuration is missing.");
            return Path.Combine(basePath, "documents", "egitimler");
        }
    }

    public string GetPath(string file) => Path.Combine(Root, Path.GetFileName(file));

    public Task<string> UploadVideoAsync(IReadOnlyList<IFormFile> files, int targetId, int userId, CancellationToken ct) =>
        UploadAsync(files, targetId, userId, isImage: false, ct);

    public Task<string> UploadImageAsync(IReadOnlyList<IFormFile> files, int targetId, int userId, CancellationToken ct) =>
        UploadAsync(files, targetId, userId, isImage: true, ct);

    private async Task<string> UploadAsync(IReadOnlyList<IFormFile> files, int targetId, int userId, bool isImage, CancellationToken ct)
    {
        if (targetId <= 0) throw new ArgumentOutOfRangeException(nameof(targetId));
        if (files.Count == 0) throw new InvalidOperationException("At least one file is required.");
        Directory.CreateDirectory(Root);
        var entity = await repository.GetByIdAsync(targetId, ct) ?? throw new KeyNotFoundException($"Education video {targetId} was not found.");
        string? lastFile = null;

        foreach (var file in files.Where(x => x.Length > 0))
        {
            var extension = Path.GetExtension(file.FileName);
            var name = $"{Path.GetFileNameWithoutExtension(file.FileName)}-{DateTime.Now:yyyyMMddHHmmssfff}{extension}";
            await using (var stream = File.Create(GetPath(name))) await file.CopyToAsync(stream, ct);
            lastFile = name;

            db.AttachedFile.Add(new AttachedFile
            {
                moduleId = EducationModuleId,
                targetId = targetId,
                createdUserId = userId,
                title = name,
                filePath = name,
                visitorCount = 0,
                createdDate = DateTime.Now,
                enabled = true
            });

            if (isImage) entity.imagePath = name; else entity.videoPath = name;
        }

        await repository.SaveAsync(entity, ct);
        await db.SaveChangesAsync(ct);
        return lastFile ?? string.Empty;
    }

    public async Task<ResponseByteArray> DownloadAsync(string file, CancellationToken ct)
    {
        var safeName = Path.GetFileName(file);
        var bytes = await File.ReadAllBytesAsync(GetPath(safeName), ct);
        return new ResponseByteArray { fileName = safeName, name = safeName, file = bytes };
    }

    public async Task<ResponseByteArray> DownloadPictureAsync(int videoId, CancellationToken ct)
    {
        var video = await repository.GetByIdAsync(videoId, ct) ?? throw new KeyNotFoundException();
        if (string.IsNullOrWhiteSpace(video.imagePath)) return new ResponseByteArray { fileName = string.Empty, name = string.Empty, file = [] };
        return await DownloadAsync(video.imagePath, ct);
    }
}

public sealed class EducationVideoDurationService(IEducationVideoDurationRepository repository)
    : EducationCrudService<EgitimVideoIzlemeTable>(repository), IEducationVideoDurationService
{
    public override async Task<EgitimVideoIzlemeTable> SaveAsync(EgitimVideoIzlemeTable entity, int userId, CancellationToken ct)
    {
        if (entity.Id == 0)
        {
            entity.userId = userId;
            entity.izlemeTarihi = DateTime.Now;
            await repository.DisableExistingAsync(entity.videoId, userId, ct);
        }
        return await base.SaveAsync(entity, userId, ct);
    }

    public Task<List<EgitimVideoIzlemeTable>> ListAsync(int id, int? userId, CancellationToken ct) => repository.ListAsync(id, userId, ct);
}

public sealed class EgitimSoruCevapService(IEgitimSoruCevapRepository repository)
    : EducationCrudService<EgitimSoruCevap>(repository), IEgitimSoruCevapService
{
    public override async Task<EgitimSoruCevap> SaveAsync(EgitimSoruCevap entity, int userId, CancellationToken ct)
    {
        if (entity.Id == 0)
        {
            entity.userId = userId;
            await repository.DisableExistingAsync(entity.soruId, userId, ct);
        }
        return await base.SaveAsync(entity, userId, ct);
    }

    public Task<List<EgitimSoruCevap>> ListAsync(int id, int? userId, CancellationToken ct) => repository.ListAsync(id, userId, ct);
}

public sealed class EducationQuestionService(IEducationQuestionRepository repository)
    : EducationCrudService<EducationQuestionsTable>(repository), IEducationQuestionService
{
    public Task<List<EducationQuestionsTable>> ListBySectionAsync(int id, CancellationToken ct) => repository.ListBySectionAsync(id, ct);
}

public sealed class EducationQuestionSectionService(
    IEducationQuestionSectionRepository repository,
    IEducationQuestionRepository questions,
    DBDataContext db) : EducationCrudService<EducationQuestionSectionTable>(repository), IEducationQuestionSectionService
{
    public async Task<PageReturn<EducationQuestionSectionDto>> FilterAsync(FilterPageParam<EducationQuestionSectionSearchRequest> request, CancellationToken ct)
    {
        var filter = request.liste ?? new EducationQuestionSectionSearchRequest();
        var userId = filter.userId ?? request.userId;
        var user = userId.HasValue ? await db.AdminUser.AsNoTracking().FirstOrDefaultAsync(x => x.Id == userId.Value, ct) : null;
        var page = await PageAsync(repository.Search(user?.roleId == 1, userId, filter.sectionId, filter.courseName), request.page ?? 0, request.size ?? 20, ct);
        foreach (var row in page.content ?? []) row.listEducationQuestionDto = await questions.ListDtosAsync(row.id, ct);
        return page;
    }
}

public sealed class EducationQuestionAnswerService(IEducationQuestionAnswerRepository repository)
    : EducationCrudService<EducationQuestionAnswerTable>(repository), IEducationQuestionAnswerService
{
    public override async Task<EducationQuestionAnswerTable> SaveAsync(EducationQuestionAnswerTable entity, int userId, CancellationToken ct)
    {
        var effectiveUserId = entity.userId ?? userId;
        var existing = await repository.FindCurrentAsync(effectiveUserId, entity.soruId ?? 0, ct);
        if (existing is not null)
        {
            existing.cevap = entity.cevap;
            return await base.SaveAsync(existing, userId, ct);
        }
        entity.userId = effectiveUserId;
        return await base.SaveAsync(entity, userId, ct);
    }

    public Task<List<EducationQuestionAnswerTable>> ListAsync(int id, int? userId, CancellationToken ct) => repository.ListAsync(id, userId, ct);
}
