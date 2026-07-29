using AskalePortal.Constants;
using AskalePortal.Data.Models;
using AskalePortal.Data.RequestModel;
using AskalePortal.Data.RequestParams;
using AskalePortal.Data.ResponseModels;
using AskalePortal.Data.ResponseParams;
using AskalePortal.Data.SAP.OutputParams;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Utilities;
using static AskalePortal.Constants.CommonConstants;

namespace AskalePortal.BLL
{
    public partial class BLLActions
    {
        public class ActiveProcesses : BaseBLL<AskalePortal.Data.Models.ActiveProcess>
        {
            private IConfiguration _configuration;
            private IWebHostEnvironment _env;
            private IMapper _mapper;
            public ActiveProcesses(IConfiguration configuration, IWebHostEnvironment env, IMapper mapper) : base(configuration, env)
            {
                _configuration = configuration;
                _env = env;
                _mapper = mapper;
            }


            public bool HasActiveProcess(int approvalProcessTypeId, string relatedDataId)
            {
                var q = dal.Get(x => x.approvalProcess.typeId == approvalProcessTypeId && x.relatedDataId == relatedDataId
                && (x.currentStateId != (int)CommonConstants.PROCESS_STATES.SAP_COMPLETED
                && x.currentStateId != (int)CommonConstants.PROCESS_STATES.COMPLETED
                && x.currentStateId != (int)CommonConstants.PROCESS_STATES.DECLINED) && x.enabled == true).FirstOrDefault();
                return q != null;
            }
            public List<AskalePortal.Data.Models.ActiveProcess> GetAll(int? stateId, string title, int[] typeId, int activePage, int recordsPerPage = 10)
            {
                var q = dal.Get(k => typeId.Contains(k.approvalProcess.typeId) &&
                                    (stateId == null || k.currentStateId == stateId) &&
                                    (string.IsNullOrEmpty(title) || k.relatedDataPrimaryDesc.Contains(title) || k.relatedDataDesc.Contains(title) || k.relatedDataId.Contains(title) || k.relatedDataPrimaryId.Contains(title)) &&
                                    k.enabled == true)
                                    .OrderByDescending(k => k.createdDate)
                                    .Skip(activePage * recordsPerPage).Take(recordsPerPage).ToList();
                return q;
            }
            public List<AskalePortal.Data.Models.ActiveProcess> GetAll(int? stateId, string title, int typeId)
            {
                var q = dal.Get(k => k.approvalProcess.typeId == typeId &&
                                    (stateId == null || k.currentStateId == stateId) &&
                                    (string.IsNullOrEmpty(title) || k.relatedDataPrimaryDesc.Contains(title) || k.relatedDataDesc.Contains(title) || k.relatedDataId.Contains(title) || k.relatedDataPrimaryId.Contains(title)) &&
                                    k.enabled == true)
                                    .OrderByDescending(k => k.createdDate).ToList();
                return q;
            }
            public List<AskalePortal.Data.Models.ActiveProcess> GetAllOfThisUser(int? enabled, int userId, int[] typeId, int activePage, int recordsPerPage = 10)
            {
                BLLActions.ActiveProcessDetails temp = new ActiveProcessDetails(_configuration, _env);
                var c = temp.GetAboutMe(enabled, userId, typeId)
                                    .Skip(activePage * recordsPerPage).Take(recordsPerPage).ToList();
                return c;
            }
            public List<AskalePortal.Data.Models.ActiveProcess> GetAllOfThisUser(int? enabled, int userId, int[] typeId)
            {
                BLLActions.ActiveProcessDetails temp = new ActiveProcessDetails(_configuration, _env);
                var c = temp.GetAboutMe(enabled, userId, typeId).ToList();

                return c;
            }
            public List<AskalePortal.Data.Models.ActiveProcess> GetAllOfThisUser(int userId, int typeId, int activePage, int recordsPerPage = 10)
            {
                BLLActions.ActiveProcessDetails temp = new ActiveProcessDetails(_configuration, _env);
                var c = temp.GetAboutMe(userId, typeId)
                                    .Skip(activePage * recordsPerPage).Take(recordsPerPage).ToList();
                return c;
            }
            public List<AskalePortal.Data.Models.ActiveProcess> GetAllOfThisUser(int userId, int typeId)
            {
                BLLActions.ActiveProcessDetails temp = new ActiveProcessDetails(_configuration, _env);
                var c = temp.GetAboutMe(userId, typeId);
                return c;
            }
            public List<AskalePortal.Data.Models.ActiveProcess> GetAllOfThisUser(int currentStateId, int userId, int[] typeId)
            {
                BLLActions.ActiveProcessDetails temp = new ActiveProcessDetails(_configuration, _env);
                var c = temp.GetAboutMe(currentStateId, userId, typeId);
                return c;
            }

            public List<ActiveProcess> GetAllOfThisUser(int vekaletverenId)
            {
                return dal.Get(u => u.currentUserId == vekaletverenId && u.currentStateId == 1 && u.enabled == true).ToList();
            }

            public List<AskalePortal.Data.Models.ActiveProcess> getMyList(int enabled, int userId, int[] listTypeId, int activePage, int pageSize)
            {
                return dal.Get(u => u.currentStateId == enabled && u.ActiveProcessDetail.Any(y => y.enabled == true && y.approved == null && y.userId == userId) && listTypeId.Contains(u.approvalProcess.typeId) && u.enabled == true).OrderByDescending(u => u.Id).Skip(activePage * pageSize).Take(pageSize).ToList();
            }
            //public bool HasActiveProcess(int approvalProcessTypeId, string relatedDataId, string relatedDataDesc)
            //{
            //    var q = dal.Get(x => x.relatedDataDesc == relatedDataDesc && x.approvalProcess.typeId == approvalProcessTypeId
            //    && x.relatedDataId == relatedDataId && (x.currentStateId != (int)CommonConstants.PROCESS_STATES.SAP_COMPLETED
            //    && x.currentStateId != (int)CommonConstants.PROCESS_STATES.COMPLETED
            //    && x.currentStateId != (int)CommonConstants.PROCESS_STATES.DECLINED)).FirstOrDefault();
            //    return q != null;
            //}
            public bool hasActiveProcess(int processType, string relatedDataId, string? relatedDataDesc)
            {
                int deger = dal.Get(u => u.approvalProcess.typeId == processType &&
                    u.currentStateId == 1 &&
                    u.relatedDataId == relatedDataId &&
                    u.enabled &&
                    (relatedDataDesc == "" || u.relatedDataDesc == relatedDataDesc)).Count();
                bool donenDeger = deger > 0;
                return donenDeger;
            }

