using AskalePortal.Constants;
using AskalePortal.Data.Contracts.Detached;
using AskalePortal.Data.Models;
using AskalePortal.Data.RequestModel;
using AskalePortal.Data.RequestParams;
using AskalePortal.Data.ResponseModels;
using AskalePortal.Data.ResponseParams;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System.Globalization;
using System.Linq.Expressions;

namespace AskalePortal.BLL
{
    public partial class BLLActions
    {
        public class ISGAksiyonTable
            : BaseBLL<AskalePortal.Data.Models.ISGAksiyonTable>
        {
            private readonly IConfiguration _configuration;
            private readonly IWebHostEnvironment _env;
            private readonly IMapper _mapper;

            public ISGAksiyonTable(
                IConfiguration configuration,
                IWebHostEnvironment env,
                IMapper mapper)
                : base(configuration, env)
            {
                _configuration = configuration;
                _env = env;
                _mapper = mapper;
            }

            public List<Data.Models.ISGAksiyonTable> GetByCompanyID(
                int userId,
                int activePage,
                int pageSize)
            {
                return dal.Get(u =>
                        u.bidirimdeBulunan == userId &&
                        u.enabled == true)
                    .OrderByDescending(u => u.Id)
                    .Skip(activePage * pageSize)
                    .Take(pageSize)
                    .ToList();
            }

            public List<Data.Models.ISGAksiyonTable> GetAllWithPages(
                int activePage,
                int pageSize)
            {
                return dal.Get(u => u.enabled == true)
                    .OrderByDescending(u => u.Id)
                    .Skip(activePage * pageSize)
                    .Take(pageSize)
                    .ToList();
            }

            public List<Data.Models.ISGAksiyonTable> GetByUser(
                int userId,
                int activePage,
                int pageSize,
                int? id)
            {
                string userID = userId.ToString();

                return dal.Get(u =>
                        (
                            u.bidirimdeBulunan == userId ||
                            u.ISGAksiyonTakipTable.Any(y =>
                                y.aksiyonSorumlulari != null &&
                                y.aksiyonSorumlulari.Contains(userID))
                        ) &&
                        u.enabled == true &&
                        (
                            id.HasValue
                                ? u.Id == id.Value
                                : true
                        ))
                    .OrderByDescending(u => u.Id)
                    .Skip(activePage * pageSize)
                    .Take(pageSize)
                    .ToList();
            }

            public List<Data.Models.ISGAksiyonTable> GetAllWithPages(
                int[] companies,
                int activePage,
                int pageSize)
            {
                return dal.Get(u =>
                        u.enabled == true &&
                        companies.Contains(u.companyId))
                    .OrderByDescending(u => u.Id)
                    .Skip(activePage * pageSize)
                    .Take(pageSize)
                    .ToList();
            }

            public List<Data.Models.ISGAksiyonTable> GetAllWithPages(
                int[] companies)
            {
                return dal.Get(u =>
                        u.enabled == true &&
                        companies.Contains(u.companyId))
                    .OrderByDescending(u => u.Id)
                    .ToList();
            }

            public List<Data.Models.ISGAksiyonTable> GetAllWithPages(
                int[] companies,
                int activePage,
                int pageSize,
                int? id)
            {
                return dal.Get(u =>
                        u.enabled == true &&
                        companies.Contains(u.companyId) &&
                        (
                            id.HasValue
                                ? u.Id == id.Value
                                : true
                        ))
                    .OrderByDescending(u => u.Id)
                    .Skip(activePage * pageSize)
                    .Take(pageSize)
                    .ToList();
            }

            public int approvalCount(int userId)
            {
                var sorumluKisiAcanKisi =
                    (
                        from c in dal.dB.ISGAksiyonTable

                        join b in dal.dB.ISGAksiyonTakipTable
                            on c.Id equals b.aksiyonId
                            into takipJoin

                        from b in takipJoin.DefaultIfEmpty()

                        join a in dal.dB.ISGUser
                            on c.companyId equals a.companyId
                            into userJoin

                        from a in userJoin.DefaultIfEmpty()

                        where
                            c.bittiMi == false &&
                            c.enabled &&
                            (
                                (
                                    b != null &&
                                    b.aksiyonSorumlulari != null &&
                                    b.aksiyonSorumlulari.Contains(
                                        "[" + userId + "]")
                                ) ||
                                c.bidirimdeBulunan == userId ||
                                (
                                    a != null &&
                                    a.userId == userId
                                )
                            )

                        select c
                    )
                    .Distinct()
                    .ToList();

                return sorumluKisiAcanKisi.Count();
            }

            public async Task<ISGAksiyonTableSaveDto> save(
     ISGAksiyonTableSaveDto entity,
     int userId)
            {
                BLLActions.AdminUsers bllAdminUsers =
                    new BLLActions.AdminUsers(
                        _configuration,
                        _env,
                        _mapper);

                AdminUser? user = bllAdminUsers.GetByID(userId);

                if (user == null)
                {
                    throw new KeyNotFoundException(
                        $"Kullanıcı bulunamadı. Id: {userId}");
                }

                Data.Models.ISGAksiyonTable saveEntity;

                if (entity.id == null)
                {
                    // Yeni kayıt
                    entity.createdUserId = userId;
                    entity.createdDate = DateTime.Now.ToString();
                    entity.bidirimdeBulunan = userId;
                    entity.enabled = true;

                    Data.Models.ISGAksiyonTable newEntity =
                        _mapper.Map<Data.Models.ISGAksiyonTable>(entity);

                    Data.Models.ISGAksiyonTable? addedEntity =
                        await Add(newEntity);

                    if (addedEntity == null)
                    {
                        throw new InvalidOperationException(
                            "İSG aksiyon kaydı oluşturulamadı.");
                    }

                    saveEntity = addedEntity;

                    try
                    {
                        BLLActions.ISGUser bllIsgUser =
                            new BLLActions.ISGUser(
                                _configuration,
                                _env);

                        Data.Models.ISGUser? isgUser =
                            bllIsgUser.findByCompanyId(user.companyId);

                        if (isgUser != null &&
                            !string.IsNullOrWhiteSpace(
                                isgUser.planliBakimEmail))
                        {
                            string planliBakimEmail =
                                isgUser.planliBakimEmail.TrimEnd(';');

                            string[] emailList =
                                planliBakimEmail.Split(
                                    ';',
                                    StringSplitOptions.RemoveEmptyEntries |
                                    StringSplitOptions.TrimEntries);

                            foreach (string mail in emailList)
                            {
                                EmailMessage msg1 = new EmailMessage
                                {
                                    subject =
                                        user.name + " tarafından " +
                                        saveEntity.Id +
                                        " nolu açılmış İSG İş Bildirimidir.",

                                    toAddress = mail,
                                    isSent = false,
                                    plannedDate = DateTime.Now,
                                    mailTuru = 1
                                };

                                BLLActions.EmailReaderFile bllEmailReaderFile =
                                    new BLLActions.EmailReaderFile();

                                msg1.emailText =
                                    bllEmailReaderFile.IsgAksiyonEmailTemplate(
                                        _configuration,
                                        _env,
                                        "Sayın Yetkili",
                                        user.name +
                                        " tarafından " +
                                        saveEntity.Id +
                                        " id'li İSG iş bildirimi açılmıştır. " +
                                        "Açıklama: " +
                                        entity.uygunsuzlukAciklama +
                                        " <br /><br />Saygılarımızla.");

                                BLLActions.EmailMessages bllEmailMessages =
                                    new BLLActions.EmailMessages(
                                        _configuration,
                                        _env);

                                await bllEmailMessages.Add(msg1);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
                else
                {
                    // Güncelleme
                    Data.Models.ISGAksiyonTable? eskiISGAksiyonTable =
                        GetByID(entity.id.Value);

                    if (eskiISGAksiyonTable == null)
                    {
                        throw new KeyNotFoundException(
                            "İSG aksiyon kaydı bulunamadı. Id: " +
                            entity.id.Value);
                    }

                    // Map işleminden önce eski şirket değerini sakla.
                    int oncekiCompanyId =
                        eskiISGAksiyonTable.companyId;

                    /*
                     * Önemli:
                     * Yeni bir ISGAksiyonTable nesnesi oluşturmuyoruz.
                     * DTO değerlerini EF Core'un takip ettiği mevcut
                     * entity üzerine aktarıyoruz.
                     */
                    _mapper.Map(
                        entity,
                        eskiISGAksiyonTable);

                    eskiISGAksiyonTable.updatedUserId = userId;
                    eskiISGAksiyonTable.updatedDate =
                        DateTime.Now;

                    Data.Models.ISGAksiyonTable? updatedEntity =
                        await Update(eskiISGAksiyonTable);

                    if (updatedEntity == null)
                    {
                        throw new InvalidOperationException(
                            "İSG aksiyon kaydı güncellenemedi. Id: " +
                            entity.id.Value);
                    }

                    saveEntity = updatedEntity;

                    // Aksiyonun şirketi değiştirildiyse e-posta gönder.
                    if (oncekiCompanyId != saveEntity.companyId)
                    {
                        try
                        {
                            BLLActions.ISGUser bllISGUser =
                                new BLLActions.ISGUser(
                                    _configuration,
                                    _env);

                            Data.Models.ISGUser? isgUser =
                                bllISGUser.findByCompanyId(
                                    saveEntity.companyId);

                            if (isgUser != null &&
                                !string.IsNullOrWhiteSpace(
                                    isgUser.planliBakimEmail))
                            {
                                string planliBakimEmail =
                                    isgUser.planliBakimEmail.TrimEnd(';');

                                string[] emailList =
                                    planliBakimEmail.Split(
                                        ';',
                                        StringSplitOptions.RemoveEmptyEntries |
                                        StringSplitOptions.TrimEntries);

                                foreach (string mail in emailList)
                                {
                                    EmailMessage msg1 = new EmailMessage
                                    {
                                        subject =
                                            user.name + " tarafından " +
                                            saveEntity.Id +
                                            " nolu İSG İş Bildirimi güncellenmiştir.",

                                        toAddress = mail,
                                        isSent = false,
                                        plannedDate = DateTime.Now,
                                        mailTuru = 1
                                    };

                                    BLLActions.EmailReaderFile
                                        bllEmailReaderFile =
                                            new BLLActions.EmailReaderFile();

                                    msg1.emailText =
                                        bllEmailReaderFile
                                            .IsgAksiyonEmailTemplate(
                                                _configuration,
                                                _env,
                                                "Sayın Yetkili",
                                                user.name +
                                                " tarafından " +
                                                saveEntity.Id +
                                                " id'li İSG iş bildirimi " +
                                                "güncellenmiştir. Açıklama: " +
                                                entity.uygunsuzlukAciklama +
                                                " <br /><br />" +
                                                "Saygılarımızla.");

                                    BLLActions.EmailMessages
                                        bllEmailMessages =
                                            new BLLActions.EmailMessages(
                                                _configuration,
                                                _env);

                                    await bllEmailMessages.Add(msg1);
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }
                    }
                }

                ISGAksiyonTableSaveDto? result =
                    _mapper.Map<ISGAksiyonTableSaveDto>(
                        saveEntity);

                if (result == null)
                {
                    throw new InvalidOperationException(
                        "İSG aksiyon kaydı DTO'ya dönüştürülemedi.");
                }

                return result;
            }
            public PageReturn<ISGAksiyonTableDto> listByPageable(
                FilterPageParam<IsgAksiyonTablePageableListParameter>
                    filterPageParam)
            {
                PageReturn<ISGAksiyonTableDto> result = new();

                int pageSize =
                    filterPageParam.size ?? 20;

                int pageNumber =
                    filterPageParam.page ?? 0;

                int? id =
                    filterPageParam.liste?.id;

                int? companyId =
                    filterPageParam.liste?.companyId;

                int? uygunsuzlukKaynagiId =
                    filterPageParam.liste?.uygunsuzlukKaynagiId;

                int? userId =
                    filterPageParam.liste?.userId;

                int? uniteId =
                    filterPageParam.liste?.uniteId;

                string? aciklama =
                    filterPageParam.liste?.aciklama;

                bool? bittiMi =
                    filterPageParam.liste?.bittiMi;

                DateTime? baslangicTarihi =
                    ParseDate(
                        filterPageParam.liste?.baslangicTarihi);

                DateTime? bitisTarihi =
                    ParseDate(
                        filterPageParam.liste?.bitisTarihi);

                DateTime? bitisTarihiExclusive =
                    bitisTarihi?.Date.AddDays(1);

                BLLActions.AdminUsers bllAdminUsers =
                    new BLLActions.AdminUsers(
                        _configuration,
                        _env,
                        _mapper);

                AdminUser? user =
                    bllAdminUsers.GetByID(userId ?? 0);

                if (user == null)
                {
                    result.content =
                        new List<ISGAksiyonTableDto>();

                    result.totalElements = 0;
                    result.number = 0;
                    result.size = pageSize;

                    return result;
                }

                BLLActions.RoleDetails bllRoleDetails =
                    new BLLActions.RoleDetails(
                        _configuration,
                        _env,
                        _mapper);

                RoleDetail? rdAksiyon =
                    bllRoleDetails.GetByRoleIDAndModuleID(
                        user.roleId,
                        (int)CommonConstants.MODULES.ISGAKSIYON);

                RoleDetail? rdAksiyonTakip =
                    bllRoleDetails.GetByRoleIDAndModuleID(
                        user.roleId,
                        (int)CommonConstants.MODULES.ISGAKSIYONTAKIP);

                BLLActions.Roles bllRoles =
                    new BLLActions.Roles(
                        _configuration,
                        _env,
                        _mapper);

                Role? role =
                    bllRoles.GetByID(user.roleId);

                IQueryable<Data.Models.ISGAksiyonTable> query =
                    dal.Get(a => a.enabled)
                        .Where(a =>
                            (
                                string.IsNullOrEmpty(aciklama) ||
                                (
                                    a.uygunsuzlukAciklama ?? ""
                                ).Contains(aciklama)
                            ) &&
                            (
                                !id.HasValue ||
                                a.Id == id.Value
                            ) &&
                            (
                                !companyId.HasValue ||
                                a.companyId == companyId.Value
                            ) &&
                            (
                                !uygunsuzlukKaynagiId.HasValue ||
                                a.uygunsuzlukKaynagiId ==
                                uygunsuzlukKaynagiId.Value
                            ) &&
                            (
                                !uniteId.HasValue ||
                                a.uygunsuzlukBulunanUniteId ==
                                uniteId.Value
                            ) &&
                            (
                                !bittiMi.HasValue ||
                                a.bittiMi == bittiMi.Value
                            ) &&
                            (
                                !baslangicTarihi.HasValue ||
                                a.uygunsuzlukTarihi >=
                                baslangicTarihi.Value
                            ) &&
                            (
                                !bitisTarihiExclusive.HasValue ||
                                a.uygunsuzlukTarihi <
                                bitisTarihiExclusive.Value
                            ));

                bool canSeeAll =
                    user.roleId == 1 ||
                    (
                        rdAksiyon != null &&
                        rdAksiyonTakip != null &&
                        rdAksiyon.canSeeLogs &&
                        rdAksiyonTakip.canSeeLogs
                    );

                if (canSeeAll)
                {
                    List<string> companyCodes =
                        GetCompanyCodes(role);

                    query = query.Where(a =>
                        companyCodes.Contains(
                            a.company.vkorg));
                }
                else
                {
                    if (!userId.HasValue)
                    {
                        result.content =
                            new List<ISGAksiyonTableDto>();

                        result.totalElements = 0;
                        result.number = 0;
                        result.size = pageSize;

                        return result;
                    }

                    string userIdText =
                        userId.Value.ToString();

                    query = query.Where(a =>
                        a.bidirimdeBulunan == userId.Value ||
                        a.ISGAksiyonTakipTable.Any(f =>
                            f.aksiyonSorumlulari != null &&
                            f.aksiyonSorumlulari.Contains(
                                userIdText)) ||
                        a.company.ISGUser.Any(g =>
                            g.userId == userId.Value));
                }

                query =
                    ApplySorting(query, filterPageParam);

                result.totalElements =
                    query.Count();

                List<ISGAksiyonTableDto> content =
                    query
                        .Skip(pageNumber * pageSize)
                        .Take(pageSize)
                        .Select(a =>
                            new ISGAksiyonTableDto
                            {
                                id = a.Id,
                                fileOnceki = a.fileOnceki,
                                fileSonraki = a.fileSonraki,

                                vtext =
                                    a.company.vtext,

                                uygunsuzlukKaynagi =
                                    a.uygunsuzlukKaynagi
                                        .uygunsuzlukKaynagi,

                                uygunsuzlukTarihi =
                                    a.uygunsuzlukTarihi,

                                section =
                                    a.uygunsuzlukBulunanUnite
                                        .section,

                                name =
                                    a.bidirimdeBulunanNavigation
                                        .name,

                                uygunsuzlukAciklama =
                                    a.uygunsuzlukAciklama,

                                uygunsuzlukOneri =
                                    a.uygunsuzlukOneri,

                                bittiMi =
                                    a.bittiMi
                            })
                        .ToList();

                result.content = content;
                result.number = content.Count;
                result.size = pageSize;

                return result;
            }

            private static DateTime? ParseDate(string? value)
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    return null;
                }

                if (DateTime.TryParseExact(
                        value,
                        "yyyy-MM-dd",
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.None,
                        out DateTime date))
                {
                    return date.Date;
                }

                return null;
            }

            private static List<string> GetCompanyCodes(Role? role)
            {
                if (string.IsNullOrWhiteSpace(role?.companies))
                {
                    return new List<string>();
                }

                return role.companies
                    .Replace("[", "")
                    .Replace("]", "")
                    .Split(
                        ',',
                        StringSplitOptions.RemoveEmptyEntries |
                        StringSplitOptions.TrimEntries)
                    .ToList();
            }

            private static
                IQueryable<Data.Models.ISGAksiyonTable>
                ApplySorting(
                    IQueryable<Data.Models.ISGAksiyonTable> query,
                    FilterPageParam<
                        IsgAksiyonTablePageableListParameter>
                        filterPageParam)
            {
                IOrderedQueryable<
                    Data.Models.ISGAksiyonTable>? orderedQuery = null;

                void ApplyOrder<TKey>(
                    Expression<Func<
                        Data.Models.ISGAksiyonTable,
                        TKey>> expression,
                    bool descending)
                {
                    if (orderedQuery == null)
                    {
                        orderedQuery = descending
                            ? query.OrderByDescending(expression)
                            : query.OrderBy(expression);
                    }
                    else
                    {
                        orderedQuery = descending
                            ? orderedQuery.ThenByDescending(expression)
                            : orderedQuery.ThenBy(expression);
                    }
                }

                if (filterPageParam.sorting != null)
                {
                    foreach (
                        var sorting in filterPageParam.sorting)
                    {
                        bool descending =
                            sorting.sorting == "sorting_desc";

                        switch (sorting.key)
                        {
                            case "id":
                                ApplyOrder(
                                    a => a.Id,
                                    descending);
                                break;

                            case "companyId":
                                ApplyOrder(
                                    a => a.companyId,
                                    descending);
                                break;

                            case "uygunsuzlukKaynagiId":
                                ApplyOrder(
                                    a => a.uygunsuzlukKaynagiId,
                                    descending);
                                break;

                            case "uniteId":
                            case "uygunsuzlukBulunanUniteId":
                                ApplyOrder(
                                    a =>
                                        a.uygunsuzlukBulunanUniteId,
                                    descending);
                                break;

                            case "uygunsuzlukTarihi":
                                ApplyOrder(
                                    a => a.uygunsuzlukTarihi,
                                    descending);
                                break;

                            case "bittiMi":
                                ApplyOrder(
                                    a => a.bittiMi,
                                    descending);
                                break;

                            case "aciklama":
                            case "uygunsuzlukAciklama":
                                ApplyOrder(
                                    a => a.uygunsuzlukAciklama,
                                    descending);
                                break;
                        }
                    }
                }

                return orderedQuery ??
                       query.OrderByDescending(a => a.Id);
            }

            public List<ISGAksiyonTableDto> listFilter(
                FilterParam<
                    IsgAksiyonTablePageableListParameter>
                    filterParam)
            {
                int? id =
                    filterParam.liste?.id;

                int? companyId =
                    filterParam.liste?.companyId;

                int? uygunsuzlukKaynagiId =
                    filterParam.liste?.uygunsuzlukKaynagiId;

                int? userId =
                    filterParam.liste?.userId;

                int? uniteId =
                    filterParam.liste?.uniteId;

                string? aciklama =
                    filterParam.liste?.aciklama;

                DateTime? uygunsuzlukTarihi =
                    ParseDate(
                        filterParam.liste?.uygunsuzlukTarihi);

                BLLActions.AdminUsers bllAdminUsers =
                    new BLLActions.AdminUsers(
                        _configuration,
                        _env,
                        _mapper);

                AdminUser? user =
                    bllAdminUsers.GetByID(userId ?? 0);

                if (user == null)
                {
                    return new List<ISGAksiyonTableDto>();
                }

                IQueryable<Data.Models.ISGAksiyonTable> query =
                    dal.Get(a => a.enabled)
                        .Where(a =>
                            (
                                string.IsNullOrEmpty(aciklama) ||
                                (
                                    a.uygunsuzlukAciklama ?? ""
                                ).Contains(aciklama)
                            ) &&
                            (
                                !id.HasValue ||
                                a.Id == id.Value
                            ) &&
                            (
                                !companyId.HasValue ||
                                a.companyId == companyId.Value
                            ) &&
                            (
                                !uygunsuzlukKaynagiId.HasValue ||
                                a.uygunsuzlukKaynagiId ==
                                uygunsuzlukKaynagiId.Value
                            ) &&
                            (
                                !uniteId.HasValue ||
                                a.uygunsuzlukBulunanUniteId ==
                                uniteId.Value
                            ));

                if (uygunsuzlukTarihi.HasValue)
                {
                    DateTime dateStart =
                        uygunsuzlukTarihi.Value.Date;

                    DateTime dateEnd =
                        dateStart.AddDays(1);

                    query = query.Where(a =>
                        a.uygunsuzlukTarihi >= dateStart &&
                        a.uygunsuzlukTarihi < dateEnd);
                }

                if (user.roleId != 1)
                {
                    if (!userId.HasValue)
                    {
                        return new List<ISGAksiyonTableDto>();
                    }

                    query = query.Where(a =>
                        a.bidirimdeBulunan ==
                        userId.Value);
                }

                return query
                    .OrderByDescending(a => a.Id)
                    .Select(a =>
                        new ISGAksiyonTableDto
                        {
                            id = a.Id,
                            fileOnceki = a.fileOnceki,
                            fileSonraki = a.fileSonraki,

                            vtext =
                                a.company.vtext,

                            uygunsuzlukKaynagi =
                                a.uygunsuzlukKaynagi
                                    .uygunsuzlukKaynagi,

                            uygunsuzlukTarihi =
                                a.uygunsuzlukTarihi,

                            section =
                                a.uygunsuzlukBulunanUnite
                                    .section,

                            name =
                                a.bidirimdeBulunanNavigation
                                    .name,

                            uygunsuzlukAciklama =
                                a.uygunsuzlukAciklama,

                            bittiMi =
                                a.bittiMi
                        })
                    .ToList();
            }

            public async Task<string> deleteFile(
                int id,
                string fileName)
            {
                Data.Models.ISGAksiyonTable? isgAksiyonTable =
                    GetByID(id);

                if (isgAksiyonTable == null)
                {
                    return "0";
                }

                if (string.IsNullOrEmpty(
                        isgAksiyonTable.fileOnceki))
                {
                    return "0";
                }

                string? newFile = null;

                string[] files =
                    isgAksiyonTable.fileOnceki.Split("$");

                foreach (string file in files)
                {
                    if (!fileName.Equals(file))
                    {
                        newFile += file + "$";
                    }
                }

                if (newFile != null)
                {
                    newFile = newFile.Substring(
                        0,
                        newFile.Count() - 1);
                }

                isgAksiyonTable.fileOnceki = newFile;

                Data.Models.ISGAksiyonTable? updatedEntity =
                    await Update(isgAksiyonTable);

                if (updatedEntity == null)
                {
                    return "0";
                }

                return "";
            }

            public async Task<string> completed(int id)
            {
                Data.Models.ISGAksiyonTable? isgAksiyonTable =
                    GetByID(id);

                if (isgAksiyonTable == null)
                {
                    return "0";
                }

                isgAksiyonTable.bittiMi = true;

                Data.Models.ISGAksiyonTable?
                    isgAksiyonTableReturn =
                        await Update(isgAksiyonTable);

                if (isgAksiyonTableReturn == null)
                {
                    return "0";
                }

                return "1";
            }
        }
    }
}