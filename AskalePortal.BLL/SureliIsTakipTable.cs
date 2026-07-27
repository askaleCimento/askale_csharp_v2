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

            public PageReturn<SureliIsTakipDto>? FilterPageableDto(FilterPageParam<SureliIslerTakipDtoParameter> filterPageParam)
            {
                PageReturn<SureliIsTakipDto>? result = new PageReturn<SureliIsTakipDto>();
                int pageSize = filterPageParam.size ?? 20;
                int pageNumber = filterPageParam.page ?? 0;

                int? userId = filterPageParam.liste?.userId;
                int? filterUserId= filterPageParam.liste?.filterUserId;
                int? filterCompanyId= filterPageParam.liste?.filterCompanyId;
                string? filterAciklama = filterPageParam.liste?.filterAciklama;


                BLLActions.AdminUsers bllAdminUsers = new BLLActions.AdminUsers(_configuration, _env, _mapper);
                AdminUser user = bllAdminUsers.GetByID(userId??0);
                IQueryable<Data.Models.SureliIsTakipTable> query = dal.Get(u => u.enabled &&
                user.roleId == 1 ? true : u.createdUserId == userId
                && ((filterUserId == null || filterUserId == 0) ? true : u.createdUserId == filterUserId)
                && (filterCompanyId == null || filterCompanyId == 0 ? true : u.companyId == filterCompanyId)
                && (filterAciklama == null || filterAciklama == "" ? true : u.aciklama.Contains(filterAciklama))

                ).OrderByDescending(u=>u.Id);
                result.content = query
                  .Skip(pageSize * pageNumber).Take(pageSize)

                    .Select(u => new SureliIsTakipDto()
                    {
                        aciklama=u.aciklama,
                        baslamaTarihi=u.baslamaTarihi,
                        fabrika=u.company.vtext,
                        fileNames=u.files,
                        id=u.Id,
                        ilgililer=u.muhattaplar,
                        isinTanimi=u.isinTanimi,
                        mailSuresi=u.mailSuresi,
                        olusturanKisi=u.createdUser.name,
                        olusturanKisiId=u.createdUserId,
                        takipSorumlusu=u.takipSorumlusu,
                        tamamlandimi=u.tamamlandi,
                        terminTarihi = u.terminTarihi


                    }).ToList();
                result.totalElements = query.Count();
                result.number = result.content.Count();
                result.size = pageSize;

                return result;
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