            public async Task<bool> changeLimit(string name1, string kunnr, string klimk, string dagitimKanali, decimal amount, string description, int userId, int processId)
            {

                string ok_link = OkNoLinks.OK_LINK;
                string no_link = OkNoLinks.NO_LINK;
                try
                {
                    BLLActions.AdminUsers bllAdminUsers = new BLLActions.AdminUsers(_configuration, _env, _mapper);
                    AdminUser user = bllAdminUsers.GetByID(userId)!;
                    int companyId = user.companyId;

                    BLLActions.ApprovalProcesses bllApprovalProcesses = new BLLActions.ApprovalProcesses(_configuration, _env, _mapper);
                    ApprovalProcess? approvalProcess = new ApprovalProcess();

                    approvalProcess = bllApprovalProcesses.findByCompanyIdAndTypeIdAndDagitimKanaliAndEnabled(companyId,
                            processId, dagitimKanali, true);
                    if (approvalProcess != null)
                    {


                        BLLActions.ApprovalProcessDetails bllApprovalProcessDetails = new BLLActions.ApprovalProcessDetails(_configuration, _env, _mapper);
                        ApprovalProcessDetail? approvalProcessDetail = bllApprovalProcessDetails
                                .findByProcessIdAndDataOrderAndEnabled(approvalProcess.Id, 1, true);

                        if (approvalProcessDetail != null)
                        {


                            AdminUser firstUser = bllAdminUsers.GetByID(approvalProcessDetail.userId)!;

                            BLLActions.ActiveProcessVekalet bllActiveProcessVekalet = new BLLActions.ActiveProcessVekalet(_configuration, _env);
                            Data.Models.ActiveProcessVekalet? activeProcessVekalet = bllActiveProcessVekalet.GetByAlanUserId(approvalProcessDetail.userId);

                            ActiveProcess activeProcess = new ActiveProcess();
                            if (activeProcessVekalet != null)
                            {
                                activeProcess.userVekaletId = activeProcessVekalet.VekaletAlanId;
                            }

                            activeProcess.approvalProcessId = approvalProcess.Id;
                            activeProcess.createdUserId = user.Id;
                            activeProcess.createdDate = DateTime.Now;
                            activeProcess.dagitimKanali = dagitimKanali;
                            activeProcess.newValue = amount.ToString();
                            activeProcess.oldValue = klimk;
                            activeProcess.currentStateId = (int)CommonConstants.PROCESS_STATES.ACTIVE;
                            activeProcess.relatedData = "KUNNR";
                            activeProcess.relatedDataId = kunnr;
                            activeProcess.relatedDataDesc = name1;
                            activeProcess.description = description;
                            activeProcess.currentUserId = firstUser.Id;
                            activeProcess.dataType = "decimal";
                            activeProcess.enabled = true;
                            if (kunnr.StartsWith("00006"))
                            {
                                activeProcess.relatedColumn = "Motorin Kredi Limiti";
                            }
                            else
                            {
                                activeProcess.relatedColumn = "Kredi Limiti";
                            }
                            if (approvalProcess.typeId == (int)APPROVAL_PROCESSES.HAFTALIK_MUSTERI)
                            {
                                activeProcess.relatedColumn = "Haftalik Limit";
                            }

                            ActiveProcess? activeProcessSave = await Add(activeProcess);
                            ActiveProcessDetail activeProcessDetail = new ActiveProcessDetail();
                            activeProcessDetail.activeProcessId = activeProcessSave!.Id;
                            if (activeProcessVekalet != null)
                            {
                                activeProcessDetail.vekaletId = activeProcessVekalet.VekaletAlanId;
                            }
                            activeProcessDetail.approved = null;
                            activeProcessDetail.createdUserId = userId;
                            activeProcessDetail.createdDate = DateTime.Now;
                            activeProcessDetail.description = "";
                            activeProcessDetail.replyDate = null;
                            activeProcessDetail.isReplied = false;
                            activeProcessDetail.guid = Guid.NewGuid();
                            activeProcessDetail.enabled = true;
                            activeProcessDetail.userId = approvalProcessDetail.userId;

                            BLLActions.ActiveProcessDetails bllActiveProcessDetails = new BLLActions.ActiveProcessDetails(_configuration, _env);
                            await bllActiveProcessDetails.Add(activeProcessDetail);

                            EmailMessage emailMessage = new EmailMessage();
                            emailMessage.subject = "Müşteri Kredi Limiti Değişikliği";
                            emailMessage.toAddress = firstUser.email;
                            BLLActions.EmailReaderFile bllEmailReaderFile = new BLLActions.EmailReaderFile();
                            string mailMessage = bllEmailReaderFile.CreditEmailTemplate(_configuration, _env, "Müşteri Kredi Limiti Değişikliği", firstUser.name, kunnr, name1,
                                    klimk, amount.ToString(), description, ok_link, no_link);
                            emailMessage.emailText = mailMessage;
                            emailMessage.mailTuru = 2;
                            emailMessage.enabled = true;
                            emailMessage.isSent = false;
                            emailMessage.plannedDate = DateTime.Now;
                            BLLActions.EmailMessages bllEmailMessages = new BLLActions.EmailMessages(_configuration, _env);
                            await bllEmailMessages.Add(emailMessage);

                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (Exception e)
                {
                    BLLActions.AdminUsers bllAdminUsers = new BLLActions.AdminUsers(_configuration, _env, _mapper);
                    UserByNameEMailDto userByNameEMailDto = bllAdminUsers.getUserByNameAndEmail(userId);
                    Console.WriteLine(userByNameEMailDto.name + "," + processId.ToString()
                            + " id'li kredi limitini onaylayamadı. Hata: " + e.Message);
                    return false;
                }

            }

            public async Task<bool> changeAllLimit(bool approved, List<int> listInt, int userId)
            {
                string ok_link = OkNoLinks.OK_LINK;
                string no_link = OkNoLinks.NO_LINK;
                List<ActiveProcess> listActiveProcess = dal.Get(u => listInt.Contains(u.Id) && u.enabled).ToList();
                if (approved)
                {

                    foreach (ActiveProcess activeProcess in listActiveProcess)
                    {
                        BLLActions.ActiveProcessDetails bllActiveProcessDetails = new BLLActions.ActiveProcessDetails(_configuration, _env);
                        ActiveProcessDetail? activeProcessDetail = bllActiveProcessDetails
                                .findByActiveProcessIdAndUserIdAndApprovedAndEnabled(activeProcess.Id, userId, null, true);

                        BLLActions.ApprovalProcessDetails bllApprovalProcessDetails = new BLLActions.ApprovalProcessDetails(_configuration, _env, _mapper);
                        ApprovalProcessDetail approvalProcessDetail = bllApprovalProcessDetails
                                .findByProcessIdAndUserIdAndEnabled(activeProcess.approvalProcessId, userId, true)!;

                        AdminUser? nextUser = null;

                        if (activeProcess.relatedColumn == "Vade Tarihi")
                        {
                            BLLActions.CustomerDocumentSap bllCustomerDocumentSap = new BLLActions.CustomerDocumentSap(_configuration, _env, _mapper);

                            CustomerDocumentDto customerDocumentDto = bllCustomerDocumentSap
                                    .getCustomerDocument(activeProcess.relatedDataPrimaryId)
                                    .Where(u => u.BELNR == activeProcess.relatedDataId).ToList()[0];
                            double degerZterm = 0.0;
                            if (customerDocumentDto.ZTERM != "")
                            {
                                degerZterm = Convert.ToDouble(customerDocumentDto?.ZTERM?.Substring(1));
                            }

                            double onceki = int.Parse(customerDocumentDto?.ZBD1T ?? "0") - degerZterm;
                            double newValue = onceki + Convert.ToDouble(activeProcess.newValue);
                            int deger;
                            if (newValue <= 5)
                            {
                                deger = 1;
                            }
                            else if (newValue <= 15)
                            {
                                deger = 2;
                            }
                            else if (newValue <= 20)
                            {
                                deger = 3;

                            }
                            else
                            {
                                deger = 4;
                            }
                            ApprovalProcessDetail? approvalProcessDetailLast = bllApprovalProcessDetails
                                    .findByProcessIdAndDataOrderAndEnabled(approvalProcessDetail.processId, deger, true);
                            if (approvalProcessDetailLast != null)
                            {
                                BLLActions.AdminUsers bllAdminUsers = new BLLActions.AdminUsers(_configuration, _env, _mapper);
                                AdminUser? userLast = bllAdminUsers.GetByID(approvalProcessDetailLast.userId);
                                if (userLast?.Id != userId)
                                {

                                    ApprovalProcessDetail? approvalProcessDetailNext = bllApprovalProcessDetails
                                            .findByProcessIdAndDataOrderAndEnabled(approvalProcessDetail.processId,
                                                    approvalProcessDetail.dataOrder + 1, true);
                                    if (approvalProcessDetailNext != null)
                                    {
                                        nextUser = bllAdminUsers.GetByID(approvalProcessDetailNext.userId);
                                    }

                                }
                            }


                        }
                        else if (activeProcess.relatedColumn == "Kredi Limiti")
                        {
                            int deger = 0;
                            if (Convert.ToDouble(activeProcess.newValue) <= 150000)
                            {
                                deger = 1;
                            }
                            else if (Convert.ToDouble(activeProcess.newValue) <= 500000)
                            {
                                deger = 2;
                            }

                            ApprovalProcessDetail? approvalProcessDetailLast = bllApprovalProcessDetails
                                    .findByProcessIdAndDataOrderAndEnabled(approvalProcessDetail.processId, deger, true);
                            if (approvalProcessDetailLast != null)
                            {
                                BLLActions.AdminUsers bllAdminUsers = new BLLActions.AdminUsers(_configuration, _env, _mapper);
                                AdminUser userLast = bllAdminUsers.GetByID(approvalProcessDetailLast.userId)!;
                                if (userLast.Id != userId)
                                {

                                    ApprovalProcessDetail? approvalProcessDetailNext = bllApprovalProcessDetails
                                            .findByProcessIdAndDataOrderAndEnabled(approvalProcessDetail.processId,
                                                    approvalProcessDetail.dataOrder + 1, true);
                                    if (approvalProcessDetailNext != null)
                                    {
                                        nextUser = bllAdminUsers.GetByID(approvalProcessDetailNext.userId);

                                    }

                                }

                            }

                        }
                        else if (activeProcess.relatedColumn == "Haftalik Limit")
                        {
                            BLLActions.Customers bllCustomers = new BLLActions.Customers(_configuration, _env);
                            Data.SAP.Models.CustomerCredit? q2 = bllCustomers.getCustomerCredit(activeProcess.relatedDataId);
                            int deger;
                            if (q2 != null)
                            {


                                if (Convert.ToDouble(activeProcess.newValue) + double.Parse(q2.SNLMT!) <= 500000)
                                {
                                    deger = 1;
                                }
                                else if (Convert.ToDouble(activeProcess.newValue) + double.Parse(q2.SNLMT!) <= 1500000)
                                {
                                    deger = 2;
                                }
                                else if (Convert.ToDouble(activeProcess.newValue) + double.Parse(q2.SNLMT!) <= 3500000)
                                {
                                    deger = 3;
                                }
                                else
                                {
                                    deger = 4;
                                }
                                ApprovalProcessDetail? approvalProcessDetailLast = bllApprovalProcessDetails
                                        .findByProcessIdAndDataOrderAndEnabled(approvalProcessDetail.processId, deger, true);
                                if (approvalProcessDetailLast != null)
                                {
                                    BLLActions.AdminUsers bllAdminUsers = new BLLActions.AdminUsers(_configuration, _env, _mapper);
                                    AdminUser userLast = bllAdminUsers.GetByID(approvalProcessDetailLast.userId)!;
                                    if (userLast.Id != userId)
                                    {
                                        ApprovalProcessDetail? approvalProcessDetailNext = bllApprovalProcessDetails
                                                .findByProcessIdAndDataOrderAndEnabled(approvalProcessDetail.processId,
                                                        approvalProcessDetail.dataOrder + 1, true);
                                        if (approvalProcessDetailNext != null)
                                        {
                                            nextUser = bllAdminUsers.GetByID(approvalProcessDetailNext.userId);
                                        }

                                    }
                                }
                            }

                        }
                        if (nextUser == null)
                        {
                            if (activeProcessDetail != null)
                            {
                                activeProcessDetail.approved = true;
                                activeProcessDetail.isReplied = true;
                                activeProcessDetail.replyDate = DateTime.Now;
                                await bllActiveProcessDetails.Update(activeProcessDetail);
                            }

                            string returnString = "";

                            if (activeProcess.relatedColumn == "Kredi Limiti"
                                    || activeProcess.relatedColumn == "Haftalik Limit")
                            {

                                try
                                {
                                    returnString = changeCreditLimitSap(activeProcess.relatedDataId,
                                            Convert.ToDouble(activeProcess.newValue));
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine(e.ToString());
                                }

                            }
                            else if (activeProcess.relatedColumn == "Vade Tarihi")
                            {

                                try
                                {
                                    returnString = changeVadeSap(activeProcess.relatedDataDesc.Split("-")[0],
                                            activeProcess.relatedDataId, activeProcess.relatedDataDesc.Split("-")[1],
                                            int.Parse(activeProcess.newValue));
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine(e.Message.ToString());
                                }

                            }
                            if (returnString == "OK")
                            {
                                activeProcess.currentStateId = (int)CommonConstants.PROCESS_STATES.SAP_COMPLETED;
                            }
                            else
                            {
                                activeProcess.currentStateId = (int)CommonConstants.PROCESS_STATES.COMPLETED;
                            }

                            await Update(activeProcess);
                            EmailMessage emailMessage = new EmailMessage();
                            BLLActions.AdminUsers bllAdminUsers = new BLLActions.AdminUsers(_configuration, _env, _mapper);
                            AdminUser userCreated = bllAdminUsers.GetByID(activeProcess.createdUserId)!;
                            emailMessage.mailTuru = 2;
                            emailMessage.enabled = true;
                            emailMessage.isSent = false;
                            emailMessage.plannedDate = DateTime.Now;
                            emailMessage.toAddress = userCreated.email;
                            string mailMessage;
                            if (activeProcess.relatedColumn == "Vade Tarihi")
                            {
                                //DateTimeFormatter simpleDateFormat = DateTimeFormatter.ofPattern("dd.MM.yyyy");
                                DateTime tarihZf = DateTime.Parse(activeProcess.oldValue);
                                string tarih = tarihZf.ToString("dd.MM.yyyy");
                                emailMessage.subject = "Müşteri Vade Gün Değişikliği";
                                BLLActions.EmailReaderFile bllEmailReaderFile = new BLLActions.EmailReaderFile();
                                mailMessage = bllEmailReaderFile.VadeEmailTemplate(_configuration, _env,
                                    "Talep onaylandı", userCreated.name,
                                        activeProcess.relatedDataPrimaryId, activeProcess.relatedDataPrimaryDesc,
                                        activeProcess.relatedDataId, tarih, activeProcess.newValue,
                                        activeProcess.description, ok_link, no_link);
                                emailMessage.emailText = mailMessage;

                            }
                            else if (activeProcess.relatedColumn == "Kredi Limiti")
                            {
                                emailMessage.subject = "Müşteri Kredi Limiti Değişikliği";
                                BLLActions.EmailReaderFile bllEmailReaderFile = new BLLActions.EmailReaderFile();
                                mailMessage = bllEmailReaderFile.CreditEmailTemplate(_configuration, _env, "Talep onaylandı", userCreated.name,
                                        activeProcess.relatedDataId, activeProcess.relatedDataDesc,
                                        activeProcess.oldValue, activeProcess.newValue,
                                        activeProcess.description, ok_link, no_link);
                                emailMessage.emailText = mailMessage;
                            }
                            else if (activeProcess.relatedColumn == "Haftalik Limit")
                            {
                                emailMessage.subject = "Müşteri Haftalık Kredi Limiti Değişikliği";
                                BLLActions.EmailReaderFile bllEmailReaderFile = new BLLActions.EmailReaderFile();

                                mailMessage = bllEmailReaderFile.CreditEmailTemplate(_configuration, _env, "Müşteri Haftalık Kre"
                                        + "  di Limiti Değişikliği", userCreated.name,
                                        activeProcess.relatedDataId, activeProcess.relatedDataDesc,
                                        activeProcess.oldValue, activeProcess.newValue,
                                        activeProcess.description, ok_link, no_link);
                                emailMessage.emailText = mailMessage;
                            }
                            BLLActions.EmailMessages bllEmailMessages = new BLLActions.EmailMessages(_configuration, _env);
                            await bllEmailMessages.Add(emailMessage);

                        }
                        else
                        {
                            if (activeProcessDetail != null)
                            {
                                activeProcessDetail.approved = true; ;
                                activeProcessDetail.isReplied = true;
                                activeProcessDetail.replyDate = DateTime.Now;

                                await bllActiveProcessDetails.Update(activeProcessDetail);

                            }

                            ActiveProcessDetail activeProcessDetailNext = new ActiveProcessDetail();
                            activeProcessDetailNext.userId = nextUser.Id;
                            activeProcessDetailNext.enabled = true;
                            activeProcessDetailNext.guid = Guid.NewGuid();
                            activeProcessDetailNext.createdDate = DateTime.Now;
                            BLLActions.ActiveProcessVekalet bllActiveProcessVekalet = new BLLActions.ActiveProcessVekalet(_configuration, _env);
                            Data.Models.ActiveProcessVekalet? activeProcessVekalet = bllActiveProcessVekalet.GetByAlanUserId(nextUser.Id);
                            if (activeProcessVekalet != null)
                            {
                                activeProcessDetailNext.vekaletId = activeProcessVekalet.VekaletVerenId;
                            }
                            activeProcessDetailNext.isReplied = false;
                            activeProcessDetailNext.activeProcessId = activeProcess.Id;

                            await bllActiveProcessDetails.Add(activeProcessDetailNext);
                            activeProcess.currentUserId = nextUser.Id;
                            await Update(activeProcess);

                            EmailMessage emailMessage = new EmailMessage();

                            emailMessage.mailTuru = 2;
                            emailMessage.enabled = true;
                            emailMessage.isSent = false;
                            emailMessage.plannedDate = DateTime.Now;
                            emailMessage.toAddress = nextUser.email;
                            string mailMessage = "";
                            if (activeProcess.relatedColumn == "Vade Tarihi")
                            {
                                //DateTimeFormatter simpleDateFormat = DateTimeFormatter.ofPattern("dd.MM.yyyy");
                                DateTime tarihZf = DateTime.Parse(activeProcess.oldValue);
                                string tarih = tarihZf.ToString("dd.MM.yyyy");
                                emailMessage.subject = "Müşteri Vade Gün Değişikliği";
                                BLLActions.EmailReaderFile bllEmailReaderFile = new BLLActions.EmailReaderFile();
                                mailMessage = bllEmailReaderFile.VadeEmailTemplate(_configuration, _env, "Müşteri Vade Gün Değişikliği", nextUser.name,
                                        activeProcess.relatedDataPrimaryId, activeProcess.relatedDataPrimaryDesc,
                                        activeProcess.relatedDataId, tarih, activeProcess.newValue,
                                        activeProcess.description, ok_link, no_link);
                                emailMessage.emailText = mailMessage;

                            }
                            else if (activeProcess.relatedColumn == "Kredi Limiti")
                            {
                                emailMessage.subject = "Müşteri Kredi Limiti Değişikliği";
                                BLLActions.EmailReaderFile bllEmailReaderFile = new BLLActions.EmailReaderFile();
                                mailMessage = bllEmailReaderFile.CreditEmailTemplate(_configuration, _env, "Müşteri Kredi Limiti Değişikliği", nextUser.name,
                                        activeProcess.relatedDataId, activeProcess.relatedDataDesc,
                                        activeProcess.oldValue, activeProcess.newValue,
                                        activeProcess.description, ok_link, no_link);
                                emailMessage.emailText = mailMessage;
                            }
                            else if (activeProcess.relatedColumn == "Haftalik Limit")
                            {
                                emailMessage.subject = "Müşteri Haftalık Kredi Limiti Değişikliği";
                                BLLActions.EmailReaderFile bllEmailReaderFile = new BLLActions.EmailReaderFile();
                                mailMessage = bllEmailReaderFile.CreditEmailTemplate(_configuration, _env, "Müşteri Haftalık Kredi Limiti Değişikliği", nextUser.name,
                                        activeProcess.relatedDataId, activeProcess.relatedDataDesc,
                                        activeProcess.oldValue, activeProcess.newValue,
                                        activeProcess.description, ok_link, no_link);
                                emailMessage.emailText = mailMessage;
                            }
                            BLLActions.EmailMessages bllEmailMessages = new BLLActions.EmailMessages(_configuration, _env);
                            await bllEmailMessages.Add(emailMessage);

                        }
                    }
                    return true;
                }
                else
                {
                    foreach (ActiveProcess activeProcess in listActiveProcess)
                    {
                        BLLActions.ActiveProcessDetails bllActiveProcessDetail = new BLLActions.ActiveProcessDetails(_configuration, _env);
                        ActiveProcessDetail? activeProcessDetail = bllActiveProcessDetail
                                .findByActiveProcessIdAndUserIdAndApprovedAndEnabled(activeProcess.Id, userId, null, true);
                        if (activeProcessDetail != null)
                        {
                            activeProcessDetail.isReplied = true;
                            activeProcessDetail.approved = false;
                            activeProcessDetail.replyDate = DateTime.Now;

                            await bllActiveProcessDetail.Update(activeProcessDetail);
                        }
                        activeProcess.currentStateId = 2;
                        EmailMessage emailMessage = new EmailMessage();
                        BLLActions.AdminUsers bllAdminUsers = new BLLActions.AdminUsers(_configuration, _env, _mapper);
                        AdminUser userCreated = bllAdminUsers.GetByID(activeProcess.createdUserId)!;
                        emailMessage.mailTuru = 2;
                        emailMessage.enabled = true;
                        emailMessage.isSent = false;
                        emailMessage.plannedDate = DateTime.Now;
                        emailMessage.toAddress = userCreated.email;
                        string mailMessage = "";
                        if (activeProcess.relatedColumn == "Vade Tarihi")
                        {
                            //DateTimeFormatter simpleDateFormat = DateTimeFormatter.ofPattern("dd.MM.yyyy");
                            DateTime tarihZf = DateTime.Parse(activeProcess.oldValue);
                            string tarih = tarihZf.ToString("dd.MM.yyyy");
                            BLLActions.EmailReaderFile bllEmailReaderFile = new BLLActions.EmailReaderFile();
                            mailMessage = bllEmailReaderFile.VadeEmailTemplate(_configuration, _env, "Talep Red edildi", userCreated.name,
                                    activeProcess.relatedDataPrimaryId, activeProcess.relatedDataPrimaryDesc,
                                    activeProcess.relatedDataId, tarih, activeProcess.newValue,
                                    activeProcess.description, ok_link, no_link);
                            emailMessage.emailText = mailMessage;
                        }
                        else if (activeProcess.relatedColumn == "Kredi Limiti")
                        {
                            emailMessage.subject = "Müşteri Vade Gün Değişikliği";
                            BLLActions.EmailReaderFile bllEmailReaderFile = new BLLActions.EmailReaderFile();
                            mailMessage = bllEmailReaderFile.CreditEmailTemplate(_configuration, _env, "Müşteri Kredi Limiti Değişikliği", userCreated.name,
                                    activeProcess.relatedDataId, activeProcess.relatedDataDesc,
                                    activeProcess.oldValue, activeProcess.newValue, activeProcess.description,
                                    ok_link, no_link);
                            emailMessage.emailText = mailMessage;
                        }
                        else if (activeProcess.relatedColumn == "Haftalik Limit")
                        {
                            emailMessage.subject = "Müşteri Vade Gün Değişikliği";
                            BLLActions.EmailReaderFile bllEmailReaderFile = new BLLActions.EmailReaderFile();
                            mailMessage = bllEmailReaderFile.CreditEmailTemplate(_configuration, _env, "Müşteri Kredi Limiti Değişikliği", userCreated.name,
                                    activeProcess.relatedDataId, activeProcess.relatedDataDesc,
                                    activeProcess.oldValue, activeProcess.newValue, activeProcess.description,
                                    ok_link, no_link);
                            emailMessage.emailText = mailMessage;
                        }
                        BLLActions.EmailMessages bllEmailMessages = new BLLActions.EmailMessages(_configuration, _env);
                        await bllEmailMessages.Add(emailMessage);

                    }
                    return true;
                }
            }

            private string changeVadeSap(string bukrs, string belnr, string gjahr, int day)
            {
                BLLActions.ChangeCreditOrVadeLimitSap bllChangeCreditOrVadeLimitSap = new BLLActions.ChangeCreditOrVadeLimitSap(_configuration, _env, _mapper);
                return bllChangeCreditOrVadeLimitSap.changeVadeSap(bukrs, belnr, gjahr, day);
            }

            public string changeCreditLimitSap(string kunnr, double dmbtr)
            {
                BLLActions.ChangeCreditOrVadeLimitSap bllChangeCreditOrVadeLimitSap = new BLLActions.ChangeCreditOrVadeLimitSap(_configuration, _env, _mapper);

                return bllChangeCreditOrVadeLimitSap.changeCreditLimitSap(kunnr, dmbtr);
            }

            public async Task<bool> changeAllDate(bool approved, List<int> listInt, int userId)
            {
                List<ActiveProcess> listActiveProcessDocument = dal.Get(u => listInt.Contains(u.Id) && u.enabled).ToList();
                if (approved)
                {

                    foreach (ActiveProcess activeProcess in listActiveProcessDocument)
                    {
                        BLLActions.ActiveProcessDetails bllActiveProcessDetails = new BLLActions.ActiveProcessDetails(_configuration, _env);
                        ActiveProcessDetail? activeProcessDetail = bllActiveProcessDetails
                                .findByActiveProcessIdAndUserIdAndApprovedAndEnabled(activeProcess.Id, userId, null, true);
                        BLLActions.ApprovalProcessDetails bllApprovalProcessDetails = new BLLActions.ApprovalProcessDetails(_configuration, _env, _mapper);
                        ApprovalProcessDetail? approvalProcessDetail = bllApprovalProcessDetails
                                .findByProcessIdAndUserIdAndEnabled(activeProcess.approvalProcessId, userId, true);
                        AdminUser? nextUser = null;
                        if (approvalProcessDetail == null || activeProcessDetail == null)
                        {
                            continue;
                        }
                        BLLActions.HRVekaletTable bllHRVekaletTable = new BLLActions.HRVekaletTable(_configuration, _env);
                        Data.Models.HRVekaletTable activeProcessVekalet = bllHRVekaletTable.GetByAlanUserId(approvalProcessDetail.userId);

                        if (activeProcess.relatedColumn.Equals("Vade Tarihi"))
                        {
                            CustomerDocumentDto? customerDocumentDto;
                            if (activeProcess.relatedDataId.Contains("Çek ile Vade"))
                            {
                                BLLActions.ActiveProcessInvoices bllActiveProcessInvoices = new BLLActions.ActiveProcessInvoices(_configuration, _env, _mapper);
                                List<ActiveProcessInvoice> listFatura = bllActiveProcessInvoices
                                        .getByActiveProcessId(activeProcess.Id);
                                BLLActions.CustomerDocumentSap bllCustomerDocumentSap = new BLLActions.CustomerDocumentSap(_configuration, _env, _mapper);
                                customerDocumentDto = bllCustomerDocumentSap
                                        .getCustomerDocument(activeProcess.relatedDataPrimaryId)
                                        .Where(u => (u.BELNR ?? "0").Equals(listFatura[0].belnr)).FirstOrDefault();
                            }
                            else
                            {
                                BLLActions.CustomerDocumentSap bllCustomerDocumentSap = new BLLActions.CustomerDocumentSap(_configuration, _env, _mapper);

                                customerDocumentDto = bllCustomerDocumentSap
                                        .getCustomerDocument(activeProcess.relatedDataPrimaryId)
                                        .Where(u => (u.BELNR ?? "0").Equals(activeProcess.relatedDataId)).FirstOrDefault();
                            }
                            int deger = 0;
                            if (customerDocumentDto != null)
                            {
                                int fark = 0;
                                try
                                {
                                    if (Objects.Equals(customerDocumentDto.ZTERM, "")
                                            || Objects.Equals(customerDocumentDto.ZTERM, null))
                                    {
                                        fark = int.Parse(customerDocumentDto?.ZBD1T ?? "0");

                                    }
                                    else
                                    {
                                        int ZBD1T = int.Parse(customerDocumentDto.ZBD1T ?? "0");
                                        int ZTERM = int.Parse(customerDocumentDto.ZTERM.Substring(1));
                                        fark = ZBD1T - ZTERM;
                                    }

                                }
                                catch
                                {
                                    fark = 0;
                                }

                                activeProcess.oncekiArtirim = fark;
                                double newValue = fark + int.Parse(activeProcess.newValue);
                                if (newValue <= 10)
                                {
                                    deger = 1;
                                }
                                else if (newValue <= 15)
                                {
                                    deger = 2;
                                }
                                else if (newValue <= 20)
                                {
                                    deger = 3;
                                }
                                else
                                {
                                    deger = 4;
                                }
                                int? lastUserId = bllApprovalProcessDetails.findByProcessIdAndDataOrderAndEnabled(
                                        activeProcess.approvalProcessId, deger, true)?.userId;
                                if (userId != lastUserId)
                                {
                                    nextUser = bllApprovalProcessDetails.GetNextUser(userId,
                                            activeProcess.approvalProcessId, true);
                                }

                            }
                            else
                            {
                                Delete(activeProcess.Id);

                                bllActiveProcessDetails.Delete(approvalProcessDetail.Id);
                                BLLActions.AdminUsers bllAdminUsers = new BLLActions.AdminUsers(_configuration, _env, _mapper);
                                AdminUser? createdUser = bllAdminUsers.GetByID(activeProcess.createdUserId);
                                if (createdUser != null)
                                {
                                    sendErrorEmail(activeProcess, createdUser, createdUser.Id);
                                }

                                continue;

                            }
                        }
                        else
                        {
                            nextUser = bllApprovalProcessDetails.GetNextUser(userId, activeProcess.approvalProcessId,
                                    true);
                        }
                        if (!Objects.Equals(nextUser, null))
                        {
                            activeProcessDetail.approved = true;
                            activeProcessDetail.replyDate = DateTime.Now;
                            activeProcessDetail.isReplied = true;

                            await bllActiveProcessDetails.Update(activeProcessDetail);
                            ActiveProcessDetail d = new ActiveProcessDetail();
                            if (!Objects.Equals(activeProcessVekalet, null))
                            {
                                d.vekaletId = activeProcessVekalet.vekaletAlanId;
                            }
                            d.activeProcessId = activeProcessDetail.activeProcessId;
                            d.approved = null;
                            d.createdDate = DateTime.Now;
                            d.description = "";
                            d.isReplied = false;
                            d.guid = Guid.NewGuid();
                            d.replyDate = null;
                            d.enabled = true;
                            d.userId = nextUser.Id;

                            await bllActiveProcessDetails.Add(d);

                            activeProcess.currentUserId = nextUser.Id;

                            sendEmail(activeProcess, nextUser, userId);

                        }
                        else
                        {
                            string replyText;
                            if (activeProcess.relatedDataId.Contains("Çek ile Vade"))
                            {
                                BLLActions.ActiveProcessInvoices bllActiveProcessInvoices = new BLLActions.ActiveProcessInvoices(_configuration, _env, _mapper);
                                List<ActiveProcessInvoice> listActiveProcessInvoice = bllActiveProcessInvoices
                                        .getByActiveProcessId(activeProcess.Id);
                                BLLActions.ActiveProcessChecks bllActiveProcessChecks = new BLLActions.ActiveProcessChecks(_configuration, _env, _mapper);
                                List<Data.Models.ActiveProcessChecks> listActiveProcessChecks = bllActiveProcessChecks
                                        .getByActiveProcessId(activeProcess.Id);

                                List<FaturaGunFarkDto> liste = listGunHesaplama(listActiveProcessInvoice,
                                        listActiveProcessChecks);

                                foreach (FaturaGunFarkDto faturaDto in liste)
                                {
                                    BLLActions.ChangeCreditOrVadeLimitSap bllChangeCreditOrVadeLimitSap = new BLLActions.ChangeCreditOrVadeLimitSap(_configuration, _env, _mapper);
                                    replyText = bllChangeCreditOrVadeLimitSap.changeVadeSap(faturaDto.BUKRS ?? "",
                                            faturaDto.BELNR ?? "", (faturaDto.GJAHR ?? 0).ToString(), faturaDto.gunFarki ?? 0);
                                    if (replyText.Equals("OK"))
                                    {
                                        activeProcess.currentStateId = (int)CommonConstants.PROCESS_STATES.SAP_COMPLETED;
                                    }
                                    else
                                    {
                                        activeProcess.currentStateId = (int)CommonConstants.PROCESS_STATES.COMPLETED;
                                    }
                                }


                            }
                            else
                            {
                                BLLActions.ChangeCreditOrVadeLimitSap bllChangeCreditOrVadeLimitSap = new BLLActions.ChangeCreditOrVadeLimitSap(_configuration, _env, _mapper);

                                replyText = bllChangeCreditOrVadeLimitSap.changeVadeSap(
                                        activeProcess.relatedDataDesc.Substring(0, 4), activeProcess.relatedDataId,
                                        activeProcess.relatedDataDesc.Substring(5),
                                        int.Parse(activeProcess.newValue));
                                if (replyText.Equals("OK"))
                                {
                                    activeProcess.currentStateId = (int)CommonConstants.PROCESS_STATES.SAP_COMPLETED;
                                }
                                else
                                {
                                    activeProcess.currentStateId = (int)CommonConstants.PROCESS_STATES.COMPLETED;
                                }
                            }
                            BLLActions.AdminUsers bllAdminUsers = new BLLActions.AdminUsers(_configuration, _env, _mapper);
                            AdminUser? user = bllAdminUsers.GetByID(activeProcess.createdUserId);
                            if (user != null)
                                sendFinishedEmail(activeProcess, user, userId);

                            activeProcessDetail.approved = true;
                            activeProcessDetail.replyDate = DateTime.Now;
                            activeProcessDetail.isReplied = true;

                            await bllActiveProcessDetails.Update(activeProcessDetail);

                        }
                        await Update(activeProcess);
                    }

                    return true;
                }
                else
                {
                    foreach (ActiveProcess activeProcess in listActiveProcessDocument)
                    {
                        activeProcess.currentStateId = (int)CommonConstants.PROCESS_STATES.DECLINED;

                        BLLActions.AdminUsers bllAdminUsers = new BLLActions.AdminUsers(_configuration, _env, _mapper);
                        AdminUser? user = bllAdminUsers.GetByID(activeProcess.createdUserId);
                        sendDeclinedEmail(activeProcess, user, userId);
                        await Update(activeProcess);

                        BLLActions.ActiveProcessDetails bllActiveProcessDetails = new BLLActions.ActiveProcessDetails(_configuration, _env);
                        ActiveProcessDetail? detail = bllActiveProcessDetails
                                .findByActiveProcessIdAndUserIdAndApprovedAndEnabled(activeProcess.Id, userId, null, true);
                        if (detail != null)
                        {
                            detail.approved = false;
                            detail.isReplied = true;
                            detail.replyDate = DateTime.Now;
                            await bllActiveProcessDetails.Update(detail);
                        }


                    }
                    return false;
                }
            }
            private List<FaturaGunFarkDto> listGunHesaplama(
    List<Data.Models.ActiveProcessInvoice> listActiveProcessInvoice,
    List<Data.Models.ActiveProcessChecks> listActiveProcessChecks)
            {
                listActiveProcessInvoice = listActiveProcessInvoice
                    .OrderBy(x => DateTime.Parse(x!.faedt!))
                    .ToList();

                listActiveProcessChecks = listActiveProcessChecks
                    .OrderBy(x => DateTime.Parse(x.netdt!))
                    .ToList();

                List<FaturaGunFarkDto> resultList = new List<FaturaGunFarkDto>();

                int cekIndex = 0;
                double cekKalan = 0;

                foreach (var fatura in listActiveProcessInvoice)
                {
                    double faturaKalan = fatura.dmshb ?? 0;
                    DateTime faturaVade = DateTime.Parse(fatura.faedt!);

                    DateTime? kapanisCekTarihi = null;

                    while (faturaKalan > 0)
                    {
                        if (cekKalan == 0)
                        {
                            if (cekIndex >= listActiveProcessChecks.Count)
                            {
                                break;
                            }

                            var cek = listActiveProcessChecks[cekIndex];
                            cekKalan = Math.Abs(cek.wrbtr ?? 0);
                        }

                        var aktifCek = listActiveProcessChecks[cekIndex];
                        DateTime cekTarihi = DateTime.Parse(aktifCek.netdt ?? "");

                        if (cekKalan >= faturaKalan)
                        {
                            cekKalan -= faturaKalan;
                            faturaKalan = 0;

                            kapanisCekTarihi = cekTarihi;
                        }
                        else
                        {
                            faturaKalan -= cekKalan;
                            cekKalan = 0;
                            cekIndex++;
                        }
                    }

                    if (faturaKalan == 0 && kapanisCekTarihi != null)
                    {
                        int gunFarki = (int)(kapanisCekTarihi.Value - faturaVade).TotalDays;

                        resultList.Add(new FaturaGunFarkDto
                        {
                            BUKRS = fatura.bukrs,
                            BELNR = fatura.belnr,
                            GJAHR = fatura.gjahr,
                            gunFarki = gunFarki
                        });
                    }

                    if (cekIndex >= listActiveProcessChecks.Count && cekKalan == 0)
                    {
                        break;
                    }
                }

                return resultList;
            }
            public string buildVade(string title, string firstUser, string kunnr, string name1, string belnr, string zfbdt,
                    string newValue, string description, string ok_link, string no_link)
            {

                string? filePath = Path.Combine(_env.IsDevelopment() ? _configuration["FilePath:local"]! : _env.IsProduction() ? _configuration["FilePath:server"]! :
                  _configuration["FilePath:test"]!, "templates\\Email\\emailVade.html");
                StreamReader reader = new StreamReader(filePath);
                string content = reader.ReadToEnd();


                content.Replace("title", title);
                content.Replace("firstUser", firstUser);
                content.Replace("kunnr", kunnr);
                content.Replace("name1", name1);
                content.Replace("belnr", belnr);
                content.Replace("zfbdt", zfbdt);
                content.Replace("newValue", newValue);
                content.Replace("description", description);
                content.Replace("ok_link", ok_link);
                content.Replace("no_link", no_link);


                content = content.Replace("{footer}", "Copyright &copy; 2016 Aşkale Çimento");

                return content;
            }

            public string buildCredit(string title, string firstUser, string kunnr, string name1, string klimk, string amount,
                    string description, string ok_link, string no_link)
            {


                string? filePath = Path.Combine(_env.IsDevelopment() ? _configuration["FilePath:local"]! : _env.IsProduction() ? _configuration["FilePath:server"]! :
                    _configuration["FilePath:test"]!, "templates\\Email\\emailCredit.html");
                StreamReader reader = new StreamReader(filePath);
                string content = reader.ReadToEnd();

                content.Replace("title", title);
                content.Replace("firstUser", firstUser);
                content.Replace("kunnr", kunnr);
                content.Replace("name1", name1);
                content.Replace("klimk", klimk);
                content.Replace("amount", amount);
                content.Replace("description", description);
                content.Replace("ok_link", ok_link);
                content.Replace("no_link", no_link);


                content = content.Replace("{footer}", "Copyright &copy; 2016 Aşkale Çimento");

                return content;


            }

            private async void sendErrorEmail(ActiveProcess lst, AdminUser? user, int? userId)
            {
                EmailMessage email = new EmailMessage();

                email.subject = "Talep Silindi";
                email.toAddress = user?.email;
                BLLActions.EmailReaderFile bllEmailReaderFile = new BLLActions.EmailReaderFile();


                email.emailText = bllEmailReaderFile.BuildEmailTemplate(_configuration, _env, "Müşteri Belge Vadesi Değişikliği", "Sayın <strong>" + user?.name
                        + "</strong>, <br /> Talebiniz <strong>SAP'de aktif belge bulunamadığından silinmiştir.</strong>. <br /><br />"
                        + "Müşteri No: " + lst.relatedDataId + " <br />" + "Müşteri Adı: " + lst.relatedDataDesc
                        + " <br />" + "Mevcut Kredi Limiti: " + lst.oldValue + " TL <br />" + "Artırım Tutarı: "
                        + lst.newValue + " TL<br />" + "Açıklama: " + lst.description + "<br /><br />"
                        + " Saygılarımızla.");

                email.isSent = false;
                email.mailTuru = 2;
                email.plannedDate = DateTime.Now;
                BLLActions.EmailMessages bllEmailMessages = new BLLActions.EmailMessages(_configuration, _env);
                await bllEmailMessages.Add(email);
            }

            private async void sendDeclinedEmail(ActiveProcess lst, AdminUser? user, int userId)
            {
                EmailMessage email = new EmailMessage();

                email.subject = "Talep REDDEDİLDİ";
                email.toAddress = user?.email;
                BLLActions.EmailReaderFile bllEmailReaderFile = new BLLActions.EmailReaderFile();

                email.emailText = bllEmailReaderFile.BuildEmailTemplate(_configuration, _env, "Müşteri Belge Vadesi Değişikliği", "Sayın <strong>" + user?.name
                        + "</strong>, <br /> Talebiniz <strong>REDDEDİLMİŞTİR</strong>. <br /><br />" + "Müşteri No: "
                        + lst.relatedDataId + " <br />" + "Müşteri Adı: " + lst.relatedDataDesc + " <br />"
                        + "Mevcut Kredi Limiti: " + lst.oldValue + " TL <br />" + "Artırım Miktarı: " + lst.newValue
                        + " <br />" + "Açıklama: " + lst.description + "<br /><br />" + " Saygılarımızla.");

                email.isSent = false;
                email.mailTuru = 2;
                email.plannedDate = DateTime.Now;
                BLLActions.EmailMessages bllEmailMessages = new BLLActions.EmailMessages(_configuration, _env); ;
                await bllEmailMessages.Add(email);
            }


            private async void sendFinishedEmail(ActiveProcess lst, AdminUser? user, int userId)
            {
                EmailMessage email = new EmailMessage();

                email.subject = "Talep ONAYLANDI";
                email.toAddress = user?.email;

                BLLActions.EmailReaderFile bllEmailReaderFile = new BLLActions.EmailReaderFile();

                email.emailText = bllEmailReaderFile.BuildEmailTemplate(_configuration, _env, "Müşteri Belge Vadesi Değişikliği", "Sayın <strong>" + user?.name
                        + "</strong>, <br /> Talebiniz <strong>ONAYLANMIŞTIR</strong>. <br /><br />" + "Müşteri No: "
                        + lst.relatedDataId + " <br />" + "Müşteri Adı: " + lst.relatedDataDesc + " <br />"
                        + "Mevcut Kredi Limiti: " + lst.oldValue + " TL <br />" + "Artırım Tutarı: " + lst.newValue
                        + " TL<br />" + "Açıklama: " + lst.description + "<br /><br />" + " Saygılarımızla.");

                email.isSent = false;
                email.mailTuru = 2;
                email.plannedDate = DateTime.Now;
                BLLActions.EmailMessages bllEmailMessages = new BLLActions.EmailMessages(_configuration, _env); ;
                await bllEmailMessages.Add(email);
            }


            private async void sendEmail(ActiveProcess lst, AdminUser? user, int userId)
            {
                EmailMessage email = new EmailMessage();

                email.subject = "Müşteri Belge Vadesi Değişikliği";
                email.toAddress = user?.email;

                BLLActions.EmailReaderFile bllEmailReaderFile = new BLLActions.EmailReaderFile();


                email.emailText = bllEmailReaderFile.BuildEmailTemplate(_configuration, _env, "Müşteri Belge Vadesi Değişikliği", "Sayın <strong>" + user?.name
                        + "</strong>, <br />  Onaylamanız gereken 1 adet talep bulunmaktadır. <br /><br />" + "Müşteri No: "
                        + lst.relatedDataId + " <br />" + "Müşteri Adı: " + lst.relatedDataDesc + " <br />"
                        + "Mevcut Kredi Limiti: " + lst.oldValue + " TL <br />" + "Artırım Miktarı: " + lst.newValue
                        + " <br />" + "Açıklama: " + lst.description + "<br /><br />" + " Saygılarımızla.");

                email.isSent = false;
                email.mailTuru = 2;
                email.plannedDate = DateTime.Now;
                BLLActions.EmailMessages bllEmailMessages = new BLLActions.EmailMessages(_configuration, _env); ;
                await bllEmailMessages.Add(email);

            }

            public async Task<bool> changedate(string bukrs, int gjahr, string name1, string kunnr, string faedt, string belnr, string zfbdt, string dagitimKanali, int newValue, string description, int userId, string belgeTutari)
            {
                try
                {
                    string ok_link = OkNoLinks.OK_LINK;
                    string no_link = OkNoLinks.NO_LINK;
                    BLLActions.AdminUsers bllAdminUsers = new BLLActions.AdminUsers(_configuration, _env, _mapper);
                    AdminUser? user = bllAdminUsers.GetByID(userId);
                    if (user == null)
                    {
                        return false;
                    }
                    BLLActions.Companies bllCompanies = new BLLActions.Companies(_configuration, _env, _mapper);
                    int companyId = bllCompanies.getByVkorgCompany(bukrs).Id;
                    ApprovalProcess? approvalProcess = new ApprovalProcess();

                    BLLActions.ApprovalProcesses bllApprovalProcesses = new BLLActions.ApprovalProcesses(_configuration, _env, _mapper);
                    if (kunnr.StartsWith("00006"))
                    {
                        approvalProcess = bllApprovalProcesses.findByCompanyIdAndTypeIdAndDagitimKanaliAndEnabled(companyId,
                                (int)CommonConstants.APPROVAL_PROCESSES.MOTORIN_EXPIRY_DATE, dagitimKanali,
                                true);
                    }
                    else
                    {
                        approvalProcess = bllApprovalProcesses.findByCompanyIdAndTypeIdAndDagitimKanaliAndEnabled(companyId,
                                (int)CommonConstants.APPROVAL_PROCESSES.DOCUMENT_EXPIRY_DATE, dagitimKanali,
                                true);
                    }
                    if (approvalProcess == null)
                    {
                        return false;
                    }
                    BLLActions.ApprovalProcessDetails bllApprovalProcessDetails = new BLLActions.ApprovalProcessDetails(_configuration, _env, _mapper);
                    ApprovalProcessDetail? approvalProcessDetail = bllApprovalProcessDetails
                            .findByProcessIdAndDataOrderAndEnabled(approvalProcess.Id, 1, true);
                    if (approvalProcessDetail == null)
                    {
                        return false;
                    }
                    AdminUser? firstUser = bllAdminUsers.GetByID(approvalProcessDetail.userId);
                    if (firstUser == null)
                    {
                        return false;
                    }
                    BLLActions.ActiveProcessVekalet bllActiveProcessVekalet = new BLLActions.ActiveProcessVekalet(_configuration, _env);
                    Data.Models.ActiveProcessVekalet? activeProcessVekalet = bllActiveProcessVekalet.GetByAlanUserId(firstUser.Id);
                    ActiveProcess activeProcess = new ActiveProcess();
                    if (activeProcessVekalet != null)
                    {
                        activeProcess.userVekaletId = activeProcessVekalet.VekaletAlanId;
                    }

                    activeProcess.approvalProcessId = approvalProcess.Id;
                    activeProcess.createdUserId = user.Id;
                    activeProcess.createdDate = DateTime.Now;
                    activeProcess.dagitimKanali = dagitimKanali;
                    activeProcess.newValue = newValue.ToString();
                    activeProcess.oldValue = faedt;
                    activeProcess.currentStateId = (int)CommonConstants.PROCESS_STATES.ACTIVE;
                    activeProcess.relatedData = "BELNR";
                    activeProcess.relatedDataId = belnr;
                    activeProcess.relatedDataPrimary = "KUNNR";
                    activeProcess.relatedDataPrimaryId = kunnr;
                    activeProcess.relatedDataPrimaryDesc = name1;
                    activeProcess.relatedDataDesc = bukrs + "-" + gjahr;
                    activeProcess.description = description;
                    activeProcess.currentUserId = firstUser.Id;
                    activeProcess.dataType = "int";
                    activeProcess.enabled = true;
                    activeProcess.belgeTutari = belgeTutari;

                    if (kunnr.StartsWith("00006"))
                    {
                        activeProcess.relatedColumn = "Motorin Vade Tarihi";
                    }
                    else
                    {
                        activeProcess.relatedColumn = "Vade Tarihi";
                    }
                    ActiveProcess? activeProcessSave = await Add(activeProcess);
                    ActiveProcessDetail activeProcessDetail = new ActiveProcessDetail();
                    activeProcessDetail.activeProcessId = activeProcessSave?.Id ?? 0;
                    if (activeProcessVekalet != null)
                    {
                        activeProcessDetail.vekaletId = activeProcessVekalet.VekaletAlanId;
                    }
                    activeProcessDetail.approved = null;
                    activeProcessDetail.createdUserId = userId;
                    activeProcessDetail.createdDate = DateTime.Now;
                    activeProcessDetail.description = "";
                    activeProcessDetail.replyDate = null;
                    activeProcessDetail.isReplied = false;
                    activeProcessDetail.guid = Guid.NewGuid();
                    activeProcessDetail.enabled = true;
                    activeProcessDetail.userId = firstUser.Id;

                    await Add(activeProcess);
                    BLLActions.ActiveProcessDetails bllActiveProcessDetails = new BLLActions.ActiveProcessDetails(_configuration, _env);
                    await bllActiveProcessDetails.Add(activeProcessDetail);

                    EmailMessage emailMessage = new EmailMessage();
                    emailMessage.subject = "Müşteri Vade Gün Değişikliği";
                    emailMessage.toAddress = firstUser.email;

                    //DateTimeFormatter simpleDateFormat = DateTimeFormatter.ofPattern("dd.MM.yyyy");
                    DateTime tarihZf = DateTime.Parse(zfbdt);
                    string tarih = tarihZf.ToString("dd.MM.yyyy");
                    string mailMessageString = buildVade("Müşteri Vade Gün Değişikliği", firstUser.name, kunnr, name1,
                            belnr, tarih, newValue.ToString(), description, ok_link, no_link);
                    emailMessage.enabled = true;
                    emailMessage.emailText = mailMessageString;
                    emailMessage.isSent = false;
                    emailMessage.mailTuru = 2;
                    emailMessage.createdDate = DateTime.Now;
                    emailMessage.createdUserId = userId;
                    emailMessage.plannedDate = DateTime.Now;
                    BLLActions.EmailMessages bllEmailMessages = new BLLActions.EmailMessages(_configuration, _env);
                    await bllEmailMessages.Add(emailMessage);

                    return true;
                }
                catch
                {

                    return false;
                }
            }

            public PageReturn<ActiveProcessDto> listFilterByStateIdAndTypeId(FilterPageParam<ActiveProcessListParameter> filterPageParam)
            {
                PageReturn<ActiveProcessDto>? result = new PageReturn<ActiveProcessDto>();
                int pageSize = filterPageParam.size ?? 20;
                int pageNumber = filterPageParam.page ?? 0;

                string? relatedDataId = filterPageParam?.liste?.relatedDataId?.ToString();
                string? relatedDataDesc = filterPageParam?.liste?.relatedDataDesc?.ToString();
                string? relatedDataPrimaryId = filterPageParam?.liste?.relatedDataPrimaryId?.ToString();
                string? relatedDataPrimaryDesc = filterPageParam?.liste?.relatedDataPrimaryDesc?.ToString();
                int? stateId = int.Parse(filterPageParam?.liste?.stateId?.ToString() ?? "");
                string? typeString = filterPageParam?.liste?.type?.ToString().Replace("[", "").Replace("]", "").Replace(" ",
                        "");
                string[] type = typeString!.Split(",");
                HashSet<int> typeIntegers = new HashSet<int>();
                for (int i = 0; i < type.Length; i++)
                {
                    int sayInteger = int.Parse(type[i]);
                    typeIntegers.Add(sayInteger);
                }
                int userId = int.Parse(filterPageParam?.liste?.userId?.ToString() ?? "0");

                BLLActions.AdminUsers bllAdminUsers = new BLLActions.AdminUsers(_configuration, _env, _mapper);
                AdminUser? user = bllAdminUsers.GetByID(userId);

                BLLActions.RoleDetails bllRoleDetails = new BLLActions.RoleDetails(_configuration, _env, _mapper);
                RoleDetail? roleDetail = bllRoleDetails.GetByRoleIDAndModuleID(user?.roleId ?? 0, (int)CommonConstants.MODULES.CUSTOMER_CREDITS);

                if (user?.roleId == 1 || (roleDetail != null && roleDetail.canSee))
                {
                    IQueryable<ActiveProcess> query = dal.Get(u => u.enabled &&
                 type.Contains(u.approvalProcess.typeId.ToString()) &&
                 (relatedDataId == null || relatedDataId == "" ? true : u.relatedDataId == relatedDataId) &&
                 (relatedDataDesc == null || relatedDataDesc == "" ? true : u.relatedDataDesc == relatedDataDesc) &&
                  (relatedDataPrimaryId == null || relatedDataPrimaryId == "" ? true : u.relatedDataPrimaryId == relatedDataPrimaryId) &&
                    (relatedDataPrimaryDesc == null || relatedDataPrimaryDesc == "" ? true : u.relatedDataPrimaryDesc == relatedDataPrimaryDesc) &&
                    u.currentStateId == stateId);
                    result.content = query
                      .Skip(pageSize * pageNumber).Take(pageSize)

                        .Select(u => new ActiveProcessDto()
                        {
                            approvalProcess = u.approvalProcess,
                            belgeTutari = u.belgeTutari,
                            createdDate = u.createdDate,
                            createdUserId = u.createdUserId,
                            currentState = u.currentState,
                            currentUser = u.currentUser,
                            customFields = u.customFields,
                            dagitimKanali = u.dagitimKanali,
                            dataType = u.dataType,
                            description = u.description,
                            disaprovecondition = u.disaprovecondition,
                            enabled = u.enabled,
                            id = u.Id,
                            newValue = u.newValue,
                            oldValue = u.oldValue,
                            //listActiveProcessDetail = [],
                            oncekiArtirim = u.oncekiArtirim,
                            relatedColumn = u.relatedColumn,
                            relatedData = u.relatedData,
                            relatedDataDesc = u.relatedDataDesc,
                            relatedDataId = u.relatedDataId,
                            relatedDataPrimary = u.relatedDataPrimary,
                            relatedDataPrimaryDesc = u.relatedDataPrimaryDesc,
                            relatedDataPrimaryId = u.relatedDataPrimaryId,
                            userVekalet = u.userVekalet,


                        }).ToList();
                    List<int> listActiveProcessId = new List<int>();
                    foreach (ActiveProcess activeProcessDto in query.ToList())
                    {
                        listActiveProcessId.Add(activeProcessDto.Id);
                    }
                    List<ActiveProcessDetail> allActiveProcessDetails = new List<ActiveProcessDetail>();

                    if (!listActiveProcessId.IsNullOrEmpty())
                    {
                        int batchSize = 1000;
                        List<int> idList = new List<int>(listActiveProcessId);

                        for (int i = 0; i < idList.Count(); i += batchSize)
                        {
                            List<int> batch = idList
    .Skip(i)
    .Take(Math.Min(batchSize, idList.Count - i))
    .ToList();
                            BLLActions.ActiveProcessDetails bllActiveProcessDetails = new BLLActions.ActiveProcessDetails(_configuration, _env);
                            List<ActiveProcessDetail> batchResult = bllActiveProcessDetails
                                    .findAllByListActiveProcessIdAndEnabled(batch, true);
                            allActiveProcessDetails.AddRange(batchResult);
                        }

                        foreach (ActiveProcessDto activeProcessDto in result.content)
                        {
                            activeProcessDto.listActiveProcessDetail = allActiveProcessDetails.Where(detail => detail.activeProcessId.Equals(activeProcessDto.id)).ToList();
                        }


                    }
                    result.totalElements = query.Count();
                    result.number = result.content.Count();
                    result.size = pageSize;

                    return result;
                }
                else
                {
                    IQueryable<ActiveProcess> query = dal.Get(u => u.enabled &&
                type.Contains(u.approvalProcess.typeId.ToString()) &&
                (relatedDataId == null || relatedDataId == "" ? true : u.relatedDataId == relatedDataId) &&
                (relatedDataDesc == null || relatedDataDesc == "" ? true : u.relatedDataDesc == relatedDataDesc) &&
                 (relatedDataPrimaryId == null || relatedDataPrimaryId == "" ? true : u.relatedDataPrimaryId == relatedDataPrimaryId) &&
                   (relatedDataPrimaryDesc == null || relatedDataPrimaryDesc == "" ? true : u.relatedDataPrimaryDesc == relatedDataPrimaryDesc) &&
                   u.currentStateId == stateId && u.createdUserId == userId);
                    result.content = query
                      .Skip(pageSize * pageNumber).Take(pageSize)

                        .Select(u => new ActiveProcessDto()
                        {
                            approvalProcess = u.approvalProcess,
                            belgeTutari = u.belgeTutari,
                            createdDate = u.createdDate,
                            createdUserId = u.createdUserId,
                            currentState = u.currentState,
                            currentUser = u.currentUser,
                            customFields = u.customFields,
                            dagitimKanali = u.dagitimKanali,
                            dataType = u.dataType,
                            description = u.description,
                            disaprovecondition = u.disaprovecondition,
                            enabled = u.enabled,
                            id = u.Id,
                            newValue = u.newValue,
                            oldValue = u.oldValue,
                            //listActiveProcessDetail = [],
                            oncekiArtirim = u.oncekiArtirim,
                            relatedColumn = u.relatedColumn,
                            relatedData = u.relatedData,
                            relatedDataDesc = u.relatedDataDesc,
                            relatedDataId = u.relatedDataId,
                            relatedDataPrimary = u.relatedDataPrimary,
                            relatedDataPrimaryDesc = u.relatedDataPrimaryDesc,
                            relatedDataPrimaryId = u.relatedDataPrimaryId,
                            userVekalet = u.userVekalet,


                        }).ToList();
                    result.totalElements = query.Count();
                    result.number = result.content.Count();
                    result.size = pageSize;

                    return result;


                }


            }

            public object? mylist(FilterPageParam<ActiveProsessMyListDtoParameter> filterPageParam)
            {

                PageReturn<ActiveProcessDto> result = new PageReturn<ActiveProcessDto>();

                int pageSize = filterPageParam.size ?? 20;
                int pageNumber = filterPageParam.page ?? 0;

                string? relatedDataId = filterPageParam.liste?.relatedDataId;
                string? relatedDataDesc = filterPageParam.liste?.relatedDataDesc;
                string? relatedDataPrimaryId = filterPageParam.liste?.relatedDataPrimaryId;
                string? relatedDataPrimaryDesc = filterPageParam.liste?.relatedDataPrimaryDesc;

                int stateId = int.TryParse(filterPageParam?.liste?.stateId.ToString(), out int tempStateId) ? tempStateId : 0;

                string typeString = filterPageParam?.liste?.type?
                    .ToString()
                    .Replace("[", "")
                    .Replace("]", "")
                    .Replace(" ", "") ?? "";

                string[] type = typeString.Split(",");
                HashSet<int> typeIntegers = type.Select(int.Parse).ToHashSet();

                int.TryParse(filterPageParam?.liste?.userId.ToString(), out int userId);

                IQueryable<ActiveProcess> query = dal.Get(u =>
                    u.enabled &&
                    u.currentUserId == userId &&
                    u.currentStateId == stateId &&
                    typeIntegers.Contains(u.approvalProcess.typeId) &&

                    (string.IsNullOrEmpty(relatedDataId)
                        ? u.relatedDataId == null
                        : u.relatedDataId.Contains(relatedDataId)) &&

                    (string.IsNullOrEmpty(relatedDataDesc)
                        ? u.relatedDataDesc == null
                        : u.relatedDataDesc.Contains(relatedDataDesc)) &&

                    (string.IsNullOrEmpty(relatedDataPrimaryId)
                        ? u.relatedDataPrimaryId == null
                        : u.relatedDataPrimaryId.Contains(relatedDataPrimaryId)) &&

                    (string.IsNullOrEmpty(relatedDataPrimaryDesc)
                        ? u.relatedDataPrimaryDesc == null
                        : u.relatedDataPrimaryDesc.Contains(relatedDataPrimaryDesc))
                );

                result.content = query
                    .Skip(pageNumber * pageSize)
                    .Take(pageSize)
                    .Select(u => new ActiveProcessDto
                    {
                        id = u.Id,
                        enabled = u.enabled,
                        currentState = u.currentState,
                        currentUser = u.currentUser,
                        userVekalet = u.userVekalet,
                        approvalProcess = u.approvalProcess,

                        dagitimKanali = u.dagitimKanali,
                        relatedData = u.relatedData,
                        relatedDataId = u.relatedDataId,
                        relatedDataDesc = u.relatedDataDesc,

                        relatedDataPrimary = u.relatedDataPrimary,
                        relatedDataPrimaryId = u.relatedDataPrimaryId,
                        relatedDataPrimaryDesc = u.relatedDataPrimaryDesc,

                        relatedColumn = u.relatedColumn,
                        dataType = u.dataType,
                        oldValue = u.oldValue,
                        newValue = u.newValue,
                        description = u.description,
                        customFields = u.customFields,
                        disaprovecondition = u.disaprovecondition,
                        oncekiArtirim = u.oncekiArtirim,

                        createdDate = u.createdDate,
                        createdUserId = u.createdUserId,

                        belgeTutari = u.belgeTutari,

                        avgDays = u.avg_days,
                        avgVade = u.avg_vade
                    })
                    .ToList();

                result.totalElements = query.Count();
                result.number = result.content.Count;
                result.size = pageSize;

                return result;
            }

            public string approved(string guid, AdminUser? user)
            {
                throw new NotImplementedException();
            }

            public string reject(string guid, AdminUser? user)
            {
                throw new NotImplementedException();
            }

            public string setCustomerSanalLimit(string kunnr, double dmbtr, string yeniMusteriMi, string nameString)
            {
                throw new NotImplementedException();
            }

            public AvgVadeDaysDto getAvgVadeDays(string kunnr)
            {
                throw new NotImplementedException();
            }
        }

    }
}