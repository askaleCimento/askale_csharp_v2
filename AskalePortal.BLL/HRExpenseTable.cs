using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AskalePortal.Constants;
using AskalePortal.Data.Models;
using AskalePortal.Data.RequestModel;
using AskalePortal.Data.RequestParams;
using AskalePortal.Data.ResponseModels;
using AskalePortal.Data.ResponseParams;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static AskalePortal.BLL.BLLActions;

namespace AskalePortal.BLL
{
    public partial class BLLActions
    {
        public class HRExpenseTable : BaseBLL<AskalePortal.Data.Models.HRExpenseTable>
        {
            private IConfiguration _configuration; private IWebHostEnvironment _env; private readonly IMapper _mapper;
            public HRExpenseTable(IConfiguration configuration, IWebHostEnvironment env, IMapper mapper) : base(configuration, env)
            {
                _configuration = configuration;
                _env = env;
                _mapper = mapper;
            }


            public int GetMaxtripId()
            {
                return GetAll().Count() == 0 ? 0 : GetAll().Max(u => u.tripId);
            }

            public List<AskalePortal.Data.Models.HRExpenseTable> GetByTrip(int tripId)
            {
                return dal.Get(u => u.tripId == tripId && u.enabled == true).ToList();
            }



            public List<AskalePortal.Data.Models.HRExpenseTable> GetUnapproved(int userId)
            {
                var q = dal.Get(u => (u.currentUserId == userId) && ((u.currentStateId == 1 && u.approval == null) || (u.currentStateId == 2 && u.approval == false)) && u.enabled == true && u.lastApproved == null).ToList();
                return q;
            }

            public List<AskalePortal.Data.Models.HRExpenseTable> GetUnapprovedByTripId(int userId, int tripId)
            {
                var q = dal.Get(u => (u.createdUserId == userId || u.currentUserId == userId) && u.tripId == tripId && ((u.currentStateId == 1 && u.approval == null) || (u.currentStateId == 2 && u.approval == false)) && u.enabled == true && u.lastApproved == null).ToList();
                return q;
            }

            public List<AskalePortal.Data.Models.HRExpenseTable> GetByTripAndDateAndExpenseType(int tripID, DateTime spendingTime, int expenseTypeId)
            {
                return dal.Get(u => u.enabled == true && u.tripId == tripID && u.spendingTime == spendingTime && u.expenseTypeId == expenseTypeId).ToList();

            }

            public List<AskalePortal.Data.Models.HRExpenseTable> GetAllByUser(int userId)
            {
                return dal.Get(u => u.trip.userId == userId && u.enabled == true).OrderByDescending(u => u.tripId).ToList();
            }



            public static string StripHTML(string input)
            {
                if (string.IsNullOrEmpty(input))
                    return "";
                return Regex.Replace(input, "(<([^>]+)>|&nbsp;)", string.Empty);
            }

            public List<AskalePortal.Data.Models.HRExpenseTable> GetByExpenseTypeAndTripIdByTotal(int expenseTypeId, int tripID)
            {
                return dal.Get(u => u.tripId == tripID && u.expenseTypeId == expenseTypeId && u.expenseType.harcamaBoyu == true && u.enabled == true).ToList();
            }
            public List<AskalePortal.Data.Models.HRExpenseTable> GetByExpenseTypeAndTripIdByDay(int expenseTypeId, int tripID)
            {
                return dal.Get(u => u.tripId == tripID && u.expenseTypeId == expenseTypeId && u.expenseType.toplamaNo == true && u.enabled == true).ToList();
            }

            public List<AskalePortal.Data.Models.HRExpenseTable> GetAllActiveByUser(int hrmanager, int userId)
            {
                return dal.Get(u => u.enabled == true && u.currentStateId == 1 && u.trip.userId == userId && u.currentUserId == hrmanager).ToList();

            }

            public int approvalCount(int userId)
            {
                int deger = dal.Get(u => u.enabled == true && u.currentUserId == userId && u.currentStateId == 1).GroupBy(k => k.tripId).Count();
                return deger;
            }

            public List<Data.Models.HRExpenseTable> findByUserIdActive(int? currentUserId, int tripUserId)
            {
                List<Data.Models.HRExpenseTable> liste = dal.Get(u => u.enabled && u.trip.userId == tripUserId && u.currentStateId == 1 && u.currentUserId == currentUserId).ToList();
                return liste;
            }

            public List<Data.Models.HRExpenseTable> listByTripId(int tripId)
            {
                List<Data.Models.HRExpenseTable>? liste = dal.Get(u => u.tripId == tripId && u.enabled && u.currentStateId != 2).ToList();
                return liste ?? [];
            }

