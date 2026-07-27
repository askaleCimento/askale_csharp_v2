using AskalePortal.Data.Models;
using AskalePortal.Data.ResponseModels.Education;
using Microsoft.EntityFrameworkCore;

namespace AskalePortal.DAL.Education;

public sealed class EducationRepository(DBDataContext db)
    : EducationCrudRepository<EgitimTable>(db), IEducationRepository
{
    public IQueryable<EducationDto> Search(
        bool admin,
        int? userId,
        int? sectionId,
        string? courseName)
    {
        // Eğitim görünürlüğü oluşturan kullanıcıya göre sınırlandırılmaz.
        // Bölüm veya oluşturan kullanıcı kaydı bulunmayan eski eğitimler de listede kalır.
        return
            from education in Db.EgitimTable.AsNoTracking()
            join sectionRow in Db.EgitimBolumTable.AsNoTracking()
                on education.egitimBolumId equals sectionRow.Id into sectionRows
            from section in sectionRows.DefaultIfEmpty()
            join userRow in Db.AdminUser.AsNoTracking()
                on education.createdUserId equals userRow.Id into userRows
            from creator in userRows.DefaultIfEmpty()
            where education.enabled
                  && (!sectionId.HasValue || education.egitimBolumId == sectionId.Value)
                  && (string.IsNullOrWhiteSpace(courseName)
                      || (education.courseName != null && education.courseName.Contains(courseName)))
            orderby education.Id descending
            select new EducationDto
            {
                id = education.Id,
                sectionName = section != null ? section.egitimBolumu : null,
                courseName = education.courseName,
                startDate = education.startDate,
                endDate = education.endDate,
                olusturmaTarihi = education.createdDate,
                olusturanKisi = creator != null ? creator.name : null
            };
    }

    public Task<List<EgitimTable>> ListBySectionAsync(
        int sectionId,
        CancellationToken cancellationToken)
    {
        return Db.EgitimTable
            .AsNoTracking()
            .Where(x => x.enabled && x.egitimBolumId == sectionId)
            .OrderByDescending(x => x.Id)
            .ToListAsync(cancellationToken);
    }
}

public sealed class EducationSectionRepository(DBDataContext db)
    : EducationCrudRepository<EgitimBolumTable>(db), IEducationSectionRepository
{
    public IQueryable<EgitimBolumTable> Search(string? name)
    {
        return Db.EgitimBolumTable
            .AsNoTracking()
            .Where(x =>
                x.enabled
                && (string.IsNullOrWhiteSpace(name)
                    || (x.egitimBolumu != null && x.egitimBolumu.Contains(name))))
            .OrderByDescending(x => x.Id);
    }
}

public sealed class EducationVideoRepository(DBDataContext db)
    : EducationCrudRepository<EgitimVideoTable>(db), IEducationVideoRepository
{
    public Task<List<EducationVideoDto>> ListDtosAsync(
        int educationId,
        CancellationToken cancellationToken)
    {
        return (
            from video in Db.EgitimVideoTable.AsNoTracking()
            join userRow in Db.AdminUser.AsNoTracking()
                on video.createdUserId equals userRow.Id into userRows
            from creator in userRows.DefaultIfEmpty()
            where video.enabled && video.courseId == educationId
            orderby video.videoOrder, video.Id
            select new EducationVideoDto
            {
                id = video.Id,
                videoName = video.videoName,
                videoOrder = video.videoOrder,
                olusturmaTarihi = video.createdDate,
                olusturanKisi = creator != null ? creator.name : null,
                fileName = video.videoPath
            })
            .ToListAsync(cancellationToken);
    }
}

public sealed class EgitimSorulariRepository(DBDataContext db)
    : EducationCrudRepository<EgitimSorulariTable>(db), IEgitimSorulariRepository
{
    public Task<List<EgitimSorulariTable>> ListByVideoAsync(
        int videoId,
        CancellationToken cancellationToken)
    {
        return Db.EgitimSorulariTable
            .AsNoTracking()
            .Where(x => x.enabled && x.videoId == videoId)
            .OrderBy(x => x.Id)
            .ToListAsync(cancellationToken);
    }

    public Task<List<EgitimSorulariDto>> ListDtosAsync(
        int videoId,
        CancellationToken cancellationToken)
    {
        return (
            from question in Db.EgitimSorulariTable.AsNoTracking()
            join userRow in Db.AdminUser.AsNoTracking()
                on question.createdUserId equals userRow.Id into userRows
            from creator in userRows.DefaultIfEmpty()
            where question.enabled && question.videoId == videoId
            orderby question.Id
            select new EgitimSorulariDto
            {
                id = question.Id,
                soru = question.soru,
                sikA = question.sikA,
                sikB = question.sikB,
                sikC = question.sikC,
                sikD = question.sikD,
                sikE = question.sikE,
                dogruCevap = question.dogruCevap,
                showVideoTime = question.showVideoTime,
                olusturmaTarihi = question.createdDate,
                olusturanKisi = creator != null ? creator.name : null
            })
            .ToListAsync(cancellationToken);
    }
}

public sealed class EducationVideoDurationRepository(DBDataContext db)
    : EducationCrudRepository<EgitimVideoIzlemeTable>(db), IEducationVideoDurationRepository
{
    public Task<List<EgitimVideoIzlemeTable>> ListAsync(
        int videoId,
        int? userId,
        CancellationToken cancellationToken)
    {
        return Db.EgitimVideoIzlemeTable
            .AsNoTracking()
            .Where(x =>
                x.enabled
                && x.videoId == videoId
                && (!userId.HasValue || x.userId == userId.Value))
            .ToListAsync(cancellationToken);
    }

    public async Task DisableExistingAsync(
        int videoId,
        int userId,
        CancellationToken cancellationToken)
    {
        var rows = await Db.EgitimVideoIzlemeTable
            .Where(x => x.enabled && x.videoId == videoId && x.userId == userId)
            .ToListAsync(cancellationToken);

        foreach (var row in rows)
        {
            row.enabled = false;
        }
    }
}

