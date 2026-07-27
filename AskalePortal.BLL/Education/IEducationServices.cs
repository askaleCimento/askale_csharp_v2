using AskalePortal.Data.Models;
using AskalePortal.Data.RequestModels.Education;
using AskalePortal.Data.RequestParams;
using AskalePortal.Data.ResponseModels.Education;
using AskalePortal.Data.ResponseParams;
using AskalePortal.Data.ResponseModels;
using Microsoft.AspNetCore.Http;

namespace AskalePortal.BLL.Education;

public interface IEducationCrudService<T> where T : class
{
    Task<T> SaveAsync(T entity, int userId, CancellationToken ct);
    Task<int> DeleteAsync(int id, CancellationToken ct);
    Task<T?> GetByIdAsync(int id, CancellationToken ct);
    Task<List<T>> GetAllAsync(CancellationToken ct);
    Task<PageReturn<T>> FilterPageableAsync(FilterPageParam<object> request, CancellationToken ct);
    Task<List<T>> GetAllFilterAsync(object? request, CancellationToken ct);
}

public interface IEducationService : IEducationCrudService<EgitimTable>
{
    Task<PageReturn<EducationDto>> FilterAsync(FilterPageParam<EducationSearchRequest> request, CancellationToken ct);
    Task<List<EgitimTable>> ListBySectionAsync(int id, CancellationToken ct);
}
public interface IEducationSectionService : IEducationCrudService<EgitimBolumTable>
{
    Task<PageReturn<EgitimBolumTable>> FilterAsync(FilterPageParam<EducationSectionSearchRequest> request, CancellationToken ct);
}
public interface IEducationVideoService : IEducationCrudService<EgitimVideoTable>
{
    Task<List<EducationVideoDto>> ListAsync(int educationId, CancellationToken ct);
    Task<string> UploadVideoAsync(IReadOnlyList<IFormFile> files, int targetId, int userId, CancellationToken ct);
    Task<string> UploadImageAsync(IReadOnlyList<IFormFile> files, int targetId, int userId, CancellationToken ct);
    Task<ResponseByteArray> DownloadAsync(string file, CancellationToken ct);
    Task<ResponseByteArray> DownloadPictureAsync(int videoId, CancellationToken ct);
    string GetPath(string file);
}
public interface IEgitimSorulariService : IEducationCrudService<EgitimSorulariTable>
{
    Task<List<EgitimSorulariTable>> ListByVideoAsync(int id, CancellationToken ct);
    Task<List<EgitimSorulariDto>> ListDtosAsync(int id, CancellationToken ct);
}
public interface IEducationVideoDurationService : IEducationCrudService<EgitimVideoIzlemeTable>
{
    Task<List<EgitimVideoIzlemeTable>> ListAsync(int videoId, int? userId, CancellationToken ct);
}
public interface IEgitimSoruCevapService : IEducationCrudService<EgitimSoruCevap>
{
    Task<List<EgitimSoruCevap>> ListAsync(int videoId, int? userId, CancellationToken ct);
}
public interface IEducationQuestionService : IEducationCrudService<EducationQuestionsTable>
{
    Task<List<EducationQuestionsTable>> ListBySectionAsync(int id, CancellationToken ct);
}
public interface IEducationQuestionSectionService : IEducationCrudService<EducationQuestionSectionTable>
{
    Task<PageReturn<EducationQuestionSectionDto>> FilterAsync(FilterPageParam<EducationQuestionSectionSearchRequest> request, CancellationToken ct);
}
public interface IEducationQuestionAnswerService : IEducationCrudService<EducationQuestionAnswerTable>
{
    Task<List<EducationQuestionAnswerTable>> ListAsync(int sectionId, int? userId, CancellationToken ct);
}
