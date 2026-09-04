using AskalePortal.Data.Models;
using AskalePortal.Data.RequestModel;
using AskalePortal.Data.RequestParams;
using AskalePortal.Data.ResponseModels;
using AskalePortal.Data.ResponseParams;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.BLL
{
	public partial class BLLActions
	{
        public class SureliIsTakipTable : BaseBLL<AskalePortal.Data.Models.SureliIsTakipTable>
        {
            private readonly IConfiguration _configuration;
            private readonly IWebHostEnvironment _env;
            private readonly IMapper _mapper;
            public SureliIsTakipTable(IConfiguration configuration, IWebHostEnvironment env,IMapper mapper) : base(configuration, env)
            {
                _configuration = configuration;
                _env = env;
                _mapper = mapper;
            }

            public async Task<Data.Models.SureliIsTakipTable?> deleteData(int id, int userId)
            {
                Data.Models.SureliIsTakipTable? sureliIsTakipTable = GetByID(id);
                sureliIsTakipTable.enabled = false;
                Data.Models.SureliIsTakipTable saveTable =await Update(sureliIsTakipTable);
                return saveTable;
            }

            public PageReturn<SureliIsTakipDto> FilterPageableDto(
      FilterPageParam<SureliIslerTakipDtoParameter> filterPageParam)
            {
                PageReturn<SureliIsTakipDto> result = new();

                int pageSize = filterPageParam.size.GetValueOrDefault(20);
                int pageNumber = filterPageParam.page.GetValueOrDefault(0);

                if (pageSize <= 0)
                {
                    pageSize = 20;
                }

                if (pageNumber < 0)
                {
                    pageNumber = 0;
                }

                int? userId = filterPageParam.liste?.userId;
                int? filterUserId = filterPageParam.liste?.filterUser;
                int? filterCompanyId = filterPageParam.liste?.filterCompany;

                string filterAciklama =
                    filterPageParam.liste?.filterAciklama?.Trim() ?? "";

                // Dropdown'daki "Seçiniz" değeri 0 gelirse filtre uygulanmasın.
                if (filterUserId == 0)
                {
                    filterUserId = null;
                }

                if (filterCompanyId == 0)
                {
                    filterCompanyId = null;
                }

                if (!userId.HasValue || userId.Value <= 0)
                {
                    result.content = new List<SureliIsTakipDto>();
                    result.totalElements = 0;
                    result.number = 0;
                    result.size = pageSize;

                    return result;
                }

                BLLActions.AdminUsers bllAdminUsers =
                    new BLLActions.AdminUsers(
                        _configuration,
                        _env,
                        _mapper);

                AdminUser? user =
                    bllAdminUsers.GetByID(userId.Value);

                if (user == null)
                {
                    result.content = new List<SureliIsTakipDto>();
                    result.totalElements = 0;
                    result.number = 0;
                    result.size = pageSize;

                    return result;
                }

                /*
                 * Önce bütün kullanıcılar için geçerli ortak koşullar ekleniyor.
                 *
                 * Java:
                 * c.enabled = true
                 * filterUser
                 * filterCompany
                 * filterAciklama
                 */
                IQueryable<Data.Models.SureliIsTakipTable> query =
                    dal.Get(u => u.enabled)
                        .Where(u =>
                            (!filterUserId.HasValue ||
                                u.createdUserId == filterUserId.Value) &&

                            (!filterCompanyId.HasValue ||
                                u.companyId == filterCompanyId.Value) &&

                            (string.IsNullOrEmpty(filterAciklama) ||
                                (u.aciklama != null &&
                                 u.aciklama.Contains(filterAciklama))));

                /*
                 * Java:
                 *
                 * roleId == 1:
                 *     findByEnabled(...)
                 *
                 * roleId != 1:
                 *     findByEnabledByUserId(...)
                 */
                if (user.roleId != 1)
                {
                    query = query.Where(
                        u => u.createdUserId == userId.Value);
                }

                // Sayfalama yapılmadan önce sıralama uygulanmalı.
                query = ApplySureliIsTakipSorting(
                    query,
                    filterPageParam);

                // Count mutlaka Skip/Take işleminden önce alınmalı.
                result.totalElements = query.Count();

                List<SureliIsTakipDto> content =
                    query
                        .Skip(pageNumber * pageSize)
                        .Take(pageSize)
                        .Select(u => new SureliIsTakipDto
                        {
                            id = u.Id,

                            fabrika = u.company != null
                                ? u.company.vtext
                                : "",

                            isinTanimi = u.isinTanimi,

                            baslamaTarihi =
                                u.baslamaTarihi.ToString("dd.MM.yyyy"),

                            terminTarihi =
                                u.terminTarihi.ToString("dd.MM.yyyy"),

                            mailSuresi = u.mailSuresi,

                            /*
                             * Bu iki alan şu aşamada kullanıcı ID listesidir.
                             * Aşağıda kullanıcı adlarına çevrilecek.
                             */
                            takipSorumlusu = u.takipSorumlusu,
                            ilgililer = u.muhattaplar,

                            tamamlandimi = u.tamamlandi,
                            aciklama = u.aciklama,
                            fileNames = u.files,

                            olusturanKisi = u.createdUser != null
                                ? u.createdUser.name
                                : "",

                            olusturanKisiId = u.createdUserId
                        })
                        .ToList();

                /*
                 * Java kodundaki muhattaplar ve takipSorumlusu ID'lerini
                 * kullanıcı adlarına çeviren bölüm.
                 */
                foreach (SureliIsTakipDto item in content)
                {
                    item.ilgililer = ResolveUserNames(
                        item.ilgililer,
                        bllAdminUsers);

                    item.takipSorumlusu = ResolveUserNames(
                        item.takipSorumlusu,
                        bllAdminUsers);
                }

                result.content = content;
                result.number = content.Count;
                result.size = pageSize;

                return result;
            }

            private static IQueryable<Data.Models.SureliIsTakipTable>
    ApplySureliIsTakipSorting(
        IQueryable<Data.Models.SureliIsTakipTable> query,
        FilterPageParam<SureliIslerTakipDtoParameter> filterPageParam)
            {
                IOrderedQueryable<Data.Models.SureliIsTakipTable>?
                    orderedQuery = null;

                void ApplyOrder<TKey>(
                    Expression<Func<
                        Data.Models.SureliIsTakipTable,
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
                    foreach (var sorting in filterPageParam.sorting)
                    {
                        bool isAscending =
                            sorting.sorting == "sorting_asc";

                        bool isDescending =
                            sorting.sorting == "sorting_desc";

                        // "none" veya tanımsız değer geldiyse sıralama uygulama.
                        if (!isAscending && !isDescending)
                        {
                            continue;
                        }

                        switch (sorting.key)
                        {
                            case "id":
                                ApplyOrder(
                                    u => u.Id,
                                    isDescending);
                                break;

                            case "fabrika":
                            case "companyId":
                                ApplyOrder(
                                    u => u.companyId,
                                    isDescending);
                                break;

                            case "isinTanimi":
                                ApplyOrder(
                                    u => u.isinTanimi,
                                    isDescending);
                                break;

                            case "baslamaTarihi":
                                ApplyOrder(
                                    u => u.baslamaTarihi,
                                    isDescending);
                                break;

                            case "terminTarihi":
                                ApplyOrder(
                                    u => u.terminTarihi,
                                    isDescending);
                                break;

                            case "mailSuresi":
                                ApplyOrder(
                                    u => u.mailSuresi,
                                    isDescending);
                                break;

                            case "takipSorumlusu":
                                ApplyOrder(
                                    u => u.takipSorumlusu,
                                    isDescending);
                                break;

                            case "ilgililer":
                            case "muhattaplar":
                                ApplyOrder(
                                    u => u.muhattaplar,
                                    isDescending);
                                break;

                            case "tamamlandimi":
                            case "tamamlandi":
                                ApplyOrder(
                                    u => u.tamamlandi,
                                    isDescending);
                                break;

                            case "aciklama":
                                ApplyOrder(
                                    u => u.aciklama,
                                    isDescending);
                                break;

                            case "olusturanKisi":
                            case "createdUserId":
                                ApplyOrder(
                                    u => u.createdUserId,
                                    isDescending);
                                break;
                        }
                    }
                }

                // Geçerli bir sıralama gönderilmediyse varsayılan.
                return orderedQuery ??
                       query.OrderByDescending(u => u.Id);
            }

            private static string ResolveUserNames(
    string? userIds,
    BLLActions.AdminUsers bllAdminUsers)
            {
                if (string.IsNullOrWhiteSpace(userIds))
                {
                    return "";
                }

                List<string> names = new();

                string[] values = userIds.Split(
                    ',',
                    StringSplitOptions.RemoveEmptyEntries |
                    StringSplitOptions.TrimEntries);

                foreach (string value in values)
                {
                    if (!int.TryParse(value, out int id))
                    {
                        continue;
                    }

                    AdminUser? relatedUser =
                        bllAdminUsers.GetByID(id);

                    if (relatedUser == null ||
                        string.IsNullOrWhiteSpace(relatedUser.name))
                    {
                        continue;
                    }

                    names.Add(relatedUser.name);
                }

                return string.Join(",", names);
            }
            public List<AskalePortal.Data.Models.SureliIsTakipTable> GetByUserId(int ID)
            {
                return dal.Get(u => (u.muhattaplar.Contains(ID.ToString()) || u.takipSorumlusu.Contains(ID.ToString()) || u.createdUserId == ID) && u.enabled == true).ToList();
            }

            public async Task<Data.Models.SureliIsTakipTable> save(Data.ResponseModels.SureliIsTakipSaveDto entity, Data.ResponseModels.SureliIsTakipSaveDto? isTakipTableEski, int userId)
            {
                if (entity.id == null)
                {
                    entity.createdUserId=(userId);
                    entity.createdDate=(DateTime.Now.ToString());
                    entity.enabled=(true);

                    Data.Models.SureliIsTakipTable? sureliIsTakipTable = await Add(_mapper.Map<Data.Models.SureliIsTakipTable>(entity));

                    List<string> ids = new List<string>();

                    foreach (string a in sureliIsTakipTable.muhattaplar.Split(","))
                    {
                        ids.Add(a);
                    }
                    foreach (string b in sureliIsTakipTable.takipSorumlusu.Split(","))
                    {
                        ids.Add(b);
                    }

                    foreach (string item in ids)
                    {
                        int id = int.Parse(item);
                        BLLActions.AdminUsers bllAdminUsers = new BLLActions.AdminUsers(_configuration, _env, _mapper);
                        UserByNameEMailDto user = bllAdminUsers.getUserByNameEMailDto(id);
                        for (int i = 0; i < sureliIsTakipTable.mailSuresi; i++)
                        {

                            if (sureliIsTakipTable.terminTarihi.AddDays(-i * 7) > DateTime.Now)
                            {
                                BLLActions.EmailMessages bllEmailMessages = new BLLActions.EmailMessages(_configuration, _env);
                                EmailMessage emailMessage = new EmailMessage();
                                emailMessage.subject=("Süreli İşler Takip (" + sureliIsTakipTable.Id.ToString() + ")");

                                emailMessage.toAddress=(user.email);
                                BLLActions.EmailReaderFile bllEmailReaderFile = new EmailReaderFile();
                                string mailMessage = bllEmailReaderFile.CreateIsTakipMailString(_configuration, _env, _mapper, "Süreli İş Takip", sureliIsTakipTable);
                                emailMessage.emailText=(mailMessage);
                                emailMessage.mailTuru=(1);
                                emailMessage.enabled=(true);
                                emailMessage.isSent=(false);
                                DateTime tarih = sureliIsTakipTable.terminTarihi.AddDays(-i * 7);
                                DateTime plannedDate = tarih.Date.Add(new TimeSpan(9, 0, 0));
                                emailMessage.plannedDate=(plannedDate);
                               await bllEmailMessages.Add(emailMessage);

                            }
                        }
                    }

                    return sureliIsTakipTable;

                }
                else
                {
                    BLLActions.EmailMessages bllEmailMessages = new BLLActions.EmailMessages(_configuration, _env);
                    //Data.ResponseModels.SureliIsTakipSaveDto isTakipTableEski = _mapper.Map<Data.ResponseModels.SureliIsTakipSaveDto>(GetByID(entity.id ??0));
                    Data.Models.SureliIsTakipTable isTakipTableYeni = _mapper.Map<Data.Models.SureliIsTakipTable>(entity);
                    if (isTakipTableEski?.enabled != isTakipTableYeni.enabled
                            || isTakipTableEski.tamamlandi != isTakipTableYeni.tamamlandi)
                    {
                        List<EmailMessage> listEmailMessages = bllEmailMessages
                                .findByEnabledAndSubject(entity.id.ToString()??"");
                        foreach (EmailMessage emailMessage in listEmailMessages)
                        {
                            emailMessage.enabled = (false);
                          await  bllEmailMessages.Update(emailMessage);
                        }
                    }
                    else if (!isTakipTableEski.mailSuresi.Equals(isTakipTableYeni.mailSuresi)
                            || !(isTakipTableEski.muhattaplar??"").Equals(isTakipTableYeni.muhattaplar)
                            || !(isTakipTableEski.takipSorumlusu??"").Equals(isTakipTableYeni.takipSorumlusu))
                    {
                        List<EmailMessage> listEmailMessages = bllEmailMessages
                                .findByEnabledAndSubject(entity.id.ToString() ?? "");
                        foreach (EmailMessage emailMessage in listEmailMessages)
                        {
                            emailMessage.enabled = (false);
                           await bllEmailMessages.Update(emailMessage);
                        }

                        List<string> ids = new List<string>();

                        foreach (string a in isTakipTableYeni.muhattaplar.Split(","))
                        {
                            ids.Add(a);
                        }
                        foreach (string b in isTakipTableYeni.takipSorumlusu.Split(","))
                        {
                            ids.Add(b);
                        }

                        foreach (string item in ids)
                        {
                            int id = int.Parse(item);
                            BLLActions.AdminUsers bllAdminUsers = new BLLActions.AdminUsers(_configuration, _env, _mapper);
                            UserByNameEMailDto user = bllAdminUsers.getUserByNameEMailDto(id);
                            for (int i = 0; i < isTakipTableYeni.mailSuresi; i++)
                            {

                                if (isTakipTableYeni.terminTarihi.AddDays(-1 * (i * 7)) > DateTime.Now.Date)
                                {

                                    EmailMessage emailMessage = new EmailMessage();
                                    emailMessage.subject = ("Süreli İşler Takip (" + isTakipTableYeni.Id.ToString() + ")");

                                    emailMessage.toAddress = (user.email);
                                    BLLActions.EmailReaderFile bllEmailReaderFile = new EmailReaderFile();
                                    string mailMessage = bllEmailReaderFile.CreateIsTakipMailString(_configuration, _env, _mapper, "Süreli İş Takip", isTakipTableYeni);
                                    emailMessage.emailText = (mailMessage);
                                    emailMessage.mailTuru = (1);
                                    emailMessage.enabled = (true);
                                    emailMessage.isSent = (false);
                                    DateTime tarih = isTakipTableYeni.terminTarihi.AddDays(-1 * (i * 7));
                                    DateTime plannedDate = new DateTime(tarih.Year, tarih.Month, tarih.Day, 9, 0, 0);
                                    emailMessage.plannedDate = (plannedDate);
                                    await bllEmailMessages.Add(emailMessage);

                                }
                            }
                        }

                    }

                    isTakipTableYeni.updatedUserId=(userId);
                    isTakipTableYeni.updatedDate=(DateTime.Now);
                    isTakipTableYeni.enabled=(true);
                    Data.Models.SureliIsTakipTable isTakip = await Update(isTakipTableYeni);
                    return isTakip;
                }
            }
        }
    }
}