public sealed class EgitimSoruCevapRepository(DBDataContext db)
    : EducationCrudRepository<EgitimSoruCevap>(db), IEgitimSoruCevapRepository
{
    public Task<List<EgitimSoruCevap>> ListAsync(
        int videoId,
        int? userId,
        CancellationToken cancellationToken)
    {
        return (
            from answer in Db.EgitimSoruCevap.AsNoTracking()
            join question in Db.EgitimSorulariTable.AsNoTracking()
                on answer.soruId equals question.Id
            where answer.enabled
                  && question.enabled
                  && question.videoId == videoId
                  && (!userId.HasValue || answer.userId == userId.Value)
            select answer)
            .ToListAsync(cancellationToken);
    }

    public async Task DisableExistingAsync(
        int questionId,
        int userId,
        CancellationToken cancellationToken)
    {
        var rows = await Db.EgitimSoruCevap
            .Where(x => x.enabled && x.soruId == questionId && x.userId == userId)
            .ToListAsync(cancellationToken);

        foreach (var row in rows)
        {
            row.enabled = false;
        }
    }
}

public sealed class EducationQuestionRepository(DBDataContext db)
    : EducationCrudRepository<EducationQuestionsTable>(db), IEducationQuestionRepository
{
    public Task<List<EducationQuestionsTable>> ListBySectionAsync(
        int sectionId,
        CancellationToken cancellationToken)
    {
        return Db.EducationQuestionsTable
            .AsNoTracking()
            .Where(x => x.enabled && x.sectionId == sectionId)
            .OrderBy(x => x.sira)
            .ThenBy(x => x.Id)
            .ToListAsync(cancellationToken);
    }

    public Task<List<EducationQuestionDto>> ListDtosAsync(
        int sectionId,
        CancellationToken cancellationToken)
    {
        return (
            from question in Db.EducationQuestionsTable.AsNoTracking()
            join userRow in Db.AdminUser.AsNoTracking()
                on question.createdUserId equals userRow.Id into userRows
            from creator in userRows.DefaultIfEmpty()
            where question.enabled && question.sectionId == sectionId
            orderby question.sira, question.Id
            select new EducationQuestionDto
            {
                id = question.Id,
                soru = question.soru,
                sikA = question.sikA,
                sikB = question.sikB,
                sikC = question.sikC,
                sikD = question.sikD,
                sikE = question.sikE,
                dogruCevap = question.dogruCevap,
                questionOrder = question.sira,
                olusturmaTarihi = question.createdDate,
                olusturanKisi = creator != null ? creator.name : null
            })
            .ToListAsync(cancellationToken);
    }
}

public sealed class EducationQuestionSectionRepository(DBDataContext db)
    : EducationCrudRepository<EducationQuestionSectionTable>(db),
        IEducationQuestionSectionRepository
{
    public IQueryable<EducationQuestionSectionDto> Search(
        bool admin,
        int? userId,
        int? sectionId,
        string? questionName)
    {
        // Sınav bölümleri de oluşturan kullanıcıya göre kısıtlanmaz.
        return
            from questionSection in Db.EducationQuestionSectionTable.AsNoTracking()
            join sectionRow in Db.EgitimBolumTable.AsNoTracking()
                on questionSection.courseId equals sectionRow.Id into sectionRows
            from section in sectionRows.DefaultIfEmpty()
            join userRow in Db.AdminUser.AsNoTracking()
                on questionSection.createdUserId equals userRow.Id into userRows
            from creator in userRows.DefaultIfEmpty()
            where questionSection.enabled
                  && (!sectionId.HasValue || questionSection.courseId == sectionId.Value)
                  && (string.IsNullOrWhiteSpace(questionName)
                      || (questionSection.questionName != null
                          && questionSection.questionName.Contains(questionName)))
            orderby questionSection.Id descending
            select new EducationQuestionSectionDto
            {
                id = questionSection.Id,
                sectionName = section != null ? section.egitimBolumu : null,
                questionName = questionSection.questionName,
                olusturmaTarihi = questionSection.createdDate,
                olusturanKisi = creator != null ? creator.name : null
            };
    }
}

public sealed class EducationQuestionAnswerRepository(DBDataContext db)
    : EducationCrudRepository<EducationQuestionAnswerTable>(db),
        IEducationQuestionAnswerRepository
{
    public Task<List<EducationQuestionAnswerTable>> ListAsync(
        int sectionId,
        int? userId,
        CancellationToken cancellationToken)
    {
        return (
            from answer in Db.EducationQuestionAnswerTable.AsNoTracking()
            join question in Db.EducationQuestionsTable.AsNoTracking()
                on answer.soruId equals question.Id
            where answer.enabled
                  && question.enabled
                  && question.sectionId == sectionId
                  && (!userId.HasValue || answer.userId == userId.Value)
            select answer)
            .ToListAsync(cancellationToken);
    }

    public Task<EducationQuestionAnswerTable?> FindCurrentAsync(
        int userId,
        int questionId,
        CancellationToken cancellationToken)
    {
        return Db.EducationQuestionAnswerTable.FirstOrDefaultAsync(
            x => x.enabled && x.userId == userId && x.soruId == questionId,
            cancellationToken);
    }
}
