using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AskalePortal.Data.Models;
using AskalePortal.Data.ResponseModels;
using AskalePortal.Data.SAP.InputParams;
using AskalePortal.Data.SAP.OutputParams;
using AutoMapper;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using SapNwRfc;
using static System.Runtime.InteropServices.JavaScript.JSType;



namespace AskalePortal.BLL
{
    public partial class BLLActions
    {
        public class HRExpenseWithOutTable : BaseBLL<AskalePortal.Data.Models.HRExpenseWithOutTable>
        {
            private IConfiguration _configuration; private IWebHostEnvironment _env; private readonly IMapper _mapper;
            public HRExpenseWithOutTable(IConfiguration configuration, IWebHostEnvironment env, IMapper mapper) : base(configuration, env)
            {
                _configuration = configuration;
                _env = env;
                _mapper = mapper;
            }

            public int GetMaxtripId()
            {
                return GetAll().Count() == 0 ? 0 : GetAll().Max(u => u.tripId);
            }

            public List<AskalePortal.Data.Models.HRExpenseWithOutTable> GetByTrip(int tripId)
            {
                return dal.Get(u => u.tripId == tripId && u.enabled == true).Include(u => u.expenseType).Include(u => u.islemTuru).ToList();
            }



            public List<AskalePortal.Data.Models.HRExpenseWithOutTable> GetUnapproved(int userId)
            {
                var q = dal.Get(u => (u.currentUserId == userId) && ((u.currentStateId == 1 && u.approval == null) || (u.currentStateId == 2 && u.approval == false)) && u.enabled == true && u.lastApproved == null).ToList();
                return q;
            }

            public List<AskalePortal.Data.Models.HRExpenseWithOutTable> GetUnapprovedByTripId(int userId, int tripId)
            {
                var q = dal.Get(u => (u.createdUserId == userId || u.currentUserId == userId) && u.tripId == tripId && ((u.currentStateId == 1 && u.approval == null) || (u.currentStateId == 2 && u.approval == false)) && u.enabled == true && u.lastApproved == null).ToList();
                return q;
            }

            public List<AskalePortal.Data.Models.HRExpenseWithOutTable> GetByTripAndDateAndExpenseType(int tripID, DateTime spendingTime, int expenseTypeId)
            {
                return dal.Get(u => u.enabled == true && u.tripId == tripID && u.spendingTime == spendingTime && u.expenseTypeId == expenseTypeId).ToList();

            }

            public List<AskalePortal.Data.Models.HRExpenseWithOutTable> GetAllByUser(int userId)
            {
                return dal.Get(u => u.trip.userId == userId && u.enabled == true).OrderByDescending(u => u.tripId).ToList();
            }


            //public string? SavetoSap(int tripId)
            //{
            //    SapConnection? sapConn =   SAPConnectionBLL.sapConnection(_configuration,_env);
            //    if(sapConn == null)
            //    {

            //    }
            //    else
            //    {
            //        sapConn.Connect();
            //        ISapFunction function = sapConn.CreateFunction("ZWEBI033");

            //        //IRfcTable table = function.("IM_TABLE", true);

            //        List<AskalePortal.Data.Models.HRExpenseWithOutTable> list = dal.Get(u => u.tripId == tripId && u.enabled == true && u.lastApproved == true).ToList();
            //        List<AskalePortal.Data.Models.HRExpenseWithOutTable> listHRExpenseTableFinal = new List<AskalePortal.Data.Models.HRExpenseWithOutTable>();
            //        foreach (var item in list)
            //        {
            //            if (item.HRExpenseTypeTable.toplamaNo == true)
            //            {
            //                if (listHRExpenseTableFinal.Where(u => u.spendingTime == item.spendingTime && u.expenseTypeId == item.expenseTypeId).Count() == 0)
            //                {
            //                    listHRExpenseTableFinal.Add(item);
            //                }
            //                else
            //                {
            //                    Data.Models.HRExpenseWithOutTable? hRExpenseTables = listHRExpenseTableFinal.Where(u => u.spendingTime == item.spendingTime && u.expenseTypeId == item.expenseTypeId).FirstOrDefault();
            //                    if (hRExpenseTables != null)
            //                    {
            //                        hRExpenseTables.amount += item.amount;
            //                    }


            //                }
            //            }
            //            else
            //            {
            //                listHRExpenseTableFinal.Add(item);
            //            }
            //        }
            //        BLLActions.HRExpenseTypeTable bllHRExpenseTypeTable = new BLLActions.HRExpenseTypeTable(_configuration,_env);
            //        List<string> harcamalar = bllHRExpenseTypeTable.GetAll().Select(u => u.sapSide).Distinct().ToList();
            //        List<HRExpenseInsertedIntoSap> listHRExpenseInsertedIntoSaps = new List<HRExpenseInsertedIntoSap>();
            //        foreach (var item in listHRExpenseTableFinal.GroupBy(u => u.tripId))
            //        {

            //            string ara = DateTime.Now.Month < 10 ? "0" : "";
            //            string? gidilecekYer = item.FirstOrDefault()?.HRExpenseWithOutTripTable.HRDestinationLocationTable.destinationLocation == "Diğer" ? item.FirstOrDefault()?.HRExpenseWithOutTripTable.digerDestination : item.FirstOrDefault()?.HRExpenseWithOutTripTable.HRDestinationLocationTable.destinationLocation;
            //            int sayi = 0;
            //            HRExpenseInsertedIntoSap table = new HRExpenseInsertedIntoSap();

            //            table.PERNR = item.FirstOrDefault()?.HRExpenseWithOutTripTable.AdminUser.perNo;
            //            table.SUBTY = item.FirstOrDefault()?.HRExpenseWithOutTripTable.HRDestinationLocationTable.geziYeriId.ToString();
            //            table.ENDDA = new DateTime(9999, 12, 31).ToShortDatestring();
            //            table.BEGDA = item.FirstOrDefault()?.HRExpenseWithOutTripTable.gidisTarihi;
            //            table.AEDTM = DateTime.Now;
            //            table.UNAME = "ASKALEBTM";
            //            table.PAPER = Convert.ToInt64(DateTime.Now.Year.ToString() + ara + DateTime.Now.Month.ToString());
            //            table.NEDEN = item.FirstOrDefault()?.HRExpenseWithOutTripTable.tripDescription;
            //            table.GVAYER01 = gidilecekYer?.ToUpper();
            //            table.GVARDA01 = item.FirstOrDefault()?.HRExpenseWithOutTripTable.gidisTarihi;
            //            table.GVARUZ01 = "00:00:00";
            //            table.DCIYER01 = gidilecekYer?.ToUpper();
            //            table.DVARDA01 = item.FirstOrDefault()?.HRExpenseWithOutTripTable.donusTarihi;
            //            table.DVARUZ01 = "00:00:00";

            //            foreach (var items in harcamalar)
            //            {
            //                decimal deger = item.Where(u => u.HRExpenseTypeTable.sapSide == items).Sum(u => u.approvedAmount);
            //                if (deger == 0)
            //                {
            //                    continue;
            //                }
            //                else
            //                {
            //                    sayi++;
            //                    if (sayi == 1)
            //                    {
            //                        table.KOBES01= items;
            //                        table.BETRG01= deger;
            //                    }
            //                    else if (sayi == 2)
            //                    {
            //                        table.KOBES02= items;
            //                        table.BETRG02= deger;
            //                    }
            //                    else if (sayi == 3)
            //                    {
            //                        table.KOBES03= items;
            //                        table.BETRG03= deger;
            //                    }
            //                    else if (sayi == 4)
            //                    {
            //                        table.KOBES04= items;
            //                        table.BETRG04= deger;
            //                    }
            //                    else if (sayi == 5)
            //                    {
            //                        table.KOBES05= items;
            //                        table.BETRG05=deger;
            //                    }
            //                    else if (sayi == 6)
            //                    {
            //                        table.KOBES06=items;
            //                        table.BETRG06= deger;
            //                    }
            //                }
            //            }

            //            table.TMAST=item.Sum(u => u.approvedAmount);

            //            table.WAERS= "TRY";
            //            listHRExpenseInsertedIntoSaps.Add(table);

            //        }



            //        ;
            //        HRExpenseOutput donendeger = function.Invoke<HRExpenseOutput>(input: new HRExpenseInputIntoSap
            //        {
            //            list = listHRExpenseInsertedIntoSaps.ToArray()
            //        });



