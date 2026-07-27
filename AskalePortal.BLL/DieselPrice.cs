using AskalePortal.Constants;
using AskalePortal.Data.Models;
using AskalePortal.Data.RequestModel;
using AskalePortal.Data.RequestParams;
using AskalePortal.Data.ResponseModels;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.BLL
{
    public partial class BLLActions
    {
        public class DieselPrice : BaseBLL<AskalePortal.Data.Models.DieselPrice>
        {
            private readonly IConfiguration _configuration;
            private readonly IWebHostEnvironment _env;
            private readonly IMapper _mapper;
            public DieselPrice(IConfiguration configuration, IWebHostEnvironment env, IMapper mapper) : base(configuration, env)
            {
                _configuration = configuration;
                _env = env;
                _mapper = mapper;
            }

            public List<Data.Models.DieselPrice> activeMyApprovalList(FilterParam<DieselPriceListDtoParameter> filterParam)
            {
                int userId = filterParam.liste?.filterUser ?? 0;
                List<Data.Models.DieselPrice> list = dal.Get(u => u.enabled && u.currentUserId == userId && u.currentStateId == 1).OrderByDescending(u => u.Id).ToList();
                return list;
            }

            public int approvalCount(int userId)
            {
                int count = dal.Get(u => u.enabled && u.currentUserId == userId && u.currentStateId == 1).Count();
                return count;
            }

            public async Task<int> confirmSave(int id, int userId)
            {

                Data.Models.DieselPrice? dieselPrice = GetByID(id);
                BLLActions.ApprovalProcesses bllApprovalProcesses = new BLLActions.ApprovalProcesses(_configuration, _env, _mapper  );
                ApprovalProcess approvalProcess = bllApprovalProcesses.findByCompanyIdAndTypeIdAndEnabled(
                        dieselPrice?.companyId ?? 0, (int)Constants.CommonConstants.APPROVAL_PROCESSES.MOTORINFIYAT);
                BLLActions.AdminUsers bllAdminUsers = new BLLActions.AdminUsers(_configuration, _env, _mapper);
                AdminUser? fuelUser = bllAdminUsers.GetByID(dieselPrice?.createdUserId ?? 0);
                BLLActions.ApprovalProcessDetails bllApprovalProcessDetails = new BLLActions.ApprovalProcessDetails(_configuration, _env, _mapper);
                List<ApprovalProcessDetail>? listDieselPriceSaveEditApprover = bllApprovalProcessDetails
                        .findByProcessIdAndEnabledOrderByDataOrderAsc(approvalProcess.Id);
                if (listDieselPriceSaveEditApprover.Find(t => t.dataOrder == dieselPrice?.onaySirasi) != null)
                {
                    int deger = (dieselPrice?.onaySirasi + 1) ?? 0;
                    List<ApprovalProcessDetail> listeControl = listDieselPriceSaveEditApprover.FindAll(t => t.dataOrder == deger) ?? [];
                    if (listeControl.Count() == 0)
                    {

                        EmailMessage emailMessage = new EmailMessage();
                        emailMessage.subject = ("Bekleyen Motorin Kayıt Onayı hk.");
                        emailMessage.toAddress = (fuelUser?.email);
                        BLLActions.EmailMessages bllEmailMessages = new BLLActions.EmailMessages(_configuration, _env);
                        //string mailMessage = buildDiesel(fuelUser.name, "Motorin Fiyatı Kayıt Onayı hk.",
                        //        dieselPrice.Id.ToString() + " numaralı kayıt onaylanmıştır.");
                        BLLActions.EmailReaderFile bllEmailReaderFile = new BLLActions.EmailReaderFile();

                        string mailMessage = bllEmailReaderFile.BuildEmailTemplate(_configuration, _env, "Sayın " + fuelUser?.name +
                    " Motorin Fiyatı Kayıt Onayı hk.",
                            dieselPrice?.Id.ToString() + " ID'li kayıt onaylanmıştır");
                        emailMessage.emailText = (mailMessage);
                        emailMessage.mailTuru = (1);
                        emailMessage.enabled = (true);
                        emailMessage.isSent = (false);
                        emailMessage.plannedDate = (DateTime.Now);
                       await bllEmailMessages.Add(emailMessage);

                        BLLActions.DieselPriceDetail bllDieselPriceDetail = new BLLActions.DieselPriceDetail(_configuration, _env);
                        int onaySirasi = dieselPrice?.onaySirasi ?? 0;
                        Data.Models.DieselPriceDetail dieselPriceDetail = bllDieselPriceDetail.getByActive(dieselPrice?.Id??0,
                                listDieselPriceSaveEditApprover.Find(t => t.dataOrder == onaySirasi)!
                                        .userId);
                        dieselPriceDetail.approved = (true);
                        dieselPriceDetail.isReplied = (true);
                        dieselPriceDetail.replyDate = (DateTime.Now);
                        await bllDieselPriceDetail.Update(dieselPriceDetail);
                        dieselPrice!.approval = (true);
                        dieselPrice.currentStateId = (4);
                        await Update(dieselPrice);
                        BLLActions.FuelPriceDifference bllFuelPriceDifference = new BLLActions.FuelPriceDifference(_configuration, _env, _mapper);
                        FuelPriceDifferenceRaporDto raporDto = bllFuelPriceDifference.createReport(DateTime.Now,
                                dieselPrice.companyId);
                        List<FuelPriceDifferenceModelDto> liste = raporDto.liste ?? [];
                        Dictionary<string, List<FuelPriceDifferenceModelDto>> mapFirma = liste.GroupBy(t => t.yukleniciFirma).ToDictionary(g => g.Key??"", g => g.ToList());
                        //Map<String, List<FuelPriceDifferenceDto>> mapFirma = liste.stream()
                        //        .collect(Collectors.groupingBy(t->t.getYukleniciFirma()));

                        //                  string table = """
                        //<table class='table table-striped table-bordered table-hover' width="100%"\
                        //style=' margin: 0 auto; border:1px solid;text-align:center' border="1" >\
                        //""";
                        //                  table += "<thead>";
                        //                  table += "<tr>";
                        //                  table += "<th>YÜKLENİCİ FİRMA</th>";
                        //                  table += "<th>İŞİN ADI</th>";
                        //                  table += "<th>NEVİ</th>";
                        //                  table += "<th>KM</th>";
                        //                  table += "<th>ESKİ FİYAT</th>";
                        //                  table += "<th>YENİ FİYAT</th>";
                        //                  table += "<th>TOPLAM</th>";
                        //                  table += "<tr>";
                        //                  table += "</thead>";
                        //                  table += "<tbody>";

                        //                  for (var item : mapFirma.entrySet())
                        //                  {
                        //                      table += "<tr>";
                        //                      table += "<td rowspan='" + item.getValue().size() + "'>" + item.getKey() + "</td>\n";
                        //                      Map<String, List<FuelPriceDifferenceDto>> mapIs = liste.stream()
                        //                              .filter(u->u.getYukleniciFirma().equals(item.getKey()))
                        //                              .collect(Collectors.groupingBy(t->t.getIsinAdi()));
                        //                      for (var item2 : mapIs.entrySet())
                        //                      {
                        //                          table += "<td rowspan='" + item2.getValue().size() + "'>" + item2.getKey() + "</td>\n";

                        //                          int i = 0;
                        //                          for (var item3 : item2.getValue())
                        //                          {
                        //                              table += "<td>" + item3.getNevi() + "</td>\n";
                        //                              table += "<td>" + "%,.0f".formatted(item3.getKm()) + "</td>\n";
                        //                              table += "<td>" + "%,.4f".formatted(item3.getEskiFiyat()) + "</td>\n";
                        //                              table += "<td>" + "%,.4f".formatted(item3.getYenifiyat()) + "</td>\n";

                        //                              if (i == 0)
                        //                              {
                        //                                  table += "<td rowspan='" + item2.getValue().size() + "'>"
                        //                                          + "%,.4f".formatted(item2.getValue().stream()
                        //                                                  .mapToDouble(u->Double.valueOf(u.getYenifiyat().toString())).sum())
                        //                                          + "</td>\n";

                        //                              }
                        //                              table += "</tr><tr>";
                        //                              i++;
                        //                          }

                        //                      }
                        //                      table += "</tr>\n";
                        //                  }
                        //                  table += "</tbody>";
                        //                  table += "</table>";
                        //    BLLActions.FuelPriceDifferenceMail bllFuelPriceDifferenceMail = new FuelPriceDifferenceMail(_configuration, _env);
                        //    List<Data.Models.FuelPriceDifferenceMail> listFuelPriceDifferenceMail = bllFuelPriceDifferenceMail
                        //            .listAllByEnabled(true);
                        //    foreach (Data.Models.FuelPriceDifferenceMail sendEmail in listFuelPriceDifferenceMail)
                        //    {
                        //        EmailMessage email = new EmailMessage();
                        //        email.subject=("Motorin Fiyat Farkı Güncel Tablo hk.");
                        //        email.toAddress=(sendEmail.mailAdress);

                        //        //string mail = buildTable(table, "Motorin Fiyat Farkı Güncel Tablo",
                        //        //         DateTime.Now.ToString("dd.MM.yyyy"), "%,.4f".formatted(raporDto.getEskiMotorin()),
                        //        //         "%,.4f".formatted(raporDto.getYeniMotorin()), "%,.4f".formatted(raporDto.getKdvDahil()),
                        //        //         "%,.4f".formatted(raporDto.getKdvHaric()));

                        //        string mail = EmailReaderFile.BuildEmailTemplate(_configuration, _env, "Sayın " + fuelUser.name +
                        //" Motorin Fiyatı Kayıt Onayı hk.",
                        //        dieselPrice.Id.ToString() + " ID'li kayıt onayınızı beklemektedir");
                        //        email.emailText=(mail);
                        //        email.mailTuru=(1);
                        //        email.enabled=(true);
                        //        email.isSent=(false);
                        //        email.plannedDate=(DateTime.Now);
                        //        bllEmailMessages.Add(email);
                        //    }

                        return 3;
                    }
                    else
                    {
                        int sirano = listDieselPriceSaveEditApprover
                                .FindAll(t => t.dataOrder == (dieselPrice!.onaySirasi)).First().userId;
                        BLLActions.DieselPriceDetail bllDieselPriceDetail = new BLLActions.DieselPriceDetail(_configuration, _env);
                        Data.Models.DieselPriceDetail dieselPriceDetail = bllDieselPriceDetail.getByActive(dieselPrice!.Id,
                                sirano);
                        dieselPriceDetail.approved = (true);
                        dieselPriceDetail.isReplied = (true);
                        dieselPriceDetail.replyDate = (DateTime.Now);
                        await bllDieselPriceDetail.Update(dieselPriceDetail);

                        Data.Models.DieselPriceDetail dieselPriceDetailnext = new Data.Models.DieselPriceDetail();
                        dieselPriceDetailnext.dieselId = (dieselPrice.Id);
                        dieselPriceDetailnext.createdDate = (DateTime.Now);
                        dieselPriceDetailnext.userId = (listDieselPriceSaveEditApprover!
                                .Find(t => t.dataOrder == (dieselPrice.onaySirasi + 1))!
                                .userId);
                        dieselPriceDetailnext.enabled = (true);
                        dieselPriceDetailnext.guid = Guid.NewGuid();
                        await bllDieselPriceDetail.Add(dieselPriceDetailnext);
                        dieselPrice.currentUserId = (listDieselPriceSaveEditApprover.Find(t => t.dataOrder == (dieselPrice.onaySirasi + 1))!
                                .userId);
                        dieselPrice.onaySirasi = (dieselPrice.onaySirasi + 1);
                        dieselPrice.currentStateId = (1);
                        await Update(dieselPrice);

                        AdminUser? user2 = bllAdminUsers.GetByID(dieselPrice.currentUserId ?? 0);
                        EmailMessage emailMessage = new EmailMessage();
                        emailMessage.subject = ("Bekleyen Motorin Kayıt Onayı hk.");
                        emailMessage.toAddress = (user2?.email);

                        BLLActions.EmailReaderFile bllEmailReaderFile = new BLLActions.EmailReaderFile();

                        string mailMessage = bllEmailReaderFile.BuildEmailTemplate(_configuration, _env, "Sayın " + user2?.name +
                    " Motorin Fiyatı Kayıt Onayı hk.",
                            dieselPrice.Id.ToString() + " ID'li kayıt onayınızı beklemektedir");
                        emailMessage.emailText = (mailMessage);
                        emailMessage.mailTuru = (1);
                        emailMessage.enabled = (true);
                        emailMessage.isSent = (false);
                        emailMessage.plannedDate = (DateTime.Now);
                        BLLActions.EmailMessages bllEmailMessages = new BLLActions.EmailMessages(_configuration, _env);
                        await bllEmailMessages.Add(emailMessage);

                        return 1;
                    }

                }
                else
                {
                    return 2;
                }
            }

            public List<Data.Models.DieselPrice> listActive(int userId)
            {
                BLLActions.AdminUsers bllAdminUsers = new BLLActions.AdminUsers(_configuration, _env, _mapper);
                AdminUser? adminUser = bllAdminUsers.GetByID(userId);
                if (adminUser?.roleId == 1)
                {
                    List<Data.Models.DieselPrice> listDieselPrice = dal.Get(u => u.enabled && u.approval == null).OrderByDescending(u => u.Id).ToList();
                    return listDieselPrice;
                }
                else
                {
                    List<Data.Models.DieselPrice> listDieselPrice = dal.Get(u => u.enabled && u.approval == null && u.createdUserId == userId).OrderByDescending(u => u.Id).ToList();
                    return listDieselPrice;
                }
            }

            public List<Data.Models.DieselPrice> listCompleted(int userId)
            {
                BLLActions.AdminUsers bllAdminUsers = new BLLActions.AdminUsers(_configuration, _env, _mapper);
                AdminUser? adminUser = bllAdminUsers.GetByID(userId);
                if (adminUser?.roleId == 1)
                {
                    List<Data.Models.DieselPrice> listDieselPrice = dal.Get(u => u.enabled && u.currentStateId != 1).OrderByDescending(u => u.Id).ToList();
                    return listDieselPrice;
                }
                else
                {
                    List<Data.Models.DieselPrice> listDieselPrice = dal.Get(u => u.enabled && u.currentStateId != 1 && u.createdUserId == userId).OrderByDescending(u => u.Id).ToList();
                    return listDieselPrice;
                }

            }

            public async Task<Data.Models.DieselPrice> save(DieselPriceDto entity, int userId)
            {
                
                BLLActions.ApprovalProcesses bllApprovalProcesses = new BLLActions.ApprovalProcesses(_configuration, _env, _mapper);
                ApprovalProcess approvalProcess = bllApprovalProcesses.findByCompanyIdAndTypeIdAndEnabled(
                entity.companyId ?? 0, (int)Constants.CommonConstants.APPROVAL_PROCESSES.MOTORINFIYAT);
                BLLActions.ApprovalProcessDetails bllApprovalProcessDetails = new BLLActions.ApprovalProcessDetails(_configuration, _env, _mapper);
                List<ApprovalProcessDetail> listApprovalProcessDetail = bllApprovalProcessDetails
                        .findByProcessIdAndEnabledOrderByDataOrderAsc(approvalProcess.Id);
                int onaylayici1Id = listApprovalProcessDetail.Find(u => u.dataOrder == 1)!.userId;
                BLLActions.AdminUsers bllAdminUsers = new BLLActions.AdminUsers(_configuration, _env, _mapper);
                AdminUser? onaylayici1 = bllAdminUsers.GetByID(onaylayici1Id);

                if (entity.id == null)
                {

                    entity.createdUserId = (userId);
                    entity.createdDate = (DateTime.Now.ToString());
                    entity.enabled = (true);
                    entity.currentUserId = (onaylayici1Id);
                    entity.onaySirasi = (1);
                    entity.currentStateId = (1);
                    Data.Models.DieselPrice? dieselPrice = await Add(_mapper.Map<Data.Models.DieselPrice>(entity));

                    DieselPriceDetail bllDieselPriceDetail = new DieselPriceDetail(_configuration, _env);
                    Data.Models.DieselPriceDetail dieselPriceDetail = new Data.Models.DieselPriceDetail();
                    dieselPriceDetail.enabled = (true);
                    dieselPriceDetail.approved = (null);
                    dieselPriceDetail.dieselId = (dieselPrice?.Id);
                    dieselPriceDetail.userId = (onaylayici1Id);
                    dieselPriceDetail.guid = Guid.NewGuid();
                    dieselPriceDetail.createdDate = (DateTime.Now);
                    await bllDieselPriceDetail.Add(dieselPriceDetail);

                    EmailMessage emailMessage = new EmailMessage();
                    emailMessage.subject = ("Bekleyen Motorin Kayıt Onayı hk.");
                    emailMessage.toAddress = (onaylayici1?.email);
                    BLLActions.EmailMessages bllEmailMessages = new EmailMessages(_configuration, _env);
                    BLLActions.EmailReaderFile bllEmailReaderFile = new BLLActions.EmailReaderFile();

                    string mailMessage = bllEmailReaderFile.BuildEmailTemplate(_configuration, _env, "Sayın " + onaylayici1?.name +
                    " Motorin Fiyatı Kayıt Onayı hk.",
                            dieselPrice?.Id.ToString() + " ID'li kayıt onayınızı beklemektedir");


                    emailMessage.emailText = (mailMessage);
                    emailMessage.mailTuru = (1);
                    emailMessage.enabled = (true);
                    emailMessage.isSent = (false);
                    emailMessage.plannedDate = (DateTime.Now);
                    await bllEmailMessages.Add(emailMessage);
                    return dieselPrice!;
                }
                else
                {

                    Data.Models.DieselPrice? dieselPriceOld = GetByID(entity.id ?? 0);
                    if (dieselPriceOld?.fiyat.CompareTo(entity.fiyat) != 1
                            || dieselPriceOld.girisTarihi.CompareTo(entity.girisTarihi) != 1)
                    {
                        // fiyat değişmişse
                        entity.createdUserId = (userId);
                        entity.createdDate = (DateTime.Now.ToString());
                        entity.enabled = (true);
                        entity.currentUserId = (onaylayici1?.Id);
                        entity.onaySirasi = (1);
                        entity.currentStateId = (1);
                        DieselPriceDetail bllDieselPriceDetail = new DieselPriceDetail(_configuration, _env);
                        Data.Models.DieselPriceDetail dieselPriceDetailBefore = bllDieselPriceDetail.getByActive(entity.id ?? 0,
                                listApprovalProcessDetail.Find(t => t.dataOrder == entity.onaySirasi)!.userId);

                        dieselPriceDetailBefore.enabled = (false);
                        dieselPriceDetailBefore.replyDate = (DateTime.Now);
                        await bllDieselPriceDetail.Update(dieselPriceDetailBefore);

                        Data.Models.DieselPrice dieselPrice =await Update(_mapper.Map<Data.Models.DieselPrice>(entity));
                        Data.Models.DieselPriceDetail dieselPriceDetail = new Data.Models.DieselPriceDetail();
                        dieselPriceDetail.enabled = (true);
                        dieselPriceDetail.approved = (null);
                        dieselPriceDetail.dieselId = (dieselPrice.Id);
                        dieselPriceDetail.userId = (onaylayici1?.Id);
                        dieselPriceDetail.guid = Guid.NewGuid();
                        dieselPriceDetail.createdDate = (DateTime.Now);
                        await bllDieselPriceDetail.Add(dieselPriceDetail);

                        EmailMessage emailMessage = new EmailMessage();
                        emailMessage.subject = ("Bekleyen Motorin Kayıt Onayı hk.");
                        emailMessage.toAddress = (onaylayici1?.email);
                        BLLActions.EmailMessages bllEmailMessages = new BLLActions.EmailMessages(_configuration, _env);
                        BLLActions.EmailReaderFile bllEmailReaderFile = new BLLActions.EmailReaderFile();

                        string mailMessage = bllEmailReaderFile.BuildEmailTemplate(_configuration, _env, "Sayın " + onaylayici1?.name +
                    " Motorin Fiyatı Kayıt Onayı hk.",
                            dieselPrice.Id.ToString() + " ID'li kayıt onayınızı beklemektedir");

                        emailMessage.emailText = (mailMessage);
                        emailMessage.mailTuru = (1);
                        emailMessage.enabled = (true);
                        emailMessage.isSent = (false);
                        emailMessage.plannedDate = (DateTime.Now);
                        await bllEmailMessages.Add(emailMessage);
                        return dieselPrice;

                    }
                    else
                    {
                        entity.updatedUserId = (userId);
                        entity.updateDate = (DateTime.Now.ToString());
                        entity.enabled = (true);
                        Data.Models.DieselPrice data = await Update(_mapper.Map<Data.Models.DieselPrice>(entity));
                        return data;
                    }
                }
            }

            public Data.Models.DieselPrice dieselPriceByDate(DateTime date, int? companyId, bool approval)
            {
                return dal.Get(u => u.enabled && u.girisTarihi <= date && u.companyId == companyId && u.approval == approval).OrderByDescending(u => u.girisTarihi).First();
            }

            public Data.Models.DieselPrice? dieselPriceDate(DateTime date, int companyId)
            {
                Data.Models.DieselPrice? diesel = dal.Get(u => u.enabled && u.girisTarihi == date && u.companyId == companyId).FirstOrDefault();
                return diesel;
            }

            public async Task<int> rejectSave(int id, int userId)
            {
                Data.Models.DieselPrice? dieselPrice = GetByID(id);
                BLLActions.AdminUsers bllAdminUsers = new BLLActions.AdminUsers(_configuration, _env, _mapper);
                AdminUser? user = bllAdminUsers.GetByID(dieselPrice?.createdUserId ?? 0);
                int donenDeger = 0;
                try
                {
                    dieselPrice!.approval = (false);
                    dieselPrice.currentStateId = (2);
                    await Update(dieselPrice);
                    donenDeger = 1;

                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    donenDeger = 2;
                }

                try
                {
                    BLLActions.DieselPriceDetail bllDieselPriceDetail = new BLLActions.DieselPriceDetail(_configuration, _env);
                    Data.Models.DieselPriceDetail dieselPriceDetail = bllDieselPriceDetail.getByActive(id, userId);
                    dieselPriceDetail.approved = (false);
                    dieselPriceDetail.isReplied = (true);
                    dieselPriceDetail.replyDate = (DateTime.Now);
                    await bllDieselPriceDetail.Update(dieselPriceDetail);
                    donenDeger = 1;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    donenDeger = 2;
                }

                EmailMessage emailMessage = new EmailMessage();
                emailMessage.subject = ("Bekleyen Motorin Kayıt Onayı hk.");
                emailMessage.toAddress = (user?.email);
                BLLActions.EmailMessages bllEmailMessages = new BLLActions.EmailMessages(_configuration, _env);
                BLLActions.EmailReaderFile bllEmailReaderFile = new BLLActions.EmailReaderFile();

                string mailMessage = bllEmailReaderFile.BuildEmailTemplate(_configuration, _env, "Sayın " + user?.name +
            " RED Motorin Fiyatı Kayıt Onayı hk.",
                    dieselPrice?.Id.ToString() + " ID'li kayıt reddedilmiştir");

                emailMessage.emailText = (mailMessage);
                emailMessage.mailTuru = (1);
                emailMessage.enabled = (true);
                emailMessage.isSent = (false);
                emailMessage.plannedDate = (DateTime.Now);
                await bllEmailMessages.Add(emailMessage);
                return donenDeger;
            }

            public Data.Models.DieselPrice? dieselPriceByDate(DateTime date, int companyId)
            {
                Data.Models.DieselPrice? dieselPrice = dal.Get(u => u.enabled && u.companyId == companyId && u.approval == true).OrderByDescending(u => u.girisTarihi).FirstOrDefault();
                return dieselPrice;
            }
        }
    }
}
