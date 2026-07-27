using AskalePortal.Constants;
using AskalePortal.Data.Models;
using AskalePortal.Data.RequestModel;
using AskalePortal.Data.RequestParams;
using AskalePortal.Data.ResponseModels;
using AskalePortal.Data.ResponseParams;
using AutoMapper;
using Azure;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using static AskalePortal.BLL.BLLActions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace AskalePortal.BLL
{
    public partial class BLLActions
    {

        public class MusteriSikayetForm : BaseBLL<AskalePortal.Data.Models.MusteriSikayetForm>
        {
            private readonly IWebHostEnvironment _env;
            private readonly IConfiguration _configuration;
            private readonly IMapper _mapper;
            public MusteriSikayetForm(IConfiguration configuration, IWebHostEnvironment env, IMapper mapper) : base(configuration, env)
            {
                _configuration = configuration;
                _env = env;
                _mapper = mapper;
            }

            #region GetAll

            public List<AskalePortal.Data.Models.MusteriSikayetForm> GetAllMusteri()
            {

                var q = dal.Get(k => k.enabled == true).OrderBy(k => k.createdDate);
                return q.ToList();

            }

            #endregion

            #region GetByTopID

            public List<AskalePortal.Data.Models.MusteriSikayetForm> GetByTopID(int topID)
            {
                var q = dal.Get(k => k.Id == topID && k.enabled == true).OrderBy(k => k.Id);

                return q.ToList();

            }

            #endregion GetAll

            #region GetByID


            #endregion

            public List<AskalePortal.Data.Models.MusteriSikayetForm> GetAll(int? Id, int userId, int? category, string musteriKodu, string musteriAdi, List<int> bagliFabrika, int? sikayetTipiId, string malzemeTuru, decimal malzemeMiktari, int? satisTemsilcisi, string musteriTemsilcisi, string musteriTel, string musteriEmail, DateTime? createdDate, bool seeLog, bool see, int pageNumber, int pageSize)
            {

                var q = dal.Get(k => (Id == 0 ? true : k.Id == Id)
                && (category == 0 ? true : k.categoryId == category)
                && (string.IsNullOrEmpty(musteriKodu) ? true : k.musteriKodu == musteriKodu)
                && (string.IsNullOrEmpty(musteriAdi) ? true : k.musteriAdi == musteriAdi)
                && (sikayetTipiId == 0 ? true : k.sikayetTipiId == sikayetTipiId)
                && (string.IsNullOrEmpty(malzemeTuru) ? true : k.malzemeTuru == malzemeTuru)
                && (malzemeMiktari == 0 ? true : k.malzemeMiktari == malzemeMiktari)
                && (satisTemsilcisi == 0 ? true : k.userId == satisTemsilcisi)
                && (string.IsNullOrEmpty(musteriTemsilcisi) ? true : k.musteriTemsilcisi == musteriTemsilcisi)
                && (string.IsNullOrEmpty(musteriTel) ? true : k.musteriTel == musteriTel)
                && (string.IsNullOrEmpty(musteriEmail) ? true : k.musteriEmail == musteriEmail)

                && k.enabled == true)
                .OrderByDescending(k => k.createdDate);
                if (bagliFabrika != null)
                    q = q.Where(u => bagliFabrika.Contains(u.companyId)).OrderBy(u => u.createdDate);
                if (seeLog != true && see != true)
                    q = q.Where(x => x.userId == userId).OrderByDescending(k => k.createdDate);
                else if (seeLog != true && see == true) q = q.Where(x => bagliFabrika.Contains(x.companyId)).OrderByDescending(k => k.createdDate);
                q = q.OrderByDescending(u => u.Id);
                return q.Skip(pageNumber * pageSize).Take(pageSize).ToList();
            }

            public List<AskalePortal.Data.Models.MusteriSikayetForm> GetAllByRapor(int? fabrika, int? category, int? sikayetTuru, string musteri, DateTime? tarihBaslangic, DateTime? tarihBitis)
            {
                return dal.Get(u => (fabrika.HasValue ? u.companyId == fabrika : true) && (category.HasValue ? u.categoryId == category : true) && (sikayetTuru.HasValue ? u.sikayetTipiId == sikayetTuru : true) && (string.IsNullOrEmpty(musteri) ? true : u.musteriKodu == musteri) && ((tarihBaslangic.HasValue ? (u.createdDate >= tarihBaslangic) : true) && (tarihBitis.HasValue ? (u.createdDate <= tarihBitis) : true))).ToList();
            }

            public async Task<CustomerComplaintSaveDto> save(CustomerComplaintSaveDto entity, int userId)
            {

                BLLActions.AdminUsers bllAdminUsers = new BLLActions.AdminUsers(_configuration, _env, _mapper);
                AdminUser createdUser = bllAdminUsers.GetByID(userId);
                BLLActions.Companies bllCompanies = new BLLActions.Companies(_configuration, _env, _mapper);
                Company company = bllCompanies.getById(entity.companyId ?? 0);
                BLLActions.MusteriSikayetEmail bllMusteriSikayetEmail = new BLLActions.MusteriSikayetEmail(_configuration, _env);
                List<int> listEmails = bllMusteriSikayetEmail.findUserIdByCategoryIdAndEnabled(entity.categoryId, true);
                List<AdminUser> listEmail = bllAdminUsers.getUserByCompanyVkorg(listEmails, company.vkorg, true);

                BLLActions.MusteriSikayetTipi bllMusteriSikayetTipi = new BLLActions.MusteriSikayetTipi(_configuration, _env);
                Data.Models.MusteriSikayetTipi musteriSikayetTipi = bllMusteriSikayetTipi.GetByID(entity.sikayetTipiId ?? 0);

                BLLActions.MusteriSikayetCategory bllMusteriSikayetCategory = new BLLActions.MusteriSikayetCategory(_configuration, _env);
                Data.Models.MusteriSikayetCategory musteriSikayetCategory = bllMusteriSikayetCategory.GetByID(entity.categoryId ?? 0);

                Data.Models.MusteriSikayetForm musteriSikayetForm;
                if (entity.id == null)
                {
                    entity.createdDate = DateTime.Now.ToString();
                    entity.createdUserId = userId;
                    entity.enabled = true;
                    musteriSikayetForm = await Add(_mapper.Map<Data.Models.MusteriSikayetForm>(entity));
                }
                else
                {
                    entity.updateDate = DateTime.Now.ToString();
                    entity.updatedUserId = userId;
                    musteriSikayetForm = await Update(_mapper.Map<Data.Models.MusteriSikayetForm>(entity));
                }



                BLLActions.AttachedFiles bllAttachedFiles = new BLLActions.AttachedFiles(_configuration, _env);
                List<AttachedFile> attachedFiles = bllAttachedFiles.getByModuleIdAndTargetId((int)CommonConstants.MODULES.MUSTERI_SIKAYET_FORM, musteriSikayetForm.Id);
                foreach (AdminUser user in listEmail)
                {
                    EmailMessage emailMessage = new EmailMessage();

                    emailMessage.mailTuru = 5;
                    emailMessage.toAddress = user.email;
                    emailMessage.plannedDate = DateTime.Now;
                    emailMessage.isSent = false;
                    emailMessage.subject = "Müşteri Şikayet Talebi (" + musteriSikayetForm.Id + ")";

                    BLLActions.EmailReaderFile bllEmailReaderFile = new BLLActions.EmailReaderFile();

                    emailMessage.emailText =
                             bllEmailReaderFile.getMusteriEmailText(_configuration, _env, musteriSikayetForm, company.vtext, musteriSikayetTipi.sikayetTipi,
                                    musteriSikayetCategory.categoryName, createdUser.name, attachedFiles);
                    BLLActions.EmailMessages bllEmailMessages = new BLLActions.EmailMessages(_configuration, _env);
                    await bllEmailMessages.Add(emailMessage);
                }
                BLLActions.RoleDetails bllRoleDetails = new BLLActions.RoleDetails(_configuration, _env, _mapper);
                HashSet<RoleDetail> listRoleDetail = bllRoleDetails.getByModuleId((int)CommonConstants.MODULES.MUSTERI_SIKAYET_FORM);

                HashSet<int> listRoleId = new HashSet<int>();
                foreach (RoleDetail roleDetail in listRoleDetail)
                {
                    if (roleDetail.canSeeLogs)
                    {
                        listRoleId.Add(roleDetail.roleId);
                    }

                }
                List<AdminUser> listUser = bllAdminUsers.getRoleIdList(listRoleId);
                foreach (AdminUser user in listUser)
                {
                    EmailMessage emailMessage = new EmailMessage();

                    emailMessage.mailTuru = 5;
                    emailMessage.toAddress = user.email;
                    emailMessage.plannedDate = DateTime.Now;
                    emailMessage.isSent = false;
                    emailMessage.subject = "Müşteri Şikayet Talebi (" + musteriSikayetForm.Id + ")";
                    BLLActions.EmailReaderFile bllEmailReaderFile = new BLLActions.EmailReaderFile();

                    emailMessage.emailText =
                             bllEmailReaderFile.getMusteriEmailText(_configuration, _env, musteriSikayetForm, company.vtext, musteriSikayetTipi.sikayetTipi,
                                    musteriSikayetCategory.categoryName, createdUser.name, attachedFiles);
                    BLLActions.EmailMessages bllEmailMessages = new BLLActions.EmailMessages(_configuration, _env);
                    await bllEmailMessages.Add(emailMessage);
                }
                return _mapper.Map<Data.ResponseModels.CustomerComplaintSaveDto>(musteriSikayetForm);
            }

            public PageReturn<CustomerComplaintDto> listByPageable(FilterPageParam<CustomerComplaintListDtoParameter> filterPageParam)
            {
                PageReturn<CustomerComplaintDto> result = new PageReturn<CustomerComplaintDto>();
                int pageSize = filterPageParam.size ?? 20;
                int pageNumber = filterPageParam.page ?? 0;

                int? userId = filterPageParam.liste?.userId;
                string? customerName = filterPageParam.liste?.customerName;
                string? customerCode = filterPageParam.liste?.customerCode;
                string? malzemeName = filterPageParam.liste?.malzemeName;
                int? companyId = filterPageParam.liste?.companyId;
                int? categoryId = filterPageParam.liste?.categoryId;
                int? sikayetId = filterPageParam.liste?.sikayetId;

                BLLActions.AdminUsers bllAdminUsers = new BLLActions.AdminUsers(_configuration, _env, _mapper);
                AdminUser user = bllAdminUsers.GetByID(userId ?? 0);

                BLLActions.RoleDetails bllRoleDetails = new BLLActions.RoleDetails(_configuration, _env, _mapper);
                RoleDetail? roleDetail = bllRoleDetails.GetByRoleIDAndModuleID(user.roleId, (int)CommonConstants.MODULES.MUSTERI_SIKAYET_FORM);

                if (user.roleId == 1 || (roleDetail != null && roleDetail.canSeeLogs))
                {
                    IQueryable<Data.Models.MusteriSikayetForm> query = dal.Get(u => u.enabled)
    .Where(a =>
        (string.IsNullOrEmpty(customerName) || a.musteriAdi == customerName) &&
        (string.IsNullOrEmpty(customerCode) || a.musteriKodu == customerCode) &&
        (string.IsNullOrEmpty(malzemeName) || a.malzemeTuru == malzemeName) &&
        (!companyId.HasValue || a.companyId == companyId.Value) &&
        (!categoryId.HasValue || a.categoryId == categoryId.Value) &&
        (!sikayetId.HasValue || a.sikayetTipiId == sikayetId.Value)).OrderByDescending(u => u.Id);

                    result.totalElements = query.Count();

                    result.content = query
                        .Skip(pageSize * pageNumber)
                        .Take(pageSize)
                        .Select(a => new CustomerComplaintDto
                        {
                            id = a.Id,
                            companyName = a.company.vtext,
                            categoryName = a.category.categoryName,
                            customerCode = a.musteriKodu,
                            customerName = a.musteriAdi,
                            complaintName = a.sikayetTipi.sikayetTipi,
                            malzemeTuru = a.malzemeTuru,
                            malzemeMiktari = (double)a.malzemeMiktari,
                            aciklama = a.description,
                            musteriTemsilcisi = a.musteriTemsilcisi,
                            musteriTel = a.musteriTel,
                            musteriEmail = a.musteriEmail,
                            olusturmaTarihi = a.createdDate.ToString("dd.MM.yyyy")
                        })
                        .ToList();

                    result.number = result.content.Count;
                    result.size = pageSize;

                    return result;
                }
                else if (roleDetail != null && roleDetail.canSee)
                {
                    BLLActions.Companies bllCompanies = new BLLActions.Companies(_configuration, _env, _mapper);
                    List<Company> listCompany = bllCompanies.getByRoleId(user.roleId);
                    List<int> listCompanyIds = new List<int>();
                    foreach (Company company in listCompany)
                    {
                        listCompanyIds.Add(company.Id);
                    }

                    IQueryable<Data.Models.MusteriSikayetForm> query = dal.Get(u => u.enabled)
    .Where(a =>
        (string.IsNullOrEmpty(customerName) || a.musteriAdi == customerName) &&
        (string.IsNullOrEmpty(customerCode) || a.musteriKodu == customerCode) &&
        (string.IsNullOrEmpty(malzemeName) || a.malzemeTuru == malzemeName) &&
        (!categoryId.HasValue || a.categoryId == categoryId.Value) &&
        (!sikayetId.HasValue || a.sikayetTipiId == sikayetId.Value) &&
        (listCompanyIds == null || !listCompanyIds.Any() || listCompanyIds.Contains(a.companyId)));

                    result.totalElements = query.Count();

                    result.content = query
                        .Skip(pageSize * pageNumber)
                        .Take(pageSize)
                        .Select(a => new CustomerComplaintDto
                        {
                            id = a.Id,
                            companyName = a.company.vtext,
                            categoryName = a.category.categoryName,
                            customerCode = a.musteriKodu,
                            customerName = a.musteriAdi,
                            complaintName = a.sikayetTipi.sikayetTipi,
                            malzemeTuru = a.malzemeTuru,
                            malzemeMiktari = (double)a.malzemeMiktari,
                            aciklama = a.description,
                            musteriTemsilcisi = a.musteriTemsilcisi,
                            musteriTel = a.musteriTel,
                            musteriEmail = a.musteriEmail,
                            olusturmaTarihi = a.createdDate.ToString("dd.MM.yyyy")
                        })
                        .ToList();

                    result.number = result.content.Count;
                    result.size = pageSize;

                    return result;

                }

                else
                {
                    IQueryable<Data.Models.MusteriSikayetForm> query = dal.Get(u => u.enabled)
    .Where(a =>
        (string.IsNullOrEmpty(customerName) || a.musteriAdi == customerName) &&
        (string.IsNullOrEmpty(customerCode) || a.musteriKodu == customerCode) &&
        (string.IsNullOrEmpty(malzemeName) || a.malzemeTuru == malzemeName) &&
        (!companyId.HasValue || a.companyId == companyId.Value) &&
        (!categoryId.HasValue || a.categoryId == categoryId.Value) &&
        (!sikayetId.HasValue || a.sikayetTipiId == sikayetId.Value) &&
        (!userId.HasValue || a.userId == userId.Value));

                    result.totalElements = query.Count();

                    result.content = query
                        .Skip(pageSize * pageNumber)
                        .Take(pageSize)
                        .Select(a => new CustomerComplaintDto
                        {
                            id = a.Id,
                            companyName = a.company.vtext,
                            categoryName = a.category.categoryName,
                            customerCode = a.musteriKodu,
                            customerName = a.musteriAdi,
                            complaintName = a.sikayetTipi.sikayetTipi,
                            malzemeTuru = a.malzemeTuru,
                            malzemeMiktari = (double)a.malzemeMiktari,
                            aciklama = a.description,
                            musteriTemsilcisi = a.musteriTemsilcisi,
                            musteriTel = a.musteriTel,
                            musteriEmail = a.musteriEmail,
                            olusturmaTarihi = a.createdDate.ToString("dd.MM.yyyy")
                        })
                        .ToList();

                    result.number = result.content.Count;
                    result.size = pageSize;

                    return result;
                }

            }

            public List<CustomerComplaintActionDto> listCustomerComplaintAction(int customerComplaintId)
            {
                BLLActions.MusteriSikayetAction bllMusteriSikayetAction = new BLLActions.MusteriSikayetAction(_configuration, _env,_mapper);
                List<CustomerComplaintActionDto> listActionDto = bllMusteriSikayetAction
                .findAllBySikayetIdAndEnabled(customerComplaintId, true);

                foreach (CustomerComplaintActionDto complaintActionDto in listActionDto)
                {
                    BLLActions.AttachedFiles bllAttachedFiles = new BLLActions.AttachedFiles(_configuration, _env);
                    List<AttachedFile> listAttachFile = bllAttachedFiles.getByModuleIdAndTargetId(
                            (int)CommonConstants.MODULES.MUSTERI_SIKAYET_AKSIYON, complaintActionDto.id??0);
                    List<string> listFileNames = new List<string>();
                    foreach (AttachedFile attachedFile in listAttachFile)
                    {
                        listFileNames.Add(attachedFile.filePath);
                    }
                    complaintActionDto.fileNames=listFileNames;

                }
                return listActionDto;
            }
        }
    }
}
