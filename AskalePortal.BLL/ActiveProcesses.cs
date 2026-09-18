using AskalePortal.Constants;
using AskalePortal.Data.Models;
using AskalePortal.Data.RequestModel;
using AskalePortal.Data.RequestParams;
using AskalePortal.Data.ResponseModels;
using AskalePortal.Data.ResponseParams;
using AskalePortal.Data.SAP.InputParams;
using AskalePortal.Data.SAP.OutputParams;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Utilities;
using SapNwRfc;
using System.Globalization;
using System.Linq.Dynamic.Core;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
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

            private static IQueryable<ActiveProcess> ApplySorting(
                IQueryable<ActiveProcess> query,
                IEnumerable<SortingModel>? sorting)
            {
                List<string> clauses = new();

                foreach (SortingModel item in sorting ?? Enumerable.Empty<SortingModel>())
                {
                    if (string.IsNullOrWhiteSpace(item.key))
                    {
                        continue;
                    }

                    string? propertyPath = ResolvePropertyPath(typeof(ActiveProcess), item.key);
                    if (propertyPath == null)
                    {
                        continue;
                    }

                    string direction = item.sorting == "sorting_desc" ? "descending" :
                        item.sorting == "sorting_asc" ? "ascending" : "";
                    if (direction.Length > 0)
                    {
                        clauses.Add($"{propertyPath} {direction}");
                    }
                }

                return clauses.Count == 0 ? query : query.OrderBy(string.Join(", ", clauses));
            }

            private static string? ResolvePropertyPath(Type rootType, string requestedPath)
            {
                Type currentType = rootType;
                List<string> resolvedParts = new();

                foreach (string part in requestedPath.Split('.', StringSplitOptions.RemoveEmptyEntries))
                {
                    PropertyInfo? property = currentType.GetProperty(
                        part,
                        BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
                    if (property == null)
                    {
                        return null;
                    }

                    resolvedParts.Add(property.Name);
                    currentType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                }

                return resolvedParts.Count == 0 ? null : string.Join('.', resolvedParts);
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
                            activeProcess.newValue = amount.ToString(CultureInfo.InvariantCulture);
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

                            string mailMessage = bllEmailReaderFile.buildCredit(_configuration, _env, "Müşteri Kredi Limiti Değişikliği", firstUser.name, kunnr, name1,
                                    klimk, amount.ToString(CultureInfo.InvariantCulture), description, ok_link, no_link);
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
                                .findByActiveProcessIdAndUserIdAndApprovedAndEnabled(activeProcess.Id, userId, null, true) ?? throw new InvalidOperationException("Bekleyen onay detayı bulunamadı.");

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
                                degerZterm = Convert.ToDouble(customerDocumentDto?.ZTERM?.Substring(1), CultureInfo.InvariantCulture);
                            }

                            double onceki = (int)double.Parse(customerDocumentDto!.ZBD1T!, CultureInfo.InvariantCulture) - degerZterm;
                            double newValue = onceki + Convert.ToDouble(activeProcess.newValue, CultureInfo.InvariantCulture);
                            int deger;
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
                            ApprovalProcessDetail? approvalProcessDetailLast = bllApprovalProcessDetails
                                    .findByProcessIdAndDataOrderAndEnabled(approvalProcessDetail.processId, deger, true) ?? throw new InvalidOperationException("Onay sırası bulunamadı.");
                            if (approvalProcessDetailLast != null)
                            {
                                BLLActions.AdminUsers bllAdminUsers = new BLLActions.AdminUsers(_configuration, _env, _mapper);
                                AdminUser? userLast = bllAdminUsers.GetByID(approvalProcessDetailLast.userId);
                                if (userLast?.Id != userId)
                                {

                                    ApprovalProcessDetail? approvalProcessDetailNext = bllApprovalProcessDetails
                                            .findByProcessIdAndDataOrderAndEnabled(approvalProcessDetail.processId,
                                                    approvalProcessDetail.dataOrder + 1, true) ?? throw new InvalidOperationException("Onay sırası bulunamadı.");
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
                            if (Convert.ToDouble(activeProcess.newValue, CultureInfo.InvariantCulture) <= 150000)
                            {
                                deger = 1;
                            }
                            else if (Convert.ToDouble(activeProcess.newValue, CultureInfo.InvariantCulture) <= 500000)
                            {
                                deger = 2;
                            }

                            ApprovalProcessDetail? approvalProcessDetailLast = bllApprovalProcessDetails
                                    .findByProcessIdAndDataOrderAndEnabled(approvalProcessDetail.processId, deger, true) ?? throw new InvalidOperationException("Onay sırası bulunamadı.");
                            if (approvalProcessDetailLast != null)
                            {
                                BLLActions.AdminUsers bllAdminUsers = new BLLActions.AdminUsers(_configuration, _env, _mapper);
                                AdminUser userLast = bllAdminUsers.GetByID(approvalProcessDetailLast.userId)!;
                                if (userLast.Id != userId)
                                {

                                    ApprovalProcessDetail? approvalProcessDetailNext = bllApprovalProcessDetails
                                            .findByProcessIdAndDataOrderAndEnabled(approvalProcessDetail.processId,
                                                    approvalProcessDetail.dataOrder + 1, true) ?? throw new InvalidOperationException("Onay sırası bulunamadı.");
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
                            Data.SAP.Models.CustomerCreditList? q2 = bllCustomers.getCustomerCredit(activeProcess.relatedDataId) ?? throw new InvalidOperationException("SAP kredi bilgisi bulunamadı.");
                            int deger;
                            if (q2 != null)
                            {


                                if (Convert.ToDouble(activeProcess.newValue, CultureInfo.InvariantCulture) + double.Parse(q2.SNLMT!, CultureInfo.InvariantCulture) <= 1500000)
                                {
                                    deger = 1;
                                }
                                else if (Convert.ToDouble(activeProcess.newValue, CultureInfo.InvariantCulture) + double.Parse(q2.SNLMT!, CultureInfo.InvariantCulture) <= 3500000)
                                {
                                    deger = 2;
                                }
                                else if (Convert.ToDouble(activeProcess.newValue, CultureInfo.InvariantCulture) + double.Parse(q2.SNLMT!, CultureInfo.InvariantCulture) <= 7000000)
                                {
                                    deger = 3;
                                }
                                else
                                {
                                    deger = 4;
                                }
                                ApprovalProcessDetail? approvalProcessDetailLast = bllApprovalProcessDetails
                                        .findByProcessIdAndDataOrderAndEnabled(approvalProcessDetail.processId, deger, true) ?? throw new InvalidOperationException("Onay sırası bulunamadı.");
                                if (approvalProcessDetailLast != null)
                                {
                                    BLLActions.AdminUsers bllAdminUsers = new BLLActions.AdminUsers(_configuration, _env, _mapper);
                                    AdminUser userLast = bllAdminUsers.GetByID(approvalProcessDetailLast.userId)!;
                                    if (userLast.Id != userId)
                                    {
                                        ApprovalProcessDetail? approvalProcessDetailNext = bllApprovalProcessDetails
                                                .findByProcessIdAndDataOrderAndEnabled(approvalProcessDetail.processId,
                                                        approvalProcessDetail.dataOrder + 1, true) ?? throw new InvalidOperationException("Onay sırası bulunamadı.");
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
                                            Convert.ToDouble(activeProcess.newValue, CultureInfo.InvariantCulture));
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
                                .findByActiveProcessIdAndUserIdAndApprovedAndEnabled(activeProcess.Id, userId, null, true) ?? throw new InvalidOperationException("Bekleyen onay detayı bulunamadı.");
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
                                .findByActiveProcessIdAndUserIdAndApprovedAndEnabled(activeProcess.Id, userId, null, true) ?? throw new InvalidOperationException("Bekleyen onay detayı bulunamadı.");
                        BLLActions.ApprovalProcessDetails bllApprovalProcessDetails = new BLLActions.ApprovalProcessDetails(_configuration, _env, _mapper);
                        ApprovalProcessDetail? approvalProcessDetail = bllApprovalProcessDetails
                                .findByProcessIdAndUserIdAndEnabled(activeProcess.approvalProcessId, userId, true);
                        AdminUser? nextUser = null;
                        if (approvalProcessDetail == null || activeProcessDetail == null)
                        {
                            throw new InvalidOperationException("Onay kaydı bulunamadı.");
                        }
                        BLLActions.ActiveProcessVekalet bllActiveProcessVekalet =
                            new BLLActions.ActiveProcessVekalet(_configuration, _env);
                        Data.Models.ActiveProcessVekalet? activeProcessVekalet =
                            bllActiveProcessVekalet.GetByAlanUserId(approvalProcessDetail.userId);

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
                                        fark = (int)double.Parse(customerDocumentDto!.ZBD1T!, CultureInfo.InvariantCulture);

                                    }
                                    else
                                    {
                                        int ZBD1T = (int)double.Parse(customerDocumentDto.ZBD1T!, CultureInfo.InvariantCulture);
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
                                activeProcess.enabled = false;
                                await Update(activeProcess);

                                var detailToDisable = await bllActiveProcessDetails.dal.dB.Set<ActiveProcessDetail>()
                                    .SingleOrDefaultAsync(d => d.Id == approvalProcessDetail.Id);
                                // Intentional Java compatibility: use approvalProcessDetail.Id.
                                if (detailToDisable != null)
                                {
                                    detailToDisable.enabled = false;
                                    await bllActiveProcessDetails.Update(detailToDisable);
                                }
                                BLLActions.AdminUsers bllAdminUsers = new BLLActions.AdminUsers(_configuration, _env, _mapper);
                                AdminUser? createdUser = bllAdminUsers.GetByID(activeProcess.createdUserId);
                                if (createdUser != null)
                                {
                                    await sendErrorEmail(activeProcess, createdUser, createdUser.Id);
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
                                d.vekaletId = activeProcessVekalet.VekaletAlanId;
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

                            await sendEmail(activeProcess, nextUser, userId);

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
                                await sendFinishedEmail(activeProcess, user, userId);

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
                        await sendDeclinedEmail(activeProcess, user, userId);
                        await Update(activeProcess);

                        BLLActions.ActiveProcessDetails bllActiveProcessDetails = new BLLActions.ActiveProcessDetails(_configuration, _env);
                        ActiveProcessDetail? detail = bllActiveProcessDetails
                                .findByActiveProcessIdAndUserIdAndApprovedAndEnabled(activeProcess.Id, userId, null, true) ?? throw new InvalidOperationException("Bekleyen onay detayı bulunamadı.");
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
            public async Task<bool> changeAllDateWithCheck(bool approved, List<int> listInt, int userId)
            {
                List<ActiveProcess> activeProcesses =
                    dal.Get(u => listInt.Contains(u.Id) && u.enabled).ToList();

                if (approved)
                {
                    foreach (ActiveProcess activeProcess in activeProcesses)
                    {
                        BLLActions.ActiveProcessDetails detailService =
                            new BLLActions.ActiveProcessDetails(_configuration, _env);
                        ActiveProcessDetail activeDetail = detailService
                            .findByActiveProcessIdAndUserIdAndApprovedAndEnabled(
                                activeProcess.Id, userId, null, true)
                            ?? throw new InvalidOperationException(
                                $"{activeProcess.Id} süreci için bekleyen onay detayı bulunamadı.");

                        BLLActions.ApprovalProcessDetails approvalDetailService =
                            new BLLActions.ApprovalProcessDetails(_configuration, _env, _mapper);
                        ApprovalProcessDetail approvalDetail = approvalDetailService
                            .findByProcessIdAndUserIdAndEnabled(
                                activeProcess.approvalProcessId, userId, true)
                            ?? throw new InvalidOperationException(
                                $"{activeProcess.approvalProcessId} süreci için kullanıcı onay sırası bulunamadı.");

                        BLLActions.ActiveProcessVekalet vekaletService =
                            new BLLActions.ActiveProcessVekalet(_configuration, _env);
                        Data.Models.ActiveProcessVekalet? vekalet =
                            vekaletService.GetByAlanUserId(approvalDetail.userId);

                        activeProcess.oncekiArtirim = 0;
                        double newValue = Convert.ToDouble(activeProcess.newValue, CultureInfo.InvariantCulture);
                        int lastOrder = newValue <= 10 ? 1
                            : newValue <= 15 ? 2
                            : newValue <= 20 ? 3
                            : 4;

                        ApprovalProcessDetail lastApproval = approvalDetailService
                            .findByProcessIdAndDataOrderAndEnabled(
                                activeProcess.approvalProcessId, lastOrder, true)
                            ?? throw new InvalidOperationException($"{lastOrder}. onay sırası bulunamadı.");

                        AdminUser? nextUser = userId == lastApproval.userId
                            ? null
                            : approvalDetailService.GetNextUser(
                                userId, activeProcess.approvalProcessId, true);

                        if (nextUser != null)
                        {
                            activeDetail.approved = true;
                            activeDetail.replyDate = DateTime.Now;
                            activeDetail.isReplied = true;
                            activeDetail.updatedDate = DateTime.Now;
                            activeDetail.updatedUserId = userId;
                            await detailService.Update(activeDetail);

                            await detailService.Add(new ActiveProcessDetail
                            {
                                vekaletId = vekalet?.VekaletAlanId,
                                activeProcessId = activeDetail.activeProcessId,
                                approved = null,
                                createdDate = activeDetail.replyDate ?? DateTime.Now,
                                createdUserId = userId,
                                description = "",
                                isReplied = false,
                                guid = Guid.NewGuid(),
                                replyDate = null,
                                enabled = true,
                                userId = nextUser.Id
                            });

                            activeProcess.currentUserId = nextUser.Id;
                            await sendEmail(activeProcess, nextUser, userId);
                        }
                        else
                        {
                            BLLActions.ActiveProcessInvoices invoiceService =
                                new BLLActions.ActiveProcessInvoices(_configuration, _env, _mapper);
                            BLLActions.ActiveProcessChecks checkService =
                                new BLLActions.ActiveProcessChecks(_configuration, _env, _mapper);

                            foreach (FaturaGunFarkDto invoiceDate in listGunHesaplama(
                                invoiceService.getByActiveProcessId(activeProcess.Id),
                                checkService.getByActiveProcessId(activeProcess.Id)))
                            {
                                string replyText = changeVadeSap(
                                    invoiceDate.BUKRS ?? "",
                                    invoiceDate.BELNR ?? "",
                                    (invoiceDate.GJAHR ?? 0).ToString(),
                                    invoiceDate.gunFarki ?? 0);
                                activeProcess.currentStateId = replyText == "OK"
                                    ? (int)CommonConstants.PROCESS_STATES.SAP_COMPLETED
                                    : (int)CommonConstants.PROCESS_STATES.COMPLETED;
                            }

                            BLLActions.AdminUsers userService =
                                new BLLActions.AdminUsers(_configuration, _env, _mapper);
                            AdminUser? createdUser = userService.GetByID(activeProcess.createdUserId);
                            if (createdUser != null)
                            {
                                await sendFinishedEmail(activeProcess, createdUser, userId);
                            }

                            activeDetail.approved = true;
                            activeDetail.replyDate = DateTime.Now;
                            activeDetail.isReplied = true;
                            activeDetail.updatedDate = DateTime.Now;
                            activeDetail.updatedUserId = userId;
                            await detailService.Update(activeDetail);
                        }

                        await Update(activeProcess);
                    }

                    return true;
                }

                foreach (ActiveProcess activeProcess in activeProcesses)
                {
                    activeProcess.currentStateId = (int)CommonConstants.PROCESS_STATES.DECLINED;
                    await Update(activeProcess);

                    BLLActions.AdminUsers userService =
                        new BLLActions.AdminUsers(_configuration, _env, _mapper);
                    AdminUser? createdUser = userService.GetByID(activeProcess.createdUserId);
                    if (createdUser != null)
                    {
                        await sendDeclinedEmail(activeProcess, createdUser, userId);
                    }

                    BLLActions.ActiveProcessDetails detailService =
                        new BLLActions.ActiveProcessDetails(_configuration, _env);
                    ActiveProcessDetail detail = detailService
                        .findByActiveProcessIdAndUserIdAndApprovedAndEnabled(
                            activeProcess.Id, userId, null, true)
                        ?? throw new InvalidOperationException(
                            $"{activeProcess.Id} süreci için bekleyen onay detayı bulunamadı.");
                    detail.approved = false;
                    detail.isReplied = true;
                    detail.replyDate = DateTime.Now;
                    detail.updatedDate = DateTime.Now;
                    detail.updatedUserId = userId;
                    await detailService.Update(detail);
                }

                return false;
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









            private async Task sendErrorEmail(ActiveProcess lst, AdminUser? user, int? userId)
            {
                EmailMessage email = new EmailMessage();

                email.subject = "Talep Silindi";
                email.toAddress = user?.email;

                BLLActions.EmailReaderFile bllEmailReaderFile = new BLLActions.EmailReaderFile();
                email.emailText = bllEmailReaderFile.CreateMailString(_configuration, _env, "Müşteri Belge Vadesi Değişikliği", "Sayın <strong>" + user?.name
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

            private async Task sendDeclinedEmail(ActiveProcess lst, AdminUser? user, int userId)
            {
                EmailMessage email = new EmailMessage();

                email.subject = "Talep REDDEDİLDİ";
                email.toAddress = user?.email;
                BLLActions.EmailReaderFile bllEmailReaderFile = new BLLActions.EmailReaderFile();

                email.emailText = bllEmailReaderFile.CreateMailString(_configuration, _env, "Müşteri Belge Vadesi Değişikliği", "Sayın <strong>" + user?.name
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


            private async Task sendFinishedEmail(ActiveProcess lst, AdminUser? user, int userId)
            {
                EmailMessage email = new EmailMessage();

                email.subject = "Talep ONAYLANDI";
                email.toAddress = user?.email;
                BLLActions.EmailReaderFile bllEmailReaderFile = new BLLActions.EmailReaderFile();

                email.emailText = bllEmailReaderFile.CreateMailString(_configuration, _env, "Müşteri Belge Vadesi Değişikliği", "Sayın <strong>" + user?.name
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


            private async Task sendEmail(ActiveProcess lst, AdminUser? user, int userId)
            {
                EmailMessage email = new EmailMessage();

                email.subject = "Müşteri Belge Vadesi Değişikliği";
                email.toAddress = user?.email;
                BLLActions.EmailReaderFile bllEmailReaderFile = new BLLActions.EmailReaderFile();

                email.emailText = bllEmailReaderFile.CreateMailString(_configuration, _env, "Müşteri Belge Vadesi Değişikliği", "Sayın <strong>" + user?.name
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

            public async Task<ActiveProcessSaveDto> changedate(string bukrs, int gjahr, string name1, string kunnr, string faedt, string belnr, string zfbdt, string dagitimKanali, int newValue, string? description, int userId, string belgeTutari)
            {
                try
                {
                    string ok_link = OkNoLinks.OK_LINK;
                    string no_link = OkNoLinks.NO_LINK;
                    BLLActions.AdminUsers bllAdminUsers = new BLLActions.AdminUsers(_configuration, _env, _mapper);
                    AdminUser? user = bllAdminUsers.GetByID(userId);
                    if (user == null)
                    {
                        return null!;
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
                        return null!;
                    }
                    BLLActions.ApprovalProcessDetails bllApprovalProcessDetails = new BLLActions.ApprovalProcessDetails(_configuration, _env, _mapper);
                    ApprovalProcessDetail? approvalProcessDetail = bllApprovalProcessDetails
                            .findByProcessIdAndDataOrderAndEnabled(approvalProcess.Id, 1, true);
                    if (approvalProcessDetail == null)
                    {
                        return null!;
                    }
                    AdminUser? firstUser = bllAdminUsers.GetByID(approvalProcessDetail.userId);
                    if (firstUser == null)
                    {
                        return null!;
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
                    activeProcess.oldValue = belnr == "Çek ile Vade" ? "0" : faedt;
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

                    BLLActions.ActiveProcessDetails bllActiveProcessDetails = new BLLActions.ActiveProcessDetails(_configuration, _env);
                    await bllActiveProcessDetails.Add(activeProcessDetail);

                    EmailMessage emailMessage = new EmailMessage();
                    emailMessage.subject = "Müşteri Vade Gün Değişikliği";
                    emailMessage.toAddress = firstUser.email;

                    BLLActions.EmailReaderFile bllEmailReaderFile = new BLLActions.EmailReaderFile();

                    string mailMessageString = bllEmailReaderFile.buildVade(_configuration, _env, "Müşteri Vade Gün Değişikliği", firstUser.name, kunnr, name1,
                            belnr, zfbdt, newValue.ToString(), description ?? "", ok_link, no_link);
                    emailMessage.enabled = true;
                    emailMessage.emailText = mailMessageString;
                    emailMessage.isSent = false;
                    emailMessage.mailTuru = 2;
                    emailMessage.createdDate = DateTime.Now;
                    emailMessage.createdUserId = userId;
                    emailMessage.plannedDate = DateTime.Now;
                    BLLActions.EmailMessages bllEmailMessages = new BLLActions.EmailMessages(_configuration, _env);
                    await bllEmailMessages.Add(emailMessage);

                    return activeProcessSave == null ? null! : new ActiveProcessSaveDto
                    {
                        id = activeProcessSave.Id,
                        enabled = activeProcessSave.enabled,
                        createdUserId = activeProcessSave.createdUserId,
                        createdDate = activeProcessSave.createdDate.ToString("yyyy-MM-ddTHH:mm:ss.fffffff", CultureInfo.InvariantCulture),
                        updateDate = activeProcessSave.updatedDate?.ToString("yyyy-MM-ddTHH:mm:ss.fffffff", CultureInfo.InvariantCulture),
                        updatedUserId = activeProcessSave.updatedUserId,
                        approvalProcessId = activeProcessSave.approvalProcessId,
                        currentStateId = activeProcessSave.currentStateId,
                        currentUserId = activeProcessSave.currentUserId,
                        userVekaletId = activeProcessSave.userVekaletId,
                        dagitimKanali = activeProcessSave.dagitimKanali,
                        relatedData = activeProcessSave.relatedData,
                        relatedDataId = activeProcessSave.relatedDataId,
                        relatedDataDesc = activeProcessSave.relatedDataDesc,
                        relatedDataPrimary = activeProcessSave.relatedDataPrimary,
                        relatedDataPrimaryId = activeProcessSave.relatedDataPrimaryId,
                        relatedDataPrimaryDesc = activeProcessSave.relatedDataPrimaryDesc,
                        relatedColumn = activeProcessSave.relatedColumn,
                        dataType = activeProcessSave.dataType,
                        oldValue = activeProcessSave.oldValue,
                        newValue = activeProcessSave.newValue,
                        description = activeProcessSave.description,
                        customFields = activeProcessSave.customFields,
                        disaprovecondition = activeProcessSave.disaprovecondition,
                        oncekiArtirim = activeProcessSave.oncekiArtirim,
                        belgeTutari = activeProcessSave.belgeTutari,
                        avgDays = activeProcessSave.avg_days,
                        avgVade = activeProcessSave.avg_vade
                    };
                }
                catch
                {

                    return null!;
                }
            }

            public PageReturn<ActiveProcessDto> listFilterByStateIdAndTypeId(
        FilterPageParam<ActiveProcessListParameter> filterPageParam)
            {
                PageReturn<ActiveProcessDto> result =
                    new PageReturn<ActiveProcessDto>();

                int pageSize = filterPageParam.size ?? 20;
                int pageNumber = filterPageParam.page ?? 0;

                if (pageSize <= 0)
                    pageSize = 20;

                if (pageNumber < 0)
                    pageNumber = 0;

                string relatedDataId =
                    filterPageParam.liste?.relatedDataId ?? "";

                string relatedDataDesc =
                    filterPageParam.liste?.relatedDataDesc ?? "";

                string relatedDataPrimaryId =
                    filterPageParam.liste?.relatedDataPrimaryId ?? "";

                string relatedDataPrimaryDesc =
                    filterPageParam.liste?.relatedDataPrimaryDesc ?? "";

                int stateId = filterPageParam.liste?.stateId ?? 0;
                int userId = filterPageParam.liste?.userId ?? 0;

                List<int> typeIds =
                    filterPageParam.liste?.type ?? new List<int>();

                // USER
                BLLActions.AdminUsers bllAdminUsers =
                    new BLLActions.AdminUsers(
                        _configuration,
                        _env,
                        _mapper);

                AdminUser? user = bllAdminUsers.GetByID(userId);

                // ROLE
                BLLActions.RoleDetails bllRoleDetails =
                    new BLLActions.RoleDetails(
                        _configuration,
                        _env,
                        _mapper);

                RoleDetail? roleDetail =
                    bllRoleDetails.GetByRoleIDAndModuleID(
                        user?.roleId ?? 0,
                        (int)CommonConstants.MODULES.CUSTOMER_CREDITS);

                // BASE LIST
                IQueryable<ActiveProcess> query = dal.Get(u =>
                    u.enabled == true &&
                    typeIds.Contains(u.approvalProcess.typeId) &&
                    u.currentStateId == stateId
                );

                // OPSİYONEL FİLTRELER
                if (!string.IsNullOrEmpty(relatedDataId))
                {
                    query = query
                        .Where(u =>
                            u.relatedDataId != null &&
                            u.relatedDataId.Contains(relatedDataId))  ;
                }

                if (!string.IsNullOrEmpty(relatedDataDesc))
                {
                    query = query
                        .Where(u =>
                            u.relatedDataDesc != null &&
                            u.relatedDataDesc.Contains(relatedDataDesc));
                }

                if (!string.IsNullOrEmpty(relatedDataPrimaryId))
                {
                    query = query
                        .Where(u =>
                            u.relatedDataPrimaryId != null &&
                            u.relatedDataPrimaryId.Contains(relatedDataPrimaryId));
                }

                if (!string.IsNullOrEmpty(relatedDataPrimaryDesc))
                {
                    query = query
                        .Where(u =>
                            u.relatedDataPrimaryDesc != null &&
                            u.relatedDataPrimaryDesc.Contains(relatedDataPrimaryDesc));
                }

                // YETKİ
                bool canSeeAll =
                    user?.roleId == 1 ||
                    (roleDetail != null && roleDetail.canSee);

                if (!canSeeAll)
                {
                    query = query
                        .Where(u => u.createdUserId == userId);
                }

                // SORT
                query = ApplySorting(
                    query.AsQueryable(),
                    filterPageParam.sorting
                );

                // TOTAL
                result.totalElements = query.Count();

                // CONTENT
                result.content = query
                    .Skip(pageSize * pageNumber)
                    .Take(pageSize)
                    .Select(u => new ActiveProcessDto
                    {
                        id = u.Id,
                        enabled = u.enabled,
                        approvalProcess = u.approvalProcess,
                        currentState = u.currentState,
                        currentUser = u.currentUser,
                        userVekalet = u.userVekalet,
                        belgeTutari = u.belgeTutari,
                        createdDate = u.createdDate.ToString("dd.MM.yyyy"),
                        createdUserId = u.createdUserId,
                        customFields = u.customFields,
                        dagitimKanali = u.dagitimKanali,
                        dataType = u.dataType,
                        description = u.description,
                        disaprovecondition = u.disaprovecondition,
                        newValue = u.newValue,
                        oldValue = u.oldValue,
                        oncekiArtirim = u.oncekiArtirim,
                        relatedColumn = u.relatedColumn,
                        relatedData = u.relatedData,
                        relatedDataId = u.relatedDataId,
                        relatedDataDesc = u.relatedDataDesc,
                        relatedDataPrimary = u.relatedDataPrimary,
                        relatedDataPrimaryId = u.relatedDataPrimaryId,
                        relatedDataPrimaryDesc = u.relatedDataPrimaryDesc,
                        avgDays = u.avg_days,
                        avgVade = u.avg_vade
                    })
                    .ToList();

                // ACTIVE PROCESS DETAILS
                List<int> listActiveProcessId = result.content
                    .Where(x => x.id.HasValue)
                    .Select(x => x.id.Value)
                    .Distinct()
                    .ToList();

                if (listActiveProcessId.Any())
                {
                    List<Data.Contracts.Detached.ActiveProcessDetailDto>
                        allActiveProcessDetails =
                            new List<Data.Contracts.Detached.ActiveProcessDetailDto>();

                    const int batchSize = 1000;

                    BLLActions.ActiveProcessDetails bllActiveProcessDetails =
                        new BLLActions.ActiveProcessDetails(
                            _configuration,
                            _env);

                    for (int i = 0; i < listActiveProcessId.Count; i += batchSize)
                    {
                        List<int> batch = listActiveProcessId
                            .Skip(i)
                            .Take(batchSize)
                            .ToList();

                        List<Data.Contracts.Detached.ActiveProcessDetailDto>
                            batchResult = bllActiveProcessDetails
                                .findAllByListActiveProcessIdAndEnabledDto(
                                    batch,
                                    true);

                        allActiveProcessDetails.AddRange(batchResult);
                    }

                    foreach (ActiveProcessDto activeProcessDto in result.content)
                    {
                        activeProcessDto.listActiveProcessDetail =
                            allActiveProcessDetails
                                .Where(detail =>
                                    detail.activeProcessId == activeProcessDto.id)
                                .ToList();
                    }
                }

                // PAGINATION
                result.number = pageNumber;
                result.size = pageSize;

                result.NormalizePagination(pageNumber, pageSize);

                return result;
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
                        ? true
                        : EF.Functions.Like(u.relatedDataId, "%" + relatedDataId + "%")) &&

                    (string.IsNullOrEmpty(relatedDataDesc)
                        ? true
                        : EF.Functions.Like(u.relatedDataDesc, "%" + relatedDataDesc + "%")) &&

                    (string.IsNullOrEmpty(relatedDataPrimaryId)
                        ? true
                        : EF.Functions.Like(u.relatedDataPrimaryId, "%" + relatedDataPrimaryId + "%")) &&

                    (string.IsNullOrEmpty(relatedDataPrimaryDesc)
                        ? true
                        : EF.Functions.Like(u.relatedDataPrimaryDesc, "%" + relatedDataPrimaryDesc + "%"))
                );

                query = ApplySorting(query, filterPageParam.sorting);

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

                        createdDate = u.createdDate.ToString("dd.MM.yyyy"),
                        createdUserId = u.createdUserId,

                        belgeTutari = u.belgeTutari,

                        avgDays = u.avg_days,
                        avgVade = u.avg_vade
                    })
                    .ToList();

                result.totalElements = query.Count();
                result.number = pageNumber;
                result.size = pageSize;
                result.NormalizePagination(pageNumber, pageSize);

                return result;
            }

            public string approved(string guid, AdminUser? user)
            {
                return approvedInternal(guid, user).GetAwaiter().GetResult();
            }

            private async Task<string> approvedInternal(string guid, AdminUser? user)
            {
                if (user == null)
                {
                    return "2";
                }

                await using var transaction = await dal.dB.Database.BeginTransactionAsync();
                try
                {
                    Guid uuid = Guid.Parse(guid);
                    ActiveProcessDetail? activeProcessDetail = await dal.dB.Set<ActiveProcessDetail>()
                        .FirstOrDefaultAsync(u => u.guid == uuid && u.approved == null && u.enabled);

                    if (activeProcessDetail == null)
                    {
                        await transaction.RollbackAsync();
                        return "1";
                    }

                    ActiveProcess activeProcess = await dal.dB.Set<ActiveProcess>()
                        .FirstAsync(u => u.Id == activeProcessDetail.activeProcessId);

                    ApprovalProcessDetail? approvalProcessDetail = await dal.dB.Set<ApprovalProcessDetail>()
                        .FirstOrDefaultAsync(u => u.processId == activeProcess.approvalProcessId
                            && u.userId == user.Id && u.enabled);

                    if (approvalProcessDetail == null)
                    {
                        await transaction.RollbackAsync();
                        return "2";
                    }



                    AdminUser? nextUser = null;
                    int lastApprovalOrder = 0;

                    if (activeProcess.relatedColumn == "Vade Tarihi")
                    {
                        BLLActions.CustomerDocumentSap bllCustomerDocumentSap =
                            new BLLActions.CustomerDocumentSap(_configuration, _env, _mapper);
                        CustomerDocumentDto customerDocumentDto = bllCustomerDocumentSap
                            .getCustomerDocument(activeProcess.relatedDataPrimaryId)
                            .First(u => u.BELNR == activeProcess.relatedDataId);

                        double zterm = string.IsNullOrEmpty(customerDocumentDto.ZTERM)
                            ? 0.0
                            : Convert.ToDouble(customerDocumentDto.ZTERM.Substring(1), CultureInfo.InvariantCulture);
                        double previousValue = Convert.ToDouble(customerDocumentDto.ZBD1T, CultureInfo.InvariantCulture) - zterm;
                        double newValue = previousValue + Convert.ToDouble(activeProcess.newValue, CultureInfo.InvariantCulture);

                        lastApprovalOrder = newValue <= 10 ? 1
                            : newValue <= 15 ? 2
                            : newValue <= 20 ? 3
                            : 4;
                    }
                    else if (activeProcess.relatedColumn == "Kredi Limiti")
                    {
                        double newValue = Convert.ToDouble(activeProcess.newValue, CultureInfo.InvariantCulture);
                        if (newValue > 500000)
                        {
                            await transaction.RollbackAsync();
                            return "5";
                        }

                        lastApprovalOrder = newValue <= 150000 ? 1 : 2;
                    }
                    else if (activeProcess.relatedColumn == "Haftalik Limit")
                    {
                        BLLActions.Customers bllCustomers = new BLLActions.Customers(_configuration, _env);
                        Data.SAP.Models.CustomerCreditList customerCredit =
                            bllCustomers.getCustomerCredit(activeProcess.relatedDataId)
                            ?? throw new InvalidOperationException("SAP müşteri kredi bilgisi bulunamadı.");

                        double totalLimit = Convert.ToDouble(activeProcess.newValue, CultureInfo.InvariantCulture)
                            + Convert.ToDouble(customerCredit.SNLMT, CultureInfo.InvariantCulture);
                        lastApprovalOrder = totalLimit <= 1500000 ? 1
                            : totalLimit <= 3500000 ? 2
                            : totalLimit <= 7000000 ? 3
                            : 4;
                    }

                    if (lastApprovalOrder != 0)
                    {
                        ApprovalProcessDetail lastApproval = await dal.dB.Set<ApprovalProcessDetail>()
                                                .FirstAsync(u => u.processId == approvalProcessDetail.processId
                                                    && u.dataOrder == lastApprovalOrder && u.enabled);
                        AdminUser lastUser = await dal.dB.Set<AdminUser>()
                            .FirstAsync(u => u.Id == lastApproval.userId);

                        if (lastUser.Id != user.Id)
                        {
                            ApprovalProcessDetail nextApproval = await dal.dB.Set<ApprovalProcessDetail>()
                                .FirstAsync(u => u.processId == approvalProcessDetail.processId
                                    && u.dataOrder == approvalProcessDetail.dataOrder + 1 && u.enabled);
                            nextUser = await dal.dB.Set<AdminUser>()
                                .FirstAsync(u => u.Id == nextApproval.userId);
                        }

                    }
                    activeProcessDetail.approved = true;
                    activeProcessDetail.isReplied = true;
                    activeProcessDetail.replyDate = DateTime.Now;
                    activeProcessDetail.updatedDate = DateTime.Now;
                    activeProcessDetail.updatedUserId = user.Id;

                    if (nextUser == null)
                    {
                        activeProcess.currentStateId = (int)CommonConstants.PROCESS_STATES.SAP_COMPLETED;

                        string sapResult = "";
                        try
                        {
                            if (activeProcess.relatedColumn == "Kredi Limiti"
                                || activeProcess.relatedColumn == "Haftalik Limit")
                            {
                                sapResult = changeCreditLimitSap(activeProcess.relatedDataId,
                                    Convert.ToDouble(activeProcess.newValue, CultureInfo.InvariantCulture));
                            }
                            else if (activeProcess.relatedColumn == "Vade Tarihi")
                            {
                                string[] documentParts = activeProcess.relatedDataDesc.Split('-');
                                if (documentParts.Length < 2)
                                {
                                    throw new InvalidOperationException(
                                        "Vade işlemi için relatedDataDesc 'BUKRS-GJAHR' biçiminde değil.");
                                }

                                sapResult = changeVadeSap(documentParts[0], activeProcess.relatedDataId,
                                    documentParts[1], int.Parse(activeProcess.newValue));
                            }
                        }
                        catch
                        {
                            sapResult = "ERROR";
                        }

                        if (sapResult == "ERROR")
                        {
                            await dal.dB.SaveChangesAsync();
                            await transaction.CommitAsync();
                            return "4";
                        }

                        AdminUser createdUser = await dal.dB.Set<AdminUser>()
                            .FirstAsync(u => u.Id == activeProcess.createdUserId);
                        dal.dB.Set<EmailMessage>().Add(
                            CreateActiveProcessEmail(activeProcess, createdUser, "Talep onaylandı"));

                        await dal.dB.SaveChangesAsync();
                        await transaction.CommitAsync();
                        return "3";
                    }

                    Data.Models.ActiveProcessVekalet? activeProcessVekalet =
                        await dal.dB.Set<Data.Models.ActiveProcessVekalet>()
                        .FirstOrDefaultAsync(u => u.VekaletAlanId == nextUser.Id && u.enabled);
                    dal.dB.Set<ActiveProcessDetail>().Add(new ActiveProcessDetail
                    {
                        userId = nextUser.Id,
                        enabled = true,
                        guid = Guid.NewGuid(),
                        createdDate = DateTime.Now,
                        createdUserId = user.Id,
                        vekaletId = activeProcessVekalet?.VekaletVerenId,
                        isReplied = false,
                        activeProcessId = activeProcess.Id
                    });

                    activeProcess.currentUserId = nextUser.Id;
                    string nextMailTitle = activeProcess.relatedColumn switch
                    {
                        "Vade Tarihi" => "Müşteri Vade Gün Değişikliği",
                        "Kredi Limiti" => "Müşteri Kredi Limiti Değişikliği",
                        _ => "Müşteri Haftalık Kredi Limiti Değişikliği"
                    };
                    dal.dB.Set<EmailMessage>().Add(
                        CreateActiveProcessEmail(activeProcess, nextUser, nextMailTitle));

                    await dal.dB.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return "4";
                }
                catch (Exception e)
                {
                    await transaction.RollbackAsync();
                    return e.ToString();
                }
            }

            public string reject(string guid, AdminUser? user)
            {
                return rejectInternal(guid, user).GetAwaiter().GetResult();
            }

            private async Task<string> rejectInternal(string guid, AdminUser? user)
            {
                if (user == null)
                {
                    return "2";
                }

                await using var transaction = await dal.dB.Database.BeginTransactionAsync();
                try
                {
                    Guid uuid = Guid.Parse(guid);
                    ActiveProcessDetail? activeProcessDetail = await dal.dB.Set<ActiveProcessDetail>()
                        .FirstOrDefaultAsync(u => u.guid == uuid && u.approved == null && u.enabled);

                    if (activeProcessDetail == null)
                    {
                        await transaction.RollbackAsync();
                        return "1";
                    }

                    ActiveProcess activeProcess = await dal.dB.Set<ActiveProcess>()
                        .FirstAsync(u => u.Id == activeProcessDetail.activeProcessId);


                    activeProcessDetail.isReplied = true;
                    activeProcessDetail.approved = false;
                    activeProcessDetail.replyDate = DateTime.Now;
                    activeProcessDetail.updatedDate = DateTime.Now;
                    activeProcessDetail.updatedUserId = user.Id;
                    activeProcess.currentStateId = (int)CommonConstants.PROCESS_STATES.DECLINED;

                    AdminUser createdUser = await dal.dB.Set<AdminUser>()
                        .FirstAsync(u => u.Id == activeProcess.createdUserId);
                    dal.dB.Set<EmailMessage>().Add(
                        CreateActiveProcessEmail(activeProcess, createdUser, "Talep Red edildi"));

                    await dal.dB.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return "4";
                }
                catch (Exception e)
                {
                    await transaction.RollbackAsync();
                    return e.ToString();
                }
            }

            private static void EnsureJavaApprovalTypeIsSupported(string relatedColumn)
            {
                if (relatedColumn != "Vade Tarihi"
                    && relatedColumn != "Kredi Limiti"
                    && relatedColumn != "Haftalik Limit")
                {
                    throw new NotSupportedException(
                        $"'{relatedColumn}' için approved/reject Java iş kuralı sağlanmadı.");
                }
            }

            private EmailMessage CreateActiveProcessEmail(
                ActiveProcess activeProcess, AdminUser recipient, string title)
            {
                string okLink = OkNoLinks.OK_LINK;
                string noLink = OkNoLinks.NO_LINK;
                EmailMessage emailMessage = new EmailMessage
                {
                    mailTuru = 2,
                    enabled = true,
                    isSent = false,
                    plannedDate = DateTime.Now,
                    createdDate = DateTime.Now,
                    toAddress = recipient.email
                };

                if (activeProcess.relatedColumn == "Vade Tarihi")
                {
                    DateTime oldDate = DateTime.Parse(activeProcess.oldValue);
                    emailMessage.subject = "Müşteri Vade Gün Değişikliği";
                    BLLActions.EmailReaderFile bllEmailReaderFile = new BLLActions.EmailReaderFile();

                    emailMessage.emailText = bllEmailReaderFile.buildVade(_configuration, _env, title, recipient.name,
                        activeProcess.relatedDataPrimaryId, activeProcess.relatedDataPrimaryDesc,
                        activeProcess.relatedDataId, oldDate.ToString("dd.MM.yyyy"),
                        activeProcess.newValue, activeProcess.description, okLink, noLink);
                }
                else
                {
                    emailMessage.subject = activeProcess.relatedColumn == "Kredi Limiti"
                        ? "Müşteri Kredi Limiti Değişikliği"
                        : "Müşteri Haftalık Kredi Limiti Değişikliği";
                    BLLActions.EmailReaderFile bllEmailReaderFile = new BLLActions.EmailReaderFile();

                    emailMessage.emailText = bllEmailReaderFile.buildCredit(_configuration, _env, title, recipient.name,
                        activeProcess.relatedDataId, activeProcess.relatedDataDesc,
                        activeProcess.oldValue, activeProcess.newValue,
                        activeProcess.description, okLink, noLink);
                }

                return emailMessage;
            }

            public string setCustomerSanalLimit(string kunnr, double dmbtr, string yeniMusteriMi, string nameString)
            {
                BLLActions.Customers bllCustomers = new BLLActions.Customers(_configuration, _env);
                return bllCustomers.SetCustomerSanal(
                    kunnr,
                    dmbtr.ToString(CultureInfo.InvariantCulture),
                    yeniMusteriMi,
                    nameString);
            }

            public AvgVadeDaysDto getAvgVadeDays(string kunnr)
            {
                AvgVadeDaysDto result = new AvgVadeDaysDto();
                BLLActions.SAPConnectionData bllSapConnection =
                    new BLLActions.SAPConnectionData(_configuration, _env);

                using SapConnection? sapConnection =
                    bllSapConnection.sapConnection(_configuration, _env);
                if (sapConnection == null)
                {
                    return result;
                }

                try
                {
                    sapConnection.Connect();
                    ISapFunction sapFunction = sapConnection.CreateFunction("ZWEBI071");
                    AvgVadeDaysSapOutput output = sapFunction.Invoke<AvgVadeDaysSapOutput>(
                        input: new AvgVadeDaysSapInput { kunnr = kunnr });
                    result.avgDays = output.avgDays;
                    result.avgVade = output.avgVade;
                }
                catch (Exception ex)
                {
                    LogError(ex);
                }
                finally
                {
                    sapConnection.Disconnect();
                }

                return result;
            }
        }

    }
}
