using AskalePortal.Constants;
using AskalePortal.Data.Models;
using AskalePortal.Data.RequestModel;
using AskalePortal.Data.RequestParams;
using AskalePortal.Data.ResponseModels;
using AskalePortal.Data.ResponseParams;
using AutoMapper;
using Azure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Reporting.Map.WebForms.BingMaps;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using Org.BouncyCastle.Bcpg;
using Org.BouncyCastle.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static AskalePortal.BLL.BLLActions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AskalePortal.BLL
{

    public partial class BLLActions
    {
        public class DahiliYazismaTable : BaseBLL<AskalePortal.Data.Models.DahiliYazismaTable>
        {
            private readonly IConfiguration _configuration;
            private readonly IWebHostEnvironment _env;
            private readonly IMapper _mapper;
            public DahiliYazismaTable(IConfiguration configuration, IWebHostEnvironment env, IMapper mapper) : base(configuration, env)
            {
                _configuration = configuration;
                _env = env;
                _mapper = mapper;
            }
            public List<AskalePortal.Data.Models.DahiliYazismaTable> GetByCreatedUserID(int ID)
            {
                return dal.Get(u => u.createdUserId == ID).ToList();
            }

            public AskalePortal.Data.Models.DahiliYazismaTable GetByApprovedID(int id)
            {
                return dal.Get(u => u.onaylandiMi == false && u.Id == id && u.bittiMi == false && u.enabled == true).FirstOrDefault() ?? new AskalePortal.Data.Models.DahiliYazismaTable();
            }
            public AskalePortal.Data.Models.DahiliYazismaTable GetByApprovedCeoID(int id)
            {
                return dal.Get(u => u.onaylandiMi == true && u.Id == id && u.bittiMi == false).FirstOrDefault() ?? new AskalePortal.Data.Models.DahiliYazismaTable();
            }
            public List<AskalePortal.Data.Models.DahiliYazismaTable> GetAllByKanalGorusu(int userId)
            {
                return dal.Get(u => u.enabled == true && u.onaylandiMi == true && (u.kanalGorusuUserId == userId || u.lastUserId == userId) && u.bittiMi == false && u.kanalGorusuOkmi == false && u.redEttiMi == false).ToList();
            }

            public AskalePortal.Data.Models.DahiliYazismaTable GetByKanalGorusu(int userId, int id)
            {
                return dal.Get(u => u.enabled == true && u.onaylandiMi == true && (u.kanalGorusuUserId == userId || u.lastUserId == userId) && u.bittiMi == false && u.kanalGorusuOkmi == false && u.redEttiMi == false).FirstOrDefault() ?? new AskalePortal.Data.Models.DahiliYazismaTable();
            }

            public List<AskalePortal.Data.Models.DahiliYazismaTable> GetByBitis(int userId)
            {
                return dal.Get(u => u.enabled == true && ((u.lastUserId == userId && u.mudurBittiMi == false) || (u.lastUserId2 == userId && u.mudurBittiMi == true)) && u.bittiMi == false && u.onaylandiMi == true && u.redEttiMi == false).ToList();
            }

            public List<AskalePortal.Data.Models.DahiliYazismaTable> GetMyAll(int userId, string name, int? belgeno, int? companyId, string servisi, string konu, int? kanalId, bool? onayDurumu)
            {
                return dal.Get(u => (string.IsNullOrEmpty(name) ? true : u.createdUser.name.ToLower().Contains(name.ToLower())) && (belgeno.HasValue ? u.Id == belgeno : true) && (companyId.HasValue ? u.companyId == companyId : true) &&
                (string.IsNullOrEmpty(servisi) ? true : u.servisi.ToLower().Contains(servisi.ToLower())) && (string.IsNullOrEmpty(konu) ? true : u.konu.ToLower().Contains(konu.ToLower())) &&
                (kanalId.HasValue ? u.kanalId == kanalId.Value : true) && (onayDurumu.HasValue ? (onayDurumu.Value == true ? u.redEttiMi == true : u.onaylandiMi == true) : (u.onaylandiMi == true || u.redEttiMi == true)) &&
                u.enabled == true && (u.onaylayici1 == userId || u.onaylayici2 == userId || u.onaylayici3 == userId ||
                u.onaylayici4 == userId || u.createdUserId == userId || u.lastUserId == userId || u.lastUserId2 == userId) && u.bittiMi == true &&
                (u.onaylandiMi == true || u.redEttiMi == true)).ToList();

            }

            public List<AskalePortal.Data.Models.DahiliYazismaTable> GetMyActiveAll(int userId)
            {
                return dal.Get(u => u.enabled == true && (u.onaylayici1 == userId || u.onaylayici2 == userId || u.onaylayici3 == userId || u.onaylayici4 == userId || u.createdUserId == userId || u.kanalGorusuUserId == userId || u.kanal.userId == userId) && u.bittiMi == false && u.redEttiMi == false).ToList();
            }

            public List<AskalePortal.Data.Models.DahiliYazismaTable> GetMyAllBilgi(int userId)
            {
                return dal.Get(u => u.enabled == true && (u.bilgiUserId1 == userId || u.bilgiUserId2 == userId || u.bilgiUserId3 == userId || u.bilgiUserId4 == userId || u.bilgiUserId5 == userId) && u.bittiMi == false && u.redEttiMi == false).ToList();
            }

            public List<AskalePortal.Data.Models.DahiliYazismaTable> GetAllBySuperUser(string name, int? belgeno, int? companyId, string servisi, string konu, int? kanalId, bool? onayDurumu)
            {
                return dal.Get(u => (string.IsNullOrEmpty(name) ? true : u.createdUser.name.ToLower().Contains(name.ToLower())) && (belgeno.HasValue ? u.Id == belgeno : true) && (companyId.HasValue ? u.companyId == companyId : true) &&
                (string.IsNullOrEmpty(servisi) ? true : u.servisi.ToLower().Contains(servisi.ToLower())) && (string.IsNullOrEmpty(konu) ? true : u.konu.ToLower().Contains(konu.ToLower())) &&
                (kanalId.HasValue ? u.kanalId == kanalId.Value : true) && (onayDurumu.HasValue ? (onayDurumu.Value == true ? u.redEttiMi == true : u.onaylandiMi == true) : (u.onaylandiMi == true || u.redEttiMi == true)) && u.enabled == true && u.bittiMi == true).ToList();
            }

            public List<AskalePortal.Data.Models.DahiliYazismaTable> GetAllActiveForSuperUser()
            {
                return dal.Get(u => u.enabled == true && u.bittiMi == false && u.redEttiMi == false).ToList();
            }

            public int approvalCount(int userId)
            {
                BLLActions.DahiliYazismalarDetayTable bllDahiliYazismalarDetayTable = new BLLActions.DahiliYazismalarDetayTable(_configuration, _env);


                int count = bllDahiliYazismalarDetayTable.approvalCount(userId);
                return count;
            }

            public async Task<InternalCorrespondenceSaveDto> save(InternalCorrespondenceSaveDto entity, int userId)
            {


                if (entity.id == null)
                {

                    if (entity.onaylayici4 != null)
                    {
                        entity.birimAmiriId = entity.onaylayici4;
                    }
                    else if (entity.onaylayici3 != null)
                    {
                        entity.birimAmiriId = entity.onaylayici3;
                    }
                    else if (entity.onaylayici2 != null)
                    {
                        entity.birimAmiriId = entity.onaylayici2;
                    }
                    else if (entity.onaylayici1 != null)
                    {
                        entity.birimAmiriId = entity.onaylayici1;
                    }
                    else
                    {
                        entity.birimAmiriId = userId;
                    }
                    entity.createdUserId = userId;
                    entity.createdDate = DateTime.Now.ToString();
                    entity.bilgiBittiMi = false;
                    entity.bittiMi = false;
                    entity.kanalBittiMi = false;
                    entity.kanalGorusuOkmi = false;
                    entity.mudurBittiMi = false;
                    entity.onay1Ok = false;
                    entity.onay2Ok = false;
                    entity.onay3Ok = false;
                    entity.onay4Ok = false;
                    entity.onaylandiMi = false;
                    entity.redEttiMi = false;
                    Data.Models.DahiliYazismaTable? dahiliYazismaTableDto = _mapper.Map<Data.Models.DahiliYazismaTable>(entity);
                    Data.Models.DahiliYazismaTable? dahiliYazismaTable = await Add(dahiliYazismaTableDto);
                    BLLActions.BolumUserHierarchyTable bllBolumUserHierarchyTable = new BLLActions.BolumUserHierarchyTable(_configuration, _env);
                    Data.Models.BolumUserHierarchyTable? bolumUserHierarchyTable = dahiliYazismaTable?.kanalId == null ? null
                            : bllBolumUserHierarchyTable.GetByID(dahiliYazismaTable.kanalId ?? 0);
                    BLLActions.CeoTable bllCeoTable = new BLLActions.CeoTable(_configuration, _env);
                    Data.Models.CeoTable? ceoTable = bllCeoTable.GetByID(1);
                    BLLActions.AdminUsers bllAdminUsers = new BLLActions.AdminUsers(_configuration, _env, _mapper);
                    Data.Models.AdminUser? ceoUser = bllAdminUsers.GetByID(ceoTable!.userId);
                    UserByNameEMailDto? kanalUser = bolumUserHierarchyTable == null ? null
                            : bllAdminUsers.getUserByNameAndEmail(bolumUserHierarchyTable.userId);
                    if (dahiliYazismaTable?.onaylayici1 == null)
                    {
                        if (dahiliYazismaTable?.kanalId == null)
                        {

                            Data.Models.DahiliYazismalarDetayTable dahiliYazismalarDetayTable = new Data.Models.DahiliYazismalarDetayTable();
                            dahiliYazismalarDetayTable.createdDate = DateTime.Now;
                            dahiliYazismalarDetayTable.userId = ceoTable.userId;
                            dahiliYazismalarDetayTable.dahiliYazismaId = dahiliYazismaTable!.Id;

                            dahiliYazismalarDetayTable.enabled = true;

                            BLLActions.DahiliYazismalarDetayTable bllDahiliYazismalarDetayTable = new BLLActions.DahiliYazismalarDetayTable(_configuration, _env);
                            await bllDahiliYazismalarDetayTable.Add(dahiliYazismalarDetayTable);

                            //						EmailMessage emailMessage = new EmailMessage();
                            //						emailMessage.subject=dahiliYazismaTable.konu + " hk.");
                            //						emailMessage.toAddress=ceoUser.email;
                            //
                            //						String mailMessage = buildDahiliYazisma(dahiliYazismaTable);
                            //						emailMessage.emailText(mailMessage);
                            //						emailMessage.mailTuru=4;
                            //						emailMessage.enabled=true;
                            //						emailMessage.isSent=false;;
                            //						emailMessage.plannedDate=DateTime.Now;
                            //
                            //						bllEmailMessages.save(emailMessage, userId);
                            SMSMessage smsMessage = new SMSMessage();
                            smsMessage.plannedDate = DateTime.Now;
                            smsMessage.isSent = false;
                            smsMessage.smsText = (dahiliYazismaTable.Id.ToString() + " Id'li " + dahiliYazismaTable.konu
                                            + " konulu " + " Dahili Yazışma onayınızı beklemektedir.");
                            smsMessage.toNumbers = ceoUser?.mobile;

                            BLLActions.SMSMessages bllSMSMessage = new BLLActions.SMSMessages(_configuration, _env);
                            await bllSMSMessage.Add(smsMessage);
                        }
                        else
                        {

                            Data.Models.DahiliYazismalarDetayTable dahiliYazismalarDetayTable = new Data.Models.DahiliYazismalarDetayTable();
                            dahiliYazismalarDetayTable.createdDate = DateTime.Now;
                            dahiliYazismalarDetayTable.userId = bolumUserHierarchyTable!.userId;
                            dahiliYazismalarDetayTable.dahiliYazismaId = dahiliYazismaTable.Id;
                            dahiliYazismalarDetayTable.enabled = true;

                            BLLActions.DahiliYazismalarDetayTable bllDahiliYazismalarDetayTable = new BLLActions.DahiliYazismalarDetayTable(_configuration, _env);
                            await bllDahiliYazismalarDetayTable.Add(dahiliYazismalarDetayTable);

                            EmailMessage emailMessage = new EmailMessage();
                            emailMessage.subject = dahiliYazismaTable.konu + " hk.";
                            emailMessage.toAddress = kanalUser?.email;

                            string mailMessage = buildDahiliYazisma(dahiliYazismaTable) ?? "";
                            emailMessage.emailText = mailMessage;
                            emailMessage.mailTuru = 4;
                            emailMessage.enabled = true;
                            emailMessage.isSent = false;
                            emailMessage.plannedDate = DateTime.Now;

                            BLLActions.EmailMessages bllEmailMessages = new BLLActions.EmailMessages(_configuration, _env);
                            await bllEmailMessages.Add(emailMessage);
                        }

                    }
                    else
                    {
                        UserByNameEMailDto nextUser = bllAdminUsers
                                .getUserByNameAndEmail(dahiliYazismaTable.onaylayici1 ?? 0);
                        Data.Models.DahiliYazismalarDetayTable dahiliYazismalarDetayTable = new Data.Models.DahiliYazismalarDetayTable();
                        dahiliYazismalarDetayTable.createdDate = DateTime.Now;
                        dahiliYazismalarDetayTable.userId = dahiliYazismaTable.onaylayici1 ?? 0;
                        dahiliYazismalarDetayTable.dahiliYazismaId = dahiliYazismaTable.Id;
                        dahiliYazismalarDetayTable.enabled = true;
                        BLLActions.DahiliYazismalarDetayTable bllDahiliYazismalarDetayTable = new BLLActions.DahiliYazismalarDetayTable(_configuration, _env);
                        await bllDahiliYazismalarDetayTable.Add(dahiliYazismalarDetayTable);
                        EmailMessage emailMessage = new EmailMessage();
                        emailMessage.subject = dahiliYazismaTable.konu + " hk.";
                        emailMessage.toAddress = nextUser.email;

                        string mailMessage = buildDahiliYazisma(dahiliYazismaTable) ?? "";
                        emailMessage.emailText = mailMessage;
                        emailMessage.mailTuru = 4;
                        emailMessage.enabled = true;
                        emailMessage.isSent = false;
                        emailMessage.plannedDate = DateTime.Now;

                        BLLActions.EmailMessages bllEmailMessages = new BLLActions.EmailMessages(_configuration, _env);
                        await bllEmailMessages.Add(emailMessage);
                    }
                    return _mapper.Map<InternalCorrespondenceSaveDto>(dahiliYazismaTable);
                }
                else
                {
                    entity.updatedUserId = userId;
                    entity.updateDate = DateTime.Now.ToString();
                    Data.Models.DahiliYazismaTable updateDto = await Update(_mapper.Map<Data.Models.DahiliYazismaTable>(entity));
                    return _mapper.Map<InternalCorrespondenceSaveDto>(updateDto);
                }


            }

            public string? buildDahiliYazisma(Data.Models.DahiliYazismaTable dahiliYazismaTable)
            {
                string mailstring = "<link rel='stylesheet' href='//netdna.bootstrapcdn.com/bootstrap/3.0.0/css/bootstrap.min.css'>"
                        + "<div>" + "<script src='//netdna.bootstrapcdn.com/bootstrap/3.0.0/js/bootstrap.min.js'></script>" +

                        "<div class='form-group'>" + "<label class='col-sm-3'>Servisi:</label>"
                        + "<div class='col-sm-9 vcenter-form'>" + "<strong>" + dahiliYazismaTable.servisi + "</strong>"
                        + "</div>" + "</div>" + "<div class='form-group'>" + "<label class='col-sm-3'>Konu:</label>"
                        + "<div class='col-sm-9 vcenter-form'>" + "<strong>" + dahiliYazismaTable.konu + "</strong>"
                        + "</div>" + "</div>" + "<div class='form-group'>" + "<label class='col-sm-3'>Tarih:</label>"
                        + "<div class='col-sm-9 vcenter-form'>" + "<strong>"
                        + (dahiliYazismaTable.tarih ?? DateTime.Now).ToString("dd.MM.yyyy") + " </strong>" + "</div>" + "</div>" +

                        "<div class='form-group'>" + "<label class='col-sm-3'>Sayı:</label>"
                        + "<div class='col-sm-9 vcenter-form'>" + "<strong>" + dahiliYazismaTable.Id.ToString()
                        + "</strong>" + "</div>" + "</div>" +

                        "<div class='form-group'>" + "<label class='col-sm-3'>Kanal:</label>"
                        + "<div class='col-sm-9 vcenter-form'>" + "<strong>" + dahiliYazismaTable.kanalGorusu + "</strong>"
                        + "</div>" + "</div>" + "<div class='form-group'>" +

                        "<div class='col-sm-9 vcenter-form'>" + dahiliYazismaTable.icerik + "</div>" + "</div>" +

                        "<div class='form-group'>" + "<label class='col-sm-3 control-label no-padding-right'></label>"
                        + "<div class='col-sm-9 vcenter-form'>";

                mailstring += "</div></div>";

                return mailstring;
            }

            public PageReturn<InternalCorrespondenceDto>? list(FilterPageParam<InternalCorrespondenceListParameterDto> filterPageParam)
            {
                PageReturn<InternalCorrespondenceDto>? result = new PageReturn<InternalCorrespondenceDto>();
                int pageSize = filterPageParam.size ?? 20;
                int pageNumber = filterPageParam.page ?? 0;

                int? id = filterPageParam?.liste?.id;
                int? companyId = filterPageParam?.liste?.companyId;
                string? servisi = filterPageParam?.liste?.servisi;
                string? konu = filterPageParam?.liste?.konu;
                bool? bittiMi = filterPageParam?.liste?.bittimi;
                bool? redEttiMi = filterPageParam?.liste?.redEttiMi;
                int userId = filterPageParam?.liste?.userId ?? 0;
                BLLActions.AdminUsers bllAdminUsers = new BLLActions.AdminUsers(_configuration, _env, _mapper);
                AdminUser? user = bllAdminUsers.GetByID(userId);
                if (user != null)
                {
                    BLLActions.RoleDetails bllRoleDetails = new BLLActions.RoleDetails(_configuration, _env, _mapper);
                    RoleDetail? roleDetail = bllRoleDetails.GetByRoleIDAndModuleID(user.roleId, (int)CommonConstants.MODULES.DAHILIYAZISMA);
                    if (user.roleId == 1)
                    {


                        var query =
    from u in dal.dB.DahiliYazismaTable
    join nu in dal.dB.AdminUser
        on u.noteUserId equals nu.Id into noteUserJoin
    from nu in noteUserJoin.DefaultIfEmpty()
    where
        u.enabled &&
        (id == null || u.Id == id) &&
        (string.IsNullOrEmpty(konu) || u.konu.Contains(konu)) &&
        (companyId == null || companyId == 0 || u.companyId == companyId) &&
        (string.IsNullOrEmpty(servisi) || u.servisi.Contains(servisi)) &&
        (redEttiMi == null || u.redEttiMi == redEttiMi) &&
        (bittiMi == null || u.bittiMi == bittiMi)
    orderby u.Id descending
    select new
    {
        u,
        NoteUserName = nu != null ? nu.name : ""
    };
                        result.content = query
    .Skip(pageSize * pageNumber)
    .Take(pageSize)
    .Select(x => new InternalCorrespondenceDto
    {
        id = x.u.Id,
        companyName = x.u.company.vtext,
        servisi = x.u.servisi,
        createdDate = x.u.createdDate,
        lastApproveName = "",
        note = x.u.note,
        noteUserName = x.NoteUserName,
        createdUser = x.u.createdUser.name,
        createdUserId = x.u.createdUserId,
        kanal = x.u.kanal.bolumAdi,
        konu = x.u.konu,
        onay1Ok = x.u.onay1Ok,
        status = x.u.redEttiMi
    })
    .ToList();


                        result.totalElements = query.Count();
                        result.number = result.content.Count();
                        result.size = pageSize;

                        return result;
                    }
                    else if (roleDetail != null && roleDetail.canSeeLogs)
                    {
                        BLLActions.Roles bllRoles = new BLLActions.Roles(_configuration, _env, _mapper);
                        Role? role = bllRoles.GetByID(user.roleId);
                        string[] listCompanyIds = role?.companies.Replace("[", "").Replace("]", "").Split(",") ?? [];
                        List<int> listCompanyIdsint = new List<int>();
                        foreach (string ids in listCompanyIds)
                        {
                            BLLActions.Companies bllCompanies = new BLLActions.Companies(_configuration, _env, _mapper);
                            Company company = bllCompanies.getByVkorgCompany(ids);
                            listCompanyIdsint.Add(company.Id);
                        }
                        var query =
    from u in dal.dB.DahiliYazismaTable
    join nu in dal.dB.AdminUser
        on u.noteUserId equals nu.Id into noteUserJoin
    from nu in noteUserJoin.DefaultIfEmpty()
    where
        u.enabled &&
        (id == null || u.Id == id) &&
        (string.IsNullOrEmpty(konu) || u.konu.Contains(konu)) &&
        ((companyId == null || companyId == 0) || u.companyId == companyId) &&
        (string.IsNullOrEmpty(servisi) || u.servisi.Contains(servisi)) &&
        (redEttiMi == null || u.redEttiMi == redEttiMi) &&
        (bittiMi == null || u.bittiMi == bittiMi) &&
        listCompanyIdsint.Contains(u.companyId)
    orderby u.Id descending
    select new
    {
        u,
        NoteUserName = nu != null ? nu.name : ""
    };

                        result.content = query
    .Skip(pageSize * pageNumber)
    .Take(pageSize)
    .Select(x => new InternalCorrespondenceDto
    {
        id = x.u.Id,
        companyName = x.u.company.vtext,
        servisi = x.u.servisi,
        createdDate = x.u.createdDate,
        lastApproveName = "",
        note = x.u.note,
        noteUserName = x.NoteUserName,
        createdUser = x.u.createdUser.name,
        createdUserId = x.u.createdUserId,
        kanal = x.u.kanal.bolumAdi,
        konu = x.u.konu,
        onay1Ok = x.u.onay1Ok,
        status = x.u.redEttiMi
    })
    .ToList();
                        result.totalElements = query.Count();
                        result.number = result.content.Count();
                        result.size = pageSize;

                        return result;

                    }
                    else
                    {

                        var query =
    from u in dal.dB.DahiliYazismaTable
    join nu in dal.dB.AdminUser
        on u.noteUserId equals nu.Id into noteUserJoin
    from nu in noteUserJoin.DefaultIfEmpty()
    where
        u.enabled &&
        (id == null || u.Id == id) &&
        (string.IsNullOrEmpty(konu) || u.konu.Contains(konu)) &&
        ((companyId == null || companyId == 0) || u.companyId == companyId) &&
        (string.IsNullOrEmpty(servisi) || u.servisi.Contains(servisi)) &&
        (redEttiMi == null || u.redEttiMi == redEttiMi) &&
        (bittiMi == null || u.bittiMi == bittiMi) &&
        (
            u.lastUserId == userId ||
            u.lastUserId2 == userId ||
            u.createdUserId == userId ||
            u.onaylayici1 == userId ||
            u.onaylayici2 == userId ||
            u.onaylayici3 == userId ||
            u.onaylayici4 == userId ||
            u.kanalGorusuUserId == userId ||
            u.kanal.userId == userId
        )
    orderby u.Id descending
    select new
    {
        u,
        NoteUserName = nu != null ? nu.name : ""
    };

                        result.content = query
    .Skip(pageSize * pageNumber)
    .Take(pageSize)
    .Select(x => new InternalCorrespondenceDto
    {
        id = x.u.Id,
        companyName = x.u.company.vtext,
        servisi = x.u.servisi,
        createdDate = x.u.createdDate,
        lastApproveName = "",
        note = x.u.note,
        noteUserName = x.NoteUserName,
        createdUser = x.u.createdUser.name,
        createdUserId = x.u.createdUserId,
        kanal = x.u.kanal.bolumAdi,
        konu = x.u.konu,
        onay1Ok = x.u.onay1Ok,
        status = x.u.redEttiMi
    })
    .ToList();
                        //          IQueryable<Data.Models.DahiliYazismaTable> query = dal.Get(u => u.enabled
                        //&& id == null ? true : u.Id == id
                        //&& (konu == null || konu == "" ? true : u.konu.Contains(konu))
                        //&& ((companyId == null || companyId == 0) ? true : u.companyId == companyId)
                        //&& (servisi == null || servisi == "" ? true : u.servisi.Contains(servisi))
                        //&& (redEttiMi == null || u.redEttiMi == redEttiMi)
                        //&& (bittiMi == null || u.bittiMi == bittiMi)
                        //&& (u.lastUserId == userId || u.lastUserId2 ==userId || u.createdUserId ==userId || u.onaylayici1 ==userId || u.onaylayici2 ==userId || u.onaylayici3 ==userId || u.onaylayici4 ==userId ||  u.kanalGorusuUserId ==userId ||  u.kanal.userId ==userId) 

                        //);
                        //          result.content = query
                        //            .Skip(pageSize * pageNumber).Take(pageSize)

                        //              .Select(u => new InternalCorrespondenceDto()
                        //              {
                        //                  id = u.Id,
                        //                  companyName = u.company.vtext,
                        //                  servisi = u.servisi,
                        //                  createdDate = u.createdDate,
                        //                  lastApproveName="",
                        //                  note=u.note,
                        //                  noteUserName=u.noteUser.name,
                        //                  createdUser = u.createdUser.name,
                        //                  createdUserId = u.createdUserId,
                        //                  kanal = u.kanal.bolumAdi,
                        //                  konu = u.konu,
                        //                  onay1Ok = u.onay1Ok,
                        //                  status = u.redEttiMi,



                        //              }).ToList();
                        //          result.totalElements = query.Count();
                        //          result.number = result.content.Count();
                        //          result.size = pageSize;
                        if (bittiMi == false)
                        {
                            foreach (var dto in result.content)
                            {
                                BLLActions.DahiliYazismalarDetayTable bllDahiliYazismalarDetayTable = new BLLActions.DahiliYazismalarDetayTable(_configuration, _env);
                                List<string> names = bllDahiliYazismalarDetayTable.getByLastUserName(dto.id);
                                string deger = (names != null && names.Count > 0 && names[0] != null) ? names[0] : "";
                                dto.lastApproveName = deger;

                            }
                        }
                        result.totalElements = query.Count();
                        result.number = result.content.Count();
                        result.size = pageSize;

                        return result;
                    }
                }
                else
                {
                    return null;
                }
            }

            public InternalCorrespondenceDetailDto? getDetail(InternalCorrespondenceDto internalCorrespondenceDto, int userId)
            {
                BLLActions.AdminUsers bllAdminUsers = new BLLActions.AdminUsers(_configuration, _env, _mapper);
                AdminUser user = bllAdminUsers.GetByID(userId)!;
                BLLActions.AuditorTable bllAuditorTable = new BLLActions.AuditorTable(_configuration, _env);
                List<Data.Models.AuditorTable> listAuditorTables = bllAuditorTable.listAllByEnabled(true);
                BLLActions.CeoTable bllCeoTable = new BLLActions.CeoTable(_configuration, _env);
                Data.Models.CeoTable ceoTable = bllCeoTable.GetByID(1)!;

                Data.Models.DahiliYazismaTable? dahiliYazisma = GetByID(internalCorrespondenceDto.id ?? 0);
                if (dahiliYazisma != null)
                {
                    BLLActions.Companies bllCompanies = new BLLActions.Companies(_configuration, _env, _mapper);
                    Company? company = bllCompanies.GetByID(dahiliYazisma.companyId);

                    BLLActions.DahiliYazismalarDetayTable bllDahiliYazismalarDetayTable = new BLLActions.DahiliYazismalarDetayTable(_configuration, _env);
                    List<Data.Models.DahiliYazismalarDetayTable> listDahiliYazismalarDetayTables = bllDahiliYazismalarDetayTable
                            .findAllByEnabledAndDahiliYazismaId(true, internalCorrespondenceDto.id);
                    InternalCorrespondenceDetailDto correspondenceDetailDto = new InternalCorrespondenceDetailDto();

                    BLLActions.AttachedFiles bllAttachedFiles = new BLLActions.AttachedFiles(_configuration, _env);
                    List<AttachedFile> listAttachedFiles = bllAttachedFiles
                            .getByModuleIdAndTargetId((int)CommonConstants.MODULES.DAHILIYAZISMA, dahiliYazisma.Id);

                    correspondenceDetailDto.id = internalCorrespondenceDto.id;
                    correspondenceDetailDto.companyName = internalCorrespondenceDto.companyName;
                    correspondenceDetailDto.createdDate = dahiliYazisma.tarih;
                    correspondenceDetailDto.createdUser = internalCorrespondenceDto.createdUser;
                    correspondenceDetailDto.konu = internalCorrespondenceDto.konu;
                    correspondenceDetailDto.servisi = internalCorrespondenceDto.servisi;
                    correspondenceDetailDto.icerik = dahiliYazisma.icerik;
                    correspondenceDetailDto.kanal = internalCorrespondenceDto.kanal;
                    correspondenceDetailDto.companyTitle = company?.companyTitle;
                    correspondenceDetailDto.companyName = company?.companyLongName;
                    correspondenceDetailDto.listAttachedFile = listAttachedFiles;
                    if (dahiliYazisma.noteUserId != null)
                    {
                        correspondenceDetailDto.note = dahiliYazisma.note;
                        AdminUser? noteUser = bllAdminUsers.GetByID(dahiliYazisma.noteUserId ?? 0);
                        correspondenceDetailDto.noteUserName = noteUser?.name;
                    }

                    List<OnaylayiciDto> listOnaylayiciDtos = new List<OnaylayiciDto>();
                    if (dahiliYazisma.onaylayici1 != null)
                    {
                        listOnaylayiciDtos.Add(getOnaylayiciDto(dahiliYazisma.onaylayici1 ?? 0, dahiliYazisma,
                                listDahiliYazismalarDetayTables, false, ceoTable));
                    }
                    if (dahiliYazisma.onaylayici2 != null)
                    {
                        listOnaylayiciDtos.Add(getOnaylayiciDto(dahiliYazisma.onaylayici2 ?? 0, dahiliYazisma,
                                listDahiliYazismalarDetayTables, false, ceoTable));
                    }
                    if (dahiliYazisma.onaylayici3 != null)
                    {
                        listOnaylayiciDtos.Add(getOnaylayiciDto(dahiliYazisma.onaylayici3 ?? 0, dahiliYazisma,
                                listDahiliYazismalarDetayTables, false, ceoTable));
                    }
                    if (dahiliYazisma.onaylayici4 != null)
                    {
                        listOnaylayiciDtos.Add(getOnaylayiciDto(dahiliYazisma.onaylayici4 ?? 0, dahiliYazisma,
                                listDahiliYazismalarDetayTables, false, ceoTable));
                    }
                    if (dahiliYazisma.kanalId != null)
                    {
                        BLLActions.BolumUserHierarchyTable bllBolumUserHierarchyTable = new BLLActions.BolumUserHierarchyTable(_configuration, _env);
                        Data.Models.BolumUserHierarchyTable? bolumUserHierarchyTable = bllBolumUserHierarchyTable
                                .GetByID(dahiliYazisma.kanalId ?? 0);
                        if (bolumUserHierarchyTable != null)
                        {
                            listOnaylayiciDtos.Add(getOnaylayiciDto(bolumUserHierarchyTable.userId, dahiliYazisma,
                                               listDahiliYazismalarDetayTables, false, ceoTable));
                        }

                    }
                    listOnaylayiciDtos.Add(getOnaylayiciDto(ceoTable.userId, dahiliYazisma, listDahiliYazismalarDetayTables,
                            false, ceoTable));
                    if (dahiliYazisma.kanalGorusuUserId != null && dahiliYazisma.kanalGorusuUserId != 0)
                    {
                        listOnaylayiciDtos.Add(getOnaylayiciDto(dahiliYazisma.kanalGorusuUserId ?? 0, dahiliYazisma,
                                listDahiliYazismalarDetayTables, true, ceoTable));
                    }
                    if (dahiliYazisma.lastUserId != null)
                    {
                        listOnaylayiciDtos.Add(getOnaylayiciDto(dahiliYazisma.lastUserId ?? 0, dahiliYazisma,
                                listDahiliYazismalarDetayTables, true, ceoTable));
                    }
                    if (dahiliYazisma.lastUserId2 != null)
                    {
                        listOnaylayiciDtos.Add(getOnaylayiciDto(dahiliYazisma.lastUserId2 ?? 0, dahiliYazisma,
                                listDahiliYazismalarDetayTables, true, ceoTable));
                    }
                    correspondenceDetailDto.listOnayDurumu = listOnaylayiciDtos;
                    List<InternalCorrespondenceMessageDto> listMessageDtos = getMessageDtos(dahiliYazisma.Id, user, ceoTable,
                            listAuditorTables);
                    correspondenceDetailDto.listOnayDurumu = listOnaylayiciDtos;
                    correspondenceDetailDto.listInternalCorrespondenceMessageDtos = listMessageDtos;
                    return correspondenceDetailDto;
                }
                else
                {
                    return null;
                }
            }

            private List<InternalCorrespondenceMessageDto> getMessageDtos(int dahiliYazismaId, AdminUser user, Data.Models.CeoTable ceoTable,
            List<Data.Models.AuditorTable> listAuditorTables)
            {

                List<InternalCorrespondenceMessageDto> listeCorrespondenceMessageDtos = new List<InternalCorrespondenceMessageDto>();
                BLLActions.DahiliYazismaMessage bllDahiliYazismaMessage = new BLLActions.DahiliYazismaMessage(_configuration, _env);
                List<Data.Models.DahiliYazismaMessage> listDahiliYazismaMessage = bllDahiliYazismaMessage
                        .GetAllById(dahiliYazismaId);
                bool hasAutditor = listAuditorTables.Any(u => u.userId.Equals(user.Id));
                bool hasCeo = ceoTable.userId == user.Id;
                foreach (Data.Models.DahiliYazismaMessage dahiliYazismaMessage in listDahiliYazismaMessage)
                {
                    bool hasMessageUser = false;
                    if (dahiliYazismaMessage.sendUserId != null)
                    {
                        hasMessageUser = dahiliYazismaMessage.sendUserId.Equals(user.Id);
                    }

                    if (dahiliYazismaMessage.showAll || hasAutditor || hasCeo || hasMessageUser || user.roleId == 1)
                    {
                        BLLActions.AdminUsers bllAdminUsers = new BLLActions.AdminUsers(_configuration, _env, _mapper);
                        UserByNameEMailDto userByNameEMailDto = bllAdminUsers
                                .getUserByNameAndEmail(dahiliYazismaMessage.userId);
                        InternalCorrespondenceMessageDto correspondenceMessageDto = new InternalCorrespondenceMessageDto();
                        correspondenceMessageDto.id = dahiliYazismaMessage.Id;
                        correspondenceMessageDto.message = dahiliYazismaMessage.message;
                        correspondenceMessageDto.time = dahiliYazismaMessage.createdDate.ToString("dd.MM.yyyy hh:mm:ss");
                        correspondenceMessageDto.username = userByNameEMailDto.name;
                        listeCorrespondenceMessageDtos.Add(correspondenceMessageDto);
                    }
                }

                return listeCorrespondenceMessageDtos;

            }
            private OnaylayiciDto getOnaylayiciDto(int userId, Data.Models.DahiliYazismaTable dahiliYazisma,
            List<Data.Models.DahiliYazismalarDetayTable> listDahiliYazismalarDetayTables, bool before, Data.Models.CeoTable ceoTable)
            {

                BLLActions.AdminUsers bllAdminUsers = new BLLActions.AdminUsers(_configuration, _env, _mapper);
                AdminUser? user = bllAdminUsers.GetByID(userId);
                OnaylayiciDto onaylayiciDto = new OnaylayiciDto();

                if (user != null)
                {
                    string filename = user.imageUrl;

                    string directoryName = _env.IsDevelopment()
        ? _configuration["FilePath:local"]!
        : _env.IsProduction()
            ? _configuration["FilePath:server"]!
            : _configuration["FilePath:test"]! + "adminusers/images/";
                    string fullPath = Path.Combine(directoryName, filename);

                    List<int> listInt = new List<int>();


                    byte[] contentInBytes = File.ReadAllBytes(fullPath);

                    foreach (byte b in contentInBytes)
                    {
                        int byteSayi = b;
                        listInt.Add(byteSayi);
                    }


                    onaylayiciDto.userName = user.name;
                    bool onaylayiciVarMi;
                    if (Objects.Equals(userId, ceoTable.userId))
                    {
                        onaylayiciVarMi = listDahiliYazismalarDetayTables.Any(u => u.userId.Equals(userId));
                    }
                    else
                    {
                        onaylayiciVarMi = listDahiliYazismalarDetayTables.Any(u => u.userId.Equals(userId) && u.sonOnayMi == before);
                    }

                    onaylayiciDto.file = listInt;
                    if (onaylayiciVarMi)
                    {
                        Data.Models.DahiliYazismalarDetayTable dahiliYazismalarDetayTable;
                        if (userId.Equals(ceoTable.userId))
                        {
                            dahiliYazismalarDetayTable = listDahiliYazismalarDetayTables.First(u => u.userId == userId);
                        }
                        else
                        {
                            dahiliYazismalarDetayTable = listDahiliYazismalarDetayTables.First(u => Equals(u.userId, userId) && u.sonOnayMi == before);

                        }

                        if (dahiliYazismalarDetayTable.approved == null)
                        {
                            onaylayiciDto.onayDurumu = ("Onay Bekleniyor");
                            onaylayiciDto.durum = 3;
                        }
                        else if (dahiliYazismalarDetayTable.approved == true)
                        {
                            onaylayiciDto.onayDurumu = (
                                    "Onaylama Tarihi: " + dahiliYazismalarDetayTable.replyDate?.ToString("dd.MM.yyyy"));
                            onaylayiciDto.durum = 1;
                        }
                        else if (dahiliYazismalarDetayTable.approved == false)
                        {
                            onaylayiciDto.onayDurumu = (
                                    "Red Tarihi: " + dahiliYazismalarDetayTable.replyDate?.ToString("dd.MM.yyyy"));
                            onaylayiciDto.durum = 2;
                        }
                    }
                    else
                    {
                        onaylayiciDto.onayDurumu = ("");
                        onaylayiciDto.durum = 4;
                    }
                }

                return onaylayiciDto;
            }

            public int approvalKanalGorusuCount(int userId)
            {
                var count =
     (from a in dal.dB.DahiliYazismaTable
      join b in dal.dB.DahiliYazismalarDetayTable
          on a.Id equals b.dahiliYazismaId
      where a.onaylandiMi == true
            && a.kanalGorusuUserId == userId
            && a.kanalGorusuOkmi == false
            && a.redEttiMi == false
            && b.enabled == true
            && b.userId == userId
            && b.approved == null
            && a.enabled == true
      select a)
     .Count();

                return count;
            }

            public int kanalGorusuBitenCount(int userId)
            {
                var count = dal.Get(a => a.onaylandiMi == true && (
                     (a.lastUserId == userId && a.mudurBittiMi == false) || (a.lastUserId2 == userId && a.mudurBittiMi == true)
                   )
                && a.kanalGorusuOkmi == true
                && a.bittiMi == false
                && a.enabled == true).Count();
                return count;
            }

            public PageReturn<InternalCorrespondenceDto> listPageableBilgi(FilterPageParam<InternalCorrespondencePageableListBilgiDtoParameter> filterPageParam)
            {
                int? id = filterPageParam?.liste?.id;
                int? companyId = filterPageParam?.liste?.companyId;
                string? servisi = filterPageParam?.liste?.servisi;
                string? konu = filterPageParam?.liste?.konu;
                bool? bittiMi = filterPageParam?.liste?.bittiMi;
                bool? redEttiMi = filterPageParam?.liste?.redEttiMi;
                int? userId = filterPageParam?.liste?.userId;

                PageReturn<InternalCorrespondenceDto>? result = new PageReturn<InternalCorrespondenceDto>();
                int pageSize = filterPageParam?.size ?? 20;
                int pageNumber = filterPageParam?.page ?? 0;

                IQueryable<Data.Models.DahiliYazismaTable> query = dal.Get(u => u.enabled
               && id == null ? true : u.Id == id
               && (konu == null || konu == "" ? true : u.konu.Contains(konu))
               && ((companyId == null || companyId == 0) ? true : u.companyId == companyId)
               && (servisi == null || servisi == "" ? true : u.servisi.Contains(servisi))
               && (redEttiMi == null ? true : u.redEttiMi == redEttiMi)
               && u.bittiMi == bittiMi
               && (u.bilgiUserId1 == userId || u.bilgiUserId2 == userId || u.bilgiUserId3 == userId || u.bilgiUserId4 == userId || u.bilgiUserId5 == userId)
               && u.enabled);

                result.content = query.OrderByDescending(u => u.Id)
                  .Skip(pageSize * pageNumber).Take(pageSize)

                    .Select(u => new InternalCorrespondenceDto()
                    {

                        createdDate = u.createdDate,
                        createdUserId = u.createdUserId,
                        id = u.Id,
                        companyName = u.company.companyLongName,
                        createdUser = u.createdUser.name,
                        kanal = u.kanal.bolumAdi,
                        lastApproveName = "",
                        note = u.note,
                        noteUserName = "",
                        konu = u.konu,
                        servisi = u.servisi,

                    }).ToList();
                result.totalElements = query.Count();
                result.number = result.content.Count();
                result.size = pageSize;

                return result;


            }

            public PageReturn<InternalCorrespondenceDto> mylist(FilterPageParam<InternalCorrespondenceListParameterDto> filterPageParam)
            {
                int? id = filterPageParam?.liste?.id;
                int? companyId = filterPageParam?.liste?.companyId;
                string? servisi = filterPageParam?.liste?.servisi;
                string? konu = filterPageParam?.liste?.konu;
                bool? bittiMi = filterPageParam?.liste?.bittimi;
                bool? redEttiMi = filterPageParam?.liste?.redEttiMi;
                int? userId = filterPageParam?.liste?.userId;

                PageReturn<InternalCorrespondenceDto>? result = new PageReturn<InternalCorrespondenceDto>();
                int pageSize = filterPageParam?.size ?? 20;
                int pageNumber = filterPageParam?.page ?? 0;

                BLLActions.AuditorTable bllAuditorTable = new BLLActions.AuditorTable(_configuration, _env);
                List<Data.Models.AuditorTable> listAuditorTable = bllAuditorTable.listAllByEnabled(true);

                if (listAuditorTable.Any(u => u.userId == userId))
                {
                    var query =
     from a in dal.dB.DahiliYazismaTable
     join c in dal.dB.Company
         on a.companyId equals c.Id
     join d in dal.dB.AdminUser
         on a.createdUserId equals d.Id
     join f in dal.dB.AdminUser
         on a.noteUserId equals f.Id into noteUserJoin
     from f in noteUserJoin.DefaultIfEmpty()
     join b in dal.dB.BolumUserHierarchyTable
         on a.kanalId equals b.Id into kanalJoin
     from b in kanalJoin.DefaultIfEmpty()
     where
         a.enabled &&
         (id == null || a.Id == id) &&
         (string.IsNullOrEmpty(konu) || a.konu.Contains(konu)) &&
         (companyId == null || a.companyId == companyId) &&
         (string.IsNullOrEmpty(servisi) || a.servisi == servisi) &&
         (redEttiMi == null || a.redEttiMi == redEttiMi) &&
         (bittiMi == null || a.bittiMi == bittiMi)
     orderby a.Id descending
     select new InternalCorrespondenceDto
     {
         id = a.Id,
         companyName = c.vtext,
         servisi = a.servisi,
         konu = a.konu,
         createdDate = a.tarih,
         kanal = b != null ? b.bolumAdi : "",
         createdUser = d.name,
         status = a.redEttiMi,
         createdUserId = a.createdUserId,
         onay1Ok = a.onay1Ok,
         note = a.note,
         noteUserName = f != null ? f.name : ""

     };
                    result.totalElements = query.Count();

                    result.content = query
                        .Skip(pageSize * pageNumber)
                        .Take(pageSize)
                        .ToList();

                    result.number = pageNumber;
                    result.size = pageSize;
                    return result;
                }
                else
                {
                    var query =
       from a in dal.dB.DahiliYazismaTable

       join c in dal.dB.Company
           on a.companyId equals c.Id

       join d in dal.dB.AdminUser
           on a.createdUserId equals d.Id

       join b in dal.dB.DahiliYazismalarDetayTable
           on a.Id equals b.dahiliYazismaId into detayJoin
       from b in detayJoin.DefaultIfEmpty()

       join f in dal.dB.AdminUser
           on a.noteUserId equals f.Id into noteUserJoin
       from f in noteUserJoin.DefaultIfEmpty()

       join e in dal.dB.BolumUserHierarchyTable
           on a.kanalId equals e.Id into bolumJoin
       from e in bolumJoin.DefaultIfEmpty()

       where
            ((a.Id == id) || (id == null)) &&
            b.approved == null && b.enabled && b.userId == userId && a.onaylandiMi == false &&
            ((konu == "" && a.konu == null) || a.konu.Contains(konu ?? "")) &&
            (a.companyId == companyId || (companyId == null)) &&
            ((servisi == "" || servisi == null) || a.servisi.Contains(servisi ?? "")) &&
            (a.redEttiMi == redEttiMi || redEttiMi == null) &&
            (a.bittiMi == bittiMi) &&
            a.enabled

       orderby a.Id descending

       select new InternalCorrespondenceDto
       {
           id = a.Id,
           companyName = c.vtext,
           servisi = a.servisi,
           konu = a.konu,
           createdDate = a.tarih,
           kanal = e != null ? e.bolumAdi : "",
           createdUser = d.name,
           status = a.redEttiMi,
           createdUserId = a.createdUserId,
           onay1Ok = a.onay1Ok,
           note = a.note,
           noteUserName = f != null ? f.name : ""
       };

                    result.totalElements = query.Count();

                    result.content = query
                        .Skip(pageSize * pageNumber)
                        .Take(pageSize)
                        .ToList();

                    result.number = pageNumber;
                    result.size = pageSize;
                    return result;
                }
            }

            public PageReturn<InternalCorrespondenceDto> mylistcanal(FilterPageParam<InternalCorrespondencePageableListBilgiDtoParameter> filterPageParam)
            {

                int? id = filterPageParam?.liste?.id;
                int? companyId = filterPageParam?.liste?.companyId;
                string? servisi = filterPageParam?.liste?.servisi;
                string? konu = filterPageParam?.liste?.konu;
                bool? bittiMi = filterPageParam?.liste?.bittiMi;
                bool? redEttiMi = filterPageParam?.liste?.redEttiMi;
                int? userId = filterPageParam?.liste?.userId;

                PageReturn<InternalCorrespondenceDto>? result = new PageReturn<InternalCorrespondenceDto>();
                int pageSize = filterPageParam?.size ?? 20;
                int pageNumber = filterPageParam?.page ?? 0;

                BLLActions.AdminUsers bllAdminUsers = new BLLActions.AdminUsers(_configuration, _env, _mapper);

                AdminUser? user = bllAdminUsers.GetByID(userId ?? 0);

                if (user != null && user.roleId == 1)
                {
                    var query =
    from a in dal.dB.DahiliYazismaTable
    join c in dal.dB.Company
        on a.companyId equals c.Id
    join d in dal.dB.AdminUser
        on a.createdUserId equals d.Id
    join f in dal.dB.AdminUser
        on a.noteUserId equals f.Id into noteUserJoin
    from f in noteUserJoin.DefaultIfEmpty() // LEFT JOIN User f
    join e in dal.dB.BolumUserHierarchyTable
        on a.kanalId equals e.Id into bolumJoin
    from e in bolumJoin.DefaultIfEmpty() // LEFT JOIN Bolum
    where
        a.enabled &&
        a.onaylandiMi == true &&
        a.kanalGorusuOkmi == false &&
        (id == null || a.Id == id) &&
        (string.IsNullOrEmpty(konu) || a.konu.Contains(konu)) &&
        (companyId == null || a.companyId == companyId) &&
        (string.IsNullOrEmpty(servisi) || a.servisi == servisi) &&
        (redEttiMi == null || a.redEttiMi == redEttiMi) &&
        (bittiMi == null || a.bittiMi == bittiMi)
    orderby a.Id descending
    select new InternalCorrespondenceDto
    {
        id = a.Id,
        companyName = c.vtext,
        servisi = a.servisi,
        konu = a.konu,
        createdDate = a.tarih,
        kanal = e != null ? e.bolumAdi : "",
        createdUser = d.name,
        status = a.redEttiMi,
        createdUserId = a.createdUserId,
        onay1Ok = a.onay1Ok,
        note = a.note,
        noteUserName = f != null ? f.name : ""
    };
                    result.totalElements = query.Count();

                    result.content = query
                        .Skip(pageSize * pageNumber)
                        .Take(pageSize)
                        .ToList();

                    result.number = pageNumber;
                    result.size = pageSize;
                    return result;
                }
                else
                {
                    var query =
    from a in dal.dB.DahiliYazismaTable
    join c in dal.dB.Company
        on a.companyId equals c.Id
    join d in dal.dB.AdminUser
        on a.createdUserId equals d.Id
    join f in dal.dB.AdminUser
        on a.noteUserId equals f.Id into noteUserJoin
    from f in noteUserJoin.DefaultIfEmpty() // LEFT JOIN User f
    join e in dal.dB.BolumUserHierarchyTable
        on a.kanalId equals e.Id into bolumJoin
    from e in bolumJoin.DefaultIfEmpty() // LEFT JOIN Bolum
    where
        a.enabled &&
        a.onaylandiMi == true &&
        a.kanalGorusuOkmi == false &&
        a.kanalGorusuUserId == userId &&
        (id == null || a.Id == id) &&
        (string.IsNullOrEmpty(konu) || a.konu.Contains(konu)) &&
        (companyId == null || a.companyId == companyId) &&
        (string.IsNullOrEmpty(servisi) || a.servisi == servisi) &&
        (redEttiMi == null || a.redEttiMi == redEttiMi) &&
        (bittiMi == null || a.bittiMi == bittiMi)
    orderby a.Id descending
    select new InternalCorrespondenceDto
    {
        id = a.Id,
        companyName = c.vtext,
        servisi = a.servisi,
        konu = a.konu,
        createdDate = a.tarih,
        kanal = e != null ? e.bolumAdi : "",
        createdUser = d.name,
        status = a.redEttiMi,
        createdUserId = a.createdUserId,
        onay1Ok = a.onay1Ok,
        note = a.note,
        noteUserName = f != null ? f.name : ""
    };
                    result.totalElements = query.Count();

                    result.content = query
                        .Skip(pageSize * pageNumber)
                        .Take(pageSize)
                        .ToList();

                    result.number = pageNumber;
                    result.size = pageSize;
                    return result;
                }
            }

            public async Task<int> approve(ResponseMyList responseMyList, int userId)
            {
                try
                {
                    InternalCorrespondenceSaveDto? dahiliYazismaTable = responseMyList.dahiliYazismaTable;

                    BLLActions.AdminUsers bllAdminUsers = new BLLActions.AdminUsers(_configuration, _env, _mapper);
                    AdminUser user = bllAdminUsers.GetByID(userId)!;
                    if (dahiliYazismaTable != null)
                    {
                        AdminUser? onaylayici2 = dahiliYazismaTable.onaylayici2 != null
                                ? bllAdminUsers.GetByID(dahiliYazismaTable.onaylayici2 ?? 0)
                                : null;
                        AdminUser? onaylayici3 = dahiliYazismaTable.onaylayici3 != null
                                ? bllAdminUsers.GetByID(dahiliYazismaTable.onaylayici3 ?? 0)
                                : null;
                        AdminUser? onaylayici4 = dahiliYazismaTable.onaylayici4 != null
                                ? bllAdminUsers.GetByID(dahiliYazismaTable.onaylayici4 ?? 0)
                                : null;
                        BLLActions.BolumUserHierarchyTable bllBolumUserHierarchyTable = new BLLActions.BolumUserHierarchyTable(_configuration, _env);
                        Data.Models.BolumUserHierarchyTable? bolumUserHierarchyTable = dahiliYazismaTable.kanalId == null ? null
                                : bllBolumUserHierarchyTable.GetByID(dahiliYazismaTable.kanalId ?? 0);
                        AdminUser? kanalUser = bolumUserHierarchyTable != null ? bllAdminUsers.GetByID(bolumUserHierarchyTable.userId)
                                : null;
                        BLLActions.CeoTable bllCeoTable = new BLLActions.CeoTable(_configuration, _env);
                        Data.Models.CeoTable ceoTable = bllCeoTable.GetByID(1)!;
                        AdminUser ceoUser = bllAdminUsers.GetByID(ceoTable.userId)!;

                        if (dahiliYazismaTable.onaylandiMi != true)
                        {
                            if (dahiliYazismaTable.onaylayici1 != null && dahiliYazismaTable.onaylayici1 == user.Id && dahiliYazismaTable.onay1Ok != true)
                            {
                                dahiliYazismaTable.onay1Ok = true;


                                Data.Models.DahiliYazismaMessage dahiliYazismaMessage = new Data.Models.DahiliYazismaMessage();
                                dahiliYazismaMessage.createdDate = DateTime.Now;
                                dahiliYazismaMessage.showAll = true;
                                dahiliYazismaMessage.dahiliYazismaId = dahiliYazismaTable.id ?? 0;
                                dahiliYazismaMessage.userId = user.Id;
                                dahiliYazismaMessage.message = responseMyList.kanalGorusuFirst;
                                dahiliYazismaMessage.enabled = true;
                                BLLActions.DahiliYazismaMessage bllDahiliYazismaMessage = new BLLActions.DahiliYazismaMessage(_configuration, _env);
                                await bllDahiliYazismaMessage.Add(dahiliYazismaMessage);

                                BLLActions.DahiliYazismalarDetayTable bllDahiliYazismalarDetayTable = new BLLActions.DahiliYazismalarDetayTable(_configuration, _env);
                                List<Data.Models.DahiliYazismalarDetayTable> listDahiliYazismalarDetayTable = bllDahiliYazismalarDetayTable
                                        .findAllByDahiliYazismaIdAndApprovedAndUserIdAndEnabled(dahiliYazismaTable.id, null,
                                                user.Id, true);

                                for (int j = 1; j < listDahiliYazismalarDetayTable.Count(); j++)
                                {
                                    int id = listDahiliYazismalarDetayTable[j].Id;
                                    bllDahiliYazismalarDetayTable.Delete(id);
                                }
                                Data.Models.DahiliYazismalarDetayTable dahiliYazismalarDetayTable = listDahiliYazismalarDetayTable[0];
                                dahiliYazismalarDetayTable.isReplied = true;
                                dahiliYazismalarDetayTable.approved = true;
                                dahiliYazismalarDetayTable.replyDate = DateTime.Now;
                                await bllDahiliYazismalarDetayTable.Update(dahiliYazismalarDetayTable);
                                if (dahiliYazismaTable.onaylayici2 != null)
                                {
                                    Data.Models.DahiliYazismalarDetayTable dahiliYazismalarDetayTableNext = new Data.Models.DahiliYazismalarDetayTable();
                                    dahiliYazismalarDetayTableNext.userId = dahiliYazismaTable.onaylayici2 ?? 0;
                                    dahiliYazismalarDetayTableNext.createdDate = DateTime.Now;
                                    dahiliYazismalarDetayTableNext.approved = null;
                                    dahiliYazismalarDetayTableNext.dahiliYazismaId = dahiliYazismaTable.id ?? 0;
                                    dahiliYazismalarDetayTableNext.isReplied = false;
                                    dahiliYazismalarDetayTableNext.replyDate = null;
                                    dahiliYazismalarDetayTableNext.enabled = true;


                                    await bllDahiliYazismalarDetayTable.Add(dahiliYazismalarDetayTableNext);

                                    EmailMessage emailMessage = new EmailMessage();
                                    emailMessage.toAddress = onaylayici2!.email;
                                    emailMessage.subject = dahiliYazismaTable.konu + " hk.";
                                    emailMessage.isSent = false;
                                    emailMessage.emailText = buildDahiliYazisma(_mapper.Map<Data.Models.DahiliYazismaTable>(_mapper.Map<Data.Models.DahiliYazismaTable>(dahiliYazismaTable)));
                                    emailMessage.mailTuru = 4;
                                    emailMessage.plannedDate = DateTime.Now;
                                    emailMessage.enabled = true;
                                    BLLActions.EmailMessages bllEmailMessages = new BLLActions.EmailMessages(_configuration, _env);
                                    await bllEmailMessages.Add(emailMessage);
                                }
                                else if (bolumUserHierarchyTable != null && dahiliYazismaTable.kanalBittiMi != true)
                                {
                                    Data.Models.DahiliYazismalarDetayTable dahiliYazismalarDetayTableNext = new Data.Models.DahiliYazismalarDetayTable();
                                    dahiliYazismalarDetayTableNext.userId = bolumUserHierarchyTable.userId;
                                    dahiliYazismalarDetayTableNext.createdDate = DateTime.Now;
                                    dahiliYazismalarDetayTableNext.approved = null;
                                    dahiliYazismalarDetayTableNext.dahiliYazismaId = dahiliYazismaTable.id ?? 0;
                                    dahiliYazismalarDetayTableNext.isReplied = false;
                                    dahiliYazismalarDetayTableNext.replyDate = null;
                                    dahiliYazismalarDetayTableNext.enabled = true;

                                    await bllDahiliYazismalarDetayTable.Add(dahiliYazismalarDetayTableNext);
                                    EmailMessage emailMessage = new EmailMessage();
                                    emailMessage.toAddress = kanalUser?.email;
                                    emailMessage.subject = dahiliYazismaTable.konu + " hk.";
                                    emailMessage.isSent = false;
                                    emailMessage.emailText = buildDahiliYazisma(_mapper.Map<Data.Models.DahiliYazismaTable>(dahiliYazismaTable));
                                    emailMessage.mailTuru = 4;
                                    emailMessage.plannedDate = DateTime.Now;
                                    emailMessage.enabled = true;
                                    BLLActions.EmailMessages bllEmailMessages = new BLLActions.EmailMessages(_configuration, _env);
                                    await bllEmailMessages.Add(emailMessage);
                                }
                                else
                                {

                                    Data.Models.DahiliYazismalarDetayTable dahiliYazismalarDetayTableNext = new Data.Models.DahiliYazismalarDetayTable();
                                    dahiliYazismalarDetayTableNext.userId = ceoTable.userId;
                                    dahiliYazismalarDetayTableNext.createdDate = DateTime.Now;
                                    dahiliYazismalarDetayTableNext.approved = null;
                                    dahiliYazismalarDetayTableNext.dahiliYazismaId = dahiliYazismaTable.id ?? 0;
                                    dahiliYazismalarDetayTableNext.isReplied = false;
                                    dahiliYazismalarDetayTableNext.replyDate = null;
                                    dahiliYazismalarDetayTableNext.enabled = true;
                                    dahiliYazismalarDetayTableNext.sonOnayMi = true;

                                    await bllDahiliYazismalarDetayTable.Add(dahiliYazismalarDetayTableNext);
                                    //						EmailMessage emailMessage = new EmailMessage();
                                    //						emailMessage.toAddress=ceoUser.email;
                                    //						emailMessage.subject=dahiliYazismaTable.konu + " hk.");
                                    //						emailMessage.isSent=false;;
                                    //						emailMessage.emailText=buildDahiliYazisma(_mapper.Map<Data.Models.DahiliYazismaTable>(dahiliYazismaTable)));
                                    //						emailMessage.mailTuru=4;
                                    //						emailMessage.plannedDate=DateTime.Now;
                                    //						emailMessage.enabled=true;
                                    //						await bllEmailMessages.Add(emailMessage);

                                    SMSMessage smsMessage = new SMSMessage();
                                    smsMessage.plannedDate = DateTime.Now;
                                    smsMessage.isSent = false;
                                    smsMessage.smsText = (
                                            dahiliYazismaTable.id.ToString() + "Id'li" + dahiliYazismaTable.konu
                                                    + " konulu " + " Dahili Yazışma onayınızı beklemektedir.");
                                    smsMessage.toNumbers = ceoUser.mobile;

                                    BLLActions.SMSMessages bllSMSMessages = new BLLActions.SMSMessages(_configuration, _env);
                                    await bllSMSMessages.Add(smsMessage);
                                }
                                if (dahiliYazismaTable.id == null)
                                {
                                    await Add(_mapper.Map<Data.Models.DahiliYazismaTable>(dahiliYazismaTable));
                                }
                                else
                                {
                                    await Update(_mapper.Map<Data.Models.DahiliYazismaTable>(dahiliYazismaTable));
                                }
                                return 1;
                            }
                            else if (dahiliYazismaTable.onaylayici1 != null && dahiliYazismaTable.onay1Ok != true && dahiliYazismaTable.onaylayici1 != user.Id)
                            {
                                return 2;
                            }
                            else if (dahiliYazismaTable.onaylayici1 != null && dahiliYazismaTable.onay1Ok == true)
                            {
                                if (dahiliYazismaTable.onaylayici2 == user.Id && dahiliYazismaTable.onay2Ok != true)
                                {
                                    dahiliYazismaTable.onay2Ok = true;

                                    Data.Models.DahiliYazismaMessage dahiliYazismaMessage = new Data.Models.DahiliYazismaMessage();
                                    dahiliYazismaMessage.createdDate = DateTime.Now;
                                    dahiliYazismaMessage.showAll = true;
                                    dahiliYazismaMessage.userId = user.Id;
                                    dahiliYazismaMessage.message = responseMyList.kanalGorusuFirst;
                                    dahiliYazismaMessage.enabled = true;
                                    dahiliYazismaMessage.dahiliYazismaId = dahiliYazismaTable.id ?? 0;

                                    BLLActions.DahiliYazismaMessage bllDahiliYazismaMessage = new BLLActions.DahiliYazismaMessage(_configuration, _env);
                                    await bllDahiliYazismaMessage.Add(dahiliYazismaMessage);

                                    BLLActions.DahiliYazismalarDetayTable bllDahiliYazismalarDetayTable = new BLLActions.DahiliYazismalarDetayTable(_configuration, _env);
                                    List<Data.Models.DahiliYazismalarDetayTable> listDahiliYazismalarDetayTable = bllDahiliYazismalarDetayTable
                                            .findAllByDahiliYazismaIdAndApprovedAndUserIdAndEnabled(dahiliYazismaTable.id,
                                                    null, user.Id, true);

                                    for (int j = 1; j < listDahiliYazismalarDetayTable.Count(); j++)
                                    {
                                        int id = listDahiliYazismalarDetayTable[j].Id;
                                        bllDahiliYazismalarDetayTable.Delete(id);
                                    }
                                    Data.Models.DahiliYazismalarDetayTable dahiliYazismalarDetayTable = listDahiliYazismalarDetayTable[0];
                                    dahiliYazismalarDetayTable.isReplied = true;
                                    dahiliYazismalarDetayTable.approved = true;
                                    dahiliYazismalarDetayTable.replyDate = DateTime.Now;
                                    await bllDahiliYazismalarDetayTable.Update(dahiliYazismalarDetayTable);
                                    if (dahiliYazismaTable.onaylayici3 != null)
                                    {
                                        Data.Models.DahiliYazismalarDetayTable dahiliYazismalarDetayTableNext = new Data.Models.DahiliYazismalarDetayTable();
                                        dahiliYazismalarDetayTableNext.userId = dahiliYazismaTable.onaylayici3 ?? 0;
                                        dahiliYazismalarDetayTableNext.createdDate = DateTime.Now;
                                        dahiliYazismalarDetayTableNext.approved = null;
                                        dahiliYazismalarDetayTableNext.dahiliYazismaId = dahiliYazismaTable.id ?? 0;
                                        dahiliYazismalarDetayTableNext.isReplied = false;
                                        dahiliYazismalarDetayTableNext.replyDate = null;
                                        dahiliYazismalarDetayTableNext.enabled = true;

                                        await bllDahiliYazismalarDetayTable.Add(dahiliYazismalarDetayTableNext);

                                        EmailMessage emailMessage = new EmailMessage();
                                        emailMessage.toAddress = onaylayici3?.email;
                                        emailMessage.subject = dahiliYazismaTable.konu + " hk.";
                                        emailMessage.isSent = false;
                                        emailMessage.emailText = buildDahiliYazisma(_mapper.Map<Data.Models.DahiliYazismaTable>(dahiliYazismaTable));
                                        emailMessage.mailTuru = 4;
                                        emailMessage.plannedDate = DateTime.Now;
                                        emailMessage.enabled = true;
                                        BLLActions.EmailMessages bllEmailMessages = new BLLActions.EmailMessages(_configuration, _env);
                                        await bllEmailMessages.Add(emailMessage);
                                    }
                                    else if (bolumUserHierarchyTable != null && dahiliYazismaTable.kanalBittiMi != true)
                                    {
                                        Data.Models.DahiliYazismalarDetayTable dahiliYazismalarDetayTableNext = new Data.Models.DahiliYazismalarDetayTable();
                                        dahiliYazismalarDetayTableNext.userId = bolumUserHierarchyTable.userId;
                                        dahiliYazismalarDetayTableNext.createdDate = DateTime.Now;
                                        dahiliYazismalarDetayTableNext.approved = null;
                                        dahiliYazismalarDetayTableNext.dahiliYazismaId = dahiliYazismaTable.id ?? 0;
                                        dahiliYazismalarDetayTableNext.isReplied = false;
                                        dahiliYazismalarDetayTableNext.replyDate = null;
                                        dahiliYazismalarDetayTableNext.enabled = true;

                                        await bllDahiliYazismalarDetayTable.Add(dahiliYazismalarDetayTableNext);
                                        EmailMessage emailMessage = new EmailMessage();
                                        emailMessage.toAddress = kanalUser?.email;
                                        emailMessage.subject = dahiliYazismaTable.konu + " hk.";
                                        emailMessage.isSent = false;
                                        emailMessage.emailText = buildDahiliYazisma(_mapper.Map<Data.Models.DahiliYazismaTable>(dahiliYazismaTable));
                                        emailMessage.mailTuru = 4;
                                        emailMessage.plannedDate = DateTime.Now;
                                        emailMessage.enabled = true;
                                        BLLActions.EmailMessages bllEmailMessages = new BLLActions.EmailMessages(_configuration, _env);
                                        await bllEmailMessages.Add(emailMessage);
                                    }
                                    else
                                    {

                                        Data.Models.DahiliYazismalarDetayTable dahiliYazismalarDetayTableNext = new Data.Models.DahiliYazismalarDetayTable();
                                        dahiliYazismalarDetayTableNext.userId = ceoTable.userId;
                                        dahiliYazismalarDetayTableNext.createdDate = DateTime.Now;
                                        dahiliYazismalarDetayTableNext.approved = null;
                                        dahiliYazismalarDetayTableNext.dahiliYazismaId = dahiliYazismaTable.id ?? 0;
                                        dahiliYazismalarDetayTableNext.isReplied = false;
                                        dahiliYazismalarDetayTableNext.replyDate = null;
                                        dahiliYazismalarDetayTableNext.enabled = true;
                                        dahiliYazismalarDetayTableNext.sonOnayMi = true;
                                        await bllDahiliYazismalarDetayTable.Add(dahiliYazismalarDetayTableNext);

                                        //							EmailMessage emailMessage = new EmailMessage();
                                        //							emailMessage.toAddress=ceoUser.email;
                                        //							emailMessage.subject=dahiliYazismaTable.konu + " hk.");
                                        //							emailMessage.isSent=false;;
                                        //							emailMessage.emailText=buildDahiliYazisma(_mapper.Map<Data.Models.DahiliYazismaTable>(dahiliYazismaTable)));
                                        //							emailMessage.mailTuru=4;
                                        //							emailMessage.plannedDate=DateTime.Now;
                                        //							emailMessage.enabled=true;
                                        //							await bllEmailMessages.Add(emailMessage);

                                        SMSMessage smsMessage = new SMSMessage();
                                        smsMessage.plannedDate = DateTime.Now;
                                        smsMessage.isSent = false;
                                        smsMessage.smsText = (
                                                dahiliYazismaTable.id.ToString() + "Id'li" + dahiliYazismaTable.konu
                                                        + " konulu " + " Dahili Yazışma onayınızı beklemektedir.");
                                        smsMessage.toNumbers = ceoUser.mobile;
                                        BLLActions.SMSMessages bllSMSMessages = new BLLActions.SMSMessages(_configuration, _env);
                                        await bllSMSMessages.Add(smsMessage);
                                    }
                                    if (dahiliYazismaTable.id == null)
                                    {
                                        await Add(_mapper.Map<Data.Models.DahiliYazismaTable>(dahiliYazismaTable));
                                    }
                                    else
                                    {
                                        await Update(_mapper.Map<Data.Models.DahiliYazismaTable>(dahiliYazismaTable));
                                    }
                                    return 1;
                                }
                                else if (dahiliYazismaTable.onaylayici2 != null && dahiliYazismaTable.onay2Ok != true && dahiliYazismaTable.onaylayici2 != user.Id)
                                {
                                    return 2;
                                }
                                else if (dahiliYazismaTable.onaylayici2 != null && dahiliYazismaTable.onay2Ok == true)
                                {
                                    if (dahiliYazismaTable.onaylayici3 == user.Id && dahiliYazismaTable.onay3Ok != true)
                                    {
                                        dahiliYazismaTable.onay3Ok = true;

                                        Data.Models.DahiliYazismaMessage dahiliYazismaMessage = new Data.Models.DahiliYazismaMessage();
                                        dahiliYazismaMessage.createdDate = DateTime.Now;
                                        dahiliYazismaMessage.showAll = true;
                                        dahiliYazismaMessage.dahiliYazismaId = dahiliYazismaTable.id ?? 0;
                                        dahiliYazismaMessage.userId = user.Id;
                                        dahiliYazismaMessage.message = responseMyList.kanalGorusuFirst;
                                        dahiliYazismaMessage.enabled = true;
                                        BLLActions.DahiliYazismaMessage bllDahiliYazismaMessage = new BLLActions.DahiliYazismaMessage(_configuration, _env);
                                        await bllDahiliYazismaMessage.Add(dahiliYazismaMessage);

                                        BLLActions.DahiliYazismalarDetayTable bllDahiliYazismalarDetayTable = new BLLActions.DahiliYazismalarDetayTable(_configuration, _env);
                                        List<Data.Models.DahiliYazismalarDetayTable> listDahiliYazismalarDetayTable = bllDahiliYazismalarDetayTable
                                                .findAllByDahiliYazismaIdAndApprovedAndUserIdAndEnabled(dahiliYazismaTable.id,
                                                        null, user.Id, true);

                                        for (int j = 1; j < listDahiliYazismalarDetayTable.Count(); j++)
                                        {
                                            int id = listDahiliYazismalarDetayTable[j].Id;
                                            bllDahiliYazismalarDetayTable.Delete(id);
                                        }
                                        Data.Models.DahiliYazismalarDetayTable dahiliYazismalarDetayTable = listDahiliYazismalarDetayTable[0];

                                        dahiliYazismalarDetayTable.isReplied = true;
                                        dahiliYazismalarDetayTable.approved = true;
                                        dahiliYazismalarDetayTable.replyDate = DateTime.Now;
                                        await bllDahiliYazismalarDetayTable.Update(dahiliYazismalarDetayTable);
                                        if (dahiliYazismaTable.onaylayici4 != null)
                                        {
                                            Data.Models.DahiliYazismalarDetayTable dahiliYazismalarDetayTableNext = new Data.Models.DahiliYazismalarDetayTable();
                                            dahiliYazismalarDetayTableNext.userId = dahiliYazismaTable.onaylayici4 ?? 0;
                                            dahiliYazismalarDetayTableNext.createdDate = DateTime.Now;
                                            dahiliYazismalarDetayTableNext.approved = null;
                                            dahiliYazismalarDetayTableNext.dahiliYazismaId = dahiliYazismaTable.id ?? 0;
                                            dahiliYazismalarDetayTableNext.isReplied = false;
                                            dahiliYazismalarDetayTableNext.replyDate = null;
                                            dahiliYazismalarDetayTableNext.enabled = true;

                                            await bllDahiliYazismalarDetayTable.Add(dahiliYazismalarDetayTableNext);

                                            EmailMessage emailMessage = new EmailMessage();
                                            emailMessage.toAddress = onaylayici4?.email;
                                            emailMessage.subject = dahiliYazismaTable.konu + " hk.";
                                            emailMessage.isSent = false;
                                            emailMessage.emailText = buildDahiliYazisma(_mapper.Map<Data.Models.DahiliYazismaTable>(dahiliYazismaTable));
                                            emailMessage.mailTuru = 4;
                                            emailMessage.plannedDate = DateTime.Now;
                                            emailMessage.enabled = true;
                                            BLLActions.EmailMessages bllEmailMessages = new BLLActions.EmailMessages(_configuration, _env);
                                            await bllEmailMessages.Add(emailMessage);
                                        }
                                        else if (bolumUserHierarchyTable != null && dahiliYazismaTable.kanalBittiMi != true)
                                        {
                                            Data.Models.DahiliYazismalarDetayTable dahiliYazismalarDetayTableNext = new Data.Models.DahiliYazismalarDetayTable();
                                            dahiliYazismalarDetayTableNext.userId = bolumUserHierarchyTable.userId;
                                            dahiliYazismalarDetayTableNext.createdDate = DateTime.Now;
                                            dahiliYazismalarDetayTableNext.approved = null;
                                            dahiliYazismalarDetayTableNext.dahiliYazismaId = dahiliYazismaTable.id ?? 0;
                                            dahiliYazismalarDetayTableNext.isReplied = false;
                                            dahiliYazismalarDetayTableNext.replyDate = null;
                                            dahiliYazismalarDetayTableNext.enabled = true;

                                            await bllDahiliYazismalarDetayTable.Add(dahiliYazismalarDetayTableNext);
                                            EmailMessage emailMessage = new EmailMessage();
                                            emailMessage.toAddress = kanalUser?.email;
                                            emailMessage.subject = dahiliYazismaTable.konu + " hk.";
                                            emailMessage.isSent = false;
                                            emailMessage.emailText = buildDahiliYazisma(_mapper.Map<Data.Models.DahiliYazismaTable>(dahiliYazismaTable));
                                            emailMessage.mailTuru = 4;
                                            emailMessage.plannedDate = DateTime.Now;
                                            emailMessage.enabled = true;

                                            BLLActions.EmailMessages bllEmailMessages = new BLLActions.EmailMessages(_configuration, _env);
                                            await bllEmailMessages.Add(emailMessage);
                                        }
                                        else
                                        {

                                            Data.Models.DahiliYazismalarDetayTable dahiliYazismalarDetayTableNext = new Data.Models.DahiliYazismalarDetayTable();
                                            dahiliYazismalarDetayTableNext.userId = ceoTable.userId;
                                            dahiliYazismalarDetayTableNext.createdDate = DateTime.Now;
                                            dahiliYazismalarDetayTableNext.approved = null;
                                            dahiliYazismalarDetayTableNext.dahiliYazismaId = dahiliYazismaTable.id ?? 0;
                                            dahiliYazismalarDetayTableNext.isReplied = false;
                                            dahiliYazismalarDetayTableNext.replyDate = null;
                                            dahiliYazismalarDetayTableNext.enabled = true;
                                            dahiliYazismalarDetayTableNext.sonOnayMi = true;
                                            await bllDahiliYazismalarDetayTable.Add(dahiliYazismalarDetayTableNext);

                                            //								EmailMessage emailMessage = new EmailMessage();
                                            //								emailMessage.toAddress=ceoUser.email;
                                            //								emailMessage.subject=dahiliYazismaTable.konu + " hk.");
                                            //								emailMessage.isSent=false;;
                                            //								emailMessage.emailText=buildDahiliYazisma(_mapper.Map<Data.Models.DahiliYazismaTable>(dahiliYazismaTable)));
                                            //								emailMessage.mailTuru=4;
                                            //								emailMessage.plannedDate=DateTime.Now;
                                            //								emailMessage.enabled=true;
                                            //								await bllEmailMessages.Add(emailMessage);

                                            SMSMessage smsMessage = new SMSMessage();
                                            smsMessage.plannedDate = DateTime.Now;
                                            smsMessage.isSent = false;
                                            smsMessage.smsText =
                                                    dahiliYazismaTable.id.ToString() + "Id'li" + dahiliYazismaTable.konu
                                                            + " konulu " + " Dahili Yazışma onayınızı beklemektedir.";
                                            smsMessage.toNumbers = ceoUser.mobile;

                                            BLLActions.SMSMessages bllSMSMessages = new BLLActions.SMSMessages(_configuration, _env);
                                            await bllSMSMessages.Add(smsMessage);
                                        }
                                        if (dahiliYazismaTable.id == null)
                                        {
                                            await Add(_mapper.Map<Data.Models.DahiliYazismaTable>(dahiliYazismaTable));
                                        }
                                        else
                                        {
                                            await Update(_mapper.Map<Data.Models.DahiliYazismaTable>(dahiliYazismaTable));
                                        }
                                        return 1;
                                    }
                                    else if (dahiliYazismaTable.onaylayici3 != null && dahiliYazismaTable.onay3Ok != true && dahiliYazismaTable.onaylayici3 != user.Id)
                                    {
                                        return 2;
                                    }
                                    else if (dahiliYazismaTable.onaylayici3 != null && dahiliYazismaTable.onay3Ok == true)
                                    {
                                        if (dahiliYazismaTable.onaylayici4 == user.Id && dahiliYazismaTable.onay4Ok != true)
                                        {
                                            dahiliYazismaTable.onay4Ok = true;

                                            Data.Models.DahiliYazismaMessage dahiliYazismaMessage = new Data.Models.DahiliYazismaMessage();
                                            dahiliYazismaMessage.createdDate = DateTime.Now;
                                            dahiliYazismaMessage.showAll = true;
                                            dahiliYazismaMessage.dahiliYazismaId = dahiliYazismaTable.id ?? 0;
                                            dahiliYazismaMessage.userId = user.Id;
                                            dahiliYazismaMessage.message = responseMyList.kanalGorusuFirst;
                                            dahiliYazismaMessage.enabled = true;
                                            BLLActions.DahiliYazismaMessage bllDahiliYazismaMessage = new BLLActions.DahiliYazismaMessage(_configuration, _env);
                                            await bllDahiliYazismaMessage.Add(dahiliYazismaMessage);
                                            BLLActions.DahiliYazismalarDetayTable bllDahiliYazismalarDetayTable = new BLLActions.DahiliYazismalarDetayTable(_configuration, _env);
                                            List<Data.Models.DahiliYazismalarDetayTable> listDahiliYazismalarDetayTable = bllDahiliYazismalarDetayTable
                                                    .findAllByDahiliYazismaIdAndApprovedAndUserIdAndEnabled(
                                                            dahiliYazismaTable.id, null, user.Id, true);

                                            for (int j = 1; j < listDahiliYazismalarDetayTable.Count(); j++)
                                            {
                                                int id = listDahiliYazismalarDetayTable[j].Id;
                                                bllDahiliYazismalarDetayTable.Delete(id);
                                            }
                                            Data.Models.DahiliYazismalarDetayTable dahiliYazismalarDetayTable = listDahiliYazismalarDetayTable[0];

                                            dahiliYazismalarDetayTable.isReplied = true;
                                            dahiliYazismalarDetayTable.approved = true;
                                            dahiliYazismalarDetayTable.replyDate = DateTime.Now;

                                            await bllDahiliYazismalarDetayTable.Update(dahiliYazismalarDetayTable);
                                            if (bolumUserHierarchyTable != null && dahiliYazismaTable.kanalBittiMi != true)
                                            {
                                                Data.Models.DahiliYazismalarDetayTable dahiliYazismalarDetayTableNext = new Data.Models.DahiliYazismalarDetayTable();
                                                dahiliYazismalarDetayTableNext.userId = bolumUserHierarchyTable.userId;
                                                dahiliYazismalarDetayTableNext.createdDate = DateTime.Now;
                                                dahiliYazismalarDetayTableNext.approved = null;
                                                dahiliYazismalarDetayTableNext.dahiliYazismaId = dahiliYazismaTable.id ?? 0;
                                                dahiliYazismalarDetayTableNext.isReplied = false;
                                                dahiliYazismalarDetayTableNext.replyDate = null;
                                                dahiliYazismalarDetayTableNext.enabled = true;

                                                await bllDahiliYazismalarDetayTable.Add(dahiliYazismalarDetayTableNext);
                                                EmailMessage emailMessage = new EmailMessage();
                                                emailMessage.toAddress = kanalUser?.email;
                                                emailMessage.subject = dahiliYazismaTable.konu + " hk.";
                                                emailMessage.isSent = false;
                                                emailMessage.emailText = buildDahiliYazisma(_mapper.Map<Data.Models.DahiliYazismaTable>(dahiliYazismaTable));
                                                emailMessage.mailTuru = 4;
                                                emailMessage.plannedDate = DateTime.Now;
                                                emailMessage.enabled = true;
                                                BLLActions.EmailMessages bllEmailMessages = new BLLActions.EmailMessages(_configuration, _env);
                                                await bllEmailMessages.Add(emailMessage);
                                            }
                                            else
                                            {

                                                Data.Models.DahiliYazismalarDetayTable dahiliYazismalarDetayTableNext = new Data.Models.DahiliYazismalarDetayTable();
                                                dahiliYazismalarDetayTableNext.userId = ceoTable.userId;
                                                dahiliYazismalarDetayTableNext.createdDate = DateTime.Now;
                                                dahiliYazismalarDetayTableNext.approved = null;
                                                dahiliYazismalarDetayTableNext.dahiliYazismaId = dahiliYazismaTable.id ?? 0;
                                                dahiliYazismalarDetayTableNext.isReplied = false;
                                                dahiliYazismalarDetayTableNext.replyDate = null;
                                                dahiliYazismalarDetayTableNext.enabled = true;
                                                dahiliYazismalarDetayTableNext.sonOnayMi = true;
                                                await bllDahiliYazismalarDetayTable.Add(dahiliYazismalarDetayTableNext);

                                                //									EmailMessage emailMessage = new EmailMessage();
                                                //									emailMessage.toAddress=ceoUser.email;
                                                //									emailMessage.subject=dahiliYazismaTable.konu + " hk.");
                                                //									emailMessage.isSent=false;;
                                                //									emailMessage.emailText=buildDahiliYazisma(_mapper.Map<Data.Models.DahiliYazismaTable>(dahiliYazismaTable)));
                                                //									emailMessage.mailTuru=4;
                                                //									emailMessage.plannedDate=DateTime.Now;
                                                //									emailMessage.enabled=true;
                                                //									await bllEmailMessages.Add(emailMessage);

                                                SMSMessage smsMessage = new SMSMessage();
                                                smsMessage.plannedDate = DateTime.Now;
                                                smsMessage.isSent = false;
                                                smsMessage.smsText = (dahiliYazismaTable.id.ToString() + "Id'li"
                                                        + dahiliYazismaTable.konu + " konulu "
                                                        + " Dahili Yazışma onayınızı beklemektedir.");
                                                smsMessage.toNumbers = ceoUser.mobile;

                                                BLLActions.SMSMessages bllSMSMessages = new BLLActions.SMSMessages(_configuration, _env);
                                                await bllSMSMessages.Add(smsMessage);
                                            }
                                            if (dahiliYazismaTable.id == null)
                                            {
                                                await Add(_mapper.Map<Data.Models.DahiliYazismaTable>(dahiliYazismaTable));
                                            }
                                            else
                                            {
                                                await Update(_mapper.Map<Data.Models.DahiliYazismaTable>(dahiliYazismaTable));
                                            }
                                            return 1;
                                        }
                                        else if (dahiliYazismaTable.onaylayici4 != null && dahiliYazismaTable.onay4Ok != true && dahiliYazismaTable.onaylayici4 != user.Id)
                                        {
                                            return 2;
                                        }
                                        else if (dahiliYazismaTable.onaylayici4 != null && dahiliYazismaTable.onay4Ok == true)
                                        {

                                            if (bolumUserHierarchyTable != null && dahiliYazismaTable.kanalBittiMi != true && bolumUserHierarchyTable.userId == user.Id)
                                            {
                                                BLLActions.DahiliYazismalarDetayTable bllDahiliYazismalarDetayTable = new BLLActions.DahiliYazismalarDetayTable(_configuration, _env);
                                                List<Data.Models.DahiliYazismalarDetayTable> listDahiliYazismalarDetayTable = bllDahiliYazismalarDetayTable
                                                        .findAllByDahiliYazismaIdAndApprovedAndUserIdAndEnabled(
                                                                dahiliYazismaTable.id, null, user.Id, true);

                                                for (int j = 1; j < listDahiliYazismalarDetayTable.Count(); j++)
                                                {
                                                    int id = listDahiliYazismalarDetayTable[j].Id;
                                                    bllDahiliYazismalarDetayTable.Delete(id);
                                                }
                                                Data.Models.DahiliYazismalarDetayTable dahiliYazismalarDetayTable = listDahiliYazismalarDetayTable[0];

                                                dahiliYazismalarDetayTable.isReplied = true;
                                                dahiliYazismalarDetayTable.approved = true;
                                                dahiliYazismalarDetayTable.replyDate = DateTime.Now;
                                                dahiliYazismalarDetayTable.kanalBittiMi = true;
                                                await bllDahiliYazismalarDetayTable.Update(dahiliYazismalarDetayTable);

                                                dahiliYazismaTable.kanalBittiMi = true;

                                                Data.Models.DahiliYazismaMessage dahiliYazismaMessage = new Data.Models.DahiliYazismaMessage();
                                                dahiliYazismaMessage.createdDate = DateTime.Now;
                                                dahiliYazismaMessage.showAll = true;
                                                dahiliYazismaMessage.dahiliYazismaId = dahiliYazismaTable.id ?? 0;
                                                dahiliYazismaMessage.userId = user.Id;
                                                dahiliYazismaMessage.message = responseMyList.kanalGorusuFirst;
                                                dahiliYazismaMessage.enabled = true;
                                                BLLActions.DahiliYazismaMessage bllDahiliYazismaMessage = new BLLActions.DahiliYazismaMessage(_configuration, _env);
                                                await bllDahiliYazismaMessage.Add(dahiliYazismaMessage);

                                                Data.Models.DahiliYazismalarDetayTable dahiliYazismalarDetayTableNext = new Data.Models.DahiliYazismalarDetayTable();
                                                dahiliYazismalarDetayTableNext.userId = ceoTable.userId;
                                                dahiliYazismalarDetayTableNext.createdDate = DateTime.Now;
                                                dahiliYazismalarDetayTableNext.approved = null;
                                                dahiliYazismalarDetayTableNext.dahiliYazismaId = dahiliYazismaTable.id ?? 0;
                                                dahiliYazismalarDetayTableNext.isReplied = false;
                                                dahiliYazismalarDetayTableNext.replyDate = null;
                                                dahiliYazismalarDetayTableNext.enabled = true;

                                                await bllDahiliYazismalarDetayTable.Add(dahiliYazismalarDetayTableNext);
                                                //									EmailMessage emailMessage = new EmailMessage();
                                                //									emailMessage.toAddress=ceoUser.email;
                                                //									emailMessage.subject=dahiliYazismaTable.konu + " hk.");
                                                //									emailMessage.isSent=false;;
                                                //									emailMessage.emailText=buildDahiliYazisma(_mapper.Map<Data.Models.DahiliYazismaTable>(dahiliYazismaTable)));
                                                //									emailMessage.mailTuru=4;
                                                //									emailMessage.plannedDate=DateTime.Now;
                                                //									emailMessage.enabled=true;
                                                //									await bllEmailMessages.Add(emailMessage);

                                                SMSMessage smsMessage = new SMSMessage();
                                                smsMessage.plannedDate = DateTime.Now;
                                                smsMessage.isSent = false;
                                                smsMessage.smsText = (dahiliYazismaTable.id.ToString() + "Id'li"
                                                        + dahiliYazismaTable.konu + " konulu "
                                                        + " Dahili Yazışma onayınızı beklemektedir.");
                                                smsMessage.toNumbers = ceoUser.mobile;

                                                BLLActions.SMSMessages bllSMSMessages = new BLLActions.SMSMessages(_configuration, _env);
                                                await bllSMSMessages.Add(smsMessage);

                                                if (dahiliYazismaTable.id == null)
                                                {
                                                    await Add(_mapper.Map<Data.Models.DahiliYazismaTable>(dahiliYazismaTable));
                                                }
                                                else
                                                {
                                                    await Update(_mapper.Map<Data.Models.DahiliYazismaTable>(dahiliYazismaTable));
                                                }
                                                return 1;
                                            }
                                            else
                                            {
                                                if (ceoTable.userId == user.Id)
                                                {

                                                    if (dahiliYazismaTable.kanalGorusuUserId != null)
                                                    {

                                                        UserByNameEMailDto kanalGorusuUser = bllAdminUsers
                                                                .getUserByNameAndEmail(dahiliYazismaTable.kanalGorusuUserId ?? 0);
                                                        BLLActions.DahiliYazismalarDetayTable bllDahiliYazismalarDetayTable = new BLLActions.DahiliYazismalarDetayTable(_configuration, _env);
                                                        List<Data.Models.DahiliYazismalarDetayTable> listDahiliYazismalarDetayTable = bllDahiliYazismalarDetayTable
                                                                .findAllByDahiliYazismaIdAndApprovedAndUserIdAndEnabled(
                                                                        dahiliYazismaTable.id, null, user.Id, true);

                                                        for (int j = 1; j < listDahiliYazismalarDetayTable.Count(); j++)
                                                        {
                                                            int id = listDahiliYazismalarDetayTable[j].Id;
                                                            bllDahiliYazismalarDetayTable.Delete(id);
                                                        }
                                                        Data.Models.DahiliYazismalarDetayTable dahiliYazismalarDetayTable = listDahiliYazismalarDetayTable
                                                                [0];
                                                        dahiliYazismalarDetayTable.isReplied = true;
                                                        dahiliYazismalarDetayTable.approved = true;
                                                        dahiliYazismalarDetayTable.replyDate = DateTime.Now;
                                                        await bllDahiliYazismalarDetayTable.Update(dahiliYazismalarDetayTable);

                                                        dahiliYazismaTable.onaylandiMi = true;

                                                        Data.Models.DahiliYazismaMessage dahiliYazismaMessage = new Data.Models.DahiliYazismaMessage();
                                                        dahiliYazismaMessage.createdDate = DateTime.Now;
                                                        dahiliYazismaMessage.showAll = true;
                                                        dahiliYazismaMessage.dahiliYazismaId = dahiliYazismaTable.id ?? 0;
                                                        dahiliYazismaMessage.userId = user.Id;
                                                        dahiliYazismaMessage.message = responseMyList.kanalGorusuFirst;
                                                        dahiliYazismaMessage.enabled = true;
                                                        BLLActions.DahiliYazismaMessage bllDahiliYazismaMessage = new BLLActions.DahiliYazismaMessage(_configuration, _env);
                                                        await bllDahiliYazismaMessage.Add(dahiliYazismaMessage);

                                                        Data.Models.DahiliYazismalarDetayTable dahiliYazismalarDetayTableNext = new Data.Models.DahiliYazismalarDetayTable();
                                                        dahiliYazismalarDetayTableNext.userId = dahiliYazismaTable.kanalGorusuUserId ?? 0;
                                                        dahiliYazismalarDetayTableNext.createdDate = DateTime.Now;
                                                        dahiliYazismalarDetayTableNext.approved = null;
                                                        dahiliYazismalarDetayTableNext
                                                                .dahiliYazismaId = dahiliYazismaTable.id ?? 0;
                                                        dahiliYazismalarDetayTableNext.sonOnayMi = true;
                                                        dahiliYazismalarDetayTableNext.isReplied = false;
                                                        dahiliYazismalarDetayTableNext.replyDate = null;
                                                        dahiliYazismalarDetayTableNext.enabled = true;

                                                        await bllDahiliYazismalarDetayTable.Add(dahiliYazismalarDetayTableNext);
                                                        EmailMessage emailMessage = new EmailMessage();
                                                        emailMessage.toAddress = kanalGorusuUser.email;
                                                        emailMessage.subject = dahiliYazismaTable.konu + " hk.";
                                                        emailMessage.isSent = false; ;
                                                        emailMessage.emailText = buildDahiliYazisma(_mapper.Map<Data.Models.DahiliYazismaTable>(dahiliYazismaTable));
                                                        emailMessage.mailTuru = 4;
                                                        emailMessage.plannedDate = DateTime.Now;
                                                        emailMessage.enabled = true;
                                                        BLLActions.EmailMessages bllEmailMessages = new BLLActions.EmailMessages(_configuration, _env);
                                                        await bllEmailMessages.Add(emailMessage);

                                                        if (dahiliYazismaTable.id == null)
                                                        {
                                                            await Add(_mapper.Map<Data.Models.DahiliYazismaTable>(dahiliYazismaTable));
                                                        }
                                                        else
                                                        {
                                                            await Update(_mapper.Map<Data.Models.DahiliYazismaTable>(dahiliYazismaTable));
                                                        }
                                                        if (dahiliYazismaTable.bilgiUserId1 != null)
                                                        {
                                                            UserByNameEMailDto bilgiUser1 = bllAdminUsers
                                                                    .getUserByNameAndEmail(dahiliYazismaTable.bilgiUserId1 ?? 0);
                                                            EmailMessage emailMessageBilgi = new EmailMessage();
                                                            emailMessageBilgi.toAddress = bilgiUser1.email;
                                                            emailMessageBilgi.subject =
                                                                    dahiliYazismaTable.konu + " Bilgiye Eklendiniz";
                                                            emailMessageBilgi.isSent = false; ;
                                                            emailMessageBilgi.emailText = buildDahiliYazisma(_mapper.Map<Data.Models.DahiliYazismaTable>(dahiliYazismaTable));
                                                            emailMessageBilgi.mailTuru = 4;
                                                            emailMessageBilgi.plannedDate = DateTime.Now;
                                                            emailMessageBilgi.enabled = true;
                                                            await bllEmailMessages.Add(emailMessage);
                                                        }
                                                        if (dahiliYazismaTable.bilgiUserId2 != null)
                                                        {
                                                            UserByNameEMailDto bilgiUser2 = bllAdminUsers
                                                                    .getUserByNameAndEmail(dahiliYazismaTable.bilgiUserId2 ?? 0);
                                                            EmailMessage emailMessageBilgi = new EmailMessage();
                                                            emailMessageBilgi.toAddress = bilgiUser2.email;
                                                            emailMessageBilgi.subject =
                                                                    dahiliYazismaTable.konu + " Bilgiye Eklendiniz";
                                                            emailMessageBilgi.isSent = false; ;
                                                            emailMessageBilgi.emailText = buildDahiliYazisma(_mapper.Map<Data.Models.DahiliYazismaTable>(dahiliYazismaTable));
                                                            emailMessageBilgi.mailTuru = 4;
                                                            emailMessageBilgi.plannedDate = DateTime.Now;
                                                            emailMessageBilgi.enabled = true;
                                                            await bllEmailMessages.Add(emailMessage);
                                                        }
                                                        if (dahiliYazismaTable.bilgiUserId3 != null)
                                                        {
                                                            UserByNameEMailDto bilgiUser3 = bllAdminUsers
                                                                    .getUserByNameAndEmail(dahiliYazismaTable.bilgiUserId3 ?? 0);
                                                            EmailMessage emailMessageBilgi = new EmailMessage();
                                                            emailMessageBilgi.toAddress = bilgiUser3.email;
                                                            emailMessageBilgi.subject =
                                                                    dahiliYazismaTable.konu + " Bilgiye Eklendiniz";
                                                            emailMessageBilgi.isSent = false; ;
                                                            emailMessageBilgi.emailText = buildDahiliYazisma(_mapper.Map<Data.Models.DahiliYazismaTable>(dahiliYazismaTable));
                                                            emailMessageBilgi.mailTuru = 4;
                                                            emailMessageBilgi.plannedDate = DateTime.Now;
                                                            emailMessageBilgi.enabled = true;
                                                            await bllEmailMessages.Add(emailMessage);
                                                        }
                                                        if (dahiliYazismaTable.bilgiUserId4 != null)
                                                        {
                                                            UserByNameEMailDto bilgiUser4 = bllAdminUsers
                                                                    .getUserByNameAndEmail(dahiliYazismaTable.bilgiUserId4 ?? 0);
                                                            EmailMessage emailMessageBilgi = new EmailMessage();
                                                            emailMessageBilgi.toAddress = bilgiUser4.email;
                                                            emailMessageBilgi.subject =
                                                                    dahiliYazismaTable.konu + " Bilgiye Eklendiniz";
                                                            emailMessageBilgi.isSent = false;
                                                            emailMessageBilgi.emailText = buildDahiliYazisma(_mapper.Map<Data.Models.DahiliYazismaTable>(dahiliYazismaTable));
                                                            emailMessageBilgi.mailTuru = 4;
                                                            emailMessageBilgi.plannedDate = DateTime.Now;
                                                            emailMessageBilgi.enabled = true;
                                                            await bllEmailMessages.Add(emailMessage);
                                                        }
                                                        if (dahiliYazismaTable.bilgiUserId5 != null)
                                                        {
                                                            UserByNameEMailDto bilgiUser5 = bllAdminUsers
                                                                    .getUserByNameAndEmail(dahiliYazismaTable.bilgiUserId5 ?? 0);
                                                            EmailMessage emailMessageBilgi = new EmailMessage();
                                                            emailMessageBilgi.toAddress = bilgiUser5.email;
                                                            emailMessageBilgi.subject =
                                                                    dahiliYazismaTable.konu + " Bilgiye Eklendiniz";
                                                            emailMessageBilgi.isSent = false; ;
                                                            emailMessageBilgi.emailText = buildDahiliYazisma(_mapper.Map<Data.Models.DahiliYazismaTable>(dahiliYazismaTable));
                                                            emailMessageBilgi.mailTuru = 4;
                                                            emailMessageBilgi.plannedDate = DateTime.Now;
                                                            emailMessageBilgi.enabled = true;
                                                            await bllEmailMessages.Add(emailMessage);
                                                        }
                                                        if (dahiliYazismaTable.id == null)
                                                        {
                                                            await Add(_mapper.Map<Data.Models.DahiliYazismaTable>(dahiliYazismaTable));
                                                        }
                                                        else
                                                        {
                                                            await Update(_mapper.Map<Data.Models.DahiliYazismaTable>(dahiliYazismaTable));
                                                        }
                                                        return 1;
                                                    }
                                                    else
                                                    {
                                                        return 3;
                                                    }

                                                }
                                                else
                                                {
                                                    return 2;
                                                }

                                            }

                                        }
                                        else
                                        {

                                            if (bolumUserHierarchyTable != null && dahiliYazismaTable.kanalBittiMi != true && bolumUserHierarchyTable.userId == user.Id)
                                            {
                                                BLLActions.DahiliYazismalarDetayTable bllDahiliYazismalarDetayTable = new BLLActions.DahiliYazismalarDetayTable(_configuration, _env);
                                                List<Data.Models.DahiliYazismalarDetayTable> listDahiliYazismalarDetayTable = bllDahiliYazismalarDetayTable
                                                        .findAllByDahiliYazismaIdAndApprovedAndUserIdAndEnabled(
                                                                dahiliYazismaTable.id, null, user.Id, true);

                                                for (int j = 1; j < listDahiliYazismalarDetayTable.Count(); j++)
                                                {
                                                    int id = listDahiliYazismalarDetayTable[j].Id;
                                                    bllDahiliYazismalarDetayTable.Delete(id);
                                                }
                                                Data.Models.DahiliYazismalarDetayTable dahiliYazismalarDetayTable = listDahiliYazismalarDetayTable
                                                        [0];
                                                dahiliYazismalarDetayTable.isReplied = true;
                                                dahiliYazismalarDetayTable.approved = true;
                                                dahiliYazismalarDetayTable.replyDate = DateTime.Now;
                                                dahiliYazismalarDetayTable.kanalBittiMi = true;
                                                await bllDahiliYazismalarDetayTable.Update(dahiliYazismalarDetayTable);
                                                dahiliYazismaTable.kanalBittiMi = true;

                                                Data.Models.DahiliYazismaMessage dahiliYazismaMessage = new Data.Models.DahiliYazismaMessage();
                                                dahiliYazismaMessage.createdDate = DateTime.Now;
                                                dahiliYazismaMessage.showAll = true;
                                                dahiliYazismaMessage.dahiliYazismaId = dahiliYazismaTable.id ?? 0;
                                                dahiliYazismaMessage.userId = user.Id;
                                                dahiliYazismaMessage.message = responseMyList.kanalGorusuFirst;
                                                dahiliYazismaMessage.enabled = true;
                                                BLLActions.DahiliYazismaMessage bllDahiliYazismaMessage = new BLLActions.DahiliYazismaMessage(_configuration, _env);
                                                await bllDahiliYazismaMessage.Add(dahiliYazismaMessage);

                                                Data.Models.DahiliYazismalarDetayTable dahiliYazismalarDetayTableNext = new Data.Models.DahiliYazismalarDetayTable();
                                                dahiliYazismalarDetayTableNext.userId = ceoTable.userId;
                                                dahiliYazismalarDetayTableNext.createdDate = DateTime.Now;
                                                dahiliYazismalarDetayTableNext.approved = null;
                                                dahiliYazismalarDetayTableNext.dahiliYazismaId = dahiliYazismaTable.id ?? 0;
                                                dahiliYazismalarDetayTableNext.isReplied = false;
                                                dahiliYazismalarDetayTableNext.replyDate = null;
                                                dahiliYazismalarDetayTableNext.enabled = true;

                                                await bllDahiliYazismalarDetayTable.Add(dahiliYazismalarDetayTableNext);
                                                //									EmailMessage emailMessage = new EmailMessage();
                                                //									emailMessage.toAddress=ceoUser.email;
                                                //									emailMessage.subject=dahiliYazismaTable.konu + " hk.");
                                                //									emailMessage.isSent=false;;
                                                //									emailMessage.emailText=buildDahiliYazisma(_mapper.Map<Data.Models.DahiliYazismaTable>(dahiliYazismaTable)));
                                                //									emailMessage.mailTuru=4;
                                                //									emailMessage.plannedDate=DateTime.Now;
                                                //									emailMessage.enabled=true;
                                                //									await bllEmailMessages.Add(emailMessage);

                                                SMSMessage smsMessage = new SMSMessage();
                                                smsMessage.plannedDate = DateTime.Now;
                                                smsMessage.isSent = false; ;
                                                smsMessage.smsText = (dahiliYazismaTable.id.ToString() + "Id'li"
                                                        + dahiliYazismaTable.konu + " konulu "
                                                        + " Dahili Yazışma onayınızı beklemektedir.");
                                                smsMessage.toNumbers = ceoUser.mobile;

                                                BLLActions.SMSMessages bllSMSMessages = new BLLActions.SMSMessages(_configuration, _env);
                                                await bllSMSMessages.Add(smsMessage);

                                                if (dahiliYazismaTable.id == null)
                                                {
                                                    await Add(_mapper.Map<Data.Models.DahiliYazismaTable>(dahiliYazismaTable));
                                                }
                                                else
                                                {
                                                    await Update(_mapper.Map<Data.Models.DahiliYazismaTable>(dahiliYazismaTable));
                                                }
                                                return 1;
                                            }
                                            else if (ceoTable.userId.Equals(user.Id))
                                            {

                                                if (dahiliYazismaTable.kanalGorusuUserId != null)
                                                {
                                                    UserByNameEMailDto kanalGorusuUser = bllAdminUsers
                                                            .getUserByNameAndEmail(dahiliYazismaTable.kanalGorusuUserId ?? 0);
                                                    BLLActions.DahiliYazismalarDetayTable bllDahiliYazismalarDetayTable = new BLLActions.DahiliYazismalarDetayTable(_configuration, _env);
                                                    List<Data.Models.DahiliYazismalarDetayTable> listDahiliYazismalarDetayTable = bllDahiliYazismalarDetayTable
                                                            .findAllByDahiliYazismaIdAndApprovedAndUserIdAndEnabled(
                                                                    dahiliYazismaTable.id, null, user.Id, true);

                                                    for (int j = 1; j < listDahiliYazismalarDetayTable.Count(); j++)
                                                    {
                                                        int id = listDahiliYazismalarDetayTable[j].Id;
                                                        bllDahiliYazismalarDetayTable.Delete(id);
                                                    }
                                                    Data.Models.DahiliYazismalarDetayTable dahiliYazismalarDetayTable = listDahiliYazismalarDetayTable
                                                            [0];
                                                    dahiliYazismalarDetayTable.isReplied = true;
                                                    dahiliYazismalarDetayTable.approved = true;
                                                    dahiliYazismalarDetayTable.replyDate = DateTime.Now;
                                                    await bllDahiliYazismalarDetayTable.Update(dahiliYazismalarDetayTable);

                                                    dahiliYazismaTable.onaylandiMi = true;

                                                    Data.Models.DahiliYazismaMessage dahiliYazismaMessage = new Data.Models.DahiliYazismaMessage();
                                                    dahiliYazismaMessage.createdDate = DateTime.Now;
                                                    dahiliYazismaMessage.showAll = true;
                                                    dahiliYazismaMessage.dahiliYazismaId = dahiliYazismaTable.id ?? 0;
                                                    dahiliYazismaMessage.userId = user.Id;
                                                    dahiliYazismaMessage.message = responseMyList.kanalGorusuFirst;
                                                    dahiliYazismaMessage.enabled = true;
                                                    BLLActions.DahiliYazismaMessage bllDahiliYazismaMessage = new BLLActions.DahiliYazismaMessage(_configuration, _env);
                                                    await bllDahiliYazismaMessage.Add(dahiliYazismaMessage);

                                                    Data.Models.DahiliYazismalarDetayTable dahiliYazismalarDetayTableNext = new Data.Models.DahiliYazismalarDetayTable();
                                                    dahiliYazismalarDetayTableNext.userId = dahiliYazismaTable.kanalGorusuUserId ?? 0;
                                                    dahiliYazismalarDetayTableNext.createdDate = DateTime.Now;
                                                    dahiliYazismalarDetayTableNext.approved = null;
                                                    dahiliYazismalarDetayTableNext.dahiliYazismaId = dahiliYazismaTable.id ?? 0;
                                                    dahiliYazismalarDetayTableNext.sonOnayMi = true;
                                                    dahiliYazismalarDetayTableNext.isReplied = false;
                                                    dahiliYazismalarDetayTableNext.replyDate = null;
                                                    dahiliYazismalarDetayTableNext.enabled = true;

                                                    await bllDahiliYazismalarDetayTable.Add(dahiliYazismalarDetayTableNext);

                                                    EmailMessage emailMessage = new EmailMessage();
                                                    emailMessage.toAddress = kanalGorusuUser.email;
                                                    emailMessage.subject = dahiliYazismaTable.konu + " hk.";
                                                    emailMessage.isSent = false; ;
                                                    emailMessage.emailText = buildDahiliYazisma(_mapper.Map<Data.Models.DahiliYazismaTable>(dahiliYazismaTable));
                                                    emailMessage.mailTuru = 4;
                                                    emailMessage.plannedDate = DateTime.Now;
                                                    emailMessage.enabled = true;
                                                    BLLActions.EmailMessages bllEmailMessages = new BLLActions.EmailMessages(_configuration, _env);
                                                    await bllEmailMessages.Add(emailMessage);
                                                    if (dahiliYazismaTable.id == null)
                                                    {
                                                        await Add(_mapper.Map<Data.Models.DahiliYazismaTable>(dahiliYazismaTable));
                                                    }
                                                    else
                                                    {
                                                        await Update(_mapper.Map<Data.Models.DahiliYazismaTable>(dahiliYazismaTable));
                                                    }
                                                    if (dahiliYazismaTable.bilgiUserId1 != null)
                                                    {
                                                        UserByNameEMailDto bilgiUser1 = bllAdminUsers
                                                                .getUserByNameAndEmail(dahiliYazismaTable.bilgiUserId1 ?? 0);
                                                        EmailMessage emailMessageBilgi = new EmailMessage();
                                                        emailMessageBilgi.toAddress = bilgiUser1.email;
                                                        emailMessageBilgi
                                                                .subject = dahiliYazismaTable.konu + " Bilgiye Eklendiniz";
                                                        emailMessageBilgi.isSent = false; ;
                                                        emailMessageBilgi.emailText = buildDahiliYazisma(_mapper.Map<Data.Models.DahiliYazismaTable>(dahiliYazismaTable));
                                                        emailMessageBilgi.mailTuru = 4;
                                                        emailMessageBilgi.plannedDate = DateTime.Now;
                                                        emailMessageBilgi.enabled = true;
                                                        await bllEmailMessages.Add(emailMessage);
                                                    }
                                                    if (dahiliYazismaTable.bilgiUserId2 != null)
                                                    {
                                                        UserByNameEMailDto bilgiUser2 = bllAdminUsers
                                                                .getUserByNameAndEmail(dahiliYazismaTable.bilgiUserId2 ?? 0);
                                                        EmailMessage emailMessageBilgi = new EmailMessage();
                                                        emailMessageBilgi.toAddress = bilgiUser2.email;
                                                        emailMessageBilgi
                                                                .subject = dahiliYazismaTable.konu + " Bilgiye Eklendiniz";
                                                        emailMessageBilgi.isSent = false; ;
                                                        emailMessageBilgi.emailText = buildDahiliYazisma(_mapper.Map<Data.Models.DahiliYazismaTable>(dahiliYazismaTable));
                                                        emailMessageBilgi.mailTuru = 4;
                                                        emailMessageBilgi.plannedDate = DateTime.Now;
                                                        emailMessageBilgi.enabled = true;
                                                        await bllEmailMessages.Add(emailMessage);
                                                    }
                                                    if (dahiliYazismaTable.bilgiUserId3 != null)
                                                    {
                                                        UserByNameEMailDto bilgiUser3 = bllAdminUsers
                                                                .getUserByNameAndEmail(dahiliYazismaTable.bilgiUserId3 ?? 0);
                                                        EmailMessage emailMessageBilgi = new EmailMessage();
                                                        emailMessageBilgi.toAddress = bilgiUser3.email;
                                                        emailMessageBilgi
                                                                .subject = dahiliYazismaTable.konu + " Bilgiye Eklendiniz";
                                                        emailMessageBilgi.isSent = false; ;
                                                        emailMessageBilgi.emailText = buildDahiliYazisma(_mapper.Map<Data.Models.DahiliYazismaTable>(dahiliYazismaTable));
                                                        emailMessageBilgi.mailTuru = 4;
                                                        emailMessageBilgi.plannedDate = DateTime.Now;
                                                        emailMessageBilgi.enabled = true;
                                                        await bllEmailMessages.Add(emailMessage);
                                                    }
                                                    if (dahiliYazismaTable.bilgiUserId4 != null)
                                                    {
                                                        UserByNameEMailDto bilgiUser4 = bllAdminUsers
                                                                .getUserByNameAndEmail(dahiliYazismaTable.bilgiUserId4 ?? 0);
                                                        EmailMessage emailMessageBilgi = new EmailMessage();
                                                        emailMessageBilgi.toAddress = bilgiUser4.email;
                                                        emailMessageBilgi
                                                                .subject = dahiliYazismaTable.konu + " Bilgiye Eklendiniz";
                                                        emailMessageBilgi.isSent = false; ;
                                                        emailMessageBilgi.emailText = buildDahiliYazisma(_mapper.Map<Data.Models.DahiliYazismaTable>(dahiliYazismaTable));
                                                        emailMessageBilgi.mailTuru = 4;
                                                        emailMessageBilgi.plannedDate = DateTime.Now;
                                                        emailMessageBilgi.enabled = true;
                                                        await bllEmailMessages.Add(emailMessage);
                                                    }
                                                    if (dahiliYazismaTable.bilgiUserId5 != null)
                                                    {
                                                        UserByNameEMailDto bilgiUser5 = bllAdminUsers
                                                                .getUserByNameAndEmail(dahiliYazismaTable.bilgiUserId5 ?? 0);
                                                        EmailMessage emailMessageBilgi = new EmailMessage();
                                                        emailMessageBilgi.toAddress = bilgiUser5.email;
                                                        emailMessageBilgi
                                                                .subject = dahiliYazismaTable.konu + " Bilgiye Eklendiniz";
                                                        emailMessageBilgi.isSent = false; ;
                                                        emailMessageBilgi.emailText = buildDahiliYazisma(_mapper.Map<Data.Models.DahiliYazismaTable>(dahiliYazismaTable));
                                                        emailMessageBilgi.mailTuru = 4;
                                                        emailMessageBilgi.plannedDate = DateTime.Now;
                                                        emailMessageBilgi.enabled = true;
                                                        await bllEmailMessages.Add(emailMessage);
                                                    }

                                                    if (dahiliYazismaTable.id == null)
                                                    {
                                                        await Add(_mapper.Map<Data.Models.DahiliYazismaTable>(dahiliYazismaTable));
                                                    }
                                                    else
                                                    {
                                                        await Update(_mapper.Map<Data.Models.DahiliYazismaTable>(dahiliYazismaTable));
                                                    }
                                                    return 1;
                                                }
                                                else
                                                {
                                                    return 3;
                                                }
                                            }
                                        }

                                    }
                                    else
                                    {

                                        if (bolumUserHierarchyTable != null && dahiliYazismaTable.kanalBittiMi != true && bolumUserHierarchyTable.userId == user.Id)
                                        {
                                            BLLActions.DahiliYazismalarDetayTable bllDahiliYazismalarDetayTable = new BLLActions.DahiliYazismalarDetayTable(_configuration, _env);
                                            List<Data.Models.DahiliYazismalarDetayTable> listDahiliYazismalarDetayTable = bllDahiliYazismalarDetayTable
                                                    .findAllByDahiliYazismaIdAndApprovedAndUserIdAndEnabled(
                                                            dahiliYazismaTable.id, null, user.Id, true);

                                            for (int j = 1; j < listDahiliYazismalarDetayTable.Count(); j++)
                                            {
                                                int id = listDahiliYazismalarDetayTable[j].Id;
                                                bllDahiliYazismalarDetayTable.Delete(id);
                                            }
                                            Data.Models.DahiliYazismalarDetayTable dahiliYazismalarDetayTable = listDahiliYazismalarDetayTable
                                                    [0];
                                            dahiliYazismalarDetayTable.isReplied = true;
                                            dahiliYazismalarDetayTable.approved = true;
                                            dahiliYazismalarDetayTable.replyDate = DateTime.Now;
                                            dahiliYazismalarDetayTable.kanalBittiMi = true;
                                            await bllDahiliYazismalarDetayTable.Update(dahiliYazismalarDetayTable);

                                            dahiliYazismaTable.kanalBittiMi = true;

                                            Data.Models.DahiliYazismalarDetayTable dahiliYazismalarDetayTableNext = new Data.Models.DahiliYazismalarDetayTable();
                                            dahiliYazismalarDetayTableNext.userId = ceoTable.userId;
                                            dahiliYazismalarDetayTableNext.createdDate = DateTime.Now;
                                            dahiliYazismalarDetayTableNext.approved = null;
                                            dahiliYazismalarDetayTableNext.dahiliYazismaId = dahiliYazismaTable.id ?? 0;
                                            dahiliYazismalarDetayTableNext.isReplied = false;
                                            dahiliYazismalarDetayTableNext.replyDate = null;
                                            dahiliYazismalarDetayTableNext.enabled = true;

                                            await bllDahiliYazismalarDetayTable.Add(dahiliYazismalarDetayTableNext);

                                            //								EmailMessage emailMessage = new EmailMessage();
                                            //								emailMessage.toAddress=ceoUser.email;
                                            //								emailMessage.subject=dahiliYazismaTable.konu + " hk.");
                                            //								emailMessage.isSent=false;;
                                            //								emailMessage.emailText=buildDahiliYazisma(_mapper.Map<Data.Models.DahiliYazismaTable>(dahiliYazismaTable)));
                                            //								emailMessage.mailTuru=4;
                                            //								emailMessage.plannedDate=DateTime.Now;
                                            //								emailMessage.enabled=true;
                                            //								await bllEmailMessages.Add(emailMessage);

                                            SMSMessage smsMessage = new SMSMessage();
                                            smsMessage.plannedDate = DateTime.Now;
                                            smsMessage.isSent = false; ;
                                            smsMessage.smsText = (
                                                    dahiliYazismaTable.id.ToString() + "Id'li" + dahiliYazismaTable.konu
                                                            + " konulu " + " Dahili Yazışma onayınızı beklemektedir.");
                                            smsMessage.toNumbers = ceoUser.mobile;

                                            BLLActions.SMSMessages bllSMSMessages = new BLLActions.SMSMessages(_configuration, _env);
                                            await bllSMSMessages.Add(smsMessage);
                                            if (dahiliYazismaTable.id == null)
                                            {
                                                await Add(_mapper.Map<Data.Models.DahiliYazismaTable>(dahiliYazismaTable));
                                            }
                                            else
                                            {
                                                await Update(_mapper.Map<Data.Models.DahiliYazismaTable>(dahiliYazismaTable));
                                            }
                                            return 1;
                                        }
                                        else if (ceoTable.userId.Equals(user.Id))
                                        {

                                            if (dahiliYazismaTable.kanalGorusuUserId != null)
                                            {
                                                UserByNameEMailDto kanalGorusuUser = bllAdminUsers
                                                        .getUserByNameAndEmail(dahiliYazismaTable.kanalGorusuUserId ?? 0);
                                                BLLActions.DahiliYazismalarDetayTable bllDahiliYazismalarDetayTable = new BLLActions.DahiliYazismalarDetayTable(_configuration, _env);
                                                List<Data.Models.DahiliYazismalarDetayTable> listDahiliYazismalarDetayTable = bllDahiliYazismalarDetayTable
                                                        .findAllByDahiliYazismaIdAndApprovedAndUserIdAndEnabled(
                                                                dahiliYazismaTable.id, null, user.Id, true);

                                                for (int j = 1; j < listDahiliYazismalarDetayTable.Count(); j++)
                                                {
                                                    int id = listDahiliYazismalarDetayTable[j].Id;
                                                    bllDahiliYazismalarDetayTable.Delete(id);
                                                }
                                                Data.Models.DahiliYazismalarDetayTable dahiliYazismalarDetayTable = listDahiliYazismalarDetayTable
                                                        [0];
                                                dahiliYazismalarDetayTable.isReplied = true;
                                                dahiliYazismalarDetayTable.approved = true;
                                                dahiliYazismalarDetayTable.replyDate = DateTime.Now;
                                                await bllDahiliYazismalarDetayTable.Update(dahiliYazismalarDetayTable);

                                                dahiliYazismaTable.onaylandiMi = true;

                                                Data.Models.DahiliYazismaMessage dahiliYazismaMessage = new Data.Models.DahiliYazismaMessage();
                                                dahiliYazismaMessage.createdDate = DateTime.Now;
                                                dahiliYazismaMessage.showAll = true;
                                                dahiliYazismaMessage.dahiliYazismaId = dahiliYazismaTable.id ?? 0;
                                                dahiliYazismaMessage.userId = user.Id;
                                                dahiliYazismaMessage.message = responseMyList.kanalGorusuFirst;
                                                dahiliYazismaMessage.enabled = true;
                                                BLLActions.DahiliYazismaMessage bllDahiliYazismaMessage = new BLLActions.DahiliYazismaMessage(_configuration, _env);
                                                await bllDahiliYazismaMessage.Add(dahiliYazismaMessage);

                                                Data.Models.DahiliYazismalarDetayTable dahiliYazismalarDetayTableNext = new Data.Models.DahiliYazismalarDetayTable();
                                                dahiliYazismalarDetayTableNext.userId = dahiliYazismaTable.kanalGorusuUserId ?? 0;
                                                dahiliYazismalarDetayTableNext.createdDate = DateTime.Now;
                                                dahiliYazismalarDetayTableNext.approved = null;
                                                dahiliYazismalarDetayTableNext.dahiliYazismaId = dahiliYazismaTable.id ?? 0;
                                                dahiliYazismalarDetayTableNext.sonOnayMi = true;
                                                dahiliYazismalarDetayTableNext.isReplied = false;
                                                dahiliYazismalarDetayTableNext.replyDate = null;
                                                dahiliYazismalarDetayTableNext.enabled = true;
                                                await bllDahiliYazismalarDetayTable.Add(dahiliYazismalarDetayTableNext);

                                                EmailMessage emailMessage = new EmailMessage();
                                                emailMessage.toAddress = kanalGorusuUser.email;
                                                emailMessage.subject = dahiliYazismaTable.konu + " hk.";
                                                emailMessage.isSent = false; ;
                                                emailMessage.emailText = buildDahiliYazisma(_mapper.Map<Data.Models.DahiliYazismaTable>(dahiliYazismaTable));
                                                emailMessage.mailTuru = 4;
                                                emailMessage.plannedDate = DateTime.Now;
                                                emailMessage.enabled = true;
                                                BLLActions.EmailMessages bllEmailMessages = new BLLActions.EmailMessages(_configuration, _env);
                                                await bllEmailMessages.Add(emailMessage);

                                                if (dahiliYazismaTable.id == null)
                                                {
                                                    await Add(_mapper.Map<Data.Models.DahiliYazismaTable>(dahiliYazismaTable));
                                                }
                                                else
                                                {
                                                    await Update(_mapper.Map<Data.Models.DahiliYazismaTable>(dahiliYazismaTable));
                                                }
                                                if (dahiliYazismaTable.bilgiUserId1 != null)
                                                {
                                                    UserByNameEMailDto bilgiUser1 = bllAdminUsers
                                                            .getUserByNameAndEmail(dahiliYazismaTable.bilgiUserId1 ?? 0);
                                                    EmailMessage emailMessageBilgi = new EmailMessage();
                                                    emailMessageBilgi.toAddress = bilgiUser1.email;
                                                    emailMessageBilgi
                                                            .subject = dahiliYazismaTable.konu + " Bilgiye Eklendiniz";
                                                    emailMessageBilgi.isSent = false; ;
                                                    emailMessageBilgi.emailText = buildDahiliYazisma(_mapper.Map<Data.Models.DahiliYazismaTable>(dahiliYazismaTable));
                                                    emailMessageBilgi.mailTuru = 4;
                                                    emailMessageBilgi.plannedDate = DateTime.Now;
                                                    emailMessageBilgi.enabled = true;
                                                    await bllEmailMessages.Add(emailMessage);
                                                }
                                                if (dahiliYazismaTable.bilgiUserId2 != null)
                                                {
                                                    UserByNameEMailDto bilgiUser2 = bllAdminUsers
                                                            .getUserByNameAndEmail(dahiliYazismaTable.bilgiUserId2 ?? 0);
                                                    EmailMessage emailMessageBilgi = new EmailMessage();
                                                    emailMessageBilgi.toAddress = bilgiUser2.email;
                                                    emailMessageBilgi
                                                            .subject = dahiliYazismaTable.konu + " Bilgiye Eklendiniz";
                                                    emailMessageBilgi.isSent = false; ;
                                                    emailMessageBilgi.emailText = buildDahiliYazisma(_mapper.Map<Data.Models.DahiliYazismaTable>(dahiliYazismaTable));
                                                    emailMessageBilgi.mailTuru = 4;
                                                    emailMessageBilgi.plannedDate = DateTime.Now;
                                                    emailMessageBilgi.enabled = true;
                                                    await bllEmailMessages.Add(emailMessage);
                                                }
                                                if (dahiliYazismaTable.bilgiUserId3 != null)
                                                {
                                                    UserByNameEMailDto bilgiUser3 = bllAdminUsers
                                                            .getUserByNameAndEmail(dahiliYazismaTable.bilgiUserId3 ?? 0);
                                                    EmailMessage emailMessageBilgi = new EmailMessage();
                                                    emailMessageBilgi.toAddress = bilgiUser3.email;
                                                    emailMessageBilgi
                                                            .subject = dahiliYazismaTable.konu + " Bilgiye Eklendiniz";
                                                    emailMessageBilgi.isSent = false; ;
                                                    emailMessageBilgi.emailText = buildDahiliYazisma(_mapper.Map<Data.Models.DahiliYazismaTable>(dahiliYazismaTable));
                                                    emailMessageBilgi.mailTuru = 4;
                                                    emailMessageBilgi.plannedDate = DateTime.Now;
                                                    emailMessageBilgi.enabled = true;
                                                    await bllEmailMessages.Add(emailMessage);
                                                }
                                                if (dahiliYazismaTable.bilgiUserId4 != null)
                                                {
                                                    UserByNameEMailDto bilgiUser4 = bllAdminUsers
                                                            .getUserByNameAndEmail(dahiliYazismaTable.bilgiUserId4 ?? 0);
                                                    EmailMessage emailMessageBilgi = new EmailMessage();
                                                    emailMessageBilgi.toAddress = bilgiUser4.email;
                                                    emailMessageBilgi
                                                            .subject = dahiliYazismaTable.konu + " Bilgiye Eklendiniz";
                                                    emailMessageBilgi.isSent = false; ;
                                                    emailMessageBilgi.emailText = buildDahiliYazisma(_mapper.Map<Data.Models.DahiliYazismaTable>(dahiliYazismaTable));
                                                    emailMessageBilgi.mailTuru = 4;
                                                    emailMessageBilgi.plannedDate = DateTime.Now;
                                                    emailMessageBilgi.enabled = true;
                                                    await bllEmailMessages.Add(emailMessage);
                                                }
                                                if (dahiliYazismaTable.bilgiUserId5 != null)
                                                {
                                                    UserByNameEMailDto bilgiUser5 = bllAdminUsers
                                                            .getUserByNameAndEmail(dahiliYazismaTable.bilgiUserId5 ?? 0);
                                                    EmailMessage emailMessageBilgi = new EmailMessage();
                                                    emailMessageBilgi.toAddress = bilgiUser5.email;
                                                    emailMessageBilgi
                                                            .subject = dahiliYazismaTable.konu + " Bilgiye Eklendiniz";
                                                    emailMessageBilgi.isSent = false; ;
                                                    emailMessageBilgi.emailText = buildDahiliYazisma(_mapper.Map<Data.Models.DahiliYazismaTable>(dahiliYazismaTable));
                                                    emailMessageBilgi.mailTuru = 4;
                                                    emailMessageBilgi.plannedDate = DateTime.Now;
                                                    emailMessageBilgi.enabled = true;
                                                    await bllEmailMessages.Add(emailMessage);
                                                }

                                                if (dahiliYazismaTable.id == null)
                                                {
                                                    await Add(_mapper.Map<Data.Models.DahiliYazismaTable>(dahiliYazismaTable));
                                                }
                                                else
                                                {
                                                    await Update(_mapper.Map<Data.Models.DahiliYazismaTable>(dahiliYazismaTable));
                                                }
                                                return 1;
                                            }
                                            else
                                            {
                                                return 3;
                                            }
                                        }
                                    }

                                }
                                else
                                {

                                    if (bolumUserHierarchyTable != null && dahiliYazismaTable.kanalBittiMi != true && bolumUserHierarchyTable.userId == user.Id)
                                    {
                                        BLLActions.DahiliYazismalarDetayTable bllDahiliYazismalarDetayTable = new BLLActions.DahiliYazismalarDetayTable(_configuration, _env);
                                        List<Data.Models.DahiliYazismalarDetayTable> listDahiliYazismalarDetayTable = bllDahiliYazismalarDetayTable
                                                .findAllByDahiliYazismaIdAndApprovedAndUserIdAndEnabled(dahiliYazismaTable.id,
                                                        null, user.Id, true);

                                        for (int j = 1; j < listDahiliYazismalarDetayTable.Count(); j++)
                                        {
                                            int id = listDahiliYazismalarDetayTable[j].Id;
                                            bllDahiliYazismalarDetayTable.Delete(id);
                                        }
                                        Data.Models.DahiliYazismalarDetayTable dahiliYazismalarDetayTable = listDahiliYazismalarDetayTable
                                                [0];
                                        dahiliYazismalarDetayTable.isReplied = true;
                                        dahiliYazismalarDetayTable.approved = true;
                                        dahiliYazismalarDetayTable.replyDate = DateTime.Now;
                                        dahiliYazismalarDetayTable.kanalBittiMi = true;
                                        await bllDahiliYazismalarDetayTable.Update(dahiliYazismalarDetayTable);
                                        dahiliYazismaTable.kanalBittiMi = true;

                                        Data.Models.DahiliYazismalarDetayTable dahiliYazismalarDetayTableNext = new Data.Models.DahiliYazismalarDetayTable();
                                        dahiliYazismalarDetayTableNext.userId = ceoTable.userId;
                                        dahiliYazismalarDetayTableNext.createdDate = DateTime.Now;
                                        dahiliYazismalarDetayTableNext.approved = null;
                                        dahiliYazismalarDetayTableNext.dahiliYazismaId = dahiliYazismaTable.id ?? 0;
                                        dahiliYazismalarDetayTableNext.isReplied = false;
                                        dahiliYazismalarDetayTableNext.replyDate = null;
                                        dahiliYazismalarDetayTableNext.enabled = true;

                                        await bllDahiliYazismalarDetayTable.Add(dahiliYazismalarDetayTableNext);
                                        //							EmailMessage emailMessage = new EmailMessage();
                                        //							emailMessage.toAddress=ceoUser.email;
                                        //							emailMessage.subject=dahiliYazismaTable.konu + " hk.");
                                        //							emailMessage.isSent=false;;
                                        //							emailMessage.emailText=buildDahiliYazisma(_mapper.Map<Data.Models.DahiliYazismaTable>(dahiliYazismaTable)));
                                        //							emailMessage.mailTuru=4;
                                        //							emailMessage.plannedDate=DateTime.Now;
                                        //							emailMessage.enabled=true;
                                        //							await bllEmailMessages.Add(emailMessage);

                                        SMSMessage smsMessage = new SMSMessage();
                                        smsMessage.plannedDate = DateTime.Now;
                                        smsMessage.isSent = false; ;
                                        smsMessage.smsText = (
                                                dahiliYazismaTable.id.ToString() + "Id'li" + dahiliYazismaTable.konu
                                                        + " konulu " + " Dahili Yazışma onayınızı beklemektedir.");
                                        smsMessage.toNumbers = ceoUser.mobile;

                                        BLLActions.SMSMessages bllSMSMessages = new BLLActions.SMSMessages(_configuration, _env);
                                        await bllSMSMessages.Add(smsMessage);
                                        if (dahiliYazismaTable.id == null)
                                        {
                                            await Add(_mapper.Map<Data.Models.DahiliYazismaTable>(dahiliYazismaTable));
                                        }
                                        else
                                        {
                                            await Update(_mapper.Map<Data.Models.DahiliYazismaTable>(dahiliYazismaTable));
                                        }
                                        return 1;
                                    }
                                    else if (ceoTable.userId.Equals(user.Id))
                                    {

                                        if (dahiliYazismaTable.kanalGorusuUserId != null)
                                        {
                                            UserByNameEMailDto kanalGorusuUser = bllAdminUsers
                                                    .getUserByNameAndEmail(dahiliYazismaTable.kanalGorusuUserId ?? 0);
                                            BLLActions.DahiliYazismalarDetayTable bllDahiliYazismalarDetayTable = new BLLActions.DahiliYazismalarDetayTable(_configuration, _env);
                                            List<Data.Models.DahiliYazismalarDetayTable> listDahiliYazismalarDetayTable = bllDahiliYazismalarDetayTable
                                                    .findAllByDahiliYazismaIdAndApprovedAndUserIdAndEnabled(
                                                            dahiliYazismaTable.id, null, user.Id, true);

                                            for (int j = 1; j < listDahiliYazismalarDetayTable.Count(); j++)
                                            {
                                                int id = listDahiliYazismalarDetayTable[j].Id;
                                                bllDahiliYazismalarDetayTable.Delete(id);
                                            }
                                            Data.Models.DahiliYazismalarDetayTable dahiliYazismalarDetayTable = listDahiliYazismalarDetayTable
                                                    [0];
                                            dahiliYazismalarDetayTable.isReplied = true;
                                            dahiliYazismalarDetayTable.approved = true;
                                            dahiliYazismalarDetayTable.replyDate = DateTime.Now;
                                            await bllDahiliYazismalarDetayTable.Update(dahiliYazismalarDetayTable);

                                            dahiliYazismaTable.onaylandiMi = true;

                                            Data.Models.DahiliYazismaMessage dahiliYazismaMessage = new Data.Models.DahiliYazismaMessage();
                                            dahiliYazismaMessage.createdDate = DateTime.Now;
                                            dahiliYazismaMessage.showAll = true;
                                            dahiliYazismaMessage.dahiliYazismaId = dahiliYazismaTable.id ?? 0;
                                            dahiliYazismaMessage.userId = user.Id;
                                            dahiliYazismaMessage.message = responseMyList.kanalGorusuFirst;
                                            dahiliYazismaMessage.enabled = true;
                                            BLLActions.DahiliYazismaMessage bllDahiliYazismaMessage = new BLLActions.DahiliYazismaMessage(_configuration, _env);
                                            await bllDahiliYazismaMessage.Add(dahiliYazismaMessage);

                                            Data.Models.DahiliYazismalarDetayTable dahiliYazismalarDetayTableNext = new Data.Models.DahiliYazismalarDetayTable();
                                            dahiliYazismalarDetayTableNext.userId = dahiliYazismaTable.kanalGorusuUserId ?? 0;
                                            dahiliYazismalarDetayTableNext.createdDate = DateTime.Now;
                                            dahiliYazismalarDetayTableNext.approved = null;
                                            dahiliYazismalarDetayTableNext.dahiliYazismaId = dahiliYazismaTable.id ?? 0;
                                            dahiliYazismalarDetayTableNext.sonOnayMi = true;
                                            dahiliYazismalarDetayTableNext.isReplied = false;
                                            dahiliYazismalarDetayTableNext.replyDate = null;
                                            dahiliYazismalarDetayTableNext.enabled = true;

                                            await bllDahiliYazismalarDetayTable.Add(dahiliYazismalarDetayTableNext);
                                            EmailMessage emailMessage = new EmailMessage();
                                            emailMessage.toAddress = kanalGorusuUser.email;
                                            emailMessage.subject = dahiliYazismaTable.konu + " hk.";
                                            emailMessage.isSent = false; ;
                                            emailMessage.emailText = buildDahiliYazisma(_mapper.Map<Data.Models.DahiliYazismaTable>(dahiliYazismaTable));
                                            emailMessage.mailTuru = 4;
                                            emailMessage.plannedDate = DateTime.Now;
                                            emailMessage.enabled = true;
                                            BLLActions.EmailMessages bllEmailMessages = new BLLActions.EmailMessages(_configuration, _env);
                                            await bllEmailMessages.Add(emailMessage);
                                            if (dahiliYazismaTable.id == null)
                                            {
                                                await Add(_mapper.Map<Data.Models.DahiliYazismaTable>(dahiliYazismaTable));
                                            }
                                            else
                                            {
                                                await Update(_mapper.Map<Data.Models.DahiliYazismaTable>(dahiliYazismaTable));
                                            }
                                            if (dahiliYazismaTable.bilgiUserId1 != null)
                                            {
                                                UserByNameEMailDto bilgiUser1 = bllAdminUsers
                                                        .getUserByNameAndEmail(dahiliYazismaTable.bilgiUserId1 ?? 0);
                                                EmailMessage emailMessageBilgi = new EmailMessage();
                                                emailMessageBilgi.toAddress = bilgiUser1.email;
                                                emailMessageBilgi.subject = dahiliYazismaTable.konu + " Bilgiye Eklendiniz";
                                                emailMessageBilgi.isSent = false; ;
                                                emailMessageBilgi.emailText = buildDahiliYazisma(_mapper.Map<Data.Models.DahiliYazismaTable>(dahiliYazismaTable));
                                                emailMessageBilgi.mailTuru = 4;
                                                emailMessageBilgi.plannedDate = DateTime.Now;
                                                emailMessageBilgi.enabled = true;
                                                await bllEmailMessages.Add(emailMessage);
                                            }
                                            if (dahiliYazismaTable.bilgiUserId2 != null)
                                            {
                                                UserByNameEMailDto bilgiUser2 = bllAdminUsers
                                                        .getUserByNameAndEmail(dahiliYazismaTable.bilgiUserId2 ?? 0);
                                                EmailMessage emailMessageBilgi = new EmailMessage();
                                                emailMessageBilgi.toAddress = bilgiUser2.email;
                                                emailMessageBilgi.subject = dahiliYazismaTable.konu + " Bilgiye Eklendiniz";
                                                emailMessageBilgi.isSent = false; ;
                                                emailMessageBilgi.emailText = buildDahiliYazisma(_mapper.Map<Data.Models.DahiliYazismaTable>(dahiliYazismaTable));
                                                emailMessageBilgi.mailTuru = 4;
                                                emailMessageBilgi.plannedDate = DateTime.Now;
                                                emailMessageBilgi.enabled = true;
                                                await bllEmailMessages.Add(emailMessage);
                                            }
                                            if (dahiliYazismaTable.bilgiUserId3 != null)
                                            {
                                                UserByNameEMailDto bilgiUser3 = bllAdminUsers
                                                        .getUserByNameAndEmail(dahiliYazismaTable.bilgiUserId3 ?? 0);
                                                EmailMessage emailMessageBilgi = new EmailMessage();
                                                emailMessageBilgi.toAddress = bilgiUser3.email;
                                                emailMessageBilgi.subject = dahiliYazismaTable.konu + " Bilgiye Eklendiniz";
                                                emailMessageBilgi.isSent = false; ;
                                                emailMessageBilgi.emailText = buildDahiliYazisma(_mapper.Map<Data.Models.DahiliYazismaTable>(dahiliYazismaTable));
                                                emailMessageBilgi.mailTuru = 4;
                                                emailMessageBilgi.plannedDate = DateTime.Now;
                                                emailMessageBilgi.enabled = true;
                                                await bllEmailMessages.Add(emailMessage);
                                            }
                                            if (dahiliYazismaTable.bilgiUserId4 != null)
                                            {
                                                UserByNameEMailDto bilgiUser4 = bllAdminUsers
                                                        .getUserByNameAndEmail(dahiliYazismaTable.bilgiUserId4 ?? 0);
                                                EmailMessage emailMessageBilgi = new EmailMessage();
                                                emailMessageBilgi.toAddress = bilgiUser4.email;
                                                emailMessageBilgi.subject = dahiliYazismaTable.konu + " Bilgiye Eklendiniz";
                                                emailMessageBilgi.isSent = false; ;
                                                emailMessageBilgi.emailText = buildDahiliYazisma(_mapper.Map<Data.Models.DahiliYazismaTable>(dahiliYazismaTable));
                                                emailMessageBilgi.mailTuru = 4;
                                                emailMessageBilgi.plannedDate = DateTime.Now;
                                                emailMessageBilgi.enabled = true;
                                                await bllEmailMessages.Add(emailMessage);
                                            }
                                            if (dahiliYazismaTable.bilgiUserId5 != null)
                                            {
                                                UserByNameEMailDto bilgiUser5 = bllAdminUsers
                                                        .getUserByNameAndEmail(dahiliYazismaTable.bilgiUserId5 ?? 0);
                                                EmailMessage emailMessageBilgi = new EmailMessage();
                                                emailMessageBilgi.toAddress = bilgiUser5.email;
                                                emailMessageBilgi.subject = dahiliYazismaTable.konu + " Bilgiye Eklendiniz";
                                                emailMessageBilgi.isSent = false; ;
                                                emailMessageBilgi.emailText = buildDahiliYazisma(_mapper.Map<Data.Models.DahiliYazismaTable>(dahiliYazismaTable));
                                                emailMessageBilgi.mailTuru = 4;
                                                emailMessageBilgi.plannedDate = DateTime.Now;
                                                emailMessageBilgi.enabled = true;
                                                await bllEmailMessages.Add(emailMessage);
                                            }
                                            if (dahiliYazismaTable.id == null)
                                            {
                                                await Add(_mapper.Map<Data.Models.DahiliYazismaTable>(dahiliYazismaTable));
                                            }
                                            else
                                            {
                                                await Update(_mapper.Map<Data.Models.DahiliYazismaTable>(dahiliYazismaTable));
                                            }
                                            return 1;
                                        }
                                        else
                                        {
                                            return 3;
                                        }
                                    }
                                }
                            }
                            else
                            {

                                if (bolumUserHierarchyTable != null && dahiliYazismaTable.kanalBittiMi != true && bolumUserHierarchyTable.userId == user.Id)
                                {
                                    BLLActions.DahiliYazismalarDetayTable bllDahiliYazismalarDetayTable = new BLLActions.DahiliYazismalarDetayTable(_configuration, _env);
                                    List<Data.Models.DahiliYazismalarDetayTable> listDahiliYazismalarDetayTable = bllDahiliYazismalarDetayTable
                                            .findAllByDahiliYazismaIdAndApprovedAndUserIdAndEnabled(dahiliYazismaTable.id,
                                                    null, user.Id, true);

                                    for (int j = 1; j < listDahiliYazismalarDetayTable.Count(); j++)
                                    {
                                        int id = listDahiliYazismalarDetayTable[j].Id;
                                        bllDahiliYazismalarDetayTable.Delete(id);
                                    }
                                    Data.Models.DahiliYazismalarDetayTable dahiliYazismalarDetayTable = listDahiliYazismalarDetayTable[0];
                                    dahiliYazismalarDetayTable.isReplied = true;
                                    dahiliYazismalarDetayTable.approved = true;
                                    dahiliYazismalarDetayTable.replyDate = DateTime.Now;
                                    dahiliYazismalarDetayTable.kanalBittiMi = true;
                                    await bllDahiliYazismalarDetayTable.Update(dahiliYazismalarDetayTable);

                                    dahiliYazismaTable.kanalBittiMi = true;
                                    Data.Models.DahiliYazismalarDetayTable dahiliYazismalarDetayTableNext = new Data.Models.DahiliYazismalarDetayTable();
                                    dahiliYazismalarDetayTableNext.userId = ceoTable.userId;
                                    dahiliYazismalarDetayTableNext.createdDate = DateTime.Now;
                                    dahiliYazismalarDetayTableNext.approved = null;
                                    dahiliYazismalarDetayTableNext.dahiliYazismaId = dahiliYazismaTable.id ?? 0;
                                    dahiliYazismalarDetayTableNext.isReplied = false;
                                    dahiliYazismalarDetayTableNext.replyDate = null;
                                    dahiliYazismalarDetayTableNext.enabled = true;

                                    await bllDahiliYazismalarDetayTable.Add(dahiliYazismalarDetayTableNext);
                                    //						EmailMessage emailMessage = new EmailMessage();
                                    //						emailMessage.toAddress=ceoUser.email;
                                    //						emailMessage.subject=dahiliYazismaTable.konu + " hk.");
                                    //						emailMessage.isSent=false;;
                                    //						emailMessage.emailText=buildDahiliYazisma(_mapper.Map<Data.Models.DahiliYazismaTable>(dahiliYazismaTable)));
                                    //						emailMessage.mailTuru=4;
                                    //						emailMessage.plannedDate=DateTime.Now;
                                    //						emailMessage.enabled=true;
                                    //						await bllEmailMessages.Add(emailMessage);

                                    SMSMessage smsMessage = new SMSMessage();
                                    smsMessage.plannedDate = DateTime.Now;
                                    smsMessage.isSent = false; ;
                                    smsMessage.smsText = (
                                            dahiliYazismaTable.id.ToString() + "Id'li" + dahiliYazismaTable.konu
                                                    + " konulu " + " Dahili Yazışma onayınızı beklemektedir.");
                                    smsMessage.toNumbers = ceoUser.mobile;

                                    BLLActions.SMSMessages bllSMSMessages = new BLLActions.SMSMessages(_configuration, _env);
                                    await bllSMSMessages.Add(smsMessage);
                                    if (dahiliYazismaTable.id == null)
                                    {
                                        await Add(_mapper.Map<Data.Models.DahiliYazismaTable>(dahiliYazismaTable));
                                    }
                                    else
                                    {
                                        await Update(_mapper.Map<Data.Models.DahiliYazismaTable>(dahiliYazismaTable));
                                    }
                                    return 1;
                                }
                                else if (ceoTable.userId.Equals(user.Id))
                                {

                                    if (dahiliYazismaTable.kanalGorusuUserId != null)
                                    {
                                        UserByNameEMailDto kanalGorusuUser = bllAdminUsers
                                                .getUserByNameAndEmail(dahiliYazismaTable.kanalGorusuUserId ?? 0);
                                        BLLActions.DahiliYazismalarDetayTable bllDahiliYazismalarDetayTable = new BLLActions.DahiliYazismalarDetayTable(_configuration, _env);
                                        List<Data.Models.DahiliYazismalarDetayTable> listDahiliYazismalarDetayTable = bllDahiliYazismalarDetayTable
                                                .findAllByDahiliYazismaIdAndApprovedAndUserIdAndEnabled(dahiliYazismaTable.id,
                                                        null, user.Id, true);

                                        for (int j = 1; j < listDahiliYazismalarDetayTable.Count(); j++)
                                        {
                                            int id = listDahiliYazismalarDetayTable[j].Id;
                                            bllDahiliYazismalarDetayTable.Delete(id);
                                        }
                                        Data.Models.DahiliYazismalarDetayTable dahiliYazismalarDetayTable = listDahiliYazismalarDetayTable
                                                [0];
                                        dahiliYazismalarDetayTable.isReplied = true;
                                        dahiliYazismalarDetayTable.approved = true;
                                        dahiliYazismalarDetayTable.replyDate = DateTime.Now;
                                        await bllDahiliYazismalarDetayTable.Update(dahiliYazismalarDetayTable);

                                        dahiliYazismaTable.onaylandiMi = true;

                                        Data.Models.DahiliYazismaMessage dahiliYazismaMessage = new Data.Models.DahiliYazismaMessage();
                                        dahiliYazismaMessage.createdDate = DateTime.Now;
                                        dahiliYazismaMessage.showAll = true;
                                        dahiliYazismaMessage.dahiliYazismaId = dahiliYazismaTable.id ?? 0;
                                        dahiliYazismaMessage.userId = user.Id;
                                        dahiliYazismaMessage.message = responseMyList.kanalGorusuFirst;
                                        dahiliYazismaMessage.enabled = true;
                                        BLLActions.DahiliYazismaMessage bllDahiliYazismaMessage = new BLLActions.DahiliYazismaMessage(_configuration, _env);
                                        await bllDahiliYazismaMessage.Add(dahiliYazismaMessage);

                                        Data.Models.DahiliYazismalarDetayTable dahiliYazismalarDetayTableNext = new Data.Models.DahiliYazismalarDetayTable();
                                        dahiliYazismalarDetayTableNext.userId = dahiliYazismaTable.kanalGorusuUserId ?? 0;
                                        dahiliYazismalarDetayTableNext.createdDate = DateTime.Now;
                                        dahiliYazismalarDetayTableNext.approved = null;
                                        dahiliYazismalarDetayTableNext.dahiliYazismaId = dahiliYazismaTable.id ?? 0;
                                        dahiliYazismalarDetayTableNext.sonOnayMi = true;
                                        dahiliYazismalarDetayTableNext.isReplied = false;
                                        dahiliYazismalarDetayTableNext.replyDate = null;
                                        dahiliYazismalarDetayTableNext.enabled = true;
                                        await bllDahiliYazismalarDetayTable.Add(dahiliYazismalarDetayTableNext);

                                        EmailMessage emailMessage = new EmailMessage();
                                        emailMessage.toAddress = kanalGorusuUser.email;
                                        emailMessage.subject = dahiliYazismaTable.konu + " hk.";
                                        emailMessage.isSent = false; ;
                                        emailMessage.emailText = buildDahiliYazisma(_mapper.Map<Data.Models.DahiliYazismaTable>(dahiliYazismaTable));
                                        emailMessage.mailTuru = 4;
                                        emailMessage.plannedDate = DateTime.Now;
                                        emailMessage.enabled = true;
                                        BLLActions.EmailMessages bllEmailMessages = new BLLActions.EmailMessages(_configuration, _env);
                                        await bllEmailMessages.Add(emailMessage);
                                        if (dahiliYazismaTable.id == null)
                                        {
                                            await Add(_mapper.Map<Data.Models.DahiliYazismaTable>(dahiliYazismaTable));
                                        }
                                        else
                                        {
                                            await Update(_mapper.Map<Data.Models.DahiliYazismaTable>(dahiliYazismaTable));
                                        }
                                        if (dahiliYazismaTable.bilgiUserId1 != null)
                                        {
                                            UserByNameEMailDto bilgiUser1 = bllAdminUsers
                                                    .getUserByNameAndEmail(dahiliYazismaTable.bilgiUserId1 ?? 0);
                                            EmailMessage emailMessageBilgi = new EmailMessage();
                                            emailMessageBilgi.toAddress = bilgiUser1.email;
                                            emailMessageBilgi.subject = dahiliYazismaTable.konu + " Bilgiye Eklendiniz";
                                            emailMessageBilgi.isSent = false; ;
                                            emailMessageBilgi.emailText = buildDahiliYazisma(_mapper.Map<Data.Models.DahiliYazismaTable>(dahiliYazismaTable));
                                            emailMessageBilgi.mailTuru = 4;
                                            emailMessageBilgi.plannedDate = DateTime.Now;
                                            emailMessageBilgi.enabled = true;
                                            await bllEmailMessages.Add(emailMessage);
                                        }
                                        if (dahiliYazismaTable.bilgiUserId2 != null)
                                        {
                                            UserByNameEMailDto bilgiUser2 = bllAdminUsers
                                                    .getUserByNameAndEmail(dahiliYazismaTable.bilgiUserId2 ?? 0);
                                            EmailMessage emailMessageBilgi = new EmailMessage();
                                            emailMessageBilgi.toAddress = bilgiUser2.email;
                                            emailMessageBilgi.subject = dahiliYazismaTable.konu + " Bilgiye Eklendiniz";
                                            emailMessageBilgi.isSent = false; ;
                                            emailMessageBilgi.emailText = buildDahiliYazisma(_mapper.Map<Data.Models.DahiliYazismaTable>(dahiliYazismaTable));
                                            emailMessageBilgi.mailTuru = 4;
                                            emailMessageBilgi.plannedDate = DateTime.Now;
                                            emailMessageBilgi.enabled = true;
                                            await bllEmailMessages.Add(emailMessage);
                                        }
                                        if (dahiliYazismaTable.bilgiUserId3 != null)
                                        {
                                            UserByNameEMailDto bilgiUser3 = bllAdminUsers
                                                    .getUserByNameAndEmail(dahiliYazismaTable.bilgiUserId3 ?? 0);
                                            EmailMessage emailMessageBilgi = new EmailMessage();
                                            emailMessageBilgi.toAddress = bilgiUser3.email;
                                            emailMessageBilgi.subject = dahiliYazismaTable.konu + " Bilgiye Eklendiniz";
                                            emailMessageBilgi.isSent = false; ;
                                            emailMessageBilgi.emailText = buildDahiliYazisma(_mapper.Map<Data.Models.DahiliYazismaTable>(dahiliYazismaTable));
                                            emailMessageBilgi.mailTuru = 4;
                                            emailMessageBilgi.plannedDate = DateTime.Now;
                                            emailMessageBilgi.enabled = true;
                                            await bllEmailMessages.Add(emailMessage);
                                        }
                                        if (dahiliYazismaTable.bilgiUserId4 != null)
                                        {
                                            UserByNameEMailDto bilgiUser4 = bllAdminUsers
                                                    .getUserByNameAndEmail(dahiliYazismaTable.bilgiUserId4 ?? 0);
                                            EmailMessage emailMessageBilgi = new EmailMessage();
                                            emailMessageBilgi.toAddress = bilgiUser4.email;
                                            emailMessageBilgi.subject = dahiliYazismaTable.konu + " Bilgiye Eklendiniz";
                                            emailMessageBilgi.isSent = false; ;
                                            emailMessageBilgi.emailText = buildDahiliYazisma(_mapper.Map<Data.Models.DahiliYazismaTable>(dahiliYazismaTable));
                                            emailMessageBilgi.mailTuru = 4;
                                            emailMessageBilgi.plannedDate = DateTime.Now;
                                            emailMessageBilgi.enabled = true;
                                            await bllEmailMessages.Add(emailMessage);
                                        }
                                        if (dahiliYazismaTable.bilgiUserId5 != null)
                                        {
                                            UserByNameEMailDto bilgiUser5 = bllAdminUsers
                                                    .getUserByNameAndEmail(dahiliYazismaTable.bilgiUserId5 ?? 0);
                                            EmailMessage emailMessageBilgi = new EmailMessage();
                                            emailMessageBilgi.toAddress = bilgiUser5.email;
                                            emailMessageBilgi.subject = dahiliYazismaTable.konu + " Bilgiye Eklendiniz";
                                            emailMessageBilgi.isSent = false; ;
                                            emailMessageBilgi.emailText = buildDahiliYazisma(_mapper.Map<Data.Models.DahiliYazismaTable>(dahiliYazismaTable));
                                            emailMessageBilgi.mailTuru = 4;
                                            emailMessageBilgi.plannedDate = DateTime.Now;
                                            emailMessageBilgi.enabled = true;
                                            await bllEmailMessages.Add(emailMessage);
                                        }
                                        if (dahiliYazismaTable.id == null)
                                        {
                                            await Add(_mapper.Map<Data.Models.DahiliYazismaTable>(dahiliYazismaTable));
                                        }
                                        else
                                        {
                                            await Update(_mapper.Map<Data.Models.DahiliYazismaTable>(dahiliYazismaTable));
                                        }
                                        return 1;
                                    }
                                    else
                                    {
                                        return 3;
                                    }
                                }
                            }

                        }
                        else

                        {
                            return 4;
                        }
                        return 0;
                    }
                    else
                    {
                        return 4;
                    }
                }
                catch
                //(Exception e)
                {
                    //System.out.println(
                    //        getUser.getUser().name + "," + responseMyList.getDahiliYazismaTable().Id.ToString()
                    //                + " id'li dahili yazışmayı onaylayamıyor. Hata: " + e.getMessage());
                    return 4;
                }
            }

            public async Task<int> approvecanal(ResponseMyList responseMyList, int userId)
            {
                Data.Models.DahiliYazismaTable? dahiliYazismaTable = _mapper.Map<Data.Models.DahiliYazismaTable>(responseMyList.dahiliYazismaTable);
                if (dahiliYazismaTable != null)
                {
                    if (dahiliYazismaTable.lastUserId == 0)
                    {
                        BLLActions.AdminUsers bllAdminUsers = new BLLActions.AdminUsers(_configuration, _env, _mapper);
                        AdminUser user = bllAdminUsers.GetByID(userId)!;

                        AdminUser createdUser = bllAdminUsers.GetByID(dahiliYazismaTable.createdUserId)!;
                        try
                        {
                            BLLActions.DahiliYazismalarDetayTable bllDahiliYazismalarDetayTable = new BLLActions.DahiliYazismalarDetayTable(_configuration, _env);
                            List<Data.Models.DahiliYazismalarDetayTable> listDahiliYazismalarDetayTable = bllDahiliYazismalarDetayTable
                                    .findAllByDahiliYazismaIdAndApprovedAndUserIdAndEnabled(dahiliYazismaTable.Id, null,
                                            user.Id, true);

                            for (int j = 1; j < listDahiliYazismalarDetayTable.Count(); j++)
                            {
                                int id = listDahiliYazismalarDetayTable[j].Id;
                                bllDahiliYazismalarDetayTable.Delete(id);
                            }
                            Data.Models.DahiliYazismalarDetayTable dahiliYazismalarDetayTable = listDahiliYazismalarDetayTable[0];
                            dahiliYazismalarDetayTable.isReplied = true;
                            dahiliYazismalarDetayTable.approved = true;
                            dahiliYazismalarDetayTable.replyDate = DateTime.Now;
                            await bllDahiliYazismalarDetayTable.Update(dahiliYazismalarDetayTable);

                            Data.Models.DahiliYazismaMessage dahiliYazismaMessage = new Data.Models.DahiliYazismaMessage();
                            dahiliYazismaMessage.createdDate = DateTime.Now;
                            dahiliYazismaMessage.showAll = true;
                            dahiliYazismaMessage.userId = user.Id;
                            dahiliYazismaMessage.message = responseMyList.kanalGorusuFirst;
                            dahiliYazismaMessage.enabled = true;
                            dahiliYazismaMessage.dahiliYazismaId = dahiliYazismaTable.Id;
                            BLLActions.DahiliYazismaMessage bllDahiliYazismaMessage = new BLLActions.DahiliYazismaMessage(_configuration, _env);
                            await bllDahiliYazismaMessage.Add(dahiliYazismaMessage);

                            dahiliYazismaTable.bittiMi = true;
                            dahiliYazismaTable.mudurBittiMi = true;
                            dahiliYazismaTable.lastUserId = null;
                            if (dahiliYazismaTable?.Id == null)
                            {
                                await Add(dahiliYazismaTable!);
                            }
                            else
                            {
                                await Update(dahiliYazismaTable);
                            }
                            EmailMessage emailMessage = new EmailMessage();
                            emailMessage.toAddress = createdUser.email;
                            emailMessage.subject = dahiliYazismaTable?.konu + " hk.";
                            emailMessage.isSent = false;
                            emailMessage.emailText = "<h2>Sayın " + createdUser.name + "</h2><br/><h4>Bir adet "
                                    + dahiliYazismaTable?.Id.ToString() + " ID'li ve " + dahiliYazismaTable?.konu
                                    + " Konulu dahili yazışma bitmiştir.<br/></h4>" + buildDahiliYazisma(dahiliYazismaTable ?? new Data.Models.DahiliYazismaTable());
                            emailMessage.mailTuru = 4;
                            emailMessage.plannedDate = DateTime.Now;
                            emailMessage.enabled = true;
                            BLLActions.EmailMessages bllEmailMessages = new BLLActions.EmailMessages(_configuration, _env);
                            await bllEmailMessages.Add(emailMessage);
                            return 1;

                        }
                        catch
                        //(Exception e)
                        {
                            //System.out.println(
                            //        getUser.getUser().name + "," + responseMyList.getDahiliYazismaTable().Id.ToString()
                            //                + " id'li dahili yazışma bitirilemiyor. Hata: " + e.getMessage());
                            return 3;
                        }

                    }
                    else
                    {
                        BLLActions.AdminUsers bllAdminUsers = new BLLActions.AdminUsers(_configuration, _env, _mapper);
                        AdminUser? lastUser = bllAdminUsers.GetByID(dahiliYazismaTable.lastUserId ?? 0);
                        AdminUser? user = bllAdminUsers.GetByID(userId);
                        try
                        {
                            if (user?.Id == dahiliYazismaTable.kanalGorusuUserId)
                            {
                                BLLActions.DahiliYazismalarDetayTable bllDahiliYazismalarDetayTable = new BLLActions.DahiliYazismalarDetayTable(_configuration, _env);
                                List<Data.Models.DahiliYazismalarDetayTable> listDahiliYazismalarDetayTable = bllDahiliYazismalarDetayTable
                                        .findAllByDahiliYazismaIdAndApprovedAndUserIdAndEnabled(dahiliYazismaTable.Id, null,
                                                user?.Id ?? 0, true);

                                for (int j = 1; j < listDahiliYazismalarDetayTable.Count(); j++)
                                {
                                    int id = listDahiliYazismalarDetayTable[j].Id;
                                    bllDahiliYazismalarDetayTable.Delete(id);
                                }
                                Data.Models.DahiliYazismalarDetayTable dahiliYazismalarDetayTable = listDahiliYazismalarDetayTable[0];
                                dahiliYazismalarDetayTable.isReplied = true;
                                dahiliYazismalarDetayTable.approved = true;
                                dahiliYazismalarDetayTable.replyDate = DateTime.Now;
                                await bllDahiliYazismalarDetayTable.Update(dahiliYazismalarDetayTable);

                                Data.Models.DahiliYazismaMessage dahiliYazismaMessage = new Data.Models.DahiliYazismaMessage();
                                dahiliYazismaMessage.createdDate = DateTime.Now;
                                dahiliYazismaMessage.showAll = true;
                                dahiliYazismaMessage.userId = user?.Id ?? 0;
                                dahiliYazismaMessage.message = responseMyList.kanalGorusuFirst;
                                dahiliYazismaMessage.enabled = true;
                                dahiliYazismaMessage.dahiliYazismaId = dahiliYazismaTable.Id;
                                BLLActions.DahiliYazismaMessage bllDahiliYazismaMessage = new BLLActions.DahiliYazismaMessage(_configuration, _env);
                                await bllDahiliYazismaMessage.Add(dahiliYazismaMessage);

                                dahiliYazismaTable.kanalGorusuOkmi = true;
                                if (dahiliYazismaTable?.Id == null)
                                {
                                    await Add(dahiliYazismaTable!);
                                }
                                else
                                {
                                    await Update(dahiliYazismaTable);
                                }

                                Data.Models.DahiliYazismalarDetayTable dahiliYazismalarDetayTableNext = new Data.Models.DahiliYazismalarDetayTable();
                                dahiliYazismalarDetayTableNext.userId = dahiliYazismaTable?.lastUserId ?? 0;
                                dahiliYazismalarDetayTableNext.createdDate = DateTime.Now;
                                dahiliYazismalarDetayTableNext.approved = null;
                                dahiliYazismalarDetayTableNext.dahiliYazismaId = dahiliYazismaTable?.Id ?? 0;
                                dahiliYazismalarDetayTableNext.isReplied = false;
                                dahiliYazismalarDetayTableNext.replyDate = null;
                                dahiliYazismalarDetayTableNext.sonOnayMi = true;
                                dahiliYazismalarDetayTableNext.enabled = true;

                                await bllDahiliYazismalarDetayTable.Add(dahiliYazismalarDetayTableNext);

                                EmailMessage emailMessage = new EmailMessage();
                                emailMessage.toAddress = lastUser?.email;
                                emailMessage.subject = dahiliYazismaTable?.konu + " hk.";
                                emailMessage.isSent = false;
                                emailMessage.emailText = "<h2>Sayın " + lastUser?.name + "</h2><br/><h4>Bir adet "
                                        + dahiliYazismaTable?.Id.ToString() + " ID'li ve " + dahiliYazismaTable?.konu
                                        + " Konulu dahili yazışma yapılacak işiniz bulunmaktadır.<br/></h4>"
                                        + buildDahiliYazisma(dahiliYazismaTable ?? new Data.Models.DahiliYazismaTable());
                                emailMessage.mailTuru = 4;
                                emailMessage.plannedDate = DateTime.Now;
                                emailMessage.enabled = true;
                                BLLActions.EmailMessages bllEmailMessages = new BLLActions.EmailMessages(_configuration, _env);
                                await bllEmailMessages.Add(emailMessage);
                                return 1;
                            }
                            else
                            {
                                return 2;
                            }

                        }
                        catch
                        //(Exception e)
                        {
                            //System.out.println(getUser.getUser().name + "," + dahiliYazismaTable.Id.ToString()
                            //        + " id'li dahili yazışmayı kanal onaylayamıyor. Hata: " + e.getMessage());
                            return 3;
                        }
                    }

                }
                else
                {
                    return 3;
                }
            }

            public async Task<int> lastoperationapprove(ResponseMyList responseMyList, int userId)
            {
                Data.Models.DahiliYazismaTable dahiliYazismaTable = _mapper.Map<Data.Models.DahiliYazismaTable>(responseMyList.dahiliYazismaTable);

                BLLActions.AdminUsers bllAdminUsers = new BLLActions.AdminUsers(_configuration, _env, _mapper);
                AdminUser user = bllAdminUsers.GetByID(userId)!;
                AdminUser? lastUser2 = bllAdminUsers.GetByID(dahiliYazismaTable.lastUserId2 ?? 0);
                try
                {
                    if (user.Id == dahiliYazismaTable.lastUserId || user.Id == dahiliYazismaTable.lastUserId2)
                    {

                        if (dahiliYazismaTable.lastUserId2 != null)
                        {
                            BLLActions.DahiliYazismalarDetayTable bllDahiliYazismalarDetayTable = new BLLActions.DahiliYazismalarDetayTable(_configuration, _env);
                            List<Data.Models.DahiliYazismalarDetayTable> listDahiliYazismalarDetayTable = bllDahiliYazismalarDetayTable
                                    .findAllByDahiliYazismaIdAndApprovedAndUserIdAndEnabled(dahiliYazismaTable.Id, null,
                                            user.Id, true);

                            for (int j = 1; j < listDahiliYazismalarDetayTable.Count(); j++)
                            {
                                int id = listDahiliYazismalarDetayTable[j].Id;
                                bllDahiliYazismalarDetayTable.Delete(id);
                            }
                            Data.Models.DahiliYazismalarDetayTable dahiliYazismalarDetayTable = listDahiliYazismalarDetayTable[0];
                            dahiliYazismalarDetayTable.isReplied = true;
                            dahiliYazismalarDetayTable.approved = true;
                            dahiliYazismalarDetayTable.replyDate = DateTime.Now;
                            await bllDahiliYazismalarDetayTable.Update(dahiliYazismalarDetayTable);
                            Data.Models.DahiliYazismalarDetayTable dahiliYazismalarDetayTableNext = new Data.Models.DahiliYazismalarDetayTable();
                            dahiliYazismalarDetayTableNext.userId = dahiliYazismaTable.lastUserId2 ?? 0;
                            dahiliYazismalarDetayTableNext.createdDate = DateTime.Now;
                            dahiliYazismalarDetayTableNext.approved = null;
                            dahiliYazismalarDetayTableNext.dahiliYazismaId = dahiliYazismaTable.Id;
                            dahiliYazismalarDetayTableNext.isReplied = false;
                            dahiliYazismalarDetayTableNext.replyDate = null;
                            dahiliYazismalarDetayTableNext.sonOnayMi = true;
                            dahiliYazismalarDetayTableNext.enabled = true;

                            await bllDahiliYazismalarDetayTable.Add(dahiliYazismalarDetayTableNext);
                            Data.Models.DahiliYazismaMessage dahiliYazismaMessage = new Data.Models.DahiliYazismaMessage();
                            dahiliYazismaMessage.createdDate = DateTime.Now;
                            dahiliYazismaMessage.showAll = true;
                            dahiliYazismaMessage.userId = user.Id;
                            dahiliYazismaMessage.message = responseMyList.kanalGorusuFirst;
                            dahiliYazismaMessage.enabled = true;
                            dahiliYazismaMessage.dahiliYazismaId = dahiliYazismaTable.Id;
                            BLLActions.DahiliYazismaMessage bllDahiliYazismaMessage = new BLLActions.DahiliYazismaMessage(_configuration, _env);
                            await bllDahiliYazismaMessage.Add(dahiliYazismaMessage);

                            dahiliYazismaTable.mudurBittiMi = true;

                            await Update(dahiliYazismaTable);

                            EmailMessage emailMessage = new EmailMessage();
                            emailMessage.toAddress = lastUser2?.email;
                            emailMessage.subject = dahiliYazismaTable.konu + " hk.";
                            emailMessage.isSent = false;
                            emailMessage.emailText = ("<h2>Sayın " + lastUser2?.name + "</h2><br/><h4>Bir adet "
                                    + dahiliYazismaTable.Id.ToString() + " ID'li ve " + dahiliYazismaTable.konu
                                    + " Konulu dahili yazışma yapılacak işiniz bulunmaktadır.<br/></h4>"
                                    + buildDahiliYazisma(dahiliYazismaTable));
                            emailMessage.mailTuru = 4;
                            emailMessage.plannedDate = DateTime.Now;
                            emailMessage.enabled = true;
                            BLLActions.EmailMessages bllEmailMessages = new BLLActions.EmailMessages(_configuration, _env);
                            await bllEmailMessages.Add(emailMessage);
                            return 1;
                        }
                        else
                        {
                            return 2;
                        }
                    }

                }
                catch
                //(Exception e)
                {

                    //Console.WriteLine(
                    //        getUser.getUser().name + "," + responseMyList.getDahiliYazismaTable().Id.ToString()
                    //                + " id'li dahili yazışmaya son onay verilemiyor. Hata: " + e.getMessage());
                    return 3;
                }
                return 3;
            }

            public PageReturn<InternalCorrespondenceDto> mylastoperation(FilterPageParam<InternalCorrespondencePageableListBilgiDtoParameter> filterPageParam)
            {

                int? id = filterPageParam?.liste?.id;
                int? companyId = filterPageParam?.liste?.companyId;
                string? servisi = filterPageParam?.liste?.servisi;
                string? konu = filterPageParam?.liste?.konu;
                bool? bittiMi = filterPageParam?.liste?.bittiMi;
                bool? redEttiMi = filterPageParam?.liste?.redEttiMi;
                int? userId = filterPageParam?.liste?.userId;

                PageReturn<InternalCorrespondenceDto>? result = new PageReturn<InternalCorrespondenceDto>();
                int pageSize = filterPageParam?.size ?? 20;
                int pageNumber = filterPageParam?.page ?? 0;

                BLLActions.AdminUsers bllAdminUsers = new BLLActions.AdminUsers(_configuration, _env, _mapper);
                AdminUser? user = bllAdminUsers.GetByID(userId ?? 0);
                if (user?.roleId == 1)
                {
                    var query =
    from a in dal.dB.DahiliYazismaTable
    join c in dal.dB.Company
        on a.companyId equals c.Id
    join d in dal.dB.AdminUser
        on a.createdUserId equals d.Id
    join f in dal.dB.AdminUser
        on a.noteUserId equals f.Id into noteUserJoin
    from f in noteUserJoin.DefaultIfEmpty()
    join e in dal.dB.BolumUserHierarchyTable
        on a.kanalId equals e.Id into bolumJoin
    from e in bolumJoin.DefaultIfEmpty()
    where
        a.enabled &&
        a.onaylandiMi == true &&
        a.kanalGorusuOkmi == false &&
        (id == null || a.Id == id) &&
        (string.IsNullOrEmpty(konu) || a.konu.Contains(konu)) &&
        (companyId == null || a.companyId == companyId) &&
        (string.IsNullOrEmpty(servisi) || a.servisi == servisi) &&
        (redEttiMi == null || a.redEttiMi == redEttiMi) &&
        (bittiMi == null || a.bittiMi == bittiMi)
    orderby a.Id descending
    select new InternalCorrespondenceDto
    {
        id = a.Id,
        companyName = c.vtext,
        servisi = a.servisi,
        konu = a.konu,
        createdDate = a.tarih,
        kanal = e != null ? e.bolumAdi : "",
        createdUser = d.name,
        status = a.redEttiMi,
        createdUserId = a.createdUserId,
        onay1Ok = a.onay1Ok,
        note = a.note,
        noteUserName = f != null ? f.name : ""
    };
                    result.totalElements = query.Count();

                    result.content = query
                        .Skip(pageSize * pageNumber)
                        .Take(pageSize)
                        .ToList();

                    result.number = pageNumber;
                    result.size = pageSize;
                    return result;

                }
                else
                {
                    var query =
    from a in dal.dB.DahiliYazismaTable
    join c in dal.dB.Company
        on a.companyId equals c.Id
    join d in dal.dB.AdminUser
        on a.createdUserId equals d.Id
    join f in dal.dB.AdminUser
        on a.noteUserId equals f.Id into noteUserJoin
    from f in noteUserJoin.DefaultIfEmpty() // LEFT JOIN User f
    join e in dal.dB.BolumUserHierarchyTable
        on a.kanalId equals e.Id into bolumJoin
    from e in bolumJoin.DefaultIfEmpty() // LEFT JOIN Bolum
    where
        a.enabled &&
        a.onaylandiMi == true &&
        a.kanalGorusuOkmi == false &&
        a.kanalGorusuUserId == userId &&
        (id == null || a.Id == id) &&
        (string.IsNullOrEmpty(konu) || a.konu.Contains(konu)) &&
        (companyId == null || a.companyId == companyId) &&
        (string.IsNullOrEmpty(servisi) || a.servisi == servisi) &&
        (redEttiMi == null || a.redEttiMi == redEttiMi) &&
        (bittiMi == null || a.bittiMi == bittiMi)
    orderby a.Id descending
    select new InternalCorrespondenceDto
    {
        id = a.Id,
        companyName = c.vtext,
        servisi = a.servisi,
        konu = a.konu,
        createdDate = a.tarih,
        kanal = e != null ? e.bolumAdi : "",
        createdUser = d.name,
        status = a.redEttiMi,
        createdUserId = a.createdUserId,
        onay1Ok = a.onay1Ok,
        note = a.note,
        noteUserName = f != null ? f.name : ""
    };
                    result.totalElements = query.Count();

                    result.content = query
                        .Skip(pageSize * pageNumber)
                        .Take(pageSize)
                        .ToList();

                    result.number = pageNumber;
                    result.size = pageSize;
                    return result;
                }
            }

            public async Task<int> backtoceo(ResponseMyList responseMyList, int userId)
            {
                try
                {
                    Data.Models.DahiliYazismaTable? dahiliYazismaTable = _mapper.Map<Data.Models.DahiliYazismaTable>(responseMyList.dahiliYazismaTable);
                    if (dahiliYazismaTable != null)
                    {

                        BLLActions.CeoTable bllCeoTable = new BLLActions.CeoTable(_configuration, _env);
                        Data.Models.CeoTable ceoTable = bllCeoTable.GetByID(1)!;

                        BLLActions.AdminUsers bllAdminUsers = new BLLActions.AdminUsers(_configuration, _env, _mapper);
                        AdminUser user = bllAdminUsers.GetByID(userId)!;
                        AdminUser ceoUser = bllAdminUsers.GetByID(ceoTable.userId)!;

                        dahiliYazismaTable.onaylandiMi = false;
                        await Update(dahiliYazismaTable);

                        BLLActions.DahiliYazismalarDetayTable bllDahiliYazismalarDetayTable = new BLLActions.DahiliYazismalarDetayTable(_configuration, _env);
                        List<Data.Models.DahiliYazismalarDetayTable> listDahiliYazismalarDetayTable = bllDahiliYazismalarDetayTable
                                .findAllByDahiliYazismaIdAndApprovedAndUserIdAndEnabled(dahiliYazismaTable.Id, null,
                                        user.Id, true);

                        for (int j = 1; j < listDahiliYazismalarDetayTable.Count(); j++)
                        {
                            int id = listDahiliYazismalarDetayTable[j].Id;
                            bllDahiliYazismalarDetayTable.Delete(id);
                        }
                        Data.Models.DahiliYazismalarDetayTable dahiliYazismalarDetayTable = listDahiliYazismalarDetayTable[0];
                        dahiliYazismalarDetayTable.enabled = false;
                        await bllDahiliYazismalarDetayTable.Update(dahiliYazismalarDetayTable);

                        Data.Models.DahiliYazismalarDetayTable dahiliYazismalarDetayTableNext = new Data.Models.DahiliYazismalarDetayTable();
                        dahiliYazismalarDetayTableNext.userId = ceoTable.userId;
                        dahiliYazismalarDetayTableNext.createdDate = DateTime.Now;
                        dahiliYazismalarDetayTableNext.approved = null;
                        dahiliYazismalarDetayTableNext.dahiliYazismaId = dahiliYazismaTable.Id;
                        dahiliYazismalarDetayTableNext.isReplied = false;
                        dahiliYazismalarDetayTableNext.replyDate = null;
                        dahiliYazismalarDetayTableNext.enabled = true;
                        await bllDahiliYazismalarDetayTable.Add(dahiliYazismalarDetayTableNext);

                        //			EmailMessage emailMessage = new EmailMessage();
                        //			emailMessage.toAddress=ceoUser.email);
                        //			emailMessage.subject=dahiliYazismaTable.konu + " geri gönderim hk.");
                        //			emailMessage.isSent=false;
                        //			emailMessage.emailText=("Geri Gönderim Sebebi:\n<b>" + responseMyList.kanalGorusuFirst + "</b>"
                        //					+ buildDahiliYazisma(dahiliYazismaTable));
                        //			emailMessage.mailTuru=4;
                        //			emailMessage.plannedDate=DateTime.Now;
                        //			emailMessage.enabled=true;
                        //			emailMessageService.save(emailMessage, user.Id);

                        SMSMessage smsMessage = new SMSMessage();
                        smsMessage.plannedDate = DateTime.Now;
                        smsMessage.isSent = false;
                        smsMessage.smsText = (dahiliYazismaTable.Id.ToString() + "Id'li" + dahiliYazismaTable.konu
                                + " konulu " + " Dahili Yazışma geri gönderilmiştir. Geri gönderim sebebi: "
                                + responseMyList.kanalGorusuFirst);
                        smsMessage.toNumbers = ceoUser.mobile;

                        BLLActions.SMSMessages bllSMSMessages = new BLLActions.SMSMessages(_configuration, _env);
                        await bllSMSMessages.Add(smsMessage);

                        Data.Models.DahiliYazismaMessage dahiliYazismaMessage = new Data.Models.DahiliYazismaMessage();
                        dahiliYazismaMessage.createdDate = DateTime.Now;
                        dahiliYazismaMessage.showAll = false;
                        dahiliYazismaMessage.dahiliYazismaId = dahiliYazismaTable.Id;
                        dahiliYazismaMessage.userId = user.Id;
                        dahiliYazismaMessage.sendUserId = ceoTable.userId;
                        dahiliYazismaMessage.message = responseMyList.kanalGorusuFirst;
                        dahiliYazismaMessage.enabled = true;
                        BLLActions.DahiliYazismaMessage bllDahiliYazismaMessage = new BLLActions.DahiliYazismaMessage(_configuration, _env);
                        await bllDahiliYazismaMessage.Add(dahiliYazismaMessage);
                        return 1;
                    }
                    else
                    {
                        return 2;
                    }
                }
                catch
                //(Exception e)
                {
                    //System.out.println(
                    //        getUser.getUser().name + "," + responseMyList.getDahiliYazismaTable().Id.ToString()
                    //                + " id'li dahili yazışmayı ceoya gönderemiyor. Hata: " + e.getMessage());
                    return 2;
                }
            }

            public async Task<int> red(InternalCorrespondenceSaveDto dto, int userId)
            {
                try
                {
                    Data.Models.DahiliYazismaTable dahiliYazismaTable = _mapper.Map<Data.Models.DahiliYazismaTable>(dto);
                    BLLActions.AdminUsers bllAdminUsers = new BLLActions.AdminUsers(_configuration, _env, _mapper);
                    AdminUser user = bllAdminUsers.GetByID(userId)!;
                    AdminUser createdUser = bllAdminUsers.GetByID(dahiliYazismaTable.createdUserId)!;
                    BLLActions.DahiliYazismalarDetayTable bllDahiliYazismalarDetayTable = new BLLActions.DahiliYazismalarDetayTable(_configuration, _env);
                    List<Data.Models.DahiliYazismalarDetayTable> listDahiliYazismalarDetayTable = bllDahiliYazismalarDetayTable
                            .findAllByDahiliYazismaIdAndApprovedAndUserIdAndEnabled(dahiliYazismaTable.Id, null,
                                    user.Id, true);

                    for (int j = 1; j < listDahiliYazismalarDetayTable.Count(); j++)
                    {
                        int id = listDahiliYazismalarDetayTable[j].Id;
                        bllDahiliYazismalarDetayTable.Delete(id);
                    }
                    Data.Models.DahiliYazismalarDetayTable dahiliYazismalarDetayTable = listDahiliYazismalarDetayTable[0];
                    dahiliYazismalarDetayTable.isReplied = true;
                    dahiliYazismalarDetayTable.approved = false;
                    dahiliYazismalarDetayTable.replyDate = DateTime.Now;
                    await bllDahiliYazismalarDetayTable.Update(dahiliYazismalarDetayTable);

                    await Update(dahiliYazismaTable);
                    EmailMessage emailMessage = new EmailMessage();
                    emailMessage.toAddress = createdUser.email;
                    emailMessage.subject = dahiliYazismaTable.konu + " hk.";
                    emailMessage.isSent = false;
                    emailMessage.emailText = ("<h2>Sayın " + createdUser.name + "</h2><br/>" + "<h4>"
                            + dahiliYazismaTable.Id.ToString() + " Id'li ve " + dahiliYazismaTable.konu + " Konulu"
                            + "dahili yazışmanız red olmuştur.<br/></h4>");
                    emailMessage.mailTuru = 4;
                    emailMessage.plannedDate = DateTime.Now;
                    emailMessage.enabled = true;
                    BLLActions.EmailMessages bllEmailMessages = new BLLActions.EmailMessages(_configuration, _env);
                    await bllEmailMessages.Add(emailMessage);
                    return 1;
                }
                catch
                //(Exception e)
                {
                    //System.out.println(getUser.getUser().name + "," + dahiliYazismaTable.Id.ToString()
                    //        + " id'li dahili yazışmayı red edemiyor. Hata: " + e.getMessage());
                    return 2;
                }
            }

            public async Task<int> gerigonder(ResponseMyList responseMyList, int userId)
            {
                try
                {
                    Data.Models.DahiliYazismaTable dahiliYazismaTable = _mapper.Map<Data.Models.DahiliYazismaTable>(responseMyList.dahiliYazismaTable);
                    BLLActions.CeoTable bllCeoTable = new BLLActions.CeoTable(_configuration, _env);
                    Data.Models.CeoTable ceoTable = bllCeoTable.GetByID(1)!;

                    BLLActions.AdminUsers bllAdminUsers = new BLLActions.AdminUsers(_configuration, _env, _mapper);
                    AdminUser user = bllAdminUsers.GetByID(userId)!;

                    if (ceoTable.userId == user.Id)
                    {
                        BLLActions.BolumUserHierarchyTable bllBolumUserHierarchyTable = new BLLActions.BolumUserHierarchyTable(_configuration, _env);
                        Data.Models.BolumUserHierarchyTable bolumUserHierarchyTable = bllBolumUserHierarchyTable
                                .findByBolumAdi(dahiliYazismaTable.servisi);
                        if (bolumUserHierarchyTable.Id == dahiliYazismaTable.kanalId)
                        {

                            AdminUser kanalUser = bllAdminUsers.GetByID(bolumUserHierarchyTable.userId)!;
                            dahiliYazismaTable.kanalBittiMi = false;
                            await Update(dahiliYazismaTable);

                            BLLActions.DahiliYazismalarDetayTable bllDahiliYazismalarDetayTable = new BLLActions.DahiliYazismalarDetayTable(_configuration, _env);
                            List<Data.Models.DahiliYazismalarDetayTable> listDahiliYazismalarDetayTable = bllDahiliYazismalarDetayTable
                                    .findAllByDahiliYazismaIdAndApprovedAndUserIdAndEnabled(dahiliYazismaTable.Id, null,
                                            user.Id, true);

                            listDahiliYazismalarDetayTable.OrderBy(x => x.Id).ToList();

                            Data.Models.DahiliYazismalarDetayTable dahiliYazismalarDetayTable = listDahiliYazismalarDetayTable[0];
                            dahiliYazismalarDetayTable.enabled = false;
                            await bllDahiliYazismalarDetayTable.Update(dahiliYazismalarDetayTable);

                            Data.Models.DahiliYazismalarDetayTable dahiliYazismalarDetayTableNext = new Data.Models.DahiliYazismalarDetayTable();
                            dahiliYazismalarDetayTableNext.userId = kanalUser.Id;
                            dahiliYazismalarDetayTableNext.createdDate = DateTime.Now;
                            dahiliYazismalarDetayTableNext.approved = null;
                            dahiliYazismalarDetayTableNext.dahiliYazismaId = dahiliYazismaTable.Id;
                            dahiliYazismalarDetayTableNext.isReplied = false;
                            dahiliYazismalarDetayTableNext.replyDate = null;
                            dahiliYazismalarDetayTableNext.enabled = true;
                            await bllDahiliYazismalarDetayTable.Add(dahiliYazismalarDetayTableNext);

                            EmailMessage emailMessage = new EmailMessage();
                            emailMessage.toAddress = kanalUser.email;
                            emailMessage.subject = dahiliYazismaTable.konu + " geri gönderim hk.";
                            emailMessage.isSent = false;
                            emailMessage.emailText = ("Geri Gönderim Sebebi:\n<b>" + responseMyList.kanalGorusuFirst
                                    + "</b>" + buildDahiliYazisma(dahiliYazismaTable));
                            emailMessage.mailTuru = 4;
                            emailMessage.plannedDate = DateTime.Now;
                            emailMessage.enabled = true;
                            BLLActions.EmailMessages bllEmailMessage = new BLLActions.EmailMessages(_configuration, _env);
                            await bllEmailMessage.Add(emailMessage);

                            Data.Models.DahiliYazismaMessage dahiliYazismaMessage = new Data.Models.DahiliYazismaMessage();
                            dahiliYazismaMessage.createdDate = DateTime.Now;
                            dahiliYazismaMessage.showAll = false;
                            dahiliYazismaMessage.dahiliYazismaId = dahiliYazismaTable.Id;
                            dahiliYazismaMessage.userId = user.Id;
                            dahiliYazismaMessage.sendUserId = kanalUser.Id;
                            dahiliYazismaMessage.message = responseMyList.kanalGorusuFirst;
                            dahiliYazismaMessage.enabled = true;
                            dahiliYazismaMessage.dahiliYazismaId = dahiliYazismaTable.Id;
                            BLLActions.DahiliYazismaMessage bllDahiliYazismaMessage = new BLLActions.DahiliYazismaMessage(_configuration, _env);
                            await bllDahiliYazismaMessage.Add(dahiliYazismaMessage);
                            return 1;
                        }
                        else

                        if (dahiliYazismaTable.onaylayici4 != null)
                        {
                            AdminUser? onaylayici4 = bllAdminUsers.GetByID(dahiliYazismaTable.onaylayici4 ?? 0);
                            dahiliYazismaTable.onay4Ok = false;
                            await Update(dahiliYazismaTable);

                            BLLActions.DahiliYazismalarDetayTable bllDahiliYazismalarDetayTable = new BLLActions.DahiliYazismalarDetayTable(_configuration, _env);
                            List<Data.Models.DahiliYazismalarDetayTable> listDahiliYazismalarDetayTable = bllDahiliYazismalarDetayTable
                                    .findAllByDahiliYazismaIdAndApprovedAndUserIdAndEnabled(dahiliYazismaTable.Id, null,
                                            user.Id, true);

                            for (int j = 1; j < listDahiliYazismalarDetayTable.Count(); j++)
                            {
                                int id = listDahiliYazismalarDetayTable[j].Id;
                                bllDahiliYazismalarDetayTable.Delete(id);
                            }
                            Data.Models.DahiliYazismalarDetayTable dahiliYazismalarDetayTable = listDahiliYazismalarDetayTable[0];
                            dahiliYazismalarDetayTable.enabled = false;
                            await bllDahiliYazismalarDetayTable.Update(dahiliYazismalarDetayTable);

                            Data.Models.DahiliYazismalarDetayTable dahiliYazismalarDetayTableNext = new Data.Models.DahiliYazismalarDetayTable();
                            dahiliYazismalarDetayTableNext.userId = dahiliYazismaTable.onaylayici4 ?? 0;
                            dahiliYazismalarDetayTableNext.createdDate = DateTime.Now;
                            dahiliYazismalarDetayTableNext.approved = null;
                            dahiliYazismalarDetayTableNext.dahiliYazismaId = dahiliYazismaTable.Id;
                            dahiliYazismalarDetayTableNext.isReplied = false;
                            dahiliYazismalarDetayTableNext.replyDate = null;
                            dahiliYazismalarDetayTableNext.enabled = true;
                            await bllDahiliYazismalarDetayTable.Add(dahiliYazismalarDetayTableNext);

                            EmailMessage emailMessage = new EmailMessage();
                            emailMessage.toAddress = onaylayici4?.email;
                            emailMessage.subject = dahiliYazismaTable.konu + " geri gönderim hk.";
                            emailMessage.isSent = false;
                            emailMessage.emailText = ("Geri Gönderim Sebebi:\n<b>" + responseMyList.kanalGorusuFirst
                                    + "</b>" + buildDahiliYazisma(dahiliYazismaTable));
                            emailMessage.mailTuru = 4;
                            emailMessage.plannedDate = DateTime.Now;
                            emailMessage.enabled = true;
                            BLLActions.EmailMessages bllEmailMessages = new BLLActions.EmailMessages(_configuration, _env);
                            await bllEmailMessages.Add(emailMessage);

                            Data.Models.DahiliYazismaMessage dahiliYazismaMessage = new Data.Models.DahiliYazismaMessage();
                            dahiliYazismaMessage.createdDate = DateTime.Now;
                            dahiliYazismaMessage.showAll = false;
                            dahiliYazismaMessage.dahiliYazismaId = dahiliYazismaTable.Id;
                            dahiliYazismaMessage.userId = user.Id;
                            dahiliYazismaMessage.sendUserId = dahiliYazismaTable.onaylayici4;
                            dahiliYazismaMessage.message = responseMyList.kanalGorusuFirst;
                            dahiliYazismaMessage.enabled = true;
                            dahiliYazismaMessage.dahiliYazismaId = dahiliYazismaTable.Id;
                            BLLActions.DahiliYazismaMessage bllDahiliYazismaMessage = new BLLActions.DahiliYazismaMessage(_configuration, _env);
                            await bllDahiliYazismaMessage.Add(dahiliYazismaMessage);
                            return 1;
                        }
                        else if (dahiliYazismaTable.onaylayici3 != null)
                        {
                            AdminUser? onaylayici3 = bllAdminUsers.GetByID(dahiliYazismaTable.onaylayici3 ?? 0);

                            dahiliYazismaTable.onay3Ok = false;
                            await Update(dahiliYazismaTable);

                            BLLActions.DahiliYazismalarDetayTable bllDahiliYazismalarDetayTable = new BLLActions.DahiliYazismalarDetayTable(_configuration, _env);
                            List<Data.Models.DahiliYazismalarDetayTable> listDahiliYazismalarDetayTable = bllDahiliYazismalarDetayTable
                                    .findAllByDahiliYazismaIdAndApprovedAndUserIdAndEnabled(dahiliYazismaTable.Id, null,
                                            user.Id, true);

                            for (int j = 1; j < listDahiliYazismalarDetayTable.Count(); j++)
                            {
                                int id = listDahiliYazismalarDetayTable[j].Id;
                                bllDahiliYazismalarDetayTable.Delete(id);
                            }
                            Data.Models.DahiliYazismalarDetayTable dahiliYazismalarDetayTable = listDahiliYazismalarDetayTable[0];
                            dahiliYazismalarDetayTable.enabled = false;
                            await bllDahiliYazismalarDetayTable.Update(dahiliYazismalarDetayTable);

                            Data.Models.DahiliYazismalarDetayTable dahiliYazismalarDetayTableNext = new Data.Models.DahiliYazismalarDetayTable();
                            dahiliYazismalarDetayTableNext.userId = dahiliYazismaTable.onaylayici3 ?? 0;
                            dahiliYazismalarDetayTableNext.createdDate = DateTime.Now;
                            dahiliYazismalarDetayTableNext.approved = null;
                            dahiliYazismalarDetayTableNext.dahiliYazismaId = dahiliYazismaTable.Id;
                            dahiliYazismalarDetayTableNext.isReplied = false;
                            dahiliYazismalarDetayTableNext.replyDate = null;
                            dahiliYazismalarDetayTableNext.enabled = true;
                            await bllDahiliYazismalarDetayTable.Add(dahiliYazismalarDetayTableNext);

                            EmailMessage emailMessage = new EmailMessage();
                            emailMessage.toAddress = onaylayici3?.email;
                            emailMessage.subject = dahiliYazismaTable.konu + " geri gönderim hk.";
                            emailMessage.isSent = false;
                            emailMessage.emailText = ("Geri Gönderim Sebebi:\n<b>" + responseMyList.kanalGorusuFirst
                                    + "</b>" + buildDahiliYazisma(dahiliYazismaTable));
                            emailMessage.mailTuru = 4;
                            emailMessage.plannedDate = DateTime.Now;
                            emailMessage.enabled = true;
                            BLLActions.EmailMessages bllEmailMessages = new BLLActions.EmailMessages(_configuration, _env);
                            await bllEmailMessages.Add(emailMessage);

                            Data.Models.DahiliYazismaMessage dahiliYazismaMessage = new Data.Models.DahiliYazismaMessage();
                            dahiliYazismaMessage.createdDate = DateTime.Now;
                            dahiliYazismaMessage.showAll = false;
                            dahiliYazismaMessage.dahiliYazismaId = dahiliYazismaTable.Id;
                            dahiliYazismaMessage.userId = user.Id;
                            dahiliYazismaMessage.sendUserId = dahiliYazismaTable.onaylayici3;
                            dahiliYazismaMessage.message = responseMyList.kanalGorusuFirst;
                            dahiliYazismaMessage.enabled = true;
                            BLLActions.DahiliYazismaMessage bllDahiliYazismaMessage = new BLLActions.DahiliYazismaMessage(_configuration, _env);
                            await bllDahiliYazismaMessage.Add(dahiliYazismaMessage);
                            return 1;
                        }
                        else if (dahiliYazismaTable.onaylayici2 != null)
                        {
                            AdminUser? onaylayici2 = bllAdminUsers.GetByID(dahiliYazismaTable.onaylayici2 ?? 0);
                            dahiliYazismaTable.onay2Ok = false;
                            await Update(dahiliYazismaTable);

                            BLLActions.DahiliYazismalarDetayTable bllDahiliYazismalarDetayTable = new BLLActions.DahiliYazismalarDetayTable(_configuration, _env);
                            List<Data.Models.DahiliYazismalarDetayTable> listDahiliYazismalarDetayTable = bllDahiliYazismalarDetayTable
                                    .findAllByDahiliYazismaIdAndApprovedAndUserIdAndEnabled(dahiliYazismaTable.Id, null,
                                            user.Id, true);

                            for (int j = 1; j < listDahiliYazismalarDetayTable.Count(); j++)
                            {
                                int id = listDahiliYazismalarDetayTable[j].Id;
                                bllDahiliYazismalarDetayTable.Delete(id);
                            }
                            Data.Models.DahiliYazismalarDetayTable dahiliYazismalarDetayTable = listDahiliYazismalarDetayTable[0];
                            dahiliYazismalarDetayTable.enabled = false;
                            await bllDahiliYazismalarDetayTable.Update(dahiliYazismalarDetayTable);

                            Data.Models.DahiliYazismalarDetayTable dahiliYazismalarDetayTableNext = new Data.Models.DahiliYazismalarDetayTable();
                            dahiliYazismalarDetayTableNext.userId = dahiliYazismaTable.onaylayici2 ?? 0;
                            dahiliYazismalarDetayTableNext.createdDate = DateTime.Now;
                            dahiliYazismalarDetayTableNext.approved = null;
                            dahiliYazismalarDetayTableNext.dahiliYazismaId = dahiliYazismaTable.Id;
                            dahiliYazismalarDetayTableNext.isReplied = false;
                            dahiliYazismalarDetayTableNext.replyDate = null;
                            dahiliYazismalarDetayTableNext.enabled = true;
                            await bllDahiliYazismalarDetayTable.Add(dahiliYazismalarDetayTableNext);

                            EmailMessage emailMessage = new EmailMessage();
                            emailMessage.toAddress = onaylayici2?.email;
                            emailMessage.subject = dahiliYazismaTable.konu + " geri gönderim hk.";
                            emailMessage.isSent = false;
                            emailMessage.emailText = ("Geri Gönderim Sebebi:\n<b>" + responseMyList.kanalGorusuFirst
                                    + "</b>" + buildDahiliYazisma(dahiliYazismaTable));
                            emailMessage.mailTuru = 4;
                            emailMessage.plannedDate = DateTime.Now;
                            emailMessage.enabled = true;
                            BLLActions.EmailMessages bllEmailMessages = new BLLActions.EmailMessages(_configuration, _env);
                            await bllEmailMessages.Add(emailMessage);

                            Data.Models.DahiliYazismaMessage dahiliYazismaMessage = new Data.Models.DahiliYazismaMessage();
                            dahiliYazismaMessage.createdDate = DateTime.Now;
                            dahiliYazismaMessage.showAll = false;
                            dahiliYazismaMessage.dahiliYazismaId = dahiliYazismaTable.Id;
                            dahiliYazismaMessage.userId = user.Id;
                            dahiliYazismaMessage.sendUserId = dahiliYazismaTable.onaylayici2;
                            dahiliYazismaMessage.message = responseMyList.kanalGorusuFirst;
                            dahiliYazismaMessage.enabled = true;
                            BLLActions.DahiliYazismaMessage bllDahiliYazismaMessage = new BLLActions.DahiliYazismaMessage(_configuration, _env);
                            await bllDahiliYazismaMessage.Add(dahiliYazismaMessage);
                            return 1;
                        }
                        else if (dahiliYazismaTable.onaylayici1 != null)
                        {
                            AdminUser? onaylayici1 = bllAdminUsers.GetByID(dahiliYazismaTable.onaylayici1 ?? 0);
                            dahiliYazismaTable.onay1Ok = false;
                            await Update(dahiliYazismaTable);
                            BLLActions.DahiliYazismalarDetayTable bllDahiliYazismalarDetayTable = new BLLActions.DahiliYazismalarDetayTable(_configuration, _env);
                            List<Data.Models.DahiliYazismalarDetayTable> listDahiliYazismalarDetayTable = bllDahiliYazismalarDetayTable
                                    .findAllByDahiliYazismaIdAndApprovedAndUserIdAndEnabled(dahiliYazismaTable.Id, null,
                                            user.Id, true);

                            for (int j = 1; j < listDahiliYazismalarDetayTable.Count(); j++)
                            {
                                int id = listDahiliYazismalarDetayTable[j].Id;
                                bllDahiliYazismalarDetayTable.Delete(id);
                            }
                            Data.Models.DahiliYazismalarDetayTable dahiliYazismalarDetayTable = listDahiliYazismalarDetayTable[0];
                            dahiliYazismalarDetayTable.enabled = false;
                            await bllDahiliYazismalarDetayTable.Update(dahiliYazismalarDetayTable);

                            Data.Models.DahiliYazismalarDetayTable dahiliYazismalarDetayTableNext = new Data.Models.DahiliYazismalarDetayTable();
                            dahiliYazismalarDetayTableNext.userId = dahiliYazismaTable.onaylayici1 ?? 0;
                            dahiliYazismalarDetayTableNext.createdDate = DateTime.Now;
                            dahiliYazismalarDetayTableNext.approved = null;
                            dahiliYazismalarDetayTableNext.dahiliYazismaId = dahiliYazismaTable.Id;
                            dahiliYazismalarDetayTableNext.isReplied = false;
                            dahiliYazismalarDetayTableNext.replyDate = null;
                            dahiliYazismalarDetayTableNext.enabled = true;
                            await bllDahiliYazismalarDetayTable.Add(dahiliYazismalarDetayTableNext);

                            EmailMessage emailMessage = new EmailMessage();
                            emailMessage.toAddress = onaylayici1?.email;
                            emailMessage.subject = dahiliYazismaTable.konu + " geri gönderim hk.";
                            emailMessage.isSent = false;
                            emailMessage.emailText = ("Geri Gönderim Sebebi:\n<b>" + responseMyList.kanalGorusuFirst
                                    + "</b>" + buildDahiliYazisma(dahiliYazismaTable));

                            emailMessage.mailTuru = 4;
                            emailMessage.plannedDate = DateTime.Now;
                            emailMessage.enabled = true;
                            BLLActions.EmailMessages bllEmailMessages = new BLLActions.EmailMessages(_configuration, _env);
                            await bllEmailMessages.Add(emailMessage);

                            Data.Models.DahiliYazismaMessage dahiliYazismaMessage = new Data.Models.DahiliYazismaMessage();
                            dahiliYazismaMessage.createdDate = DateTime.Now;
                            dahiliYazismaMessage.showAll = false;
                            dahiliYazismaMessage.dahiliYazismaId = dahiliYazismaTable.Id;
                            dahiliYazismaMessage.userId = user.Id;
                            dahiliYazismaMessage.sendUserId = dahiliYazismaTable.onaylayici1;
                            dahiliYazismaMessage.message = responseMyList.kanalGorusuFirst;
                            dahiliYazismaMessage.enabled = true;
                            BLLActions.DahiliYazismaMessage bllDahiliYazismaMessage = new BLLActions.DahiliYazismaMessage(_configuration, _env);
                            await bllDahiliYazismaMessage.Add(dahiliYazismaMessage);
                            return 1;
                        }
                        else
                        {
                            AdminUser createdUser = bllAdminUsers.GetByID(dahiliYazismaTable.createdUserId)!;
                            dahiliYazismaTable.onay1Ok = false;
                            await Update(dahiliYazismaTable);
                            BLLActions.DahiliYazismalarDetayTable bllDahiliYazismalarDetayTable = new BLLActions.DahiliYazismalarDetayTable(_configuration, _env);
                            List<Data.Models.DahiliYazismalarDetayTable> listDahiliYazismalarDetayTable = bllDahiliYazismalarDetayTable
                                    .findAllByDahiliYazismaIdAndApprovedAndUserIdAndEnabled(dahiliYazismaTable.Id, null,
                                            user.Id, true);

                            for (int j = 1; j < listDahiliYazismalarDetayTable.Count(); j++)
                            {
                                int id = listDahiliYazismalarDetayTable[j].Id;
                                bllDahiliYazismalarDetayTable.Delete(id);
                            }
                            Data.Models.DahiliYazismalarDetayTable dahiliYazismalarDetayTable = listDahiliYazismalarDetayTable[0];
                            dahiliYazismalarDetayTable.enabled = false;
                            await bllDahiliYazismalarDetayTable.Update(dahiliYazismalarDetayTable);

                            Data.Models.DahiliYazismalarDetayTable dahiliYazismalarDetayTableNext = new Data.Models.DahiliYazismalarDetayTable();
                            dahiliYazismalarDetayTableNext.userId = createdUser.Id;
                            dahiliYazismalarDetayTableNext.createdDate = DateTime.Now;
                            dahiliYazismalarDetayTableNext.approved = null;
                            dahiliYazismalarDetayTableNext.dahiliYazismaId = dahiliYazismaTable.Id;
                            dahiliYazismalarDetayTableNext.isReplied = false;
                            dahiliYazismalarDetayTableNext.replyDate = null;
                            dahiliYazismalarDetayTableNext.enabled = true;
                            await bllDahiliYazismalarDetayTable.Add(dahiliYazismalarDetayTableNext);

                            EmailMessage emailMessage = new EmailMessage();
                            emailMessage.toAddress = createdUser.email;
                            emailMessage.subject = dahiliYazismaTable.konu + " geri gönderim hk.";
                            emailMessage.isSent = false;
                            emailMessage.emailText = ("Geri Gönderim Sebebi:\n<b>" + responseMyList.kanalGorusuFirst
                                    + "</b>" + buildDahiliYazisma(dahiliYazismaTable));
                            emailMessage.mailTuru = 4;
                            emailMessage.plannedDate = DateTime.Now;
                            emailMessage.enabled = true;
                            BLLActions.EmailMessages bllEmailMessages = new BLLActions.EmailMessages(_configuration, _env);
                            await bllEmailMessages.Add(emailMessage);

                            Data.Models.DahiliYazismaMessage dahiliYazismaMessage = new Data.Models.DahiliYazismaMessage();
                            dahiliYazismaMessage.createdDate = DateTime.Now;
                            dahiliYazismaMessage.showAll = false;
                            dahiliYazismaMessage.dahiliYazismaId = dahiliYazismaTable.Id;
                            dahiliYazismaMessage.userId = user.Id;
                            dahiliYazismaMessage.sendUserId = createdUser.Id;
                            dahiliYazismaMessage.message = responseMyList.kanalGorusuFirst;
                            dahiliYazismaMessage.enabled = true;
                            BLLActions.DahiliYazismaMessage bllDahiliYazismaMessage = new BLLActions.DahiliYazismaMessage(_configuration, _env);
                            await bllDahiliYazismaMessage.Add(dahiliYazismaMessage);
                            return 1;
                        }
                    }
                    else
                    {
                        return 3;
                    }

                }
                catch
                //(Exception e)
                {
                    //System.out.println(
                    //        getUser.getUser().name + "," + responseMyList.getDahiliYazismaTable().Id.ToString()
                    //                + " id'li dahili yazışmayı geri gönderemiyor. Hata: " + e.getMessage());
                    return 2;
                }
            }

            public async Task<int> endit(ResponseMyList responseMyList, int userId)
            {
                Data.Models.DahiliYazismaTable dahiliYazismaTable = _mapper.Map<Data.Models.DahiliYazismaTable>(responseMyList.dahiliYazismaTable);

                BLLActions.AdminUsers bllAdminUsers = new BLLActions.AdminUsers(_configuration, _env, _mapper);
                AdminUser user = bllAdminUsers.GetByID(userId)!;
                AdminUser createdUser = bllAdminUsers.GetByID(dahiliYazismaTable.createdUserId)!;

                try
                {
                    if (user.Id == dahiliYazismaTable.lastUserId
                            || user.Id == dahiliYazismaTable.lastUserId2)
                    {
                        BLLActions.DahiliYazismalarDetayTable bllDahiliYazismalarDetayTable = new BLLActions.DahiliYazismalarDetayTable(_configuration, _env);
                        List<Data.Models.DahiliYazismalarDetayTable> listDahiliYazismalarDetayTable = bllDahiliYazismalarDetayTable
                                .findAllByDahiliYazismaIdAndApprovedAndUserIdAndEnabled(dahiliYazismaTable.Id, null,
                                        user.Id, true);

                        for (int j = 1; j < listDahiliYazismalarDetayTable.Count(); j++)
                        {
                            int id = listDahiliYazismalarDetayTable[j].Id;
                            bllDahiliYazismalarDetayTable.Delete(id);
                        }
                        Data.Models.DahiliYazismalarDetayTable dahiliYazismalarDetayTable = listDahiliYazismalarDetayTable[0];
                        dahiliYazismalarDetayTable.isReplied = true;
                        dahiliYazismalarDetayTable.approved = true;
                        dahiliYazismalarDetayTable.replyDate = DateTime.Now;
                        await bllDahiliYazismalarDetayTable.Update(dahiliYazismalarDetayTable);

                        Data.Models.DahiliYazismaMessage dahiliYazismaMessage = new Data.Models.DahiliYazismaMessage();
                        dahiliYazismaMessage.createdDate = DateTime.Now;
                        dahiliYazismaMessage.showAll = true;
                        dahiliYazismaMessage.userId = user.Id;
                        dahiliYazismaMessage.message = responseMyList.kanalGorusuFirst;
                        dahiliYazismaMessage.enabled = true;
                        dahiliYazismaMessage.dahiliYazismaId = dahiliYazismaTable.Id;
                        BLLActions.DahiliYazismaMessage bllDahiliYazismaMessage = new BLLActions.DahiliYazismaMessage(_configuration, _env);
                        await bllDahiliYazismaMessage.Add(dahiliYazismaMessage);

                        dahiliYazismaTable.bittiMi = true;
                        dahiliYazismaTable.mudurBittiMi = true;
                        await Update(dahiliYazismaTable);

                        EmailMessage emailMessage = new EmailMessage();
                        emailMessage.toAddress = createdUser.email;
                        emailMessage.subject = dahiliYazismaTable.konu + " hk.";
                        emailMessage.isSent = false;
                        emailMessage.emailText = ("<h2>Sayın " + createdUser.name + "</h2><br/><h4>Bir adet "
                                + dahiliYazismaTable.Id.ToString() + " ID'li ve " + dahiliYazismaTable.konu
                                + " Konulu dahili yazışma bitmiştir.<br/></h4>" + buildDahiliYazisma(dahiliYazismaTable));
                        emailMessage.mailTuru = 4;
                        emailMessage.plannedDate = DateTime.Now;
                        emailMessage.enabled = true;
                        BLLActions.EmailMessages bllEmailMessages = new BLLActions.EmailMessages(_configuration, _env);
                        await bllEmailMessages.Add(emailMessage);
                        return 1;
                    }
                    else
                    {
                        return 2;
                    }

                }
                catch
                //(Exception e)
                {
                    //System.out.println(
                    //        getUser.getUser().name + "," + responseMyList.getDahiliYazismaTable().Id.ToString()
                    //                + " id'li dahili yazışma bitirilemiyor. Hata: " + e.getMessage());
                    return 3;
                }
            }

            public async Task saveNotes(int id, string note, int noteUserId)
            {
                if (id != 0)
                {
                    Data.Models.DahiliYazismaTable dahiliYazismaTable = GetByID(id)!;
                    dahiliYazismaTable.note = note;
                    dahiliYazismaTable.noteUserId = noteUserId;

                    await Update(dahiliYazismaTable);

                }

            }
        }
    }

}