            public async Task<Data.Models.HRExpenseTable?> save(HRExpenseTableSaveDto entity, int userId)
            {
                try
                {
                    BLLActions.AdminUsers bllAdminUsers = new BLLActions.AdminUsers(_configuration, _env, _mapper);
                    AdminUser user = bllAdminUsers.GetByID(userId)!;

                    BLLActions.HRExpenseTypeTable bllHRExpenseTypeTable = new BLLActions.HRExpenseTypeTable(_configuration, _env);
                    Data.Models.HRExpenseTypeTable? hrExpenseTypeTable = bllHRExpenseTypeTable.GetByID(entity.expenseTypeId ?? 0);

                    BLLActions.HRExpenseAmount bllHRExpenseAmount = new BLLActions.HRExpenseAmount(_configuration, _env);
                    Data.Models.HRExpenseAmount hrExpenseAmount = bllHRExpenseAmount.getbycalisanturuidandharcamaturuid(
                            user.calisanTuruId, entity.expenseTypeId, entity.spendingTime.ToString());

                    if (entity.id == null)
                    {

                        if (hrExpenseTypeTable?.toplamaNo ??false)
                        {

                            decimal? oncekiHarcamalarToplam = getByTypeAndDateSumAmount(entity.tripId,
                                    entity.expenseTypeId, Convert.ToDateTime(entity.spendingTime));
                            decimal toplamSave;
                            //decimal toplam = entity.amount.Add(oncekiHarcamalarToplam == null ? Convert.ToDecimal(0) : oncekiHarcamalarToplam);
                            decimal toplam = (entity.amount ?? 0m) + (oncekiHarcamalarToplam ?? 0m);
                            List<Data.Models.HRExpenseTable> listHrExpenseTable = getByTypeAndDate(entity.tripId,
                                    entity.expenseTypeId, entity.spendingTime);

                            int totalDays =
     ((entity.kalinanGunSayisi ?? 0) + (entity.otoparkGunSayisi ?? 0)) == 0
         ? 1
         : (entity.kalinanGunSayisi ?? 0) + (entity.otoparkGunSayisi ?? 0);

                            decimal totalLimit = hrExpenseAmount.harcirahMiktari * Convert.ToDecimal(totalDays);

                            if (toplam.CompareTo(totalLimit) >= 1)
                            {
                                // toplam büyükse
                                toplamSave = totalLimit;

                            }
                            else
                            {
                                toplamSave = toplam;
                            }
                            entity.totalLimitAmount = (totalLimit);
                            entity.approvedAmount = (toplamSave);
                            foreach (Data.Models.HRExpenseTable hrExpenseTable in listHrExpenseTable)
                            {
                                hrExpenseTable.approvedAmount = (toplamSave);
                                await Update(hrExpenseTable);
                            }
                        }
                        else if ((hrExpenseTypeTable?.otoparkMi ??false) || (hrExpenseTypeTable?.harcamaBoyu ??false))
                        {
                            int totalDays = entity.kalinanGunSayisi ?? 0 + (entity.otoparkGunSayisi == 0 ? 1
                                    : entity.kalinanGunSayisi) + entity.otoparkGunSayisi ?? 0;

                            decimal totalLimit = hrExpenseAmount.harcirahMiktari * Convert.ToDecimal(totalDays);
                            entity.totalLimitAmount = (totalLimit);

                            if (entity.amount.HasValue && entity.amount.Value.CompareTo(totalLimit) >= 1)
                            {
                                entity.approvedAmount = totalLimit;
                            }
                            else
                            {
                                entity.approvedAmount = entity.amount;
                            }

                        }
                        else
                        {

                            if (entity.amount.HasValue && entity.amount.Value > hrExpenseAmount.harcirahMiktari)
                            {
                                entity.approvedAmount = hrExpenseAmount.harcirahMiktari;
                            }
                            else
                            {
                                entity.approvedAmount = entity.amount ?? 0m;
                            }

                            entity.totalLimitAmount = hrExpenseAmount.harcirahMiktari;
                        }

                        entity.createdUserId = (userId);
                        entity.createdDate = (DateTime.Now);
                        entity.enabled = (true);
                        return await Add(_mapper.Map<Data.Models.HRExpenseTable>(entity));
                    }
                    else
                    {

                        if (hrExpenseTypeTable?.toplamaNo ??false)
                        {

                            decimal? oncekiHarcamalarToplam = getByTypeAndDateSumAmountEdit(entity.tripId,
                                    entity.expenseTypeId, entity.spendingTime, entity.id);
                            decimal toplamSave;

                            decimal toplam = (entity.amount ?? 0m) + (oncekiHarcamalarToplam ?? 0m);
                            List<Data.Models.HRExpenseTable> listHrExpenseTable = getByTypeAndDate(entity.tripId,
                                    entity.expenseTypeId, entity.spendingTime);

                            int totalDays = ((entity.kalinanGunSayisi ?? 0) + (entity.otoparkGunSayisi ?? 0)) == 0 ? 1 : (entity.kalinanGunSayisi ?? 0 + entity.otoparkGunSayisi ?? 0);
                            decimal totalLimit = hrExpenseAmount.harcirahMiktari * Convert.ToDecimal(totalDays);

                            if (toplam.CompareTo(totalLimit) >= 1)
                            {
                                // toplam büyükse
                                toplamSave = totalLimit;

                            }
                            else
                            {
                                toplamSave = toplam;
                            }
                            entity.totalLimitAmount = (totalLimit);
                            entity.approvedAmount = (toplamSave);
                            foreach (Data.Models.HRExpenseTable hrExpenseTable in listHrExpenseTable)
                            {
                                hrExpenseTable.approvedAmount = (toplamSave);
                                await Update(hrExpenseTable);
                            }
                        }
                        else if ((hrExpenseTypeTable?.otoparkMi ??false) || (hrExpenseTypeTable?.harcamaBoyu ?? false))
                        {
                            int totalDays = (entity.kalinanGunSayisi ?? 0 + entity.otoparkGunSayisi ?? 0) == 0 ? 1
                                    : (entity.kalinanGunSayisi ?? 0 + entity.otoparkGunSayisi ?? 0);
                            decimal totalLimit = hrExpenseAmount.harcirahMiktari * Convert.ToDecimal(totalDays);

                            entity.totalLimitAmount = (totalLimit);

                            if (entity.amount.HasValue && entity.amount.Value > totalLimit)
                            {
                                entity.approvedAmount = totalLimit;
                            }
                            else
                            {
                                entity.approvedAmount = entity.amount ?? 0m;
                            }

                        }
                        else
                        {

                            if (entity.amount.HasValue && entity.amount.Value > hrExpenseAmount.harcirahMiktari)
                            {

                                entity.approvedAmount = hrExpenseAmount.harcirahMiktari;
                            }
                            else
                            {

                                entity.approvedAmount = entity.amount ?? 0m;
                            }

                            // Toplam limit tutarı her koşulda harcırah miktarına eşitleniyor
                            entity.totalLimitAmount = hrExpenseAmount.harcirahMiktari;

                        }
                        entity.updatedUserId = (userId);
                        entity.updateDate = (DateTime.Now);
                        entity.enabled = (true);
                        await Update(_mapper.Map<Data.Models.HRExpenseTable>(entity));
                        return _mapper.Map<Data.Models.HRExpenseTable>(entity);
                    }



                }
                catch (Exception e)
                {
                    BLLActions.AdminUsers bllAdminUsers = new BLLActions.AdminUsers(_configuration, _env, _mapper);
                    UserByNameEMailDto byNameEMailDto = bllAdminUsers.getUserByNameAndEmail(userId);

                    Console.WriteLine(
                            byNameEMailDto.name + "," + " Seyahat harcama oluşturamıyor. Hata: " + e.Message);
                    return new Data.Models.HRExpenseTable();
                }
            }

            private decimal? getByTypeAndDateSumAmountEdit(int? tripId, int? expenseTypeId, DateTime? spendingTime, int? id)
            {
                decimal? totalAmount = dal.Get(u =>
    u.enabled == true &&
    u.tripId == tripId &&
    u.expenseTypeId == expenseTypeId &&
    u.spendingTime == spendingTime &&
    u.Id != id &&
    u.currentStateId == 1
).Sum(u => (decimal?)u.amount);
                return totalAmount;
            }

            private List<Data.Models.HRExpenseTable> getByTypeAndDate(int? tripId, int? expenseTypeId, DateTime? spendingTime)
            {
                var list = dal.Get(u =>
     u.enabled == true &&
     u.tripId == tripId &&
     u.expenseTypeId == expenseTypeId &&
     u.spendingTime == spendingTime &&
     u.currentStateId == 1
 ).ToList();
                return list ?? [];
            }

            private decimal? getByTypeAndDateSumAmount(int? tripId, int? expenseTypeId, DateTime? spendingTime)
            {
                decimal? totalAmount = dal.Get(u => u.enabled && u.tripId == tripId && u.expenseTypeId == expenseTypeId &&
                u.spendingTime == spendingTime && u.currentStateId == 1).Sum(u => u.amount);
                return totalAmount;
            }

            public PageReturn<HRExpenseTripDto> mylist(FilterPageParam<HRExpenseTripTableMyListDtoParameter> filterPageParam)
            {
                BLLActions.HRExpenseTripTable bllHRExpenseTripTable = new BLLActions.HRExpenseTripTable(_configuration, _env, _mapper);
                PageReturn<HRExpenseTripDto> page = bllHRExpenseTripTable.myActiveExpense(filterPageParam);


                return page;
            }