            //        return donendeger.EV_MESSAGE;
            //    }
            //    return null;
            //}

            public HRExpenseSaveOutput? SavetoSapCo(int tripId)
            {
                HRExpenseSaveOutput? hRExpenseSaveOutput = null;
                List<AskalePortal.Data.Models.HRExpenseWithOutTable> list = dal.Get(u => u.tripId == tripId && u.enabled == true && u.lastApproved == true).ToList();
                List<AskalePortal.Data.Models.HRExpenseWithOutTable> listHRExpenseTableFinal = new List<AskalePortal.Data.Models.HRExpenseWithOutTable>();
                foreach (var item in list)
                {
                    if (item.expenseType.toplamaNo == true)
                    {
                        if (listHRExpenseTableFinal.Where(u => u.spendingTime == item.spendingTime && u.expenseTypeId == item.expenseTypeId).Count() == 0)
                        {
                            listHRExpenseTableFinal.Add(item);
                        }
                        else
                        {
                            Data.Models.HRExpenseWithOutTable? hRExpenseTables = listHRExpenseTableFinal.Where(u => u.spendingTime == item.spendingTime && u.expenseTypeId == item.expenseTypeId).FirstOrDefault();
                            if (hRExpenseTables != null)
                            {
                                hRExpenseTables.amount += item.amount;
                            }


                        }
                    }
                    else
                    {
                        listHRExpenseTableFinal.Add(item);
                    }
                }
                Data.Models.HRExpenseWithOutTable? hRExpenseTable = listHRExpenseTableFinal.FirstOrDefault();
                string text1 = hRExpenseTable?.trip.user.name.ToUpper() + " HARCAMA KAYDI";
                string text2 = hRExpenseTable?.trip.user.name.ToUpper() + " HARCAMA TOPLAMI";
                BLLActions.SAPConnectionData bllSapConnection = new BLLActions.SAPConnectionData(_configuration, _env);
                SapConnection? sapConn = bllSapConnection.sapConnection(_configuration, _env);
                if (sapConn == null)
                {

                }
                else
                {
                    sapConn.Connect();
                    ISapFunction function = sapConn.CreateFunction("ZWEBI039");
                    List<HRExpenseInput2> hRExpenseInput2s = new List<HRExpenseInput2>();


                    foreach (var item in listHRExpenseTableFinal)
                    {
                        HRExpenseInput2 table = new HRExpenseInput2();

                        table.SGTXT = item.expenseType.expenseTypeName + " : " + StripHTML(item.tripDesciption);
                        table.WRBTR = item.approvedAmount.ToString();
                        if (item.kdvOrani == "0")
                        {
                            table.MWSKZ = "V0";
                        }
                        else if (item.kdvOrani == "1")
                        {
                            table.MWSKZ = "V1";
                        }
                        else if (item.kdvOrani == "8")
                        {
                            table.MWSKZ = "V2";
                        }
                        else if (item.kdvOrani == "18")
                        {
                            table.MWSKZ = "V3";
                        }
                        if (item.islemTuru.islemTuruShort == "F")
                        {
                            table.FATURAMI = "X";
                        }
                        else
                        {
                            table.FATURAMI = " ";
                        }
                        if (item.expenseType.expenseTypeName.ToLower().Contains("ucak") ||
                            item.expenseType.expenseTypeName.ToLower().Contains("uçak"))
                        {
                            table.UCAKMI = "X";
                        }
                        else
                        {
                            table.UCAKMI = " ";
                        }
                        hRExpenseInput2s.Add(table);

                    }

                    HRExpenseInputToSave hRExpenseInput1 = new();
                    hRExpenseInput1.TARIH = hRExpenseTable?.trip.donusTarihi?.ToString("dd.MM.yyyy");
                    hRExpenseInput1.TEXT1 = text1;
                    hRExpenseInput1.TEXT2 = text2;
                    hRExpenseInput1.SIRKET = hRExpenseTable?.trip.user.company.vkorg;
                    hRExpenseInput1.PERNO = hRExpenseTable?.trip.user.perNo;

                    hRExpenseInput1.hRExpenseInput2 = hRExpenseInput2s.ToArray();



                    hRExpenseSaveOutput = function.Invoke<HRExpenseSaveOutput?>(input: hRExpenseInput1);

                }






                return hRExpenseSaveOutput;
            }
            public static string StripHTML(string input)
            {
                if (string.IsNullOrEmpty(input))
                    return "";
                return Regex.Replace(input, "(<([^>]+)>|&nbsp;)", string.Empty);
            }

            public List<AskalePortal.Data.Models.HRExpenseWithOutTable> GetByExpenseTypeAndTripIdByTotal(int expenseTypeId, int tripID)
            {
                return dal.Get(u => u.tripId == tripID && u.expenseTypeId == expenseTypeId && u.expenseType.harcamaBoyu == true && u.enabled == true).ToList();
            }
            public List<AskalePortal.Data.Models.HRExpenseWithOutTable> GetByExpenseTypeAndTripIdByDay(int expenseTypeId, int tripID)
            {
                return dal.Get(u => u.tripId == tripID && u.expenseTypeId == expenseTypeId && u.expenseType.toplamaNo == true && u.enabled == true).ToList();
            }

            public List<AskalePortal.Data.Models.HRExpenseWithOutTable> GetAllActiveByUser(int hrmanager, int userId)
            {
                return dal.Get(u => u.enabled == true && u.currentStateId == 1 && u.trip.userId == userId && u.currentUserId == hrmanager).ToList();

            }

            public List<AskalePortal.Data.Models.HRExpenseWithOutTable> GetAllFinished(string name, int? destinationLocationGidis, int? destinationLocationDonus, DateTime? gidisT, DateTime? gidisD, string aciklama, int activePage, int pageSize)
            {
                var q = dal.Get(u => (u.currentStateId == 4) && (string.IsNullOrEmpty(name) ? true : u.currentUser.name.ToLower().Contains(name)) && (destinationLocationGidis.HasValue ? u.trip.destinationLocationId == destinationLocationGidis : true)
                && (destinationLocationDonus.HasValue ? u.trip.tripDescriptionId == destinationLocationDonus : true) && (gidisT.HasValue ? u.trip.gidisTarihi == gidisT : true)
                && (gidisD.HasValue ? u.trip.gidisTarihi == gidisD : true) && (string.IsNullOrEmpty(aciklama) ? true : u.trip.tripDesciption.Contains(aciklama))
                && (u.enabled == true)).OrderByDescending(u => u.Id).ToList();
                return q;
            }

            public List<AskalePortal.Data.Models.HRExpenseWithOutTable> GetUserTripFinished(int userID, string name, int? destinationLocationGidis, int? destinationLocationDonus, DateTime? gidisT, DateTime? gidisD, string aciklama, int activePage, int pageSize)
            {
                var q = dal.Get(u => (u.trip.userId == userID) && (u.currentStateId == 4) && (string.IsNullOrEmpty(name) ? true : u.currentUser.name.ToLower().Contains(name)) && (destinationLocationGidis.HasValue ? u.trip.destinationLocationId == destinationLocationGidis : true)
              && (destinationLocationDonus.HasValue ? u.trip.tripDescriptionId == destinationLocationDonus : true) && (gidisT.HasValue ? u.trip.gidisTarihi == gidisT : true)
              && (gidisD.HasValue ? u.trip.gidisTarihi == gidisD : true) && (string.IsNullOrEmpty(aciklama) ? true : u.trip.tripDesciption.Contains(aciklama))
              && (u.enabled == true)).OrderByDescending(u => u.Id).ToList();
                return q;
            }

            public List<AskalePortal.Data.Models.HRExpenseWithOutTable> GetByTripID(int tripId)
            {
                return dal.Get(u => u.enabled == true && u.tripId == tripId).ToList();
            }

            public int approvalCount(int id)
            {
                int deger = dal.Get(k => k.enabled == true && k.currentUserId == id && k.currentStateId == 1).GroupBy(k => k.tripId).Count();
                return deger;
            }

            public List<Data.Models.HRExpenseWithOutTable> findByUserIdActive(int? currentUserId, int tripUserId)
            {
                List<Data.Models.HRExpenseWithOutTable> liste = dal.Get(u => u.enabled && u.trip.enabled && u.trip.userId == tripUserId && u.currentStateId == 1 && u.currentUserId == currentUserId).ToList();
                return liste;
            }

