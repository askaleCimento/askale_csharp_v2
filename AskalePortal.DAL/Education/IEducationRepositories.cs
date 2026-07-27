using AskalePortal.Data.Models;
using AskalePortal.Data.ResponseModels.Education;
using Microsoft.EntityFrameworkCore;

namespace AskalePortal.DAL.Education;

public interface IEducationCrudRepository<T> where T : class
{
    Task<T?> GetByIdAsync(int id, CancellationToken ct);
    Task<List<T>> GetAllEnabledAsync(CancellationToken ct);
    Task<T> SaveAsync(T entity, CancellationToken ct);
    Task<int> SoftDeleteAsync(int id, CancellationToken ct);
}

public abstract class EducationCrudRepository<T> : IEducationCrudRepository<T> where T : class
{
    protected readonly DBDataContext Db;
    protected EducationCrudRepository(DBDataContext db) => Db = db;
    public async Task<T?> GetByIdAsync(int id, CancellationToken ct) => await Db.Set<T>().FindAsync([id], ct);
    public async Task<List<T>> GetAllEnabledAsync(CancellationToken ct) => await Db.Set<T>().Where(x => EF.Property<bool>(x, "enabled")).AsNoTracking().ToListAsync(ct);
    public async Task<T> SaveAsync(T entity, CancellationToken ct)
    {
        var id = (int?)typeof(T).GetProperty("Id")?.GetValue(entity) ?? 0;
        if (id == 0) Db.Set<T>().Add(entity); else Db.Set<T>().Update(entity);
        await Db.SaveChangesAsync(ct); return entity;
    }
    public async Task<int> SoftDeleteAsync(int id, CancellationToken ct)
    {
        var entity = await Db.Set<T>().FindAsync([id], ct); if (entity is null) return 0;
        typeof(T).GetProperty("enabled")?.SetValue(entity, false);
        typeof(T).GetProperty("updatedDate")?.SetValue(entity, DateTime.Now);
        await Db.SaveChangesAsync(ct); return 1;
    }
}

public interface IEducationRepository : IEducationCrudRepository<EgitimTable>
{
    IQueryable<EducationDto> Search(bool admin, int? userId, int? sectionId, string? courseName);
    Task<List<EgitimTable>> ListBySectionAsync(int sectionId, CancellationToken ct);
}
public interface IEducationSectionRepository : IEducationCrudRepository<EgitimBolumTable> { IQueryable<EgitimBolumTable> Search(string? name); }
public interface IEducationVideoRepository : IEducationCrudRepository<EgitimVideoTable> { Task<List<EducationVideoDto>> ListDtosAsync(int educationId, CancellationToken ct); }
public interface IEgitimSorulariRepository : IEducationCrudRepository<EgitimSorulariTable> { Task<List<EgitimSorulariTable>> ListByVideoAsync(int videoId, CancellationToken ct); Task<List<EgitimSorulariDto>> ListDtosAsync(int videoId, CancellationToken ct); }
public interface IEducationVideoDurationRepository : IEducationCrudRepository<EgitimVideoIzlemeTable> { Task<List<EgitimVideoIzlemeTable>> ListAsync(int videoId, int? userId, CancellationToken ct); Task DisableExistingAsync(int videoId, int userId, CancellationToken ct); }
public interface IEgitimSoruCevapRepository : IEducationCrudRepository<EgitimSoruCevap> { Task<List<EgitimSoruCevap>> ListAsync(int videoId, int? userId, CancellationToken ct); Task DisableExistingAsync(int questionId, int userId, CancellationToken ct); }
public interface IEducationQuestionRepository : IEducationCrudRepository<EducationQuestionsTable> { Task<List<EducationQuestionsTable>> ListBySectionAsync(int sectionId, CancellationToken ct); Task<List<EducationQuestionDto>> ListDtosAsync(int sectionId, CancellationToken ct); }
public interface IEducationQuestionSectionRepository : IEducationCrudRepository<EducationQuestionSectionTable> { IQueryable<EducationQuestionSectionDto> Search(bool admin, int? userId, int? sectionId, string? questionName); }
public interface IEducationQuestionAnswerRepository : IEducationCrudRepository<EducationQuestionAnswerTable> { Task<List<EducationQuestionAnswerTable>> ListAsync(int sectionId, int? userId, CancellationToken ct); Task<EducationQuestionAnswerTable?> FindCurrentAsync(int userId, int questionId, CancellationToken ct); }