            public List<HRExpenseDto> mylistExpense(int tripId)
            {
                List<HRExpenseDto> liste = dal.Get(u => u.enabled && u.tripId == tripId).OrderByDescending(u => u.Id).Select(u => new HRExpenseDto()
                {
                    aciklama = u.expenseDescription,
                    approval = u.approval,
                    currentStateId = u.currentStateId,
                    currentUserId = u.currentUserId,
                    file = u.fileNames,
                    gunSayisi = u.kalinanGunSayisi,
                    harcamaTarihi = (u.spendingTime ?? DateTime.Now).ToString("dd.MM.yyyy"),
                    harcamaTuru = u.expenseType.expenseTypeName,
                    harcamaTutari = u.amount,
                    id = u.Id,
                    onaylananMasraf = u.approvedAmount,
                    onaySirasi = u.onaySirasi,
                    toplamLimit = u.totalLimitAmount,
                }).ToList();
                return liste;
            }
            // 1->onaylandı
            // 2-> onaylayıcıları kontrol edin
            // 3->bitti
            // 4->hata
            public async Task<int> confirmAll(int tripId, int userId)
            {
                try
                {

                    int donenDeger = 0;
                    BLLActions.CeoTable bllCeoTable = new BLLActions.CeoTable(_configuration, _env);
                    Data.Models.CeoTable ceo = bllCeoTable.GetByID(1)!;
                    BLLActions.HRExpenseTripTable bllHRExpenseTripTable = new BLLActions.HRExpenseTripTable(_configuration, _env, _mapper);
                    Data.Models.HRExpenseTripTable hrExpenseTripTable = bllHRExpenseTripTable.GetByID(tripId)!;
                    List<Data.Models.HRExpenseTable> listHRExpenseTable = findByTripIdAndCurrentUserIdAndEnabledActive(tripId,
                            userId, true);
                    BLLActions.AdminUsers bllAdminUsers = new BLLActions.AdminUsers(_configuration, _env, _mapper);
                    AdminUser? ceoUser = bllAdminUsers.GetByID(ceo.userId);
                    AdminUser? tripUser = bllAdminUsers.GetByID(hrExpenseTripTable.userId);
                    AdminUser? hrEmployer1 = bllAdminUsers.GetByID(tripUser?.hremployer1 ?? 0);
                    BLLActions.EmailMessages bllEmailMessages = new BLLActions.EmailMessages(_configuration, _env);
                    BLLActions.EmailReaderFile bllEmailReaderFile = new BLLActions.EmailReaderFile();
                    bool fazlaHarcamaVarmi = listHRExpenseTable.Any(u => u.approvedAmount.CompareTo(u.totalLimitAmount * 0.1m + u.totalLimitAmount) > 0);
                    if ((tripUser?.Id??0).Equals(userId))
                    {
                        if (hrEmployer1 != null)
                        {
                            approveHRExpenseTable(listHRExpenseTable, hrEmployer1, 1, 1, tripUser!, tripId, ceo);
                            onay(null, hrEmployer1, tripId, fazlaHarcamaVarmi, ceo);
                            donenDeger = 1;

                            EmailMessage emailMessage = new EmailMessage();
                            emailMessage.subject = ("Bekleyen Harcama Onayı hk.");
                            emailMessage.toAddress = (hrEmployer1.email);



                            string mailMessage = bllEmailReaderFile.BuildEmailTemplate(_configuration, _env, "Sayın " + hrEmployer1.name +
                            "Harcama Onayı hk.",
                                       tripId.ToString() + " ID kodlu Harcama onayınızı beklemektedir.");
                            emailMessage.emailText = (mailMessage);
                            emailMessage.mailTuru = (3);
                            emailMessage.enabled = (true);
                            emailMessage.isSent = (false);
                            emailMessage.plannedDate = DateTime.Now;
                            await bllEmailMessages.Add(emailMessage);

                            donenDeger = 1;

                        }
                        else
                        {
                            donenDeger = 2;
                        }

                    }
                    else if (userId.Equals(hrEmployer1?.Id??0))
                    {

                        if (tripUser?.manager1 != null)
                        {
                            AdminUser manager1 = bllAdminUsers.GetByID(tripUser.manager1 ?? 0)!;
                            if (!Equals(tripUser.hremployer1, tripUser.manager1))
                            {
                                approveHRExpenseTable(listHRExpenseTable, manager1, 2, 1, tripUser, tripId, ceo);
                                onay(hrEmployer1, manager1, tripId, fazlaHarcamaVarmi, ceo);
                                donenDeger = 1;
                                // CeoTable ceoTable = ceoTableRepository.findById(1).get();
                                if (Equals(manager1.Id, ceo.userId))
                                {
                                    BLLActions.SMSMessages bllSMSMessages = new BLLActions.SMSMessages(_configuration, _env);
                                    SMSMessage smsMessage = new SMSMessage();
                                    smsMessage.plannedDate = DateTime.Now;
                                    smsMessage.isSent = (false);
                                    smsMessage.smsText = (tripId.ToString() + "Id'li seyahat harcama onayınızı beklemektedir.");
                                    smsMessage.toNumbers = (ceoUser?.mobile);

                                    await bllSMSMessages.Add(smsMessage);

                                }
                                else
                                {
                                    EmailMessage emailMessage = new EmailMessage();
                                    emailMessage.subject = ("Bekleyen Harcama Onayı hk.");
                                    emailMessage.toAddress = (manager1.email);


                                    string mailMessage = bllEmailReaderFile.BuildEmailTemplate(_configuration, _env, "Sayın " + manager1.name +
                            "Harcama Onayı hk.",
                                       tripId.ToString() + " ID kodlu Harcama onayınızı beklemektedir.");
                                    emailMessage.emailText = (mailMessage);
                                    emailMessage.mailTuru = (3);
                                    emailMessage.enabled = (true);
                                    emailMessage.isSent = (false);
                                    emailMessage.plannedDate = DateTime.Now;
                                    await bllEmailMessages.Add(emailMessage);
                                }
                                donenDeger = 1;
                            }
                            else
                            {
                                if (tripUser.manager2 != null)
                                {
                                    AdminUser manager2 = bllAdminUsers.GetByID(tripUser.manager2 ?? 0)!;
                                    approveHRExpenseTable(listHRExpenseTable, manager2, 2, 1, tripUser, tripId, ceo);
                                    onay(manager1, manager2, tripId, fazlaHarcamaVarmi, ceo);
                                    donenDeger = 1;

                                    if (Equals(manager2.Id, ceo.userId))
                                    {
                                        BLLActions.SMSMessages bllSMSMessages = new BLLActions.SMSMessages(_configuration, _env);
                                        SMSMessage smsMessage = new SMSMessage();
                                        smsMessage.plannedDate = DateTime.Now;
                                        smsMessage.isSent = (false);
                                        smsMessage.smsText = (
                                                tripId.ToString() + "Id'li seyahat harcama onayınızı beklemektedir.");
                                        smsMessage.toNumbers = (ceoUser?.mobile);

                                        await bllSMSMessages.Add(smsMessage);

                                    }
                                    else
                                    {
                                        EmailMessage emailMessage = new EmailMessage();
                                        emailMessage.subject = ("Bekleyen Harcama Onayı hk.");
                                        emailMessage.toAddress = (manager2.email);


                                        string mailMessage = bllEmailReaderFile.BuildEmailTemplate(_configuration, _env, "Sayın " + manager2.name +
                          "Harcama Onayı hk.",
                                     tripId.ToString() + " ID kodlu Harcama onayınızı beklemektedir.");
                                        emailMessage.emailText = (mailMessage);
                                        emailMessage.mailTuru = (3);
                                        emailMessage.enabled = (true);
                                        emailMessage.isSent = (false);
                                        emailMessage.plannedDate = DateTime.Now;
                                        await bllEmailMessages.Add(emailMessage);
                                    }

                                }
                                else
                                {
                                    if (tripUser.manager3 == null && tripUser.manager4 == null)
                                    {

                                        approveHRExpenseTable(listHRExpenseTable, manager1, 10, 4, tripUser, tripId, ceo);
                                        onay(manager1, null, tripId, fazlaHarcamaVarmi, ceo);

                                        EmailMessage emailMessage = new EmailMessage();
                                        emailMessage.subject = ("Bekleyen Harcama Onayı hk.");
                                        emailMessage.toAddress = (tripUser.email);


                                        string mailMessage = bllEmailReaderFile.BuildEmailTemplate(_configuration, _env, "Sayın " + tripUser.name +
                         "Harcama Onayı hk.",
                                    tripId.ToString() + " ID kodlu Harcamanız onaylanmıştır.");
                                        emailMessage.emailText = (mailMessage);
                                        emailMessage.mailTuru = (3);
                                        emailMessage.enabled = (true);
                                        emailMessage.isSent = (false);
                                        emailMessage.plannedDate = DateTime.Now;
                                        await bllEmailMessages.Add(emailMessage);

                                        donenDeger = 3;

                                    }
                                    else
                                    {
                                        donenDeger = 2;
                                    }
                                }
                            }

                        }
                        else
                        {

                            donenDeger = 2;
                        }

                    }
                    else if (userId.Equals(tripUser?.manager1) && userId != ceo.userId)
                    {
                        if (tripUser.manager2 != null)
                        {
                            AdminUser manager2 = bllAdminUsers.GetByID(tripUser.manager2 ?? 0)!;
                            AdminUser manager1 = bllAdminUsers.GetByID(tripUser.manager1 ?? 0)!;
                            if (!Equals(tripUser.manager1, tripUser.manager2))
                            {
                                approveHRExpenseTable(listHRExpenseTable, manager2, 3, 1, tripUser, tripId, ceo);
                                onay(manager1, manager2, tripId, fazlaHarcamaVarmi, ceo);
                                donenDeger = 1;

                                EmailMessage emailMessage = new EmailMessage();
                                emailMessage.subject = ("Bekleyen Harcama Onayı hk.");
                                emailMessage.toAddress = (manager2.email);



                                string mailMessage = bllEmailReaderFile.BuildEmailTemplate(_configuration, _env, "Sayın " + manager2.name +
                         "Harcama Onayı hk.",
                                    tripId.ToString() + " ID kodlu Harcama onayınızı beklemektedir.");
                                emailMessage.emailText = (mailMessage);
                                emailMessage.mailTuru = (3);
                                emailMessage.enabled = (true);
                                emailMessage.isSent = (false);
                                emailMessage.plannedDate = DateTime.Now;
                                await bllEmailMessages.Add(emailMessage);

                            }
                            else
                            {
                                if (tripUser.manager3 != null)
                                {
                                    AdminUser manager3 = bllAdminUsers.GetByID(tripUser.manager3 ?? 0)!;
                                    approveHRExpenseTable(listHRExpenseTable, manager3, 4, 1, tripUser, tripId, ceo);
                                    onay(manager2, manager3, tripId, fazlaHarcamaVarmi, ceo);
                                    donenDeger = 1;
                                    if (Equals(manager3.Id, ceo.userId))
                                    {
                                        BLLActions.SMSMessages bllSMSMessages = new BLLActions.SMSMessages(_configuration, _env);
                                        SMSMessage smsMessage = new SMSMessage();
                                        smsMessage.plannedDate = DateTime.Now;
                                        smsMessage.isSent = (false);
                                        smsMessage.smsText = (
                                                tripId.ToString() + "Id'li seyahat harcama onayınızı beklemektedir.");
                                        smsMessage.toNumbers = (ceoUser?.mobile);

                                        await bllSMSMessages.Add(smsMessage);

                                    }
                                    else
                                    {
                                        EmailMessage emailMessage = new EmailMessage();
                                        emailMessage.subject = ("Bekleyen Harcama Onayı hk.");
                                        emailMessage.toAddress = (manager3.email);

                                        string mailMessage = bllEmailReaderFile.BuildEmailTemplate(_configuration, _env, "Sayın " + manager3.name +
                         "Harcama Onayı hk.",
                                    tripId.ToString() + " ID kodlu Harcama onayınızı beklemektedir.");
                                        emailMessage.emailText = (mailMessage);
                                        emailMessage.mailTuru = (3);
                                        emailMessage.enabled = (true);
                                        emailMessage.isSent = (false);
                                        emailMessage.plannedDate = DateTime.Now;
                                        await bllEmailMessages.Add(emailMessage);
                                    }
                                    donenDeger = 1;
                                }
                                else if (tripUser.manager4 == null)
                                {
                                    approveHRExpenseTable(listHRExpenseTable, manager2, 10, 4, tripUser, tripId, ceo);
                                    onay(manager2, null, tripId, fazlaHarcamaVarmi, ceo);

                                    EmailMessage emailMessage = new EmailMessage();
                                    emailMessage.subject = ("Bekleyen Harcama Onayı hk.");
                                    emailMessage.toAddress = (tripUser.email);


                                    string mailMessage = bllEmailReaderFile.BuildEmailTemplate(_configuration, _env, "Sayın " + tripUser.name +
                         "Harcama Onayı hk.",
                                    tripId.ToString() + " ID kodlu Harcamanız onaylanmıştır.");
                                    emailMessage.emailText = (mailMessage);
                                    emailMessage.mailTuru = (3);
                                    emailMessage.enabled = (true);
                                    emailMessage.isSent = (false);
                                    emailMessage.plannedDate = DateTime.Now;
                                    await bllEmailMessages.Add(emailMessage);

                                    donenDeger = 3;

                                }
                                else
                                {

                                    donenDeger = 2;
                                }
                            }

                        }
                        else
                        {
                            if (tripUser.manager3 == null && tripUser.manager4 == null)
                            {

                                AdminUser manager1 = bllAdminUsers.GetByID(tripUser.manager1 ?? 0)!;

                                approveHRExpenseTable(listHRExpenseTable, manager1, 10, 4, tripUser, tripId, ceo);
                                onay(manager1, null, tripId, fazlaHarcamaVarmi, ceo);

                                EmailMessage emailMessage = new EmailMessage();
                                emailMessage.subject = ("Bekleyen Harcama Onayı hk.");
                                emailMessage.toAddress = (tripUser.email);

                                string mailMessage = bllEmailReaderFile.BuildEmailTemplate(_configuration, _env, "Sayın " + tripUser.name +
                         "Harcama Onayı hk.",
                                    tripId.ToString() + " ID kodlu Harcamanız onaylanmıştır.");
                                emailMessage.emailText = (mailMessage);
                                emailMessage.mailTuru = (3);
                                emailMessage.enabled = (true);
                                emailMessage.isSent = (false);
                                emailMessage.plannedDate = DateTime.Now;
                                await bllEmailMessages.Add(emailMessage);

                                donenDeger = 3;

                            }
                            else
                            {
                                donenDeger = 2;
                            }
                        }

                    }
                    else if (userId.Equals(tripUser?.manager2) && userId != ceo.userId)
                    {
                        if (tripUser.manager3 != null)
                        {
                            AdminUser manager3 = bllAdminUsers.GetByID(tripUser.manager3 ?? 0)!;
                            AdminUser manager2 = bllAdminUsers.GetByID(tripUser.manager2 ?? 0)!;
                            approveHRExpenseTable(listHRExpenseTable, manager3, 4, 1, tripUser, tripId, ceo);
                            onay(manager2, manager3, tripId, fazlaHarcamaVarmi, ceo);
                            donenDeger = 1;

                            EmailMessage emailMessage = new EmailMessage();
                            emailMessage.subject = ("Bekleyen Harcama Onayı hk.");
                            emailMessage.toAddress = (manager3.email);



                            string mailMessage = bllEmailReaderFile.BuildEmailTemplate(_configuration, _env, "Sayın " + manager3.name +
                         "Harcama Onayı hk.",
                                    tripId.ToString() + " ID kodlu Harcama onayınızı beklemektedir.");
                            emailMessage.emailText = (mailMessage);
                            emailMessage.mailTuru = (3);
                            emailMessage.enabled = (true);
                            emailMessage.isSent = (false);
                            emailMessage.plannedDate = DateTime.Now;
                            await bllEmailMessages.Add(emailMessage);

                            donenDeger = 1;
                        }
                        else if (tripUser.manager4 == null)
                        {
                            AdminUser manager2 = bllAdminUsers.GetByID(tripUser.manager2 ?? 0)!;
                            approveHRExpenseTable(listHRExpenseTable, manager2, 10, 4, tripUser, tripId, ceo);
                            onay(manager2, null, tripId, fazlaHarcamaVarmi, ceo);

                            EmailMessage emailMessage = new EmailMessage();
                            emailMessage.subject = ("Bekleyen Harcama Onayı hk.");
                            emailMessage.toAddress = (tripUser.email);

                            string mailMessage = bllEmailReaderFile.BuildEmailTemplate(_configuration, _env, "Sayın " + tripUser.name +
                         "Harcama Onayı hk.",
                                    tripId.ToString() + " ID kodlu Harcamanız onaylanmıştır.");
                            emailMessage.emailText = (mailMessage);
                            emailMessage.mailTuru = (3);
                            emailMessage.enabled = (true);
                            emailMessage.isSent = (false);
                            emailMessage.plannedDate = DateTime.Now;
                            await bllEmailMessages.Add(emailMessage);

                            donenDeger = 3;

                        }
                        else
                        {

                            donenDeger = 2;
                        }

                    }
                    else if (userId.Equals(tripUser?.manager3) && userId != ceo.userId)
                    {
                        if (tripUser.manager4 != null)
                        {
                            AdminUser manager3 = bllAdminUsers.GetByID(tripUser.manager3 ?? 0)!;
                            AdminUser manager4 = bllAdminUsers.GetByID(tripUser.manager4 ?? 0)!;
                            approveHRExpenseTable(listHRExpenseTable, manager4, 5, 1, tripUser, tripId, ceo);
                            onay(manager3, manager4, tripId, fazlaHarcamaVarmi, ceo);
                            donenDeger = 1;

                            EmailMessage emailMessage = new EmailMessage();
                            emailMessage.subject = ("Bekleyen Harcama Onayı hk.");
                            emailMessage.toAddress = (manager4.email);


                            string mailMessage = bllEmailReaderFile.BuildEmailTemplate(_configuration, _env, "Sayın " + manager4.name +
                         "Harcama Onayı hk.",
                                    tripId.ToString() + " ID kodlu Harcama onayınızı beklemektedir.");
                            emailMessage.emailText = (mailMessage);
                            emailMessage.mailTuru = (3);
                            emailMessage.enabled = (true);
                            emailMessage.isSent = (false);
                            emailMessage.plannedDate = DateTime.Now;
                            await bllEmailMessages.Add(emailMessage);

                        }
                        else
                        {
                            AdminUser manager3 = bllAdminUsers.GetByID(tripUser.manager3 ?? 0)!;
                            approveHRExpenseTable(listHRExpenseTable, manager3, 10, 4, tripUser, tripId, ceo);
                            onay(manager3, null, tripId, fazlaHarcamaVarmi, ceo);
                            donenDeger = 3;
                            EmailMessage emailMessage = new EmailMessage();
                            emailMessage.subject = ("Bekleyen Harcama Onayı hk.");
                            emailMessage.toAddress = (tripUser.email);


                            string mailMessage = bllEmailReaderFile.BuildEmailTemplate(_configuration, _env, "Sayın " + tripUser.name +
                        "Harcama Onayı hk.",
                                   tripId.ToString() + " ID kodlu Harcamanız onaylanmıştır.");
                            emailMessage.emailText = (mailMessage);
                            emailMessage.mailTuru = (3);
                            emailMessage.enabled = (true);
                            emailMessage.isSent = (false);
                            emailMessage.plannedDate = DateTime.Now;
                            await bllEmailMessages.Add(emailMessage);

                        }

                    }
                    else if (userId.Equals(tripUser?.manager4) && userId != ceo.userId)
                    {
                        AdminUser manager4 = bllAdminUsers.GetByID(tripUser.manager4 ?? 0)!;
                        approveHRExpenseTable(listHRExpenseTable, manager4, 10, 4, tripUser, tripId, ceo);
                        onay(manager4, null, tripId, fazlaHarcamaVarmi, ceo);
                        donenDeger = 3;
                        EmailMessage emailMessage = new EmailMessage();
                        emailMessage.subject = ("Bekleyen Harcama Onayı hk.");
                        emailMessage.toAddress = (tripUser.email);



                        string mailMessage = bllEmailReaderFile.BuildEmailTemplate(_configuration, _env, "Sayın " + tripUser.name +
                        "Harcama Onayı hk.",
                                   tripId.ToString() + " ID kodlu Harcamanız onaylanmıştır.");
                        emailMessage.emailText = (mailMessage);
                        emailMessage.mailTuru = (3);
                        emailMessage.enabled = (true);
                        emailMessage.isSent = (false);
                        emailMessage.plannedDate = DateTime.Now;
                        await bllEmailMessages.Add(emailMessage);

                    }
                    else if (userId.Equals(ceo.userId) || userId == ceo.userId)
                    {

                        approveHRExpenseTable(listHRExpenseTable, ceoUser!, 10, 4, tripUser!, tripId, ceo);

                        BLLActions.HRExpenseDetail bllHRExpenseDetail = new BLLActions.HRExpenseDetail(_configuration, _env);
                        Data.Models.HRExpenseDetail? hrExpenseDetail = bllHRExpenseDetail.getByActive(tripId, ceoUser!.Id);
                        if (hrExpenseDetail != null)
                        {
                        hrExpenseDetail.approved = (true);
                        hrExpenseDetail.isReplied = (true);
                        hrExpenseDetail.replyDate = DateTime.Now;
                        await bllHRExpenseDetail.Update(hrExpenseDetail);
                        }
                        
                        donenDeger = 3;
                        EmailMessage emailMessage = new EmailMessage();
                        emailMessage.subject = ("Bekleyen Harcama Onayı hk.");
                        emailMessage.toAddress = (tripUser?.email);

                        string mailMessage = bllEmailReaderFile.BuildEmailTemplate(_configuration, _env, "Sayın " + tripUser?.name +
                        "Harcama Onayı hk.",
                                   tripId.ToString() + " ID kodlu Harcamanız onaylanmıştır.");
                        emailMessage.emailText = (mailMessage);
                        emailMessage.mailTuru = (3);
                        emailMessage.enabled = (true);
                        emailMessage.isSent = (false);
                        emailMessage.plannedDate = DateTime.Now;
                        await bllEmailMessages.Add(emailMessage);
                    }

                    return donenDeger;

                }
                catch (Exception e)
                {
                    BLLActions.AdminUsers bllAdminUsers = new BLLActions.AdminUsers(_configuration, _env, _mapper);
                    UserByNameEMailDto byNameEMailDto = bllAdminUsers.getUserByNameAndEmail(userId);
                    Console.WriteLine(byNameEMailDto.name + "," + tripId.ToString()
                            + " seyahat id'li sehayat harcamayı onaylayamadı. Hata: " + e.Message);
                    return 4;
                }
            }