            public async Task<Data.Models.HRExpenseWithOutTable> save(HRExpenseWithOutTableSaveDto entity, int userId)
            {
                BLLActions.AdminUsers bllAdminUsers = new BLLActions.AdminUsers(_configuration, _env, _mapper);
                AdminUser user = bllAdminUsers.GetByID(userId)!;

                BLLActions.HRExpenseTypeTable bllHRExpenseTypeTable = new BLLActions.HRExpenseTypeTable(_configuration, _env);
                Data.Models.HRExpenseTypeTable hrExpenseTypeTable = bllHRExpenseTypeTable.GetByID(entity?.expenseTypeId ?? 0)!;

                BLLActions.HRExpenseAmount bllHRExpenseAmount = new BLLActions.HRExpenseAmount(_configuration, _env);
                Data.Models.HRExpenseAmount hrExpenseAmount = bllHRExpenseAmount.getbycalisanturuidandharcamaturuid(
                        user.calisanTuruId, entity!.expenseTypeId, entity.spendingTime);

                if (entity.id == 0)
                {

                    if (hrExpenseTypeTable.toplamaNo)
                    {

                        decimal oncekiHarcamalarToplam = getByTypeAndDateSumAmount(entity.tripId,
                                entity.expenseTypeId, entity.spendingTime);
                        decimal toplamSave;
                        decimal toplam = Convert.ToDecimal(entity.amount) + (oncekiHarcamalarToplam);
                        List<Data.Models.HRExpenseWithOutTable> listHrExpenseWithOutTable =
                                getByTypeAndDate(entity.tripId, entity.expenseTypeId, entity.spendingTime);

                        int totalDays = (entity.kalinanGunSayisi + entity.otoparkGunSayisi == 0 ? 1
                                : entity.kalinanGunSayisi + entity.otoparkGunSayisi) ?? 0;
                        decimal totalLimit = hrExpenseAmount.harcirahMiktari * (Convert.ToDecimal(totalDays));

                        if (toplam.CompareTo(totalLimit) >= 1)
                        {
                            // toplam büyükse
                            toplamSave = totalLimit;

                        }
                        else
                        {
                            toplamSave = toplam;
                        }
                        entity.totalLimitAmount = totalLimit;
                        entity.approvedAmount = toplamSave;
                        foreach (Data.Models.HRExpenseWithOutTable hrExpenseWithOutTable in listHrExpenseWithOutTable)
                        {
                            hrExpenseWithOutTable.approvedAmount = (toplamSave);
                            await Update(hrExpenseWithOutTable);
                        }
                    }
                    else if (hrExpenseTypeTable.otoparkMi || hrExpenseTypeTable.harcamaBoyu)
                    {
                        int totalDays = (entity.kalinanGunSayisi + entity.otoparkGunSayisi == 0 ? 1
                                : entity.kalinanGunSayisi + entity.otoparkGunSayisi) ?? 0;
                        decimal totalLimit = hrExpenseAmount.harcirahMiktari * totalDays;

                        entity.totalLimitAmount = totalLimit;

                        if (entity.amount > totalLimit)
                        {
                            // toplam büyükse
                            entity.approvedAmount = totalLimit;

                        }
                        else
                        {
                            entity.approvedAmount = (entity.amount);
                        }

                    }
                    else
                    {

                        if (entity.amount > hrExpenseAmount.harcirahMiktari)
                        {
                            // toplam büyükse
                            entity.approvedAmount = (hrExpenseAmount.harcirahMiktari);

                        }
                        else
                        {
                            entity.approvedAmount = (entity.amount);
                        }
                        entity.totalLimitAmount = (hrExpenseAmount.harcirahMiktari);
                    }

                    entity.createdUserId = (userId);
                    entity.createdDate = (DateTime.Now).ToString();
                    entity.enabled = (true);
                    return await Add(_mapper.Map<Data.Models.HRExpenseWithOutTable>(entity)) ?? new Data.Models.HRExpenseWithOutTable();
                }
                else
                {
                    if (hrExpenseTypeTable.toplamaNo)
                    {
                        decimal oncekiHarcamalarToplam = getByTypeAndDateSumAmountEdit(entity.tripId,
                                entity.expenseTypeId, entity.spendingTime, entity.id);
                        decimal toplamSave;
                        decimal toplam = (entity.amount + oncekiHarcamalarToplam) ?? 0;
                        List<Data.Models.HRExpenseWithOutTable> listHrExpenseWithOutTable =
                                getByTypeAndDate(entity.tripId, entity.expenseTypeId, entity.spendingTime);

                        int totalDays = (entity.kalinanGunSayisi + entity.otoparkGunSayisi == 0 ? 1
                                : entity.kalinanGunSayisi + entity.otoparkGunSayisi) ?? 0;
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
                        entity.approvedAmount = (toplamSave);
                        foreach (Data.Models.HRExpenseWithOutTable hrExpenseTable in listHrExpenseWithOutTable)
                        {
                            hrExpenseTable.approvedAmount = (toplamSave);
                            await Update(hrExpenseTable);
                        }
                    }
                    else if (hrExpenseTypeTable.otoparkMi || hrExpenseTypeTable.harcamaBoyu)
                    {
                        int totalDays = (entity.kalinanGunSayisi + entity.otoparkGunSayisi == 0 ? 1
                                : entity.kalinanGunSayisi + entity.otoparkGunSayisi) ?? 0;
                        decimal totalLimit = hrExpenseAmount.harcirahMiktari * (Convert.ToDecimal(totalDays));

                        entity.totalLimitAmount = (totalLimit);

                        if (entity.amount > totalLimit)
                        {
                            // toplam büyükse
                            entity.approvedAmount = (totalLimit);

                        }
                        else
                        {
                            entity.approvedAmount = (entity.amount);
                        }

                    }
                    else
                    {

                        if (entity.amount > hrExpenseAmount.harcirahMiktari)
                        {
                            // toplam büyükse
                            entity.approvedAmount = (hrExpenseAmount.harcirahMiktari);

                        }
                        else
                        {
                            entity.approvedAmount = (entity.amount);
                        }
                        entity.totalLimitAmount = (hrExpenseAmount.harcirahMiktari);
                    }
                    entity.updatedUserId = (userId);
                    entity.updateDate = (DateTime.Now.ToString());
                    entity.enabled = (true);
                    return await Update(_mapper.Map<Data.Models.HRExpenseWithOutTable>(entity));
                }


            }

            private decimal getByTypeAndDateSumAmountEdit(int? tripId, int? expenseTypeId, string? spendingTime, int? id)
            {
                var result = dal.Get(c => c.enabled &&
                           c.tripId == tripId &&
                           c.expenseTypeId == expenseTypeId &&
                           c.spendingTime == DateTime.Parse(spendingTime??"") &&
                           c.Id != id &&
                           c.currentStateId == 1);

                // Sum hesapla
                decimal totalAmount = result.Sum(c => (decimal?)c.amount) ?? 0m;

                return totalAmount;
            }

            private List<Data.Models.HRExpenseWithOutTable> getByTypeAndDate(int? tripId, int? expenseTypeId, string? spendingTime)
            {
                List<Data.Models.HRExpenseWithOutTable> liste = dal.Get(u => u.enabled && u.tripId == tripId && u.expenseTypeId == expenseTypeId && u.spendingTime == DateTime.Parse(spendingTime??"") && u.currentStateId == 1).ToList();
                return liste;
            }

            private decimal getByTypeAndDateSumAmount(int? tripId, int? expenseTypeId, string? spendingTime)
            {
                DateTime parsedDate = DateTime.Parse(spendingTime!);

                var result = dal.Get(
                 x => x.enabled == true
                 && x.tripId == tripId
                 && x.expenseTypeId == expenseTypeId
                 && x.spendingTime == parsedDate
                 && x.currentStateId == 1).Sum(x => (decimal?)x.amount) ?? 0m;

                return result;
            }

            public List<Data.Models.HRExpenseWithOutTable> listByTripId(int tripId)
            {
                List<Data.Models.HRExpenseWithOutTable> liste = dal.Get(u => u.enabled && u.tripId == tripId && u.currentStateId != 2).OrderByDescending(u => u.Id).ToList();
                return liste;
            }

