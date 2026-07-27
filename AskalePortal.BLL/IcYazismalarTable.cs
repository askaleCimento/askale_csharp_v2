using AskalePortal.Constants;
using AskalePortal.Data.Models;
using AskalePortal.Data.RequestModel;
using AskalePortal.Data.RequestParams;
using AskalePortal.Data.ResponseModels;
using AskalePortal.Data.ResponseParams;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
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
        public class IcYazismalarTable : BaseBLL<AskalePortal.Data.Models.IcYazismalarTable>
        {
            private readonly IConfiguration _configuration;
            private readonly IWebHostEnvironment _env;
            private readonly IMapper _mapper;
            public IcYazismalarTable(IConfiguration configuration, IWebHostEnvironment env, IMapper mapper) : base(configuration, env)
            {
                _configuration = configuration;
                _env = env;
                _mapper = mapper;
            }

            public int approvalCount(int userId)
            {
                BLLActions.IcYazismalarDetayTable bllIcYazismalarDetayTable = new BLLActions.IcYazismalarDetayTable(_configuration, _env);
                int count = bllIcYazismalarDetayTable.approvalCount(userId);
                return count;
            }

            public async Task<IcYazismalarTableSaveDto?> save(IcYazismalarTableSaveDto entity, int userId)
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
                    entity.bittiMi = false;
                    entity.onay1Ok = false;
                    entity.onay2Ok = false;
                    entity.onay3Ok = false;
                    entity.onay4Ok = false;
                    entity.onaylandiMi = false;
                    entity.redEttiMi = false;

                    Data.Models.IcYazismalarTable? icYazismaTable = await Add(_mapper.Map<Data.Models.IcYazismalarTable>(entity));

                    BLLActions.IcYazismaHierarchyTable bllIcYazismaHierarchyTable = new BLLActions.IcYazismaHierarchyTable(_configuration, _env);
                    Data.Models.IcYazismaHierarchyTable? icYazismaHierarchyTable = icYazismaTable?.kanalId == null ? null
                            : bllIcYazismaHierarchyTable.GetByID(icYazismaTable.kanalId ?? 0);

                    BLLActions.AdminUsers bllAdminUsers = new BLLActions.AdminUsers(_configuration, _env, _mapper);
                    UserByNameEMailDto? kanalUser = icYazismaHierarchyTable == null ? null
                            : bllAdminUsers.getUserByNameAndEmail(icYazismaHierarchyTable.userId ?? 0);
                    if (icYazismaTable?.onaylayici1 == null)
                    {
                        if (icYazismaTable?.kanalId != null)
                        {
                            Data.Models.IcYazismalarDetayTable icYazismalarDetayTable = new Data.Models.IcYazismalarDetayTable();
                            icYazismalarDetayTable.createdDate = DateTime.Now;
                            icYazismalarDetayTable.userId = icYazismaHierarchyTable?.userId;
                            icYazismalarDetayTable.icYazismaId = icYazismaTable.Id;
                            icYazismalarDetayTable.enabled = true;

                            BLLActions.IcYazismalarDetayTable bllIcYazismalarDetayTable = new BLLActions.IcYazismalarDetayTable(_configuration, _env);
                            await bllIcYazismalarDetayTable.Add(icYazismalarDetayTable);

                            EmailMessage emailMessage = new EmailMessage();
                            emailMessage.subject = icYazismaTable.konu + " hk.";
                            emailMessage.toAddress = kanalUser?.email;

                            string mailMessage = buildIcYazisma(icYazismaTable);
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
                        UserByNameEMailDto nextUser = bllAdminUsers.getUserByNameAndEmail(icYazismaTable.onaylayici1 ?? 0);
                        Data.Models.IcYazismalarDetayTable icYazismalarDetayTable = new Data.Models.IcYazismalarDetayTable();
                        icYazismalarDetayTable.createdDate = DateTime.Now;
                        icYazismalarDetayTable.userId = icYazismaTable.onaylayici1;
                        icYazismalarDetayTable.icYazismaId = icYazismaTable.Id;
                        icYazismalarDetayTable.enabled = true;
                        BLLActions.IcYazismalarDetayTable bllIcYazismalarDetayTable = new BLLActions.IcYazismalarDetayTable(_configuration, _env);
                        await bllIcYazismalarDetayTable.Add(icYazismalarDetayTable);
                        EmailMessage emailMessage = new EmailMessage();
                        emailMessage.subject = icYazismaTable.konu + " hk.";
                        emailMessage.toAddress = nextUser.email;
                        string mailMessage = buildIcYazisma(icYazismaTable);
                        emailMessage.emailText = mailMessage;
                        emailMessage.mailTuru = 4;
                        emailMessage.enabled = true;
                        emailMessage.isSent = false;
                        emailMessage.plannedDate = DateTime.Now;

                        BLLActions.EmailMessages bllEmailMessages = new BLLActions.EmailMessages(_configuration, _env);
                        await bllEmailMessages.Add(emailMessage);
                    }
                    return _mapper.Map<IcYazismalarTableSaveDto>(icYazismaTable);
                }
                else
                {
                    Data.Models.IcYazismalarTable? eskiYazisma = GetByID(entity.id ?? 0);
                    Data.Models.IcYazismalarTable yeniYazisma = _mapper.Map<Data.Models.IcYazismalarTable>(entity);
                    if (eskiYazisma != null)
                    {
                        if (yeniYazisma.onaylayici1 != eskiYazisma.onaylayici1
                                || yeniYazisma.onaylayici2 != eskiYazisma.onaylayici2
                                || yeniYazisma.onaylayici3 != eskiYazisma.onaylayici3
                                || yeniYazisma.onaylayici4 != eskiYazisma.onaylayici4
                                || yeniYazisma.kanalId != eskiYazisma.kanalId)
                        {

                            // olanları silme
                            BLLActions.IcYazismalarDetayTable bllIcYazismalarDetayTable = new BLLActions.IcYazismalarDetayTable(_configuration, _env);
                            List<Data.Models.IcYazismalarDetayTable> icYazismalarDetayTableSilinecek = new List<Data.Models.IcYazismalarDetayTable>();
                            icYazismalarDetayTableSilinecek = bllIcYazismalarDetayTable.findAllByEnabledAndIcYazismaId(true,
                                    yeniYazisma.Id);
                            foreach (Data.Models.IcYazismalarDetayTable icYazismalarDetayTable in icYazismalarDetayTableSilinecek)
                            {
                                bllIcYazismalarDetayTable.Delete(icYazismalarDetayTable.Id);
                            }
                            BLLActions.IcYazismaHierarchyTable bllIcYazismaHierarchyTable = new BLLActions.IcYazismaHierarchyTable(_configuration, _env);
                            Data.Models.IcYazismaHierarchyTable? icYazismaHierarchyTable = yeniYazisma.kanalId == null ? null
                                    : bllIcYazismaHierarchyTable.GetByID(yeniYazisma.kanalId ?? 0);

                            BLLActions.AdminUsers bllAdminUsers = new BLLActions.AdminUsers(_configuration, _env, _mapper);
                            UserByNameEMailDto? kanalUser = icYazismaHierarchyTable == null ? null
                                    : bllAdminUsers.getUserByNameAndEmail(icYazismaHierarchyTable.userId ?? 0);
                            if (yeniYazisma.onaylayici1 == null)
                            {
                                if (yeniYazisma.kanalId != null)
                                {
                                    Data.Models.IcYazismalarDetayTable icYazismalarDetayTable = new Data.Models.IcYazismalarDetayTable();
                                    icYazismalarDetayTable.createdDate = DateTime.Now;
                                    icYazismalarDetayTable.userId = icYazismaHierarchyTable?.userId;
                                    icYazismalarDetayTable.icYazismaId = yeniYazisma.Id;
                                    icYazismalarDetayTable.enabled = true;
                                    await bllIcYazismalarDetayTable.Add(icYazismalarDetayTable);

                                    EmailMessage emailMessage = new EmailMessage();
                                    emailMessage.subject = yeniYazisma.konu + " hk.";
                                    emailMessage.toAddress = kanalUser?.email;
                                    string mailMessage = buildIcYazisma(yeniYazisma);
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
                                UserByNameEMailDto nextUser = bllAdminUsers.getUserByNameAndEmail(yeniYazisma.onaylayici1 ?? 0);
                                Data.Models.IcYazismalarDetayTable icYazismalarDetayTable = new Data.Models.IcYazismalarDetayTable();
                                icYazismalarDetayTable.createdDate = DateTime.Now;
                                icYazismalarDetayTable.userId = yeniYazisma.onaylayici1;
                                icYazismalarDetayTable.icYazismaId = yeniYazisma.Id;
                                icYazismalarDetayTable.enabled = true;
                                await bllIcYazismalarDetayTable.Add(icYazismalarDetayTable);

                                EmailMessage emailMessage = new EmailMessage();
                                emailMessage.subject = yeniYazisma.konu + " hk.";
                                emailMessage.toAddress = nextUser.email;

                                string mailMessage = buildIcYazisma(yeniYazisma);
                                emailMessage.emailText = mailMessage;
                                emailMessage.mailTuru = 4;
                                emailMessage.enabled = true;
                                emailMessage.isSent = false;
                                emailMessage.plannedDate = DateTime.Now;
                                BLLActions.EmailMessages bllEmailMessages = new BLLActions.EmailMessages(_configuration, _env);
                                await bllEmailMessages.Add(emailMessage);
                            }
                        }

                        entity.updatedUserId = userId;
                        entity.updateDate = DateTime.Now.ToString();
                        Data.Models.IcYazismalarTable updateIcYazisma = await Update(_mapper.Map<Data.Models.IcYazismalarTable>(entity));
                        return _mapper.Map<IcYazismalarTableSaveDto>(updateIcYazisma);
                    }
                    return null;
                }

            }

            public string buildIcYazisma(Data.Models.IcYazismalarTable? icYazismaTable)
            {
                if (icYazismaTable != null)
                {
                    //DateTimeFormatter dateTimeFormatter = DateTimeFormatter.ofPattern("dd.MM.yyyy");
                    string mailstring = "<link rel='stylesheet' href='//netdna.bootstrapcdn.com/bootstrap/3.0.0/css/bootstrap.min.css'>"
                            + "<div>" + "<script src='//netdna.bootstrapcdn.com/bootstrap/3.0.0/js/bootstrap.min.js'></script>" +

                            "<div class='form-group'>" + "<label class='col-sm-3'>Servisi:</label>"
                            + "<div class='col-sm-9 vcenter-form'>" + "<strong>" + icYazismaTable.servisi + "</strong>"
                            + "</div>" + "</div>" + "<div class='form-group'>" + "<label class='col-sm-3'>Konu:</label>"
                            + "<div class='col-sm-9 vcenter-form'>" + "<strong>" + icYazismaTable.konu + "</strong>" + "</div>"
                            + "</div>" + "<div class='form-group'>" + "<label class='col-sm-3'>Tarih:</label>"
                            + "<div class='col-sm-9 vcenter-form'>" + "<strong>"
                            + (icYazismaTable.tarih ?? DateTime.Now).ToString("dd.MM.yyyy") + "</strong>" + "</div>" + "</div>" +

                            "<div class='form-group'>" + "<label class='col-sm-3'>Sayı:</label>"
                            + "<div class='col-sm-9 vcenter-form'>" + "<strong>" + icYazismaTable.Id.ToString() + "</strong>"
                            + "</div>" + "</div>" +

                            "<div class='form-group'>" + "<label class='col-sm-3'>Kanal:</label>"
                            + "<div class='col-sm-9 vcenter-form'>" + "<strong>" + icYazismaTable.kanalGorusu + "</strong>"
                            + "</div>" + "</div>" + "<div class='form-group'>" +

                            "<div class='col-sm-9 vcenter-form'>" + icYazismaTable.icerik + "</div>" + "</div>" +

                            "<div class='form-group'>" + "<label class='col-sm-3 control-label no-padding-right'></label>"
                            + "<div class='col-sm-9 vcenter-form'>";

                    mailstring += "</div></div>";

                    return mailstring;
                }
                else { return ""; }
            }

            public PageReturn<IcYazismaTableDto> list(FilterPageParam<InternalCorrespondencePageableListBilgiDtoParameter> filterPageParam)
            {
                PageReturn<IcYazismaTableDto>? result = new PageReturn<IcYazismaTableDto>();
                int pageSize = filterPageParam.size ?? 20;
                int pageNumber = filterPageParam.page ?? 0;

                int? id = filterPageParam?.liste?.id;
                int? companyId = filterPageParam?.liste?.companyId;
                string? servisi = filterPageParam?.liste?.servisi;
                string? konu = filterPageParam?.liste?.konu;
                bool? bittiMi = filterPageParam?.liste?.bittiMi;
                bool? redEttiMi = filterPageParam?.liste?.redEttiMi;
                int userId = filterPageParam?.liste?.userId ?? 0;
                BLLActions.AdminUsers bllAdminUsers = new BLLActions.AdminUsers(_configuration, _env, _mapper);
                AdminUser? user = bllAdminUsers.GetByID(userId);
                BLLActions.RoleDetails bllRoleDetails = new BLLActions.RoleDetails(_configuration, _env, _mapper);
                RoleDetail? roleDetail = bllRoleDetails.GetByRoleIDAndModuleID(user.roleId, (int)CommonConstants.MODULES.ICYAZISMA);

                if (user.roleId == 1)
                {
                    var query =
    (from a in dal.dB.IcYazismalarTable
     join c in dal.dB.Company
         on a.companyId equals c.Id into companyJoin
     from c in companyJoin.DefaultIfEmpty() 
     join d in dal.dB.AdminUser
         on a.createdUserId equals d.Id
     join b in dal.dB.IcYazismaHierarchyTable
         on a.kanalId equals b.Id into hierarchyJoin
     from b in hierarchyJoin.DefaultIfEmpty()
     where
         a.enabled &&
         (id == null || a.Id == id) &&
         (string.IsNullOrEmpty(konu) || a.konu.Contains(konu)) &&
         (companyId == null || a.companyId == companyId) &&
         (string.IsNullOrEmpty(servisi) || a.servisi.Contains(servisi)) &&
         (redEttiMi == null || a.redEttiMi == redEttiMi) &&
         (bittiMi == null || a.bittiMi == bittiMi)
     select new IcYazismaTableDto
     {
         id = a.Id,
         companyName = c != null ? c.vtext : "",
         servisi = a.servisi,
         konu = a.konu,
         createdDate = a.tarih,
         kanal = b != null ? b.bolumAdi : "",
         createdUser = d.name,
         status = a.redEttiMi,
         createdUserId = a.createdUserId,
         onay1Ok = a.onay1Ok
     })
     .Distinct();
                    result.totalElements = query.Count();

                    result.content = query
                        .OrderByDescending(x => x.id)
                        .Skip(pageSize * pageNumber)
                        .Take(pageSize)
                        .ToList();

                    result.number = pageNumber;
                    result.size = pageSize;
                    return result;
                }
                else if (roleDetail != null && roleDetail.canSeeLogs)
                {
                    BLLActions.Roles bllRoles = new BLLActions.Roles(_configuration, _env, _mapper);
                    Role? role = bllRoles.GetByID(user.roleId);
                    string[] listCompanyIds = role?.companies.Replace("[", "").Replace("]", "").Split(",") ?? [];
                    List<int> listCompanyIdsint = new List<int>();
                    foreach (string companyIds in listCompanyIds)
                    {
                        BLLActions.Companies bllCompanies = new BLLActions.Companies(_configuration, _env, _mapper);
                        Company company = bllCompanies.getByVkorgCompany(companyIds);
                        listCompanyIdsint.Add(company.Id);
                    }


                    var query =
    from a in dal.dB.IcYazismalarTable
    join c in dal.dB.Company
        on a.companyId equals c.Id into companyJoin
    from c in companyJoin.DefaultIfEmpty() 
    join d in dal.dB.AdminUser
        on a.createdUserId equals d.Id
    join b in dal.dB.IcYazismaHierarchyTable
        on a.kanalId equals b.Id into hierarchyJoin
    from b in hierarchyJoin.DefaultIfEmpty() 
    where
        a.enabled &&
        (id == null || a.Id == id) &&
        (string.IsNullOrEmpty(konu) || a.konu.Contains(konu)) &&
        (companyId == null || a.companyId == companyId) &&
        (string.IsNullOrEmpty(servisi) || a.servisi.Contains(servisi)) &&
        (redEttiMi == null || a.redEttiMi == redEttiMi) &&
        (bittiMi == null || a.bittiMi == bittiMi) &&
        listCompanyIdsint.Contains(d.companyId)  
    orderby a.Id descending
    select new IcYazismaTableDto
    {
        id = a.Id,
        companyName = c != null ? c.vtext : "",
        servisi = a.servisi,
        konu = a.konu,
        createdDate = a.tarih,
        kanal = b != null ? b.bolumAdi : "",
        createdUser = d.name,
        status = a.redEttiMi,
        createdUserId = a.createdUserId,
        onay1Ok = a.onay1Ok
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
                        from a in dal.dB.IcYazismalarTable
                        join c in dal.dB.Company
                            on a.companyId equals c.Id
                        join d in dal.dB.AdminUser
                            on a.createdUserId equals d.Id
                        join b in dal.dB.IcYazismaHierarchyTable
                            on a.kanalId equals b.Id into hierarchyJoin
                        from b in hierarchyJoin.DefaultIfEmpty()
                        where
                            a.enabled &&
                            (id == null || a.Id == id) &&
                            (string.IsNullOrEmpty(konu) || a.konu.Contains(konu)) &&
                            (companyId == null || a.companyId == companyId) &&
                            (string.IsNullOrEmpty(servisi) || a.servisi == servisi) &&
                            (redEttiMi == null || a.redEttiMi == redEttiMi) &&
                            (bittiMi == null || a.bittiMi == bittiMi) &&
                            (
                                a.createdUserId == userId ||
                                a.onaylayici1 == userId ||
                                a.onaylayici2 == userId ||
                                a.onaylayici3 == userId ||
                                a.onaylayici4 == userId ||
                                (b != null && b.userId == userId)
                            )
                        orderby a.Id descending
                        select new IcYazismaTableDto
                        {
                            id = a.Id,
                            companyName = c != null ? c.vtext : "",
                            servisi = a.servisi,
                            konu = a.konu,
                            createdDate = a.tarih,
                            kanal = b != null ? b.bolumAdi : "",
                            createdUser = d.name,
                            status = a.redEttiMi,
                            createdUserId = a.createdUserId,
                            onay1Ok = a.onay1Ok
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

            public IcYazismaDetayDto getDetail(IcYazismaTableDto icYazismaTableDto, int userId)
            {
                BLLActions.AdminUsers bllAdminUsers = new BLLActions.AdminUsers(_configuration, _env, _mapper);
                AdminUser user = bllAdminUsers.GetByID(userId)!;
                BLLActions.AuditorTable bllAuditorTable = new BLLActions.AuditorTable(_configuration, _env);
                List<Data.Models.AuditorTable> listAuditorTables = bllAuditorTable.listAllByEnabled(true);
                Data.Models.IcYazismalarTable icYazisma = GetByID(icYazismaTableDto.id ?? 0)!;
                BLLActions.Companies bllCompanies = new BLLActions.Companies(_configuration, _env, _mapper);
                Company? company = bllCompanies.GetByID(icYazisma.companyId ?? 0);
                BLLActions.IcYazismalarDetayTable bllIcYazismalarDetayTable = new BLLActions.IcYazismalarDetayTable(_configuration, _env);
                List<Data.Models.IcYazismalarDetayTable> listIcYazismalarDetayTables = bllIcYazismalarDetayTable
                        .findAllByEnabledAndIcYazismaId(true, icYazismaTableDto.id ?? 0);
                IcYazismaDetayDto icYazismaDetayDto = new IcYazismaDetayDto();
                BLLActions.AttachedFiles bllAttachedFiles = new BLLActions.AttachedFiles(_configuration, _env);
                List<AttachedFile> listAttachedFiles = bllAttachedFiles
                        .getByModuleIdAndTargetId((int)CommonConstants.MODULES.ICYAZISMA, icYazisma.Id);
                icYazismaDetayDto.id = icYazismaTableDto.id;
                icYazismaDetayDto.companyName = icYazismaTableDto.companyName;
                icYazismaDetayDto.createdDate = icYazisma.tarih;
                icYazismaDetayDto.createdUser = icYazismaTableDto.createdUser;
                icYazismaDetayDto.konu = icYazismaTableDto.konu;
                icYazismaDetayDto.servisi = icYazismaTableDto.servisi;
                icYazismaDetayDto.icerik = icYazisma.icerik;
                icYazismaDetayDto.kanal = icYazismaTableDto.kanal;
                icYazismaDetayDto.companyTitle = company.companyTitle;
                icYazismaDetayDto.companyLongName = company.companyLongName;
                icYazismaDetayDto.listAttachedFile = listAttachedFiles;
                List<OnaylayiciDto> listOnaylayiciDtos = new List<OnaylayiciDto>();
                if (icYazisma.onaylayici1 != null)
                {
                    listOnaylayiciDtos
                            .Add(getOnaylayiciDto(icYazisma.onaylayici1 ?? 0, icYazisma, listIcYazismalarDetayTables, false));
                }
                if (icYazisma.onaylayici2 != null)
                {
                    listOnaylayiciDtos
                            .Add(getOnaylayiciDto(icYazisma.onaylayici2 ?? 0, icYazisma, listIcYazismalarDetayTables, false));
                }
                if (icYazisma.onaylayici3 != null)
                {
                    listOnaylayiciDtos
                            .Add(getOnaylayiciDto(icYazisma.onaylayici3 ?? 0, icYazisma, listIcYazismalarDetayTables, false));
                }
                if (icYazisma.onaylayici4 != null)
                {
                    listOnaylayiciDtos
                            .Add(getOnaylayiciDto(icYazisma.onaylayici4 ?? 0, icYazisma, listIcYazismalarDetayTables, false));
                }
                if (icYazisma.kanalId != null)
                {
                    BLLActions.IcYazismaHierarchyTable bllIcYazismaHierarchyTable = new BLLActions.IcYazismaHierarchyTable(_configuration, _env);
                    Data.Models.IcYazismaHierarchyTable icYazismaHierarchyTable = bllIcYazismaHierarchyTable
                            .GetByID(icYazisma.kanalId ?? 0)!;
                    listOnaylayiciDtos.Add(getOnaylayiciDto(icYazismaHierarchyTable.userId ?? 0, icYazisma,
                            listIcYazismalarDetayTables, false));
                }

                icYazismaDetayDto.listOnayDurumu = listOnaylayiciDtos;
                List<InternalCorrespondenceMessageDto> listMessageDtos = getMessageDtos(icYazisma.Id, user,
                        listAuditorTables);
                icYazismaDetayDto.listOnayDurumu = listOnaylayiciDtos;
                icYazismaDetayDto.listIcYazismaMesajDto = listMessageDtos;
                return icYazismaDetayDto;
            }

            private List<InternalCorrespondenceMessageDto> getMessageDtos(int icYazismaId, AdminUser user,
            List<Data.Models.AuditorTable> listAuditorTables)
            {
                //DateTimeFormatter dtFormatter = DateTimeFormatter.ofPattern("dd.MM.yyyy hh:mm:ss");
                List<InternalCorrespondenceMessageDto> listeCorrespondenceMessageDtos = new List<InternalCorrespondenceMessageDto>();
                BLLActions.IcYazismalarMesajTable bllIcYazismalarMesajTable = new BLLActions.IcYazismalarMesajTable(_configuration, _env);
                List<Data.Models.IcYazismalarMesajTable> listIcYazismaMesaj = bllIcYazismalarMesajTable
                        .findAllByIcYazismaIdAndEnabledOrderByCreatedDate(icYazismaId, true);
                bool hasAutditor = listAuditorTables.Any(u => u.userId.Equals(user.Id));

                foreach (Data.Models.IcYazismalarMesajTable icYazismaMesaj in listIcYazismaMesaj)
                {
                    bool hasMessageUser = false;
                    if (icYazismaMesaj.sendUserId != null)
                    {
                        hasMessageUser = icYazismaMesaj.sendUserId == user.Id;
                    }

                    if ((icYazismaMesaj.showAll ?? false) || hasAutditor || hasMessageUser || user.roleId == 1)
                    {
                        BLLActions.AdminUsers bllAdminUsers = new BLLActions.AdminUsers(_configuration, _env, _mapper);
                        UserByNameEMailDto userByNameEMailDto = bllAdminUsers.getUserByNameAndEmail(icYazismaMesaj.userId ?? 0);
                        InternalCorrespondenceMessageDto correspondenceMessageDto = new InternalCorrespondenceMessageDto();
                        correspondenceMessageDto.id = icYazismaMesaj.Id;
                        correspondenceMessageDto.message = icYazismaMesaj.message;
                        correspondenceMessageDto.time = (icYazismaMesaj.createdDate ?? DateTime.Now).ToString("dd.MM.yyyy hh:mm:ss");
                        correspondenceMessageDto.username = userByNameEMailDto.name;
                        listeCorrespondenceMessageDtos.Add(correspondenceMessageDto);
                    }
                }

                return listeCorrespondenceMessageDtos;

            }
            private OnaylayiciDto getOnaylayiciDto(int userId, Data.Models.IcYazismalarTable icYazisma,
            List<Data.Models.IcYazismalarDetayTable> listIcYazismalarDetayTables, bool before)
            {
                //DateTimeFormatter dateTimeFormatter = DateTimeFormatter.ofPattern("dd.MM.yyyy");
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
                    bool onaylayiciVarMi = listIcYazismalarDetayTables.Any(u => u.userId.Equals(userId) && u.sonOnayMi == before);


                    onaylayiciDto.file = listInt;
                    if (onaylayiciVarMi)
                    {
                        Data.Models.IcYazismalarDetayTable icYazismalarDetayTable = listIcYazismalarDetayTables.First(u => Equals(u.userId, userId) && u.sonOnayMi == before);

                        if (icYazismalarDetayTable.approved == null)
                        {
                            onaylayiciDto.onayDurumu = ("Onay Bekleniyor");
                            onaylayiciDto.durum = 3;
                        }
                        else if (icYazismalarDetayTable.approved == true)
                        {
                            onaylayiciDto.onayDurumu = (
                                    "Onaylama Tarihi: " + icYazismalarDetayTable.replyDate?.ToString("dd.MM.yyyy"));
                            onaylayiciDto.durum = 1;
                        }
                        else if (icYazismalarDetayTable.approved == false)
                        {
                            onaylayiciDto.onayDurumu = (
                                    "Red Tarihi: " + icYazismalarDetayTable.replyDate?.ToString("dd.MM.yyyy"));
                            onaylayiciDto.durum = 2;
                        }
                    }
                    else
                    {
                        onaylayiciDto.onayDurumu = "";
                        onaylayiciDto.durum = 4;
                    }
                }

                return onaylayiciDto;
            }

            public PageReturn<IcYazismaTableDto> mylist(FilterPageParam<InternalCorrespondencePageableListBilgiDtoParameter> filterPageParam)
            {
                PageReturn<IcYazismaTableDto>? result = new PageReturn<IcYazismaTableDto>();
                int pageSize = filterPageParam.size ?? 20;
                int pageNumber = filterPageParam.page ?? 0;

                int? id = filterPageParam?.liste?.id;
                int? companyId = filterPageParam?.liste?.companyId;
                string? servisi = filterPageParam?.liste?.servisi;
                string? konu = filterPageParam?.liste?.konu;
                bool? bittiMi = filterPageParam?.liste?.bittiMi;
                bool? redEttiMi = filterPageParam?.liste?.redEttiMi;
                int userId = filterPageParam?.liste?.userId ?? 0;
                BLLActions.AdminUsers bllAdminUsers = new BLLActions.AdminUsers(_configuration, _env, _mapper);
                AdminUser? user = bllAdminUsers.GetByID(userId);
                BLLActions.AuditorTable bllAuditorTable = new BLLActions.AuditorTable(_configuration, _env);
                List<Data.Models.AuditorTable> listAuditorTable = bllAuditorTable.listAllByEnabled(true);

                if (listAuditorTable.Any(u => u.userId == userId))
                {
                    var query =
    from a in dal.dB.IcYazismalarTable
    join c in dal.dB.Company
        on a.companyId equals c.Id
    join d in dal.dB.AdminUser
        on a.createdUserId equals d.Id
    join b in dal.dB.IcYazismaHierarchyTable
        on a.kanalId equals b.Id into hierarchyJoin
    from b in hierarchyJoin.DefaultIfEmpty() 
    where
        (( id == null ) ||(a.Id ==id)) &&
			(a.konu.Contains(konu??"") || (konu == "" || konu == null)) &&
			(( companyId == null) || (a.companyId ==companyId)) &&
			(a.servisi.Contains(servisi??"") || (servisi ==""  ||  servisi == null)) &&
			( redEttiMi == null || a.redEttiMi ==redEttiMi) &&
			( bittiMi == null || a.bittiMi == bittiMi ) &&
			a.enabled 
    orderby a.Id descending
    select new IcYazismaTableDto
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
        onay1Ok = a.onay1Ok
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
    (from a in dal.dB.IcYazismalarTable
     join c in dal.dB.Company
         on a.companyId equals c.Id
     join d in dal.dB.AdminUser
         on a.createdUserId equals d.Id
     join b in dal.dB.IcYazismalarDetayTable
         on a.Id equals b.icYazismaId
     join e in dal.dB.IcYazismaHierarchyTable
         on a.kanalId equals e.Id into hierarchyJoin
     from e in hierarchyJoin.DefaultIfEmpty() 
     where
       ((a.Id == id) || (id == null)) && 
			b.approved==null && b.enabled  && b.userId ==userId && 

            a.onaylandiMi == false &&
			(a.konu.Contains(konu??"") || (konu == ""  && a.konu == null)) &&
			(a.companyId ==companyId || (companyId == null )) &&
			(a.servisi ==servisi || (servisi == ""  ||  servisi == null)) &&
			(a.redEttiMi == redEttiMi || redEttiMi == null ) &&
			(a.bittiMi == bittiMi ) &&
			a.enabled 
     select new IcYazismaTableDto
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
         onay1Ok = a.onay1Ok
     })
     .Distinct();

                    result.totalElements = query.Count();

                    result.content = query
                        .OrderByDescending(x => x.id)
                        .Skip(pageSize * pageNumber)
                        .Take(pageSize)
                        .ToList();

                    result.number = pageNumber;
                    result.size = pageSize;
                    return result;
                }
            }

            public async Task<int> approve(IcYazismaResponseMyList responseMyList, int userId)
            {
                try
                {
                    Data.Models.IcYazismalarTable icYazismaTable = _mapper.Map < Data.Models.IcYazismalarTable > (responseMyList.icYazismalarTable);
                    BLLActions.AdminUsers bllAdminUsers = new BLLActions.AdminUsers(_configuration, _env, _mapper);
                    AdminUser user = bllAdminUsers.GetByID(userId)!;

                    AdminUser? onaylayici2 = icYazismaTable.onaylayici2 != null
                            ? bllAdminUsers.GetByID(icYazismaTable.onaylayici2??0)
                            : null;
                    AdminUser? onaylayici3 = icYazismaTable.onaylayici3 != null
                            ? bllAdminUsers.GetByID(icYazismaTable.onaylayici3??0)
                            : null;
                    AdminUser? onaylayici4 = icYazismaTable.onaylayici4 != null
                            ? bllAdminUsers.GetByID(icYazismaTable.onaylayici4??0)
                            : null;
                    BLLActions.IcYazismaHierarchyTable bllIcYazismaHierarchyTable = new BLLActions.IcYazismaHierarchyTable(_configuration, _env);
                    Data.Models.IcYazismaHierarchyTable? icYazismaHierarchyTable = icYazismaTable.kanalId == null ? null
                            : bllIcYazismaHierarchyTable.GetByID(icYazismaTable.kanalId??0);
                    AdminUser? kanalUser = icYazismaHierarchyTable != null ? bllAdminUsers.GetByID(icYazismaHierarchyTable.userId??0)
                            : null;

                    AdminUser? createdUser = bllAdminUsers.GetByID(icYazismaTable.createdUserId??0);

                    if (icYazismaTable.onaylayici1 != null
                            && icYazismaTable.onaylayici1== user.Id && icYazismaTable.onay1Ok == false)
                    {
                        icYazismaTable.onay1Ok=true;

                        Data.Models.IcYazismalarMesajTable icYazismaMesaj = new Data.Models.IcYazismalarMesajTable();
                        icYazismaMesaj.createdDate=DateTime.Now;
                        icYazismaMesaj.showAll=true;
                        icYazismaMesaj.icYazismaId=icYazismaTable.Id;
                        icYazismaMesaj.userId=user.Id;
                        icYazismaMesaj.message=responseMyList.kanalGorusuFirst;
                        icYazismaMesaj.enabled=true;

                        BLLActions.IcYazismalarMesajTable bllIcYazismalarMesajTable = new BLLActions.IcYazismalarMesajTable(_configuration, _env);
                        await bllIcYazismalarMesajTable.Add(icYazismaMesaj);

                        BLLActions.IcYazismalarDetayTable bllIcYazismalarDetayTable=new BLLActions.IcYazismalarDetayTable(_configuration,_env);
                        List<Data.Models.IcYazismalarDetayTable> listIcYazismalarDetayTable = bllIcYazismalarDetayTable
                                .findAllByIcYazismaIdAndApprovedAndUserIdAndEnabled(icYazismaTable.Id, null, user.Id,
                                        true);

                        for (int j = 1; j < listIcYazismalarDetayTable.Count(); j++)
                        {
                            int id = listIcYazismalarDetayTable[j].Id;
                            bllIcYazismalarDetayTable.Delete(id);
                        }
                        Data.Models.IcYazismalarDetayTable icYazismalarDetayTable = listIcYazismalarDetayTable[0];
                        icYazismalarDetayTable.isReplied=true;
                        icYazismalarDetayTable.approved=true;
                        icYazismalarDetayTable.replyDate=DateTime.Now;
                        await bllIcYazismalarDetayTable.Update(icYazismalarDetayTable);

                        if (icYazismaTable.onaylayici2!= null)
                        {
                            Data.Models.IcYazismalarDetayTable icYazismalarDetayTableNext = new Data.Models.IcYazismalarDetayTable();
                            icYazismalarDetayTableNext.userId=icYazismaTable.onaylayici2;
                            icYazismalarDetayTableNext.createdDate=DateTime.Now;
                            icYazismalarDetayTableNext.approved=null;
                            icYazismalarDetayTableNext.icYazismaId=icYazismaTable.Id;
                            icYazismalarDetayTableNext.isReplied=false;
                            icYazismalarDetayTableNext.replyDate=null;
                            icYazismalarDetayTableNext.enabled=true;

                            await bllIcYazismalarDetayTable.Add(icYazismalarDetayTableNext);

                            EmailMessage emailMessage = new EmailMessage();
                            emailMessage.toAddress=onaylayici2.email;
                            emailMessage.subject=icYazismaTable.konu + " hk.";
                            emailMessage.isSent=false;
                            emailMessage.emailText=buildIcYazisma(icYazismaTable);
                            emailMessage.mailTuru=4;
                            emailMessage.plannedDate=DateTime.Now;
                            emailMessage.enabled=true;
                            BLLActions.EmailMessages bllEmailMessages = new BLLActions.EmailMessages(_configuration, _env);
                            await bllEmailMessages.Add(emailMessage);
                        }
                        else if (icYazismaHierarchyTable!=null)
                        {
                            Data.Models.IcYazismalarDetayTable icYazismalarDetayTableNext = new Data.Models.IcYazismalarDetayTable();
                            icYazismalarDetayTableNext.userId=icYazismaHierarchyTable.userId;
                            icYazismalarDetayTableNext.createdDate=DateTime.Now;
                            icYazismalarDetayTableNext.approved=null;
                            icYazismalarDetayTableNext.icYazismaId=icYazismaTable.Id;
                            icYazismalarDetayTableNext.isReplied=false;
                            icYazismalarDetayTableNext.replyDate=null;
                            icYazismalarDetayTableNext.enabled=true;

                            await bllIcYazismalarDetayTable.Add(icYazismalarDetayTableNext);

                            EmailMessage emailMessage = new EmailMessage();
                            emailMessage.toAddress=kanalUser.email;
                            emailMessage.subject=icYazismaTable.konu + " hk.";
                            emailMessage.isSent=false;
                            emailMessage.emailText = buildIcYazisma(icYazismaTable);
                            emailMessage.mailTuru=4;
                            emailMessage.plannedDate=DateTime.Now;
                            emailMessage.enabled=true;
                            BLLActions.EmailMessages bllEmailMessages = new BLLActions.EmailMessages(_configuration, _env);
                            await bllEmailMessages.Add(emailMessage);
                        }
                        else
                        {

                            for (int j = 1; j < listIcYazismalarDetayTable.Count(); j++)
                            {

                                int id = listIcYazismalarDetayTable[j].Id;
                                bllIcYazismalarDetayTable.Delete(id);
                            }

                            icYazismalarDetayTable.isReplied=true;
                            icYazismalarDetayTable.approved=true;
                            icYazismalarDetayTable.replyDate=DateTime.Now;
                            await bllIcYazismalarDetayTable.Update(icYazismalarDetayTable);

                            icYazismaMesaj.createdDate=DateTime.Now;
                            icYazismaMesaj.showAll=true;
                            icYazismaMesaj.userId=user.Id;
                            icYazismaMesaj.message=responseMyList.kanalGorusuFirst;
                            icYazismaMesaj.enabled=true;
                            icYazismaMesaj.icYazismaId=icYazismaTable.Id;
                            await bllIcYazismalarMesajTable.Update(icYazismaMesaj);

                            icYazismaTable.bittiMi=true;

                            EmailMessage emailMessage = new EmailMessage();
                            emailMessage.toAddress=createdUser?.email;
                            emailMessage.subject=icYazismaTable.konu + " hk.";
                            emailMessage.isSent=false;
                            emailMessage.emailText="<h2>Sayın " + createdUser?.name + "</h2><br/><h4>Bir adet "
                                    + icYazismaTable.Id.ToString() + " ID'li ve " + icYazismaTable.konu
                                    + " Konulu iç yazışma bitmiştir.<br/></h4>" + buildIcYazisma(icYazismaTable);
                            emailMessage.mailTuru=4;
                            emailMessage.plannedDate=DateTime.Now;
                            emailMessage.enabled=true;
                             BLLActions.EmailMessages bllEmailMessages = new BLLActions.EmailMessages(_configuration, _env);

                        }

                        await Update(icYazismaTable);
                        return 1;

                    }
                    else if (icYazismaTable.onaylayici2!= null
                            && icYazismaTable.onaylayici2== user.Id && icYazismaTable.onay2Ok==false)
                    {
                        icYazismaTable.onay2Ok=true;

                        Data.Models.IcYazismalarMesajTable icYazismaMesaj = new Data.Models.IcYazismalarMesajTable();
                        icYazismaMesaj.createdDate=DateTime.Now;
                        icYazismaMesaj.showAll=true;
                        icYazismaMesaj.userId=user.Id;
                        icYazismaMesaj.message=responseMyList.kanalGorusuFirst;
                        icYazismaMesaj.enabled=true;
                        icYazismaMesaj.icYazismaId=icYazismaTable.Id;
                        BLLActions.IcYazismalarMesajTable bllIcYazismalarMesajTable = new BLLActions.IcYazismalarMesajTable(_configuration, _env);
                        await bllIcYazismalarMesajTable.Add(icYazismaMesaj);

                        BLLActions.IcYazismalarDetayTable bllIcYazismalarDetayTable = new BLLActions.IcYazismalarDetayTable(_configuration, _env);
                        List<Data.Models.IcYazismalarDetayTable> listIcYazismalarDetayTable = bllIcYazismalarDetayTable
                                .findAllByIcYazismaIdAndApprovedAndUserIdAndEnabled(icYazismaTable.Id, null, user.Id,
                                        true);

                        for (int j = 1; j < listIcYazismalarDetayTable.Count(); j++)
                        {
                            int id = listIcYazismalarDetayTable[j].Id;
                            bllIcYazismalarDetayTable.Delete(id);
                        }
                        Data.Models.IcYazismalarDetayTable icYazismalarDetayTable = listIcYazismalarDetayTable[0];
                        icYazismalarDetayTable.isReplied=true;
                        icYazismalarDetayTable.approved=true;
                        icYazismalarDetayTable.replyDate=DateTime.Now;
                        await bllIcYazismalarDetayTable.Update(icYazismalarDetayTable);

                        if (icYazismaTable.onaylayici3!= null)
                        {
                            Data.Models.IcYazismalarDetayTable icYazismalarDetayTableNext = new Data.Models.IcYazismalarDetayTable();
                            icYazismalarDetayTableNext.userId=icYazismaTable.onaylayici3;
                            icYazismalarDetayTableNext.createdDate=DateTime.Now;
                            icYazismalarDetayTableNext.approved=null;
                            icYazismalarDetayTableNext.icYazismaId=icYazismaTable.Id;
                            icYazismalarDetayTableNext.isReplied=false;
                            icYazismalarDetayTableNext.replyDate=null;
                            icYazismalarDetayTableNext.enabled=true;

                            await bllIcYazismalarDetayTable.Add(icYazismalarDetayTableNext);

                            EmailMessage emailMessage = new EmailMessage();
                            emailMessage.toAddress=onaylayici3?.email;
                            emailMessage.subject=icYazismaTable.konu + " hk.";
                            emailMessage.isSent=false;
                            emailMessage.emailText=buildIcYazisma(icYazismaTable);
                            emailMessage.mailTuru=4;
                            emailMessage.plannedDate=DateTime.Now;
                            emailMessage.enabled=true;
                             BLLActions.EmailMessages bllEmailMessages = new BLLActions.EmailMessages(_configuration, _env);
                        }
                        else if (icYazismaHierarchyTable!= null)
                        {
                            Data.Models.IcYazismalarDetayTable icYazismalarDetayTableNext = new Data.Models.IcYazismalarDetayTable();
                            icYazismalarDetayTableNext.userId=icYazismaHierarchyTable.userId;
                            icYazismalarDetayTableNext.createdDate=DateTime.Now;
                            icYazismalarDetayTableNext.approved=null;
                            icYazismalarDetayTableNext.icYazismaId=icYazismaTable.Id;
                            icYazismalarDetayTableNext.isReplied=false;
                            icYazismalarDetayTableNext.replyDate=null;
                            icYazismalarDetayTableNext.enabled=true;
                            await bllIcYazismalarDetayTable.Add(icYazismalarDetayTableNext);

                            EmailMessage emailMessage = new EmailMessage();
                            emailMessage.toAddress=kanalUser?.email;
                            emailMessage.subject=icYazismaTable.konu + " hk.";
                            emailMessage.isSent=false;
                            emailMessage.emailText=buildIcYazisma(icYazismaTable);
                            emailMessage.mailTuru=4;
                            emailMessage.plannedDate=DateTime.Now;
                            emailMessage.enabled=true;
                             BLLActions.EmailMessages bllEmailMessages = new BLLActions.EmailMessages(_configuration, _env);
                            await bllEmailMessages.Add(emailMessage);
                        }
                        else
                        {

                            for (int j = 1; j < listIcYazismalarDetayTable.Count(); j++)
                            {

                                int id = listIcYazismalarDetayTable[j].Id;
                                bllIcYazismalarDetayTable.Delete(id);
                            }

                            icYazismalarDetayTable.isReplied=true;
                            icYazismalarDetayTable.approved=true;
                            icYazismalarDetayTable.replyDate=DateTime.Now;
                            await bllIcYazismalarDetayTable.Update(icYazismalarDetayTable);

                            icYazismaMesaj.createdDate=DateTime.Now;
                            icYazismaMesaj.showAll=true;
                            icYazismaMesaj.userId=user.Id;
                            icYazismaMesaj.message=responseMyList.kanalGorusuFirst;
                            icYazismaMesaj.enabled=true;
                            icYazismaMesaj.icYazismaId=icYazismaTable.Id;
                            await bllIcYazismalarMesajTable.Add(icYazismaMesaj);
                            icYazismaTable.bittiMi=true;

                            EmailMessage emailMessage = new EmailMessage();
                            emailMessage.toAddress=createdUser?.email;
                            emailMessage.subject=icYazismaTable.konu + " hk.";
                            emailMessage.isSent=false;
                            emailMessage.emailText="<h2>Sayın " + createdUser?.name + "</h2><br/><h4>Bir adet "
                                    + icYazismaTable.Id.ToString() + " ID'li ve " + icYazismaTable.konu
                                    + " Konulu iç yazışma bitmiştir.<br/></h4>" + buildIcYazisma(icYazismaTable);
                            emailMessage.mailTuru=4;
                            emailMessage.plannedDate=DateTime.Now;
                            emailMessage.enabled=true;
                             BLLActions.EmailMessages bllEmailMessages = new BLLActions.EmailMessages(_configuration, _env);
                       await bllEmailMessages.Add(emailMessage);
                        }
                        await Update(icYazismaTable);
                        return 1;
                    }
                    else if (icYazismaTable.onaylayici3!= null
                            && icYazismaTable.onaylayici3== user.Id && icYazismaTable.onay3Ok==false)
                    {
                        icYazismaTable.onay3Ok=true;

                        Data.Models.IcYazismalarMesajTable icYazismaMesaj = new Data.Models.IcYazismalarMesajTable();
                        icYazismaMesaj.createdDate=DateTime.Now;
                        icYazismaMesaj.showAll=true;
                        icYazismaMesaj.icYazismaId=icYazismaTable.Id;
                        icYazismaMesaj.userId=user.Id;
                        icYazismaMesaj.message=responseMyList.kanalGorusuFirst;
                        icYazismaMesaj.enabled=true;
                        BLLActions.IcYazismalarMesajTable bllIcYazismalarMesajTable = new BLLActions.IcYazismalarMesajTable(_configuration, _env);
                        await bllIcYazismalarMesajTable.Add(icYazismaMesaj);

                        BLLActions.IcYazismalarDetayTable bllIcYazismalarDetayTable = new BLLActions.IcYazismalarDetayTable(_configuration, _env);
                        List<Data.Models.IcYazismalarDetayTable> listIcYazismalarDetayTable = bllIcYazismalarDetayTable
                                .findAllByIcYazismaIdAndApprovedAndUserIdAndEnabled(icYazismaTable.Id, null, user.Id,
                                        true);

                        for (int j = 1; j < listIcYazismalarDetayTable.Count(); j++)
                        {
                            int id = listIcYazismalarDetayTable[j].Id;
                            bllIcYazismalarDetayTable.Delete(id);
                        }
                        Data.Models.IcYazismalarDetayTable icYazismalarDetayTable = listIcYazismalarDetayTable[0];

                        icYazismalarDetayTable.isReplied=true;
                        icYazismalarDetayTable.approved=true;
                        icYazismalarDetayTable.replyDate=DateTime.Now;
                        await bllIcYazismalarDetayTable.Update(icYazismalarDetayTable);

                        if (icYazismaTable.onaylayici4!= null)
                        {
                            Data.Models.IcYazismalarDetayTable icYazismalarDetayTableNext = new Data.Models.IcYazismalarDetayTable();
                            icYazismalarDetayTableNext.userId=icYazismaTable.onaylayici4;
                            icYazismalarDetayTableNext.createdDate=DateTime.Now;
                            icYazismalarDetayTableNext.approved=null;
                            icYazismalarDetayTableNext.icYazismaId=icYazismaTable.Id;
                            icYazismalarDetayTableNext.isReplied=false;
                            icYazismalarDetayTableNext.replyDate=null;
                            icYazismalarDetayTableNext.enabled=true;

                            await bllIcYazismalarDetayTable.Add(icYazismalarDetayTableNext);

                            EmailMessage emailMessage = new EmailMessage();
                            emailMessage.toAddress=onaylayici4?.email;
                            emailMessage.subject=icYazismaTable.konu + " hk.";
                            emailMessage.isSent=false;
                            emailMessage.emailText=buildIcYazisma(icYazismaTable);
                            emailMessage.mailTuru=4;
                            emailMessage.plannedDate=DateTime.Now;
                            emailMessage.enabled=true;
                             BLLActions.EmailMessages bllEmailMessages = new BLLActions.EmailMessages(_configuration, _env);
                            await bllEmailMessages.Add(emailMessage);
                        }
                        else if (icYazismaHierarchyTable!= null)
                        {
                            Data.Models.IcYazismalarDetayTable icYazismalarDetayTableNext = new Data.Models.IcYazismalarDetayTable();
                            icYazismalarDetayTableNext.userId=icYazismaHierarchyTable.userId;
                            icYazismalarDetayTableNext.createdDate=DateTime.Now;
                            icYazismalarDetayTableNext.approved=null;
                            icYazismalarDetayTableNext.icYazismaId=icYazismaTable.Id;
                            icYazismalarDetayTableNext.isReplied=false;
                            icYazismalarDetayTableNext.replyDate=null;
                            icYazismalarDetayTableNext.enabled=true;

                            await bllIcYazismalarDetayTable.Add(icYazismalarDetayTableNext);
                            EmailMessage emailMessage = new EmailMessage();
                            emailMessage.toAddress=kanalUser?.email;
                            emailMessage.subject=icYazismaTable.konu + " hk.";
                            emailMessage.isSent=false;
                            emailMessage.emailText=buildIcYazisma(icYazismaTable);
                            emailMessage.mailTuru=4;
                            emailMessage.plannedDate=DateTime.Now;
                            emailMessage.enabled=true;
                             BLLActions.EmailMessages bllEmailMessages = new BLLActions.EmailMessages(_configuration, _env);
                        }
                        else
                        {

                            for (int j = 1; j < listIcYazismalarDetayTable.Count(); j++)
                            {
                                int id = listIcYazismalarDetayTable[j].Id;
                                bllIcYazismalarDetayTable.Delete(id);
                            }

                            icYazismalarDetayTable.isReplied = true;
                            icYazismalarDetayTable.approved=true;
                            icYazismalarDetayTable.replyDate=DateTime.Now;
                            await bllIcYazismalarDetayTable.Update(icYazismalarDetayTable);

                            icYazismaMesaj.createdDate=DateTime.Now;
                            icYazismaMesaj.showAll=true;
                            icYazismaMesaj.userId=user.Id;
                            icYazismaMesaj.message=responseMyList.kanalGorusuFirst;
                            icYazismaMesaj.enabled=true;
                            icYazismaMesaj.icYazismaId=icYazismaTable.Id;
                            await bllIcYazismalarMesajTable.Add(icYazismaMesaj);

                            icYazismaTable.bittiMi=true;

                            EmailMessage emailMessage = new EmailMessage();
                            emailMessage.toAddress=createdUser?.email;
                            emailMessage.subject=icYazismaTable.konu + " hk.";
                            emailMessage.isSent=false;
                            emailMessage.emailText="<h2>Sayın " + createdUser?.name + "</h2><br/><h4>Bir adet "
                                    + icYazismaTable.Id.ToString() + " ID'li ve " + icYazismaTable.konu
                                    + " Konulu iç yazışma bitmiştir.<br/></h4>" + buildIcYazisma(icYazismaTable);
                            emailMessage.mailTuru=4;
                            emailMessage.plannedDate=DateTime.Now;
                            emailMessage.enabled=true;
                             BLLActions.EmailMessages bllEmailMessages = new BLLActions.EmailMessages(_configuration, _env);
                            await bllEmailMessages.Add(emailMessage);
                        }
                        await Update(icYazismaTable);
                        return 1;
                    }
                    else if (icYazismaTable.onaylayici4!= null
                            && icYazismaTable.onaylayici4== user.Id)
                    {
                        icYazismaTable.onay4Ok=true;

                        Data.Models.IcYazismalarMesajTable icYazismaMesaj = new Data.Models.IcYazismalarMesajTable();
                        icYazismaMesaj.createdDate=DateTime.Now;
                        icYazismaMesaj.showAll=true;
                        icYazismaMesaj.icYazismaId=icYazismaTable.Id;
                        icYazismaMesaj.userId=user.Id;
                        icYazismaMesaj.message=responseMyList.kanalGorusuFirst;
                        icYazismaMesaj.enabled=true;
                        BLLActions.IcYazismalarMesajTable bllIcYazismalarMesajTable = new BLLActions.IcYazismalarMesajTable(_configuration, _env);
                        await bllIcYazismalarMesajTable.Add(icYazismaMesaj);

                        BLLActions.IcYazismalarDetayTable bllIcYazismalarDetayTable = new BLLActions.IcYazismalarDetayTable(_configuration, _env);
                        List<Data.Models.IcYazismalarDetayTable> listIcYazismalarDetayTable = bllIcYazismalarDetayTable
                                .findAllByIcYazismaIdAndApprovedAndUserIdAndEnabled(icYazismaTable.Id, null, user.Id,
                                        true);

                        for (int j = 1; j < listIcYazismalarDetayTable.Count(); j++)
                        {
                            int id = listIcYazismalarDetayTable[j].Id;
                            bllIcYazismalarDetayTable.Delete(id);
                        }
                        Data.Models.IcYazismalarDetayTable icYazismalarDetayTable = listIcYazismalarDetayTable[0];

                        icYazismalarDetayTable.isReplied=true;
                        icYazismalarDetayTable.approved=true;
                        icYazismalarDetayTable.replyDate=DateTime.Now;

                        await bllIcYazismalarDetayTable.Update(icYazismalarDetayTable);

                        if (icYazismaHierarchyTable!= null)
                        {
                            Data.Models.IcYazismalarDetayTable icYazismalarDetayTableNext = new Data.Models.IcYazismalarDetayTable();
                            icYazismalarDetayTableNext.userId=icYazismaHierarchyTable.userId;
                            icYazismalarDetayTableNext.createdDate=DateTime.Now;
                            icYazismalarDetayTableNext.approved=null;
                            icYazismalarDetayTableNext.icYazismaId=icYazismaTable.Id;
                            icYazismalarDetayTableNext.isReplied=false;
                            icYazismalarDetayTableNext.replyDate=null;
                            icYazismalarDetayTableNext.enabled=true;

                            await bllIcYazismalarDetayTable.Add(icYazismalarDetayTableNext);
                            EmailMessage emailMessage = new EmailMessage();
                            emailMessage.toAddress=kanalUser?.email;
                            emailMessage.subject=icYazismaTable.konu + " hk.";
                            emailMessage.isSent=false;
                            emailMessage.emailText=buildIcYazisma(icYazismaTable);
                            emailMessage.mailTuru=4;
                            emailMessage.plannedDate=DateTime.Now;
                            emailMessage.enabled=true;
                             BLLActions.EmailMessages bllEmailMessages = new BLLActions.EmailMessages(_configuration, _env);
                            await bllEmailMessages.Add(emailMessage);
                        }
                        else
                        {

                            for (int j = 1; j < listIcYazismalarDetayTable.Count(); j++)
                            {

                                int id = listIcYazismalarDetayTable[j].Id;
                                bllIcYazismalarDetayTable.Delete(id);
                            }

                            icYazismalarDetayTable.isReplied=true;
                            icYazismalarDetayTable.approved=true;
                            icYazismalarDetayTable.replyDate=DateTime.Now;
                            await bllIcYazismalarDetayTable.Update(icYazismalarDetayTable);

                            icYazismaMesaj.createdDate=DateTime.Now;
                            icYazismaMesaj.showAll=true;
                            icYazismaMesaj.userId=user.Id;
                            icYazismaMesaj.message=responseMyList.kanalGorusuFirst;
                            icYazismaMesaj.enabled=true;
                            icYazismaMesaj.icYazismaId=icYazismaTable.Id;
                            await bllIcYazismalarMesajTable.Add(icYazismaMesaj);

                            icYazismaTable.bittiMi=true;

                            EmailMessage emailMessage = new EmailMessage();
                            emailMessage.toAddress=createdUser?.email;
                            emailMessage.subject=icYazismaTable.konu + " hk.";
                            emailMessage.isSent=false;
                            emailMessage.emailText="<h2>Sayın " + createdUser?.name + "</h2><br/><h4>Bir adet "
                                    + icYazismaTable.Id.ToString() + " ID'li ve " + icYazismaTable.konu
                                    + " Konulu iç yazışma bitmiştir.<br/></h4>" + buildIcYazisma(icYazismaTable);
                            emailMessage.mailTuru=4;
                            emailMessage.plannedDate=DateTime.Now;
                            emailMessage.enabled=true;
                             BLLActions.EmailMessages bllEmailMessages = new BLLActions.EmailMessages(_configuration, _env);
                            await bllEmailMessages.Add(emailMessage);
                        }
                        await Update(icYazismaTable);
                        return 1;

                    }
                    else if (icYazismaHierarchyTable!= null
                            && icYazismaHierarchyTable.userId== user.Id)
                    {
                        BLLActions.IcYazismalarDetayTable bllIcYazismalarDetayTable = new BLLActions.IcYazismalarDetayTable(_configuration, _env);
                        List<Data.Models.IcYazismalarDetayTable> listIcYazismalarDetayTable = bllIcYazismalarDetayTable
                                .findAllByIcYazismaIdAndApprovedAndUserIdAndEnabled(icYazismaTable.Id, null, user.Id,
                                        true);

                        for (int j = 1; j < listIcYazismalarDetayTable.Count(); j++)
                        {
                            int id = listIcYazismalarDetayTable[j].Id;
                            bllIcYazismalarDetayTable.Delete(id);
                        }
                        Data.Models.IcYazismalarDetayTable icYazismalarDetayTable = listIcYazismalarDetayTable[0];

                        icYazismalarDetayTable.isReplied=true;
                        icYazismalarDetayTable.approved=true;
                        icYazismalarDetayTable.replyDate=DateTime.Now;
                        await bllIcYazismalarDetayTable.Update(icYazismalarDetayTable);

                        Data.Models.IcYazismalarMesajTable icYazismaMesaj = new Data.Models.IcYazismalarMesajTable();
                        icYazismaMesaj.createdDate=DateTime.Now;
                        icYazismaMesaj.showAll=true;
                        icYazismaMesaj.icYazismaId=icYazismaTable.Id;
                        icYazismaMesaj.userId=user.Id;
                        icYazismaMesaj.message=responseMyList.kanalGorusuFirst;
                        icYazismaMesaj.enabled=true;
                        BLLActions.IcYazismalarMesajTable bllIcYazismalarMesajTable=new BLLActions.IcYazismalarMesajTable(_configuration,_env);
                        await bllIcYazismalarMesajTable.Add(icYazismaMesaj);

                        icYazismaTable.bittiMi=true;
                        await Update(icYazismaTable);

                        EmailMessage emailMessage = new EmailMessage();
                        emailMessage.toAddress=createdUser?.email;
                        emailMessage.subject=icYazismaTable.konu + " hk.";
                        emailMessage.isSent=false;
                        emailMessage.emailText="<h2>Sayın " + createdUser?.name + "</h2><br/><h4>Bir adet "
                                + icYazismaTable.Id.ToString() + " ID'li ve " + icYazismaTable.konu
                                + " Konulu iç yazışma bitmiştir.<br/></h4>" + buildIcYazisma(icYazismaTable);
                        emailMessage.mailTuru=4;
                        emailMessage.plannedDate=DateTime.Now;
                        emailMessage.enabled=true;
                         BLLActions.EmailMessages bllEmailMessages = new BLLActions.EmailMessages(_configuration, _env);
                        await bllEmailMessages.Add(emailMessage);
                        return 1;
                    }
                    else
                    {
                        return 4;
                    }
                }
                catch 
                (Exception e)
                {
                    Console.WriteLine(e.Message);
                    //System.out.println(
                    //        getUser.getUser().name + "," + responseMyList.getIcYazismalarTable().Id.toString()
                    //                + " id'li iç yazışmayı onaylayamıyor. Hata: " + e.getMessage());
                    return 4;
                }
            }

            public async Task<int> red(Data.Models.IcYazismalarTable icYazismaTable, int userId)
            {
                try
                {
                    BLLActions.AdminUsers bllAdminUsers = new BLLActions.AdminUsers(_configuration, _env, _mapper);
                    AdminUser user = bllAdminUsers.GetByID(userId)!;
                    AdminUser? createdUser = bllAdminUsers.GetByID(icYazismaTable.createdUserId??0);
                    BLLActions.IcYazismalarDetayTable bllIcYazismalarDetayTable = new BLLActions.IcYazismalarDetayTable(_configuration, _env);
                    List<Data.Models.IcYazismalarDetayTable> listIcYazismalarDetayTable = bllIcYazismalarDetayTable
                            .findAllByIcYazismaIdAndApprovedAndUserIdAndEnabled(icYazismaTable.Id, null, user.Id,
                                    true);

                    for (int j = 1; j < listIcYazismalarDetayTable.Count(); j++)
                    {
                        int id = listIcYazismalarDetayTable[j].Id;
                        bllIcYazismalarDetayTable.Delete(id);
                    }
                    Data.Models.IcYazismalarDetayTable icYazismalarDetayTable = listIcYazismalarDetayTable[0];
                    icYazismalarDetayTable.isReplied=true;
                    icYazismalarDetayTable.approved=false;
                    icYazismalarDetayTable.replyDate=DateTime.Now;
                    await bllIcYazismalarDetayTable.Update(icYazismalarDetayTable);
                    await Update(icYazismaTable);
                    EmailMessage emailMessage = new EmailMessage();
                    emailMessage.toAddress=createdUser?.email;
                    emailMessage.subject=icYazismaTable.konu + " hk.";
                    emailMessage.isSent=false;
                    emailMessage.emailText="<h2>Sayın " + createdUser?.name + "</h2><br/>" + "<h4>"
                            + icYazismaTable.Id.ToString() + " Id'li ve " + icYazismaTable.konu + " Konulu"
                            + "iç yazışmanız red olmuştur.<br/></h4>";
                    emailMessage.mailTuru=4;
                    emailMessage.plannedDate=DateTime.Now;
                    emailMessage.enabled=true;
                    BLLActions.EmailMessages bllEmailMessages = new BLLActions.EmailMessages(_configuration, _env);
                    await bllEmailMessages.Add(emailMessage);
                    return 1;
                }
                catch
                //(Exception e)
                {

                    //System.out.println(getUser.getUser().getName() + "," + icYazismaTable.getId().toString()
                    //        + " id'li iç yazışmayı red edemiyor. Hata: " + e.getMessage());

                    return 2;
                }
            }
        }
    }
}