            private List<Data.Models.HRExpenseTable> findByTripIdAndCurrentUserIdAndEnabledActive(int tripId, int userId, bool enabled)
            {
                return dal.Get(u => u.tripId == tripId && u.currentUserId == userId && u.enabled == enabled && u.currentStateId == 1).ToList();
            }


            private async void approveHRExpenseTable(List<Data.Models.HRExpenseTable> listHrExpenseTables, AdminUser nextUser, int onaySirasi,
            int currentState, AdminUser tripUser, int tripId, Data.Models.CeoTable ceo)
            {
                if (currentState == 1)
                {

                    foreach (Data.Models.HRExpenseTable hrExpenseTable in listHrExpenseTables)
                    {
                        hrExpenseTable.currentUserId = (nextUser.Id);
                        hrExpenseTable.onaySirasi = (onaySirasi);
                        hrExpenseTable.currentStateId = (currentState);
                        // soneklenen
                        hrExpenseTable.approval = (true);
                        await Update(hrExpenseTable);
                    }

                }
                else if (currentState == 4)
                {

                    foreach (Data.Models.HRExpenseTable hrExpenseTable in listHrExpenseTables)
                    {
                        decimal limit = hrExpenseTable.totalLimitAmount * 0.1m + hrExpenseTable.totalLimitAmount ?? Convert.ToDecimal(0);
                        if (hrExpenseTable.approvedAmount.CompareTo(limit) > 0
                                //						hrExpenseTable.getApprovedAmount().compareTo(hrExpenseTable.getTotalLimitAmount()) > 0

                                && !Equals(nextUser.Id, ceo.userId) && ceo.fazlaHarcirahDusumu == true)
                        {

                            hrExpenseTable.currentUserId = (ceo.userId);
                            hrExpenseTable.onaySirasi = (hrExpenseTable.onaySirasi + 1);
                            // hrExpenseTable.setCurrentStateId(1);
                            // soneklenen
                            // hrExpenseTable.setLastApproved(true);
                            await Update(hrExpenseTable);

                        }
                        else
                        {
                            BLLActions.HRExpenseTripTable bllHRExpenseTripTable = new BLLActions.HRExpenseTripTable(_configuration, _env, _mapper);
                            Data.Models.HRExpenseTripTable? hrExpenseTripTable = bllHRExpenseTripTable.GetByID(tripId);
                            if (hrExpenseTripTable != null)
                            {
                            hrExpenseTable.onaySirasi = (onaySirasi);
                            hrExpenseTable.currentStateId = (currentState);
                            // burası

                            hrExpenseTripTable.approval = (true);
                            await bllHRExpenseTripTable.Update(hrExpenseTripTable);
                            }
                           
                            await Update(hrExpenseTable);

                        }

                    }

                }

            }


