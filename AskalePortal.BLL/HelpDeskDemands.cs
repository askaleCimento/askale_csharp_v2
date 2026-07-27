
using AskalePortal.Data.Models;
using AskalePortal.Data.RequestParams;
using AskalePortal.Data.ResponseModels;
using AskalePortal.Data.ResponseParams;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace AskalePortal.BLL
{
    public partial class BLLActions
    {
        
        public class HelpDeskDemands : BaseBLL<AskalePortal.Data.Models.HelpDeskDemand>
        {
            private readonly IConfiguration _configuration;
            private readonly IWebHostEnvironment _env;
            public HelpDeskDemands(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
            {
                _configuration = configuration;
                _env = env;
            }
            #region GetAll

            public List<AskalePortal.Data.Models.HelpDeskDemand> GetAll(string title, int? createdByCompanyID, int? helpDeskRoleID, int? helpDeskStatusID, int? helpDeskCategoryID, int? helpDeskTypeID, int? createdByUserID, DateTime? tarih1, DateTime? tarih2, bool? isClosed)
            {
                var q = dal.Get(k => (k.title.Contains(title) || string.IsNullOrEmpty(title))
                && (k.createdByCompanyId == createdByCompanyID || createdByCompanyID == null || createdByCompanyID == 0)
                && (k.assignedToHelpDeskRoleId == helpDeskRoleID || helpDeskRoleID == null || helpDeskRoleID == 0)
                && (k.helpDeskStatusId == helpDeskStatusID || helpDeskStatusID == null || helpDeskStatusID == 0)
                && (k.helpDeskCategoryId == helpDeskCategoryID || helpDeskCategoryID == null || helpDeskCategoryID == 0)
                && (k.helpDeskTypeId == helpDeskTypeID || helpDeskTypeID == null || helpDeskTypeID == 0)
                && (k.createdUserId == createdByUserID || createdByUserID == null || createdByUserID == 0)
                && (k.createdDate >= tarih1 || tarih1 == null)
                && (k.createdDate <= tarih2 || tarih2 == null)
                && (k.isClosed == isClosed || isClosed == null)
                && k.enabled == true)
                .OrderByDescending(k => k.createdDate);
                return q.ToList();
            }

            public List<AskalePortal.Data.Models.HelpDeskDemand> GetAll(string title, int? createdByCompanyID, int? helpDeskRoleID, int? helpDeskStatusID, int? helpDeskCategoryID, int? helpDeskTypeID, int? createdByUserID, DateTime? tarih1, DateTime? tarih2, bool? isClosed, int adminRoleID, int? adminHelpDeskRoleID, int pageNumber, int pageSize)
            {
                var q = dal.Get(k => (k.title.Contains(title) || string.IsNullOrEmpty(title))
                && (k.createdByCompanyId == createdByCompanyID || createdByCompanyID == null || createdByCompanyID == 0)
                && (k.assignedToHelpDeskRoleId == helpDeskRoleID || helpDeskRoleID == null || helpDeskRoleID == 0)
                && (k.helpDeskStatusId == helpDeskStatusID || helpDeskStatusID == null || helpDeskStatusID == 0)
                && (k.helpDeskCategoryId == helpDeskCategoryID || helpDeskCategoryID == null || helpDeskCategoryID == 0)
                && (k.helpDeskTypeId == helpDeskTypeID || helpDeskTypeID == null || helpDeskTypeID == 0)
                && (k.createdUserId == createdByUserID || createdByUserID == null || createdByUserID == 0)
                && (k.createdDate >= tarih1 || tarih1 == null)
                && (k.createdDate <= tarih2 || tarih2 == null)
                && (k.isClosed == isClosed || isClosed == null)
                && k.enabled == true)
                .OrderByDescending(k => k.createdDate);
                if (adminRoleID != 1)
                    q = q.Where(x => x.assignedToHelpDeskRoleId == adminHelpDeskRoleID).OrderByDescending(k => k.createdDate);
                return q.Skip(pageNumber * pageSize).Take(pageSize).ToList(); ;
            }

            public override List<AskalePortal.Data.Models.HelpDeskDemand> GetAll()
            {
                var q = dal.Get(k => k.enabled == true).OrderBy(k => k.title);

                return q.ToList();
            }

            #endregion GetAll

            #region GetAllWithPage

            public List<AskalePortal.Data.Models.HelpDeskDemand> GetAllWithPage(string searchQuery, int activePage, out double totalPages, out int totalRecords, int recordsPerPage = 10)
            {
                var q = dal.Get(k => (k.title.Contains(searchQuery) || string.IsNullOrEmpty(searchQuery)) &&
                                     k.enabled == true)
                                     .OrderBy(k => k.title)
                                     .GetPage(activePage, recordsPerPage, out totalPages, out totalRecords);
                return q.ToList();
            }

            public List<HelpDeskDemand> mylist(int userId, string createdByUserName)
            {

                List<HelpDeskDemand> q = dal.Get(k => k.enabled && k.createdByUserName == createdByUserName).OrderByDescending(k => k.Id).ToList();
                return q;
            }


            #endregion GetAllWithPage 
            public List<int> NumberDemandsByStatusId()
            {
                List<int> liste = dal.Get(k => k.enabled == true).Select(k => k.helpDeskStatusId).ToList();
                return liste;
            }

            //public HelpDeskDemand saveDemand(HelpDeskDemandSaveDto entity, int userId)
            //{
            //    BLLActions.AdminUsers bllAdminUsers = new BLLActions.AdminUsers(_configuration, _env);
            //    AdminUser user = bllAdminUsers.GetByID(userId);
            //    BLLActions.Companies bllCompanies = new BLLActions.Companies(_configuration, _env);
            //    Company company = bllCompanies.GetByID(user.companyId);
            //    BLLActions.HelpDeskDemandRules bllHelpDeskDemandRules = new BLLActions.HelpDeskDemandRules(_configuration,_env);

            //    if (entity.Id == 0)
            //    {
                    
            //        List<HelpDeskDemandRule> listHelpDeskDemandRule = bllHelpDeskDemandRules
            //                .findIdByCompanyAndHelpDeskCategory(company.vkorg, entity.helpDeskCategoryId.ToString());
            //        if (listHelpDeskDemandRule.Count() < 1 || listHelpDeskDemandRule.IsNullOrEmpty())
            //        {
            //            entity.assignedToHelpDeskRoleId=null;
            //        }
            //        else
            //        {
            //            int helpDeskRoleId = listHelpDeskDemandRule.FirstOrDefault()!.helpDeskRoleId;
                            
            //            entity.assignedToHelpDeskRoleId=helpDeskRoleId;
            //        }

            //        entity.createdByCompanyId=user.companyId;
            //        entity.isClosed = false;
            //        entity.createdByUserName=user.username;
            //        entity.helpDeskStatusId=1;
            //        entity.ticketNumber=null;
            //        entity.createdUserId=userId;
            //        entity.createdDate = DateTime.Now.ToString();
            //        entity.enabled=true;
                   
            //        HelpDeskDemand helpDeskDemand = Add(_mapper.Map<HelpDeskDemand>(entity));

            //        if (listHelpDeskDemandRule.IsNullOrEmpty() || helpDeskDemand.assignedToHelpDeskRoleId > 0)
            //        {

            //            AdminUser helpDeskUser = bllAdminUsers.findHelpDeskRoleId(helpDeskDemand!.assignedToHelpDeskRoleId??0);
            //            //EMAİL GELİCEK

            //            //EmailMessage emailMessage = new EmailMessage();
            //            //BLLActions.EmailMessages bllEmailMessages = new BLLActions.EmailMessages(_configuration,_env);
            //            //emailMessage.subject=("Destek Masası Talep hk.");
            //            //emailMessage.toAddress=(helpDeskUser.email);
            //            //string mailMessage = buildDemand(helpDeskUser.getName(), "Destek Masası Talebi",
            //            //        helpDeskDemand.getId().tostring() + " ID'li talebiniz oluşmuştur.");
            //            //emailMessage.setEmailText(mailMessage);
            //            //emailMessage.setMailTuru(1);
            //            //emailMessage.setEnabled(true);
            //            //emailMessage.setSent(false);
            //            //emailMessage.setPlannedDate(LocalDateTime.now());
            //            //bllEmailMessages.Add(emailMessage);


            //            //emailMessage.subject = "Destek Masası Talep hk.";
            //            //emailMessage.toAddress = helpDeskUser.email;
            //            //emailMessage.emailText = getPaymentMailstring(savedAccountPaymentSAPTable, userId);

            //            ////emailMessage.emailText = CreatePaymentMail(item.AccountPaymentSAPTable.USNAM, item.AccountPaymentSAPTable.AENAM, item.OENUM, item.POSNR, item.AccountPaymentSAPTable.BUKRS, item.AccountPaymentSAPTable.CPUDT, item.LIFNR, item.NAME1, item.WRBTR, item.IBAN, item.BANKA, item.BRNCH, item.BANKN, adminUser.name, "" + adminUser.imageUrl, adminUser.Company.VTEXT);

            //            //emailMessage.isSent = false;
            //            //emailMessage.plannedDate = DateTime.Now;
            //            //emailMessage.mailTuru = 1;


                        
            //        }

            //        return helpDeskDemand;
            //    }
            //    else
            //    {
            //        HelpDeskDemand helpDeskDemand = GetByID(entity!.id??0);
            //        if (helpDeskDemand.helpDeskCategoryId != entity.helpDeskCategoryId)
            //        {
            //            List<HelpDeskDemandRule> listHelpDeskDemandRule = bllHelpDeskDemandRules
            //                    .findIdByCompanyAndHelpDeskCategory(company.vkorg,
            //                            entity.helpDeskCategoryId.ToString());
            //            if (listHelpDeskDemandRule.Count() > 1 || listHelpDeskDemandRule.IsNullOrEmpty())
            //            {
            //                entity.assignedToHelpDeskRoleId=(null);
            //            }
            //            else
            //            {
            //                entity.assignedToHelpDeskRoleId=
            //                        listHelpDeskDemandRule.First().helpDeskRoleId;
            //            }

            //        }
            //        if (entity.helpDeskStatusId == 3)
            //        {
            //            entity.isClosed=true;
            //        }
            //        entity.updatedUserId=userId;
            //        entity.updateDate=DateTime.Now.ToString();
            //        HelpDeskDemand helpDeskDemandUpdate = Update(_mapper.Map<HelpDeskDemand>(entity));

            //        return helpDeskDemandUpdate;
            //    }
            //}

            public List<HelpDeskDemandDto> talepYonetimiDtoList(FilterParam<HelpDeskDemandParamsDto> filterParam)
            {
                string? filterbaslik = filterParam?.liste?.filterbaslik;
                int? filterCompanyId = filterParam?.liste?.filterCompanyId;
                int? filterCategoryId = filterParam?.liste?.filterCategoryId;
                int? filterTypeId = filterParam?.liste?.filterTypeId;
                int? filterStatusId = filterParam?.liste?.filterStatusId;
                int? filterRoleId = filterParam?.liste?.filterRoleId;
                int? filterUserId = filterParam?.liste?.filterUserId;

                List<HelpDeskDemandDto> liste = dal.Get(k => (k.enabled == true) &&
                (filterbaslik!=null ? k.title.Contains(filterbaslik) :true &&
               (filterCompanyId != null ? k.createdByCompanyId == filterCompanyId : true &&
               filterCategoryId != null ? k.helpDeskCategoryId == filterCategoryId : true &&
               filterTypeId != null ? k.helpDeskTypeId == filterTypeId : true &&
               filterStatusId != null ? k.helpDeskStatusId == filterStatusId : true &&
               filterRoleId != null ? k.assignedToHelpDeskRoleId == filterRoleId : true &&
               filterUserId != null ? k.createdUserId == filterUserId : true))).OrderByDescending(k => k.Id).Select(k => new HelpDeskDemandDto
               {
                   atanan = k.assignedToHelpDeskRole.title,
                   durum = k.helpDeskStatus.title,
                   Id = k.Id,
                   kategori = k.helpDeskCategory.title,
                   kullanici = k.createdUser.name,
                   oncelik = k.helpDeskType.title,
                   sirket = k.createdByCompany.vkorg,
                   talep = k.title,
                   talepNo = k.ticketNumber,
                   tarih = k.createdDate,
               }).ToList();
                return liste;

            }
        }
    }


}