            public List<HRExpenseDto> mylistExpense(int tripId)
            {
                //List<Data.Models.HRExpenseWithOutTable>? list = dal.Get(a => a.enabled && a.tripId == tripId).OrderByDescending(u => u.Id).ToList();
                //List<HRExpenseDto>? dto = list.Select(a => new HRExpenseDto
                //{
                //    id = a.Id,
                //    file = a.fileNames ?? "",
                //    harcamaTuru = a.expenseType?.expenseTypeName ?? "",
                //    harcamaTarihi = a.spendingTime,
                //    gunSayisi = a.kalinanGunSayisi,
                //    toplamLimit = a.totalLimitAmount,
                //    harcamaTutari = a.amount,
                //    onaylananMasraf = a.approvedAmount,
                //    aciklama = a.tripDesciption,
                //    approval = a.approval,
                //    currentStateId = a.currentStateId,
                //    currentUserId = a.currentUserId,
                //    onaySirasi = a.onaySirasi

                //}).ToList();

                var query = from a in dal.dB.HRExpenseWithOutTable
                            join et in dal.dB.HRExpenseTypeTable
                                 on a.expenseTypeId equals et.Id into etGroup
                            from et in etGroup.DefaultIfEmpty()
                            where a.enabled && a.tripId == tripId
                            orderby a.Id descending
                            select new HRExpenseDto
                            {
                                id = a.Id,
                                file = a.fileNames ?? "",
                                harcamaTuru = et.expenseTypeName ?? "",
                                harcamaTarihi = (a.spendingTime ?? DateTime.Now).ToString("dd.MM.yyyy"),
                                gunSayisi = a.kalinanGunSayisi,
                                toplamLimit = a.totalLimitAmount,
                                harcamaTutari = a.amount,
                                onaylananMasraf = a.approvedAmount,
                                aciklama = a.tripDesciption,
                                approval = a.approval,
                                currentStateId = a.currentStateId,
                                currentUserId = a.currentUserId,
                                onaySirasi = a.onaySirasi
                            };

                List<HRExpenseDto> dto = query.ToList();
                return dto ?? [];
            }
            // 1->onaylandı
            // 2-> onaylayıcıları kontrol edin
            // 3->bitti
            // 4->hata
            public async Task<int> confirmAll(int tripId, int userId)
            {

                //await dal.dB.Database.BeginTransactionAsync();
                //using (var transaction = await dal.dB.Database.BeginTransactionAsync())
                //{

                    try
                    {
                        int donenDeger = 0;

                        BLLActions.CeoTable bllCeoTable = new BLLActions.CeoTable(_configuration, _env);
                        var ceo = bllCeoTable.GetByID(1);

                        BLLActions.HRExpenseWithOutTripTable bllHRExpenseWithOutTripTable = new BLLActions.HRExpenseWithOutTripTable(_configuration, _env, _mapper);
                        Data.Models.HRExpenseWithOutTripTable? hrExpenseTripTable = bllHRExpenseWithOutTripTable.GetByID(tripId);

                        List<Data.Models.HRExpenseWithOutTable> listHRExpenseTable = findByTripIdAndCurrentUserIdAndEnabledActive(tripId, userId, true);
                        BLLActions.AdminUsers bllAdminUsers = new BLLActions.AdminUsers(_configuration, _env, _mapper);
                        var ceoUser = bllAdminUsers.GetByID(ceo!.userId);
                        var tripUser = bllAdminUsers.GetByID(hrExpenseTripTable!.userId);
                        var hrEmployer1 = bllAdminUsers.GetByID(tripUser!.hremployer1 ?? 0);
                        bool fazlaHarcamaVarmi = listHRExpenseTable.Any(u => u.approvedAmount > u.totalLimitAmount);
                        if (tripUser.Id.Equals(userId))
                        {
                            if (hrEmployer1 != null)
                            {
                                approveHRExpenseTable(listHRExpenseTable, hrEmployer1, 1, 1, tripUser, tripId, ceo);
                                onay(null, hrEmployer1, tripId, fazlaHarcamaVarmi, ceo);
                                donenDeger = 1;

                                BLLActions.EmailMessages bllEmailMessages = new BLLActions.EmailMessages(_configuration, _env);
                                EmailMessage emailMessage = new EmailMessage();
                                emailMessage.subject = ("Bekleyen Harcama Onayı hk.");
                                emailMessage.toAddress = (hrEmployer1.email);

                                BLLActions.EmailReaderFile bllEmailReaderFile = new BLLActions.EmailReaderFile();
                                string mailMessage = bllEmailReaderFile.BuildEmailTemplate(_configuration, _env, "Sayın " + hrEmployer1.name +
                                 "Harcama Onayı hk.",
                                            tripId.ToString() + " ID kodlu Harcama onayınızı beklemektedir.");

                                emailMessage.emailText = (mailMessage);
                                emailMessage.mailTuru = (3);
                                emailMessage.enabled = (true);
                                emailMessage.isSent = (false);
                                emailMessage.plannedDate = (DateTime.Now);
                                await bllEmailMessages.Add(emailMessage);

                            }
                            else
                            {
                                donenDeger = 2;
                            }

                        }
                        else if (userId.Equals(hrEmployer1?.Id))
                        {

                            if (tripUser.manager1 != null)
                            {
                                AdminUser? manager1 = bllAdminUsers.GetByID(tripUser.manager1 ?? 0);
                                if (tripUser.hremployer1 != tripUser.manager1)
                                {
                                    approveHRExpenseTable(listHRExpenseTable, manager1!, 2, 1, tripUser, tripId, ceo);
                                    onay(hrEmployer1, manager1, tripId, fazlaHarcamaVarmi, ceo);
                                    donenDeger = 1;
                                    if (manager1!.Id == ceo.userId)
                                    {
                                        BLLActions.SMSMessages bllSMSMessages = new BLLActions.SMSMessages(_configuration, _env);
                                        SMSMessage smsMessage = new SMSMessage();
                                        smsMessage.plannedDate = (DateTime.Now);
                                        smsMessage.isSent = (false);
                                        smsMessage.smsText = (tripId.ToString() + "Id'li bölgesel harcama onayınızı beklemektedir.");
                                        smsMessage.toNumbers = (ceoUser?.mobile);

                                        await bllSMSMessages.Add(smsMessage);

                                    }
                                    else
                                    {
                                        BLLActions.EmailMessages bllEmailMessages = new BLLActions.EmailMessages(_configuration, _env);
                                        EmailMessage emailMessage = new EmailMessage();
                                        emailMessage.subject = ("Bekleyen Harcama Onayı hk.");
                                        emailMessage.toAddress = (manager1.email);

                                        BLLActions.EmailReaderFile bllEmailReaderFile = new BLLActions.EmailReaderFile();
                                        string mailMessage = bllEmailReaderFile.BuildEmailTemplate(_configuration, _env, "Sayın " + manager1.name +
                                         "Harcama Onayı hk.",
                                                    tripId.ToString() + " ID kodlu Harcama onayınızı beklemektedir.");


                                        emailMessage.emailText = (mailMessage);
                                        emailMessage.mailTuru = (3);
                                        emailMessage.enabled = (true);
                                        emailMessage.isSent = (false);
                                        emailMessage.plannedDate = (DateTime.Now);
                                        await bllEmailMessages.Add(emailMessage);
                                    }
                                }
                                else
                                {
                                    if (tripUser.manager2 != null)
                                    {
                                        AdminUser manager2 = bllAdminUsers.GetByID(tripUser.manager2 ?? 0)!;
                                        approveHRExpenseTable(listHRExpenseTable, manager2, 3, 1, tripUser, tripId, ceo);
                                        onay(manager1, manager2, tripId, fazlaHarcamaVarmi, ceo);
                                        donenDeger = 1;
                                        if (manager2.Id == ceo.userId)
                                        {
                                            BLLActions.SMSMessages bllSMSMessages = new BLLActions.SMSMessages(_configuration, _env);
                                            SMSMessage smsMessage = new SMSMessage();
                                            smsMessage.plannedDate = (DateTime.Now);
                                            smsMessage.isSent = (false);
                                            smsMessage.smsText = (
                                                    tripId.ToString() + "Id'li bölgesel harcama onayınızı beklemektedir.");
                                            smsMessage.toNumbers = (ceoUser!.mobile);

                                            await bllSMSMessages.Add(smsMessage);

                                        }
                                        else
                                        {
                                            BLLActions.EmailMessages bllEmailMessages = new BLLActions.EmailMessages(_configuration, _env);
                                            EmailMessage emailMessage = new EmailMessage();
                                            emailMessage.subject = ("Bekleyen Harcama Onayı hk.");
                                            emailMessage.toAddress = (manager2.email);

                                            BLLActions.EmailReaderFile bllEmailReaderFile = new BLLActions.EmailReaderFile();
                                            string mailMessage = bllEmailReaderFile.BuildEmailTemplate(_configuration, _env, "Sayın " + manager2.name +
                                             "Harcama Onayı hk.",
                                                        tripId.ToString() + " ID kodlu Harcama onayınızı beklemektedir.");

                                            emailMessage.emailText = (mailMessage);
                                            emailMessage.mailTuru = (3);
                                            emailMessage.enabled = (true);
                                            emailMessage.isSent = (false);
                                            emailMessage.plannedDate = (DateTime.Now);
                                            await bllEmailMessages.Add(emailMessage);
                                        }
                                    }
                                    else
                                    {
                                        if (tripUser.manager3 == null && tripUser.manager4 == null)
                                        {
                                            approveHRExpenseTable(listHRExpenseTable, manager1!, 10, 4, tripUser, tripId, ceo);
                                            onay(manager1, null, tripId, fazlaHarcamaVarmi, ceo);

                                            BLLActions.EmailMessages bllEmailMessages = new BLLActions.EmailMessages(_configuration, _env);
                                            EmailMessage emailMessage = new EmailMessage();
                                            emailMessage.subject = ("Bekleyen Harcama Onayı hk.");
                                            emailMessage.toAddress = (tripUser.email);

                                            BLLActions.EmailReaderFile bllEmailReaderFile = new BLLActions.EmailReaderFile();
                                            string mailMessage = bllEmailReaderFile.BuildEmailTemplate(_configuration, _env, "Sayın " + tripUser.name +
                                             "Harcama Onayı hk.",
                                                        tripId.ToString() + " ID kodlu Harcamanız onaylanmıştır.");

                                            emailMessage.emailText = (mailMessage);
                                            emailMessage.mailTuru = (3);
                                            emailMessage.enabled = (true);
                                            emailMessage.isSent = (false);
                                            emailMessage.plannedDate = (DateTime.Now);
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
                        else if (userId.Equals(tripUser.manager1) && userId != ceo.userId)
                        {
                            if (tripUser.manager2 != null)
                            {
                                AdminUser manager2 = bllAdminUsers.GetByID(tripUser.manager2 ?? 0)!;
                                AdminUser manager1 = bllAdminUsers.GetByID(tripUser.manager1 ?? 0)!;
                                if (tripUser.manager1 != tripUser.manager2)
                                {
                                    approveHRExpenseTable(listHRExpenseTable, manager2, 3, 1, tripUser, tripId, ceo);
                                    onay(manager1, manager2, tripId, fazlaHarcamaVarmi, ceo);
                                    donenDeger = 1;
                                    if (manager2.Id == ceo.userId)
                                    {
                                        BLLActions.SMSMessages bllSMSMessages = new BLLActions.SMSMessages(_configuration, _env);
                                        SMSMessage smsMessage = new SMSMessage();
                                        smsMessage.plannedDate = (DateTime.Now);
                                        smsMessage.isSent = (false);
                                        smsMessage
                                                .smsText = (tripId.ToString() + "Id'li bölgesel harcama onayınızı beklemektedir.");
                                        smsMessage.toNumbers = (ceoUser?.mobile);

                                        await bllSMSMessages.Add(smsMessage);

                                    }
                                    else
                                    {
                                        BLLActions.EmailMessages bllEmailMessages = new BLLActions.EmailMessages(_configuration, _env);
                                        EmailMessage emailMessage = new EmailMessage();
                                        emailMessage.subject = ("Bekleyen Harcama Onayı hk.");
                                        emailMessage.toAddress = (manager2.email);

                                        BLLActions.EmailReaderFile bllEmailReaderFile = new BLLActions.EmailReaderFile();
                                        string mailMessage = bllEmailReaderFile.BuildEmailTemplate(_configuration, _env, "Sayın " + manager2.name +
                                         "Harcama Onayı hk.",
                                                    tripId.ToString() + " ID kodlu Harcama onayınızı beklemektedir.");

                                        emailMessage.emailText = (mailMessage);
                                        emailMessage.mailTuru = (3);
                                        emailMessage.enabled = (true);
                                        emailMessage.isSent = (false);
                                        emailMessage.plannedDate = (DateTime.Now);
                                        await bllEmailMessages.Add(emailMessage);
                                    }
                                }
                                else
                                {
                                    if (tripUser.manager3 != null)
                                    {
                                        AdminUser manager3 = bllAdminUsers.GetByID(tripUser.manager3 ?? 0)!;
                                        approveHRExpenseTable(listHRExpenseTable, manager3, 4, 1, tripUser, tripId, ceo);
                                        onay(manager2, manager3, tripId, fazlaHarcamaVarmi, ceo);
                                        donenDeger = 1;
                                        if (manager3.Id == ceo.userId)
                                        {
                                            BLLActions.SMSMessages bllSMSMessages = new BLLActions.SMSMessages(_configuration, _env);
                                            SMSMessage smsMessage = new SMSMessage();
                                            smsMessage.plannedDate = (DateTime.Now);
                                            smsMessage.isSent = (false);
                                            smsMessage.smsText = (
                                                    tripId.ToString() + "Id'li bölgesel harcama onayınızı beklemektedir.");
                                            smsMessage.toNumbers = (ceoUser?.mobile);

                                            await bllSMSMessages.Add(smsMessage);

                                        }
                                        else
                                        {
                                            BLLActions.EmailMessages bllEmailMessages = new BLLActions.EmailMessages(_configuration, _env);
                                            EmailMessage emailMessage = new EmailMessage();
                                            emailMessage.subject = ("Bekleyen Harcama Onayı hk.");
                                            emailMessage.toAddress = (manager3.email);

                                            BLLActions.EmailReaderFile bllEmailReaderFile = new BLLActions.EmailReaderFile();
                                            string mailMessage = bllEmailReaderFile.BuildEmailTemplate(_configuration, _env, "Sayın " + manager3.name +
                                             "Harcama Onayı hk.",
                                                        tripId.ToString() + " ID kodlu Harcama onayınızı beklemektedir.");

                                            emailMessage.emailText = (mailMessage);
                                            emailMessage.mailTuru = (3);
                                            emailMessage.enabled = (true);
                                            emailMessage.isSent = (false);
                                            emailMessage.plannedDate = (DateTime.Now);
                                            await bllEmailMessages.Add(emailMessage);
                                        }
                                    }
                                    else if (tripUser.manager4 == null)
                                    {
                                        approveHRExpenseTable(listHRExpenseTable, manager2, 10, 4, tripUser, tripId, ceo);
                                        onay(manager2, null, tripId, fazlaHarcamaVarmi, ceo);

                                        BLLActions.EmailMessages bllEmailMessages = new BLLActions.EmailMessages(_configuration, _env);
                                        EmailMessage emailMessage = new EmailMessage();
                                        emailMessage.subject = ("Bekleyen Harcama Onayı hk.");
                                        emailMessage.toAddress = (tripUser.email);

                                        BLLActions.EmailReaderFile bllEmailReaderFile = new BLLActions.EmailReaderFile();
                                        string mailMessage = bllEmailReaderFile.BuildEmailTemplate(_configuration, _env, "Sayın " + tripUser.name +
                                         "Harcama Onayı hk.",
                                                    tripId.ToString() + " ID kodlu Harcamanız onaylanmıştır.");
                                        emailMessage.emailText = (mailMessage);
                                        emailMessage.mailTuru = (3);
                                        emailMessage.enabled = (true);
                                        emailMessage.isSent = (false);
                                        emailMessage.plannedDate = (DateTime.Now);
                                        await bllEmailMessages.Add(emailMessage);

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

                                    BLLActions.EmailMessages bllEmailMessages = new BLLActions.EmailMessages(_configuration, _env);
                                    EmailMessage emailMessage = new EmailMessage();
                                    emailMessage.subject = ("Bekleyen Harcama Onayı hk.");
                                    emailMessage.toAddress = (tripUser.email);

                                    BLLActions.EmailReaderFile bllEmailReaderFile = new BLLActions.EmailReaderFile();
                                    string mailMessage = bllEmailReaderFile.BuildEmailTemplate(_configuration, _env, "Sayın " + tripUser.name +
                                     "Harcama Onayı hk.",
                                                tripId.ToString() + " ID kodlu Harcamanız onaylanmıştır.");

                                    emailMessage.emailText = (mailMessage);
                                    emailMessage.mailTuru = (3);
                                    emailMessage.enabled = (true);
                                    emailMessage.isSent = (false);
                                    emailMessage.plannedDate = (DateTime.Now);
                                    await bllEmailMessages.Add(emailMessage);
                                    donenDeger = 3;

                                }
                                else
                                {
                                    donenDeger = 2;
                                }
                            }

                        }
                        else if (userId.Equals(tripUser.manager2) && userId != ceo.userId)
                        {
                            if (tripUser.manager3 != null)
                            {
                                AdminUser manager3 = bllAdminUsers.GetByID(tripUser.manager3 ?? 0)!;
                                AdminUser manager2 = bllAdminUsers.GetByID(tripUser.manager2 ?? 0)!;
                                approveHRExpenseTable(listHRExpenseTable, manager3, 4, 1, tripUser, tripId, ceo);
                                onay(manager2, manager3, tripId, fazlaHarcamaVarmi, ceo);
                                donenDeger = 1;
                                if (tripUser.manager3 == ceo.userId)
                                {
                                    BLLActions.SMSMessages bllSMSMessages = new BLLActions.SMSMessages(_configuration, _env);
                                    SMSMessage smsMessage = new SMSMessage();
                                    smsMessage.plannedDate = (DateTime.Now);
                                    smsMessage.isSent = (false);
                                    smsMessage.smsText = (tripId.ToString() + "Id'li bölgesel harcama onayınızı beklemektedir.");
                                    smsMessage.toNumbers = (ceoUser?.mobile);

                                    await bllSMSMessages.Add(smsMessage);

                                }
                                else
                                {
                                    BLLActions.EmailMessages bllEmailMessages = new BLLActions.EmailMessages(_configuration, _env);
                                    EmailMessage emailMessage = new EmailMessage();
                                    emailMessage.subject = ("Bekleyen Harcama Onayı hk.");
                                    emailMessage.toAddress = (manager3.email);

                                    BLLActions.EmailReaderFile bllEmailReaderFile = new BLLActions.EmailReaderFile();
                                    string mailMessage = bllEmailReaderFile.BuildEmailTemplate(_configuration, _env, "Sayın " + manager3.name +
                                     "Harcama Onayı hk.",
                                                tripId.ToString() + " ID kodlu Harcama onayınızı beklemektedir.");

                                    emailMessage.emailText = (mailMessage);
                                    emailMessage.mailTuru = (3);
                                    emailMessage.enabled = (true);
                                    emailMessage.isSent = (false);
                                    emailMessage.plannedDate = (DateTime.Now);
                                    await bllEmailMessages.Add(emailMessage);
                                }
                            }
                            else if (tripUser.manager4 == null)
                            {
                                AdminUser manager2 = bllAdminUsers.GetByID(tripUser.manager2 ?? 0)!;
                                approveHRExpenseTable(listHRExpenseTable, manager2, 10, 4, tripUser, tripId, ceo);
                                onay(manager2, null, tripId, fazlaHarcamaVarmi, ceo);

                                BLLActions.EmailMessages bllEmailMessages = new BLLActions.EmailMessages(_configuration, _env);
                                EmailMessage emailMessage = new EmailMessage();
                                emailMessage.subject = ("Bekleyen Harcama Onayı hk.");
                                emailMessage.toAddress = (tripUser.email);

                                BLLActions.EmailReaderFile bllEmailReaderFile = new BLLActions.EmailReaderFile();
                                string mailMessage = bllEmailReaderFile.BuildEmailTemplate(_configuration, _env, "Sayın " + tripUser.name +
                                 "Harcama Onayı hk.",
                                            tripId.ToString() + " ID kodlu Harcamanız onaylanmıştır.");

                                emailMessage.emailText = (mailMessage);
                                emailMessage.mailTuru = (3);
                                emailMessage.enabled = (true);
                                emailMessage.isSent = (false);
                                emailMessage.plannedDate = (DateTime.Now);
                                await bllEmailMessages.Add(emailMessage);

                            }
                            else
                            {

                                donenDeger = 2;
                            }

                        }
                        else if (userId.Equals(tripUser.manager3) && userId != ceo.userId)
                        {
                            if (tripUser.manager4 != null)
                            {
                                AdminUser manager3 = bllAdminUsers.GetByID(tripUser.manager3 ?? 0)!;
                                AdminUser manager4 = bllAdminUsers.GetByID(tripUser.manager4 ?? 0)!;
                                approveHRExpenseTable(listHRExpenseTable, manager4, 5, 1, tripUser, tripId, ceo);
                                onay(manager3, manager4, tripId, fazlaHarcamaVarmi, ceo);
                                donenDeger = 1;
                                if (tripUser.manager3 == ceo.userId)
                                {
                                    BLLActions.SMSMessages bllSMSMessages = new BLLActions.SMSMessages(_configuration, _env);
                                    SMSMessage smsMessage = new SMSMessage();
                                    smsMessage.plannedDate = (DateTime.Now);
                                    smsMessage.isSent = (false);
                                    smsMessage.smsText = (tripId.ToString() + "Id'li bölgesel harcama onayınızı beklemektedir.");
                                    smsMessage.toNumbers = (ceoUser?.mobile);

                                    await bllSMSMessages.Add(smsMessage);

                                }
                                else
                                {
                                    BLLActions.EmailMessages bllEmailMessages = new BLLActions.EmailMessages(_configuration, _env);
                                    EmailMessage emailMessage = new EmailMessage();
                                    emailMessage.subject = ("Bekleyen Harcama Onayı hk.");
                                    emailMessage.toAddress = (manager4.email);

                                    BLLActions.EmailReaderFile bllEmailReaderFile = new BLLActions.EmailReaderFile();
                                    string mailMessage = bllEmailReaderFile.BuildEmailTemplate(_configuration, _env, "Sayın " + manager4.name +
                                     "Harcama Onayı hk.",
                                                tripId.ToString() + " ID kodlu Harcama onayınızı beklemektedir.");

                                    emailMessage.emailText = (mailMessage);
                                    emailMessage.mailTuru = (3);
                                    emailMessage.enabled = (true);
                                    emailMessage.isSent = (false);
                                    emailMessage.plannedDate = (DateTime.Now);
                                    await bllEmailMessages.Add(emailMessage);
                                }
                            }
                            else
                            {
                                AdminUser manager3 = bllAdminUsers.GetByID(tripUser.manager3 ?? 0)!;
                                approveHRExpenseTable(listHRExpenseTable, manager3, 10, 4, tripUser, tripId, ceo);
                                onay(manager3, null, tripId, fazlaHarcamaVarmi, ceo);
                                donenDeger = 3;
                                BLLActions.EmailMessages bllEmailMessages = new BLLActions.EmailMessages(_configuration, _env);
                                EmailMessage emailMessage = new EmailMessage();
                                emailMessage.subject = ("Bekleyen Harcama Onayı hk.");
                                emailMessage.toAddress = (tripUser.email);

                                BLLActions.EmailReaderFile bllEmailReaderFile = new BLLActions.EmailReaderFile();
                                string mailMessage = bllEmailReaderFile.BuildEmailTemplate(_configuration, _env, "Sayın " + tripUser.name +
                                 "Harcama Onayı hk.",
                                            tripId.ToString() + " ID kodlu Harcamanız onaylanmıştır.");

                                emailMessage.emailText = (mailMessage);
                                emailMessage.mailTuru = (3);
                                emailMessage.enabled = (true);
                                emailMessage.isSent = (false);
                                emailMessage.plannedDate = (DateTime.Now);
                                await bllEmailMessages.Add(emailMessage);

                            }

                        }
                        else if (userId.Equals(tripUser.manager4) && userId != ceo.userId)
                        {
                            AdminUser manager4 = bllAdminUsers.GetByID(tripUser.manager4 ?? 0)!;
                            approveHRExpenseTable(listHRExpenseTable, manager4, 10, 4, tripUser, tripId, ceo);
                            onay(manager4, null, tripId, fazlaHarcamaVarmi, ceo);
                            donenDeger = 3;
                            if (tripUser.manager4 == ceo.userId)
                            {
                                BLLActions.SMSMessages bllSMSMessages = new BLLActions.SMSMessages(_configuration, _env);
                                SMSMessage smsMessage = new SMSMessage();
                                smsMessage.plannedDate = (DateTime.Now);
                                smsMessage.isSent = (false);
                                smsMessage.smsText = (tripId.ToString() + "Id'li bölgesel harcama onayınızı beklemektedir.");
                                smsMessage.toNumbers = (ceoUser?.mobile);

                                await bllSMSMessages.Add(smsMessage);

                            }
                            else
                            {
                                BLLActions.EmailMessages bllEmailMessages = new BLLActions.EmailMessages(_configuration, _env);
                                EmailMessage emailMessage = new EmailMessage();
                                emailMessage.subject = ("Bekleyen Harcama Onayı hk.");
                                emailMessage.toAddress = (tripUser.email);

                                BLLActions.EmailReaderFile bllEmailReaderFile = new BLLActions.EmailReaderFile();
                                string mailMessage = bllEmailReaderFile.BuildEmailTemplate(_configuration, _env, "Sayın " + tripUser.name +
                                 "Harcama Onayı hk.",
                                            tripId.ToString() + " ID kodlu Harcamanız onaylanmıştır.");

                                emailMessage.emailText = (mailMessage);
                                emailMessage.mailTuru = (3);
                                emailMessage.enabled = (true);
                                emailMessage.isSent = (false);
                                emailMessage.plannedDate = (DateTime.Now);
                                await bllEmailMessages.Add(emailMessage);
                            }
                        }
                        else if (userId.Equals(ceo.userId) || userId == ceo.userId)
                        {

                            approveHRExpenseTable(listHRExpenseTable, ceoUser!, 10, 4, tripUser, tripId, ceo);

                            BLLActions.HRExpenseWithOutDetail bllHRExpenseWithOutDetail = new BLLActions.HRExpenseWithOutDetail(_configuration, _env);
                            Data.Models.HRExpenseWithOutDetail? hrExpenseDetail = bllHRExpenseWithOutDetail.getByActive(tripId,
                                    ceoUser!.Id);
                        if (hrExpenseDetail != null)
                        {
                            hrExpenseDetail.approved = (true);
                            hrExpenseDetail.isReplied = (true);
                            hrExpenseDetail.replyDate = (DateTime.Now);
                            await bllHRExpenseWithOutDetail.Add(hrExpenseDetail);
                        }
                            donenDeger = 3;
                            BLLActions.EmailMessages bllEmailMessages = new BLLActions.EmailMessages(_configuration, _env);
                            EmailMessage emailMessage = new EmailMessage();
                            emailMessage.subject = ("Bekleyen Harcama Onayı hk.");
                            emailMessage.toAddress = (tripUser.email);

                            BLLActions.EmailReaderFile bllEmailReaderFile = new BLLActions.EmailReaderFile();
                            string mailMessage = bllEmailReaderFile.BuildEmailTemplate(_configuration, _env, "Sayın " + tripUser.name +
                             "Harcama Onayı hk.",
                                        tripId.ToString() + " ID kodlu Harcamanız onaylanmıştır.");

                            emailMessage.emailText = (mailMessage);
                            emailMessage.mailTuru = (3);
                            emailMessage.enabled = (true);
                            emailMessage.isSent = (false);
                            emailMessage.plannedDate = (DateTime.Now);
                            await bllEmailMessages.Add(emailMessage);

                        }
                        //await dal.dB.Database.CommitTransactionAsync();
                        //await transaction.CommitAsync();

                        return donenDeger;

                    }
                    catch (Exception e)
                    {
                        //await  dal.dB.Database.RollbackTransactionAsync();
                        //await transaction.RollbackAsync();
                        BLLActions.AdminUsers bllAdminUsers = new BLLActions.AdminUsers(_configuration, _env, _mapper);
                        UserByNameEMailDto byNameEMailDto = bllAdminUsers.getUserByNameAndEmail(userId);
                        Console.WriteLine(byNameEMailDto.name + "," + tripId.ToString()
                                + " id'li bölgesel harcamayı onaylayamadı. Hata: " + e.Message);
                        return 4;
                    }
                //}
            }

            private List<Data.Models.HRExpenseWithOutTable> findByTripIdAndCurrentUserIdAndEnabledActive(int tripId, int userId, bool enabled)
            {
                List<Data.Models.HRExpenseWithOutTable>? liste = dal.Get(u => u.tripId == tripId && u.currentUserId == userId && u.enabled == enabled && u.currentStateId == 1).ToList();
                return liste ?? [];
            }


            private async void approveHRExpenseTable(List<Data.Models.HRExpenseWithOutTable> listHrExpenseWithOutTables, Data.Models.AdminUser nextUser,
            int onaySirasi, int currentState, Data.Models.AdminUser tripUser, int tripId, Data.Models.CeoTable ceo)
            {

                if (currentState == 1)
                {

                    foreach (Data.Models.HRExpenseWithOutTable hrExpenseWithOutTable in listHrExpenseWithOutTables)
                    {
                        hrExpenseWithOutTable.currentUserId = (nextUser.Id);
                        hrExpenseWithOutTable.onaySirasi = (onaySirasi);
                        hrExpenseWithOutTable.currentStateId = (currentState);
                        // soneklenen
                        hrExpenseWithOutTable.approval = (true);
                        await Update(hrExpenseWithOutTable);
                    }

                }
                else if (currentState == 4)
                {
                    BLLActions.HRExpenseWithOutTripTable bllHRExpenseWithOutTripTable = new BLLActions.HRExpenseWithOutTripTable(_configuration, _env, _mapper);
                    Data.Models.HRExpenseWithOutTripTable hrExpenseTripTable = bllHRExpenseWithOutTripTable.GetByID(tripId)!;
                    foreach (Data.Models.HRExpenseWithOutTable hrExpenseWithOutTable in listHrExpenseWithOutTables)
                    {
                        if (hrExpenseWithOutTable.approvedAmount.CompareTo(hrExpenseWithOutTable.totalLimitAmount) > 0
                                && nextUser.Id != ceo.userId && ceo.fazlaHarcirahDusumu == true)
                        {

                            hrExpenseWithOutTable.currentUserId = (ceo.userId);
                            hrExpenseWithOutTable.onaySirasi = (hrExpenseWithOutTable.onaySirasi + 1);
                            await Update(hrExpenseWithOutTable);

                        }
                        else
                        {

                            hrExpenseWithOutTable.onaySirasi = (onaySirasi);
                            hrExpenseWithOutTable.currentStateId = (currentState);
                            // burası
                            await Update(hrExpenseWithOutTable);

                            hrExpenseTripTable.approval = (true);
                            await bllHRExpenseWithOutTripTable.Update(hrExpenseTripTable);

                        }

                    }

                }

            }


            private async void onay(Data.Models.AdminUser? approvedUser, Data.Models.AdminUser? nextUser, int tripId, bool fazlaHarcamaVarmi, Data.Models.CeoTable ceo)
            {BLLActions.HRExpenseWithOutDetail bllHRExpenseWithOutDetail = new BLLActions.HRExpenseWithOutDetail(_configuration, _env);
                   
                if (approvedUser != null)
                {
                     Data.Models.HRExpenseWithOutDetail? hrExpenseWithOutDetail = bllHRExpenseWithOutDetail.getByActive(tripId,
                            approvedUser.Id);
                    if (hrExpenseWithOutDetail != null)
                    {

                        hrExpenseWithOutDetail.approved = (true);
                        hrExpenseWithOutDetail.isReplied = (true);
                        hrExpenseWithOutDetail.replyDate = (DateTime.Now);
                        await bllHRExpenseWithOutDetail.Update(hrExpenseWithOutDetail);
                    }
                }

                if (nextUser == null)
                {
                    if (fazlaHarcamaVarmi && ceo.fazlaHarcirahDusumu)
                    {

                        Data.Models.HRExpenseWithOutDetail? varmi = bllHRExpenseWithOutDetail.getByActive(tripId, ceo.userId);
                        if (varmi == null)
                        {
                            Data.Models.HRExpenseWithOutDetail hrExpenseDetailnext = new Data.Models.HRExpenseWithOutDetail();
                            hrExpenseDetailnext.tripId=(tripId);
                            hrExpenseDetailnext.createdDate=(DateTime.Now);
                            hrExpenseDetailnext.userId=(ceo.userId);
                            hrExpenseDetailnext.enabled=(true);
                            // buraya bak true olmayabilir
                            hrExpenseDetailnext.isReplied=(true);
                            hrExpenseDetailnext.guid = Guid.NewGuid();
                            await bllHRExpenseWithOutDetail.Add(hrExpenseDetailnext);
                        }
                    }
                }
                if (nextUser != null)
                {

                    Data.Models.HRExpenseWithOutDetail? varmi = bllHRExpenseWithOutDetail.getByActive(tripId, nextUser.Id);
                    if (varmi == null)
                    {
                        Data.Models.HRExpenseWithOutDetail hrExpenseDetailnext = new Data.Models.HRExpenseWithOutDetail();
                        hrExpenseDetailnext.tripId=(tripId);
                        hrExpenseDetailnext.createdDate=(DateTime.Now);
                        hrExpenseDetailnext.userId=(nextUser.Id);
                        hrExpenseDetailnext.enabled=(true);
                        // buraya bak true olmayabilir
                        hrExpenseDetailnext.isReplied=(true);
                        hrExpenseDetailnext.guid = Guid.NewGuid();
                      await  bllHRExpenseWithOutDetail.Add(hrExpenseDetailnext);
                    }

                }

            }

            public async Task<HRExpenseWithOutTableSaveDto> changeLimit(HRExpenseWithOutTableSaveDto entity)
            {
                Data.Models.HRExpenseWithOutTable saveDto = await Update(_mapper.Map<Data.Models.HRExpenseWithOutTable>(entity));
                return _mapper.Map<HRExpenseWithOutTableSaveDto>(saveDto);

            }

            public async Task<int> ceoOnayLimitTutari(int tripId, int userId)
            {
                try
                {
                    List<Data.Models.HRExpenseWithOutTable> listHrExpenseTable = findByTripIdActive(tripId);
                    foreach (Data.Models.HRExpenseWithOutTable hrExpenseTable in listHrExpenseTable)
                    {
                        decimal? tutar;
                        if (hrExpenseTable.amount.CompareTo(hrExpenseTable.totalLimitAmount) > 0)
                        {
                            tutar = hrExpenseTable.totalLimitAmount;
                        }
                        else
                        {
                            tutar = hrExpenseTable.amount;
                        }
                        hrExpenseTable.approvedAmount=tutar ?? Convert.ToDecimal(0);
                       await Update(hrExpenseTable);
                    }
                    return 1;
                }
                catch 
                {
                    return 4;
                }
            }

           
            public async Task<int> ceoOnayFaturaTutari(int tripId, int userId)
            {
                try
                {
                    List<Data.Models.HRExpenseWithOutTable> listHarcamalar = findByTripIdActive(tripId);
                    foreach (Data.Models.HRExpenseWithOutTable entity in listHarcamalar)
                    {
                        BLLActions.HRExpenseTypeTable bllHRExpenseTypeTable = new BLLActions.HRExpenseTypeTable(_configuration, _env);
                        Data.Models.HRExpenseTypeTable? hrExpenseTypeTable = bllHRExpenseTypeTable.GetByID(entity.expenseTypeId);
                        if (hrExpenseTypeTable?.toplamaNo ??false)
                        {

                            decimal oncekiHarcamalarToplam = getByTypeAndDateSumAmount(entity.tripId,
                                    entity.expenseTypeId, entity.spendingTime.ToString());

                            List<Data.Models.HRExpenseWithOutTable> listHrExpenseTable = getByTypeAndDate(entity.tripId,
                                    entity.expenseTypeId, entity.spendingTime.ToString());

                            entity.approvedAmount=oncekiHarcamalarToplam;
                            foreach (Data.Models.HRExpenseWithOutTable hrExpenseTable in listHrExpenseTable)
                            {
                                hrExpenseTable.approvedAmount=oncekiHarcamalarToplam;
                                await Update(hrExpenseTable);
                            }
                        }
                        else
                        {

                            entity.approvedAmount=(entity.amount);

                        }

                        await Update(entity);
                    }
                    return 1;

                }
                catch 
                {
                    return 4;
                }
            }

            public async Task<int> ceoOnayAmirOnayi(int tripId, int userId)
            {
                try
                {
                    List<Data.Models.HRExpenseWithOutTable> listHarcamalar = findByTripIdActive(tripId);
                    foreach (Data.Models.HRExpenseWithOutTable entity in listHarcamalar)
                    {
                        entity.approvedAmount=entity.approvedAmount;
                        await Update(entity);

                    }
                    return 1;
                }
                catch 
                {
                    return 4;
                }
            } 
            public List<Data.Models.HRExpenseWithOutTable> findByTripIdActive(int tripId)
            {
                return dal.Get(u => u.enabled && u.tripId == tripId && u.currentStateId == 1).ToList();
            }

            public async Task<int> geriGonder(int tripId, int userId)
            {
                int donenDeger = 0;

                List<Data.Models.HRExpenseWithOutTable> listHRExpenseTable = findByTripIdAndCurrentUserIdAndEnabled(tripId,
                        userId, true);
                BLLActions.HRExpenseWithOutTripTable bllHRExpenseWithOutTripTable = new BLLActions.HRExpenseWithOutTripTable(_configuration, _env, _mapper);
                Data.Models.HRExpenseWithOutTripTable? hrExpenseTripTable = bllHRExpenseWithOutTripTable.GetByID(tripId);
                foreach (Data.Models.HRExpenseWithOutTable expenseTable in listHRExpenseTable)
                {
                    try
                    {
                        expenseTable.currentStateId=1;
                        expenseTable.currentUserId=hrExpenseTripTable!.userId;
                        expenseTable.onaySirasi=1;
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
                    BLLActions.HRExpenseWithOutDetail bllHRExpenseWithOutDetail = new BLLActions.HRExpenseWithOutDetail(_configuration, _env);
                    List<Data.Models.HRExpenseWithOutDetail> listHRExpenseWithOutDetail = bllHRExpenseWithOutDetail
                            .findAllByTripIdAndEnabled(tripId, true);

                    foreach (Data.Models.HRExpenseWithOutDetail hrExpenseDetail in listHRExpenseWithOutDetail)
                    {
                        hrExpenseDetail.enabled=(false);
                       await bllHRExpenseWithOutDetail.Update(hrExpenseDetail);
                    }

                    donenDeger = 1;

                }
                catch (Exception )
                {
                    donenDeger = 2;
                }

                BLLActions.AdminUsers bllAdminUsers=new BLLActions.AdminUsers(_configuration, _env, _mapper);
                UserByNameEMailDto user = bllAdminUsers.getUserByNameAndEmail(hrExpenseTripTable!.userId);
                BLLActions.EmailReaderFile bllEmailReaderFile = new BLLActions.EmailReaderFile();
                BLLActions.EmailMessages bllEmailMessages = new BLLActions.EmailMessages(_configuration, _env);
                EmailMessage emailMessage = new EmailMessage();
                emailMessage.subject=("Bekleyen Harcama Onayı hk.");

                emailMessage.toAddress=(user.email);

              
                string mailMessage = bllEmailReaderFile.BuildEmailTemplate(_configuration, _env, "Sayın " + user.name +
                "Geri Gönderilen Harcama Onayı.",
                           tripId.ToString() + " ID kodlu Harcamanız geri gönderilmiştir.");

                emailMessage.emailText=(mailMessage);
                emailMessage.mailTuru=(3);
                emailMessage.enabled=(true);
                emailMessage.isSent=(false);
                emailMessage.plannedDate=DateTime.Now;
              await  bllEmailMessages.Add(emailMessage);
                return donenDeger;
            }

            public List<Data.Models.HRExpenseWithOutTable> findByTripIdAndCurrentUserIdAndEnabled(int tripId, int userId, bool enabled)
            {
                return dal.Get(u => u.tripId == tripId && u.currentUserId == userId && u.enabled == enabled).ToList();
            }
        }
    }
}