            private async void onay(AdminUser? approvedUser, AdminUser? nextUser, int tripId, bool fazlaHarcamaVarmi, Data.Models.CeoTable ceo)
            {
                BLLActions.HRExpenseDetail bllHRExpenseDetail = new BLLActions.HRExpenseDetail(_configuration, _env);

                if (approvedUser != null)
                {
                    Data.Models.HRExpenseDetail? hrExpenseDetail = bllHRExpenseDetail.getByActive(tripId, approvedUser.Id);
                    if (hrExpenseDetail != null)
                    {
                        hrExpenseDetail.approved = (true);
                        hrExpenseDetail.isReplied = (true);
                        hrExpenseDetail.replyDate = (DateTime.Now);
                        await bllHRExpenseDetail.Update(hrExpenseDetail);
                    }
                }
                if (nextUser == null)
                {
                    if (fazlaHarcamaVarmi && ceo.fazlaHarcirahDusumu)
                    {
                        Data.Models.HRExpenseDetail? varmi = bllHRExpenseDetail.getByActive(tripId, ceo.userId);
                        if (varmi == null)
                        {
                            Data.Models.HRExpenseDetail hrExpenseDetailnext = new Data.Models.HRExpenseDetail();
                            hrExpenseDetailnext.tripId = (tripId);
                            hrExpenseDetailnext.createdDate = (DateTime.Now);
                            hrExpenseDetailnext.userId = (ceo.userId);
                            hrExpenseDetailnext.enabled = (true);
                            // buraya bak true olmayabilir
                            hrExpenseDetailnext.isReplied = (true);
                            hrExpenseDetailnext.guid = Guid.NewGuid();
                            await bllHRExpenseDetail.Add(hrExpenseDetailnext);
                        }
                    }
                }
                if (nextUser != null)
                {

                    Data.Models.HRExpenseDetail? varmi = bllHRExpenseDetail.getByActive(tripId, nextUser.Id);
                    if (varmi == null)
                    {
                        Data.Models.HRExpenseDetail hrExpenseDetailnext = new Data.Models.HRExpenseDetail();
                        hrExpenseDetailnext.tripId = (tripId);
                        hrExpenseDetailnext.createdDate = DateTime.Now;
                        hrExpenseDetailnext.userId = nextUser.Id;
                        hrExpenseDetailnext.enabled = (true);
                        // buraya bak true olmayabilir
                        hrExpenseDetailnext.isReplied = (true);
                        hrExpenseDetailnext.guid = Guid.NewGuid();
                        await bllHRExpenseDetail.Add(hrExpenseDetailnext);
                    }

                }

            }

            public async Task<HRExpenseTableSaveDto> changeLimit(HRExpenseTableSaveDto entity)
            {
                Data.Models.HRExpenseTable update = await Update(_mapper.Map<Data.Models.HRExpenseTable>(entity));
                HRExpenseTableSaveDto donenDto = _mapper.Map<HRExpenseTableSaveDto>(update);
                return donenDto;
            }

            public async Task<int> rejectAll(int tripId, int userId)
            {
                try
                {

                    int donenDeger = 0;

                    List<Data.Models.HRExpenseTable> listHRExpenseTable = findByTripIdAndCurrentUserIdAndEnabledActive(tripId,
                            userId, true);
                    foreach (Data.Models.HRExpenseTable expenseTable in listHRExpenseTable)
                    {
                        try
                        {
                            expenseTable.approval = (false);
                            expenseTable.currentStateId = (2);
                            await Update(expenseTable);
                            donenDeger = 1;

                        }
                        catch
                        {
                            donenDeger = 2;
                        }

                    }
                    try
                    {
                        BLLActions.HRExpenseDetail bllHRExpenseDetail = new BLLActions.HRExpenseDetail(_configuration, _env);
                        Data.Models.HRExpenseDetail? hrExpenseDetail = bllHRExpenseDetail.getByActive(tripId, userId);
                        if (hrExpenseDetail != null)
                        {
                            hrExpenseDetail.approved = (false);
                            hrExpenseDetail.isReplied = (true);
                            hrExpenseDetail.replyDate = (DateTime.Now);
                            await bllHRExpenseDetail.Update(hrExpenseDetail);
                            donenDeger = 1;
                        }
                        else
                        {
                            donenDeger = 2;
                        }


                    }
                    catch (Exception )
                    {
                        donenDeger = 2;
                    }
                    BLLActions.HRExpenseTripTable bllHRExpenseTripTable = new BLLActions.HRExpenseTripTable(_configuration, _env, _mapper);
                    Data.Models.HRExpenseTripTable? hrExpenseTripTable = bllHRExpenseTripTable.GetByID(tripId);

                    BLLActions.AdminUsers bllAdminUsers = new BLLActions.AdminUsers(_configuration, _env, _mapper);
                    UserByNameEMailDto user = bllAdminUsers.getUserByNameAndEmail(hrExpenseTripTable?.userId??0);

                    BLLActions.EmailMessages bllEmailMessages = new BLLActions.EmailMessages(_configuration, _env);
                    BLLActions.EmailReaderFile bllEmailReaderFile = new BLLActions.EmailReaderFile();
                    EmailMessage emailMessage = new EmailMessage();
                    emailMessage.subject=("Bekleyen Harcama Onayı hk.");

                    emailMessage.toAddress=(user.email);

                  

                    string mailMessage = bllEmailReaderFile.BuildEmailTemplate(_configuration, _env, "Sayın " + user.name +
                        "RED Harcama Onayı hk.",
                                   tripId.ToString() + " ID kodlu Harcamanız reddedilmiştir.");
                    emailMessage.emailText=(mailMessage);
                    emailMessage.mailTuru=(3);
                    emailMessage.enabled=(true);
                    emailMessage.isSent=(false);
                    emailMessage.plannedDate=(DateTime.Now);
                    await bllEmailMessages.Add(emailMessage);
                    return donenDeger;
                }
                catch (Exception e)
                {
                    BLLActions.AdminUsers bllAdminUsers = new BLLActions.AdminUsers(_configuration, _env, _mapper);

                    UserByNameEMailDto byNameEMailDto = bllAdminUsers.getUserByNameAndEmail(userId);
                    Console.WriteLine(byNameEMailDto.name + "," + tripId.ToString()
                            + " seyahat id'li sehayat harcamayı red edemedi. Hata: " + e.Message);
                    return 2;
                }
            }

            public async Task<int> geriGonder(int tripId, int userId)
            {
                int donenDeger = 0;

                List<Data.Models.HRExpenseTable> listHRExpenseTable = findByTripIdAndCurrentUserIdAndEnabled(tripId, userId,true);

                BLLActions.HRExpenseTripTable bllHRExpenseTripTable = new BLLActions.HRExpenseTripTable(_configuration, _env, _mapper);
                Data.Models.HRExpenseTripTable? hrExpenseTripTable = bllHRExpenseTripTable.GetByID(tripId);
                if (hrExpenseTripTable != null)
                {


                    foreach (Data.Models.HRExpenseTable expenseTable in listHRExpenseTable)
                    {
                        try
                        {
                            expenseTable.currentStateId = (1);
                            expenseTable.currentUserId = (hrExpenseTripTable.userId);
                            expenseTable.onaySirasi = (1);
                            await Update(expenseTable);
                            donenDeger = 1;

                        }
                        catch
                        {
                            donenDeger = 2;
                        }

                    }
                    try
                    {
                        BLLActions.HRExpenseDetail bllHRExpenseDetail = new BLLActions.HRExpenseDetail(_configuration, _env);
                        List<Data.Models.HRExpenseDetail> listHRExpenseDetail = bllHRExpenseDetail.GetByTripId(tripId);

                        foreach (Data.Models.HRExpenseDetail hrExpenseDetail in listHRExpenseDetail)
                        {
                            hrExpenseDetail.enabled = (false);
                            await bllHRExpenseDetail.Update(hrExpenseDetail);
                        }

                        donenDeger = 1;

                    }
                    catch 
                    {
                        donenDeger = 2;
                    }
                    BLLActions.AdminUsers bllAdminUsers = new BLLActions.AdminUsers(_configuration, _env, _mapper);
                    UserByNameEMailDto user = bllAdminUsers.getUserByNameAndEmail(hrExpenseTripTable.userId);

                    BLLActions.EmailMessages bllEmailMessages = new BLLActions.EmailMessages(_configuration, _env);
                    BLLActions.EmailReaderFile bllEmailReaderFile = new BLLActions.EmailReaderFile();

                    EmailMessage emailMessage = new EmailMessage();
                    emailMessage.subject = ("Bekleyen Harcama Onayı hk.");
                    emailMessage.toAddress = (user.email);
                    string mailMessage = bllEmailReaderFile.BuildEmailTemplate(_configuration, _env, user.name +
                            "Geri Gönderilen Harcama Onayı.",
                                       tripId.ToString() + " ID kodlu Harcamanız geri gönderilmiştir.");
                    emailMessage.emailText = (mailMessage);
                    emailMessage.mailTuru = (3);
                    emailMessage.enabled = (true);
                    emailMessage.isSent = (false);
                    emailMessage.plannedDate = DateTime.Now;
                    await bllEmailMessages.Add(emailMessage);
                }
                else
                {
                    donenDeger = 2;
                }
                return donenDeger;
            }

            private List<Data.Models.HRExpenseTable> findByTripIdAndCurrentUserIdAndEnabled(int tripId, int userId, bool enabled)
            {
                return dal.Get(u => u.enabled == enabled && u.tripId == tripId && u.currentUserId == userId).ToList();
            }

            public List<HRExpenseTableSaveDto> listActive(FilterPageParam<HRExpenseTableActiveListDtoParameter> filterPageParam)
            {
                DateTime? gidisTarihi = filterPageParam.liste?.filterGidisTarihi;
                DateTime? donusTarihi = filterPageParam.liste?.filterDonusTarihi;
                int? userId = filterPageParam.liste?.filterUser;
                int? filterDestination = filterPageParam.liste?.filterDestination;
                int? filterWhereYouAre = filterPageParam.liste?.filterWhereYouAre;
                int? filterUserId = filterPageParam.liste?.filterUserId;

                BLLActions.AdminUsers bllAdminUsers = new BLLActions.AdminUsers(_configuration, _env, _mapper);
                AdminUser? user = bllAdminUsers.GetByID(userId??0);

                BLLActions.RoleDetails bllRoleDetails = new BLLActions.RoleDetails(_configuration, _env, _mapper);
                RoleDetail? roleDetail = bllRoleDetails.GetByRoleIDAndModuleID(user!.roleId, (int)CommonConstants.MODULES.HR_EXPENSE_CONTROL);

                List<Data.Models.HRExpenseTable> liste = new List<Data.Models.HRExpenseTable>();
                if (user.roleId == 1 || (roleDetail != null && roleDetail.canSeeLogs))
                {
                    liste = dal.Get(u =>
                        u.enabled &&
                        u.currentStateId == 1 &&
                        u.trip.enabled == true &&
                        (filterWhereYouAre == null ? true : u.trip.hereLocationId == filterWhereYouAre) &&
                        (filterUserId == null ? true : u.trip.userId == filterUserId) &&
                        (filterDestination == null ? true : u.trip.destinationLocationId == filterDestination) &&
                        (gidisTarihi == null ? true : u.trip.gidisTarihi == gidisTarihi) &&
                        (donusTarihi == null ? true : u.trip.donusTarihi == donusTarihi)
                    )
                    .OrderByDescending(u => u.Id)
                    .ToList();
                }
                else
                {
                    liste = dal.Get(u =>
                        u.enabled &&
                        u.currentStateId == 1 &&
                        u.trip.enabled == true &&
                        (filterWhereYouAre == null ? true : u.trip.hereLocationId == filterWhereYouAre) &&
                        (filterUserId == null ? true : u.trip.userId == filterUserId) &&
                        (filterDestination == null ? true : u.trip.destinationLocationId == filterDestination) &&
                        (gidisTarihi == null ? true : u.trip.gidisTarihi == gidisTarihi) &&
                        (donusTarihi == null ? true : u.trip.donusTarihi == donusTarihi)
                    )
                    .OrderByDescending(u => u.Id)
                    .ToList();
                }
                List<HRExpenseTableSaveDto> returnList = liste.Select(u => new HRExpenseTableSaveDto
                {
                    amount=u.amount,
                    approval=u.approval,
                    approvedAmount=u.approvedAmount,
                    aracTuruId=u.aracTuruId,
                    createdDate=u.createdDate,
                    createdUserId= u.createdUserId,
                    currentStateId = u.currentStateId,
                    currentUserId = u.currentUserId,
                    enabled = u.enabled,
                    expenseDescription = u.expenseDescription,
                    expenseTypeId = u.expenseTypeId,
                    fileNames = u.fileNames,
                    gunlukMu = u.gunlukMu,
                    hrNot=u.hrNot,
                    id = u.Id,
                    islemTuruId = u.islemTuruId,
                    kalinanGunSayisi = u.kalinanGunSayisi,
                    kdvDegeri = u.kdvDegeri,
                    kdvOrani = u.kdvOrani,
                    lastApproved = u.lastApproved,
                    onaySirasi = u.onaySirasi,
                    otoparkGunSayisi = u.otoparkGunSayisi,
                    plaka = u.plaka,
                    spendingTime = u.spendingTime,
                    totalLimitAmount = u.totalLimitAmount,
                    tripId = u.tripId,
                    updateDate = u.updatedDate,
                    updatedUserId = u.updatedUserId,
                    
                }).ToList();
                return returnList;
            }

            public PageReturn<HRExpenseTripDto> mylistAprovalStatus(FilterPageParam<HRExpenseTableApprovalStatusDtoParameter> filterPageParam)
            {
               
                BLLActions.HRExpenseTripTable bllHRExpenseTripTable = new BLLActions.HRExpenseTripTable(_configuration, _env, _mapper);
                PageReturn<HRExpenseTripDto> page = bllHRExpenseTripTable.mylistAprovalStatus(filterPageParam);
                return page;
            }

            public async Task<int> ceoOnayLimitTutari(int tripId, int userId)
            {
                try
                {
                    List<Data.Models.HRExpenseTable> listHrExpenseTable = findByTripIdActive(tripId);
                    foreach (Data.Models.HRExpenseTable hrExpenseTable in listHrExpenseTable)
                    {
                        decimal tutar;
                        if (hrExpenseTable.amount.CompareTo(hrExpenseTable.totalLimitAmount) > 0)
                        {
                            tutar = hrExpenseTable.totalLimitAmount ?? Convert.ToDecimal(0);
                        }
                        else
                        {
                            tutar = hrExpenseTable.amount;
                        }
                        hrExpenseTable.approvedAmount=tutar;
                        await Update(hrExpenseTable);
                    }
                    return 1;
                }
                catch (Exception )
                {
                    return 4;
                }
            }



            public async Task<int> ceoOnayFaturaTutari(int tripId, int userId)
            {
                try
                {
                    List<Data.Models.HRExpenseTable> listHarcamalar = findByTripIdActive(tripId);
                    foreach (Data.Models.HRExpenseTable entity in listHarcamalar)
                    {
                        BLLActions.HRExpenseTypeTable bllHRExpenseTypeTable = new BLLActions.HRExpenseTypeTable(_configuration, _env);
                        Data.Models.HRExpenseTypeTable? hrExpenseTypeTable = bllHRExpenseTypeTable.GetByID(entity.expenseTypeId);
                        if (hrExpenseTypeTable?.toplamaNo??false)
                        {

                           decimal? oncekiHarcamalarToplam = getByTypeAndDateSumAmount(entity.tripId,
                                    entity.expenseTypeId, entity.spendingTime);

                            List<Data.Models.HRExpenseTable> listHrExpenseTable = getByTypeAndDate(entity.tripId,
                                    entity.expenseTypeId, entity.spendingTime);

                            entity.approvedAmount=oncekiHarcamalarToplam ?? Convert.ToDecimal(0);
                            foreach (Data.Models. HRExpenseTable hrExpenseTable in listHrExpenseTable)
                            {
                                hrExpenseTable.approvedAmount=oncekiHarcamalarToplam ?? Convert.ToDecimal(0);
                              await Update(hrExpenseTable);
                            }
                        }
                        else
                        {

                            entity.approvedAmount=entity.amount;

                        }

                        await Update(entity);
                    }
                    return 1;

                }
                catch (Exception )
                {
                    return 4;
                }
            }

            public async Task<int> ceoOnayAmirOnayi(int tripId, int userId)
            {
                try
                {
                    List<Data.Models.HRExpenseTable> listHarcamalar = findByTripIdActive(tripId);
                    foreach (Data.Models.HRExpenseTable entity in listHarcamalar)
                    {
                        entity.approvedAmount=entity.approvedAmount;
                       await Update(entity);

                    }
                    return 1;
                }
                catch (Exception )
                {
                    return 4;
                }
            } 
            
            public List<Data.Models.HRExpenseTable> findByTripIdActive(int tripId)
            {
                return dal.Get(u => u.enabled && u.tripId == tripId && u.currentStateId == 1).ToList();
            }
        }
    }
}
