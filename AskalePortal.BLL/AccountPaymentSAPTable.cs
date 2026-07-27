using AskalePortal.Constants;
using AskalePortal.Data.Models;
using AskalePortal.Data.SAP.InputParams;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AskalePortal.Constants.CommonConstants;

namespace AskalePortal.BLL
{

    public partial class BLLActions
    {
        public class AccountPaymentSAPTable : BaseBLL<Data.Models.AccountPaymentSAPTable>
        {
            private readonly IWebHostEnvironment _env;
            private readonly IConfiguration _configuration;
            private readonly IMapper _mapper;
            private readonly ISftpServer _server;
            public AccountPaymentSAPTable(IConfiguration configuration, IWebHostEnvironment env, IMapper mapper, ISftpServer server) : base(configuration, env)
            {
                _configuration = configuration;
                _env = env;
                _mapper = mapper;
                _server = server;
            }

            [AllowAnonymous]
            public async Task<string> GetAccountPayment(string apiKey, Data.Models.AccountPaymentSAPTable odemeEmri)
            {
                if (apiKey == "Ba4nV`=tuPps}^+}py6xVTh,2;]p7zUE")
                {
                    if (odemeEmri.bankl == null)
                    {
                        return "BankaKalemiBos";

                    }
                    if (odemeEmri.bankl.Length != 10)
                    {
                        return "BankaKalemiEksik";

                    }
                    if (odemeEmri.bankl.Substring(4, 1) != "-")
                    {
                        return "BankaKalemi5";

                    }
                    foreach (var item in odemeEmri.AccountPaymentKalemSAPTable)
                    {
                        if (item.bankl == null)
                        {
                            return "BankaKalemiBos";

                        }
                        if (item.bankl.Length != 10)
                        {
                            return "BankaKalemiEksik";

                        }
                        if (item.bankl.Substring(4, 1) != "-")
                        {
                            return "BankaKalemi5";

                        }
                        try
                        {
                            Convert.ToInt32(item.bankl.Substring(0, 4));
                            Convert.ToInt32(item.bankl.Substring(5, 5));
                        }
                        catch (Exception)
                        {
                            return "BankaKalemi5";
                        }

                    }


                    BLLActions.ApprovalProcessDetails bllApprovalProcessDetails = new BLLActions.ApprovalProcessDetails(_configuration, _env, _mapper);
                    BLLActions.ApprovalProcesses bllApprovalProcesses = new BLLActions.ApprovalProcesses(_configuration, _env, _mapper);
                    BLLActions.Companies bllCompanies = new BLLActions.Companies(_configuration, _env, _mapper);
                    Company? company = bllCompanies.GetCompany(odemeEmri.bukrs);
                    int processTypeId = (int)Constants.CommonConstants.APPROVAL_PROCESSES.ACCOUNT_PAYMENT;
                    ApprovalProcess approvalProcess = bllApprovalProcesses.GetRelatedProcess(companyID: company!.Id, processTypeId, "A1");
                    BLLActions.ActivePaymentDetails bllActivePaymentDetails = new BLLActions.ActivePaymentDetails(_configuration, _env);

                    Data.Models.AccountPaymentSAPTable? accountPaymentSAPTableVarMi = GetByOENUM(odemeEmri.oenum);
                    int userId = 0;
                    if (bllApprovalProcessDetails.GetFirstUser(approvalProcess.Id).HasValue)
                    {
                        userId = bllApprovalProcessDetails.GetFirstUser(approvalProcess.Id)!.Value;
                    }

                    if (accountPaymentSAPTableVarMi == null)
                    {
                        Data.Models.AccountPaymentSAPTable accountPaymentSAPTable = new Data.Models.AccountPaymentSAPTable()
                        {
                            aedat = odemeEmri.aedat,
                            aenam = odemeEmri.aenam,
                            aeuhr = odemeEmri.aeuhr,
                            bstat = odemeEmri.bstat,
                            gjahr = odemeEmri.gjahr,
                            unva1 = odemeEmri.unva1,
                            unva2 = odemeEmri.unva2,
                            belnr = odemeEmri.belnr,
                            bukrs = odemeEmri.bukrs,
                            name1 = odemeEmri.name1,
                            name2 = odemeEmri.name2,
                            usnam = odemeEmri.usnam,
                            kurumKodu = odemeEmri.kurumKodu,
                            SubeKodu = odemeEmri.SubeKodu,
                            zsayino = odemeEmri.zsayino,
                            cpudt = odemeEmri.cpudt,
                            cputm = odemeEmri.cputm,
                            hkont = odemeEmri.hkont,
                            oenum = odemeEmri.oenum,
                            znot = odemeEmri.znot,
                            bankl = odemeEmri.bankl,
                            bankn = odemeEmri.bankn,
                            iban = odemeEmri.iban


                        };
                        await Add(accountPaymentSAPTable);




                        foreach (var item in odemeEmri.AccountPaymentKalemSAPTable)
                        {
                            BLLActions.AccountPaymentKalemSAPTable bllAccountPaymentKalemSAPTable = new BLLActions.AccountPaymentKalemSAPTable(_configuration, _env, _mapper, _server);
                            Data.Models.AccountPaymentKalemSAPTable accountPaymentKalemSAPTable = item;
                            accountPaymentKalemSAPTable.currentUserId = userId;
                            accountPaymentKalemSAPTable.currentStateId = 1;
                            await bllAccountPaymentKalemSAPTable.Add(accountPaymentKalemSAPTable);


                            Data.Models.ActivePaymentDetail activePaymentDetail = new Data.Models.ActivePaymentDetail()
                            {
                                activePaymentId = item.Id,
                                approved = null,
                                createdDate = DateTime.Now,
                                guid = Guid.NewGuid(),
                                isReplied = false,
                                replyDate = null,
                                userId = userId
                            };
                            await bllActivePaymentDetails.Add(activePaymentDetail);

                        }

                        BLLActions.AdminUsers bllAdminUsers = new BLLActions.AdminUsers(_configuration, _env, _mapper);
                        AdminUser? adminUser = bllAdminUsers.GetByID(userId);
                        BLLActions.EmailMessages bllEmailMessages = new BLLActions.EmailMessages(_configuration, _env);
                        EmailMessage emailMessage = new EmailMessage();
                        Data.Models.AccountPaymentSAPTable? savedAccountPaymentSAPTable = GetByOENUM(accountPaymentSAPTable.oenum);
                        emailMessage.subject = savedAccountPaymentSAPTable?.oenum.ToString() + " Nolu ödeme onayı hk.";
                        emailMessage.toAddress = adminUser?.email;
                        emailMessage.emailText = getPaymentMailstring(savedAccountPaymentSAPTable!, userId);

                        //emailMessage.emailText = CreatePaymentMail(item.AccountPaymentSAPTable.USNAM, item.AccountPaymentSAPTable.AENAM, item.OENUM, item.POSNR, item.AccountPaymentSAPTable.BUKRS, item.AccountPaymentSAPTable.CPUDT, item.LIFNR, item.NAME1, item.WRBTR, item.IBAN, item.BANKA, item.BRNCH, item.BANKN, adminUser.name, "" + adminUser.imageUrl, adminUser.Company.VTEXT);

                        emailMessage.isSent = false;
                        emailMessage.plannedDate = DateTime.Now;
                        emailMessage.mailTuru = 1;


                        await bllEmailMessages.Add(emailMessage);
                        return "SuccessUpdate";

                    }
                    else
                    {

                        accountPaymentSAPTableVarMi.aedat = odemeEmri.aedat;
                        accountPaymentSAPTableVarMi.aenam = odemeEmri.aenam;
                        accountPaymentSAPTableVarMi.aeuhr = odemeEmri.aeuhr;
                        accountPaymentSAPTableVarMi.bstat = odemeEmri.bstat;
                        accountPaymentSAPTableVarMi.gjahr = odemeEmri.gjahr;
                        accountPaymentSAPTableVarMi.unva1 = odemeEmri.unva1;
                        accountPaymentSAPTableVarMi.unva2 = odemeEmri.unva2;
                        accountPaymentSAPTableVarMi.belnr = odemeEmri.belnr;
                        accountPaymentSAPTableVarMi.bukrs = odemeEmri.bukrs;
                        accountPaymentSAPTableVarMi.name1 = odemeEmri.name1;
                        accountPaymentSAPTableVarMi.name2 = odemeEmri.name2;
                        accountPaymentSAPTableVarMi.usnam = odemeEmri.usnam;
                        accountPaymentSAPTableVarMi.zsayino = odemeEmri.zsayino;
                        accountPaymentSAPTableVarMi.kurumKodu = odemeEmri.kurumKodu;
                        accountPaymentSAPTableVarMi.SubeKodu = odemeEmri.SubeKodu;
                        accountPaymentSAPTableVarMi.cpudt = odemeEmri.cpudt;
                        accountPaymentSAPTableVarMi.cputm = odemeEmri.cputm;
                        accountPaymentSAPTableVarMi.hkont = odemeEmri.hkont;
                        accountPaymentSAPTableVarMi.oenum = odemeEmri.oenum;
                        accountPaymentSAPTableVarMi.bankn = odemeEmri.bankn;
                        accountPaymentSAPTableVarMi.bankl = odemeEmri.bankl;
                        accountPaymentSAPTableVarMi.iban = odemeEmri.iban;
                        accountPaymentSAPTableVarMi.znot = odemeEmri.znot;


                        await Update(accountPaymentSAPTableVarMi);
                        BLLActions.AccountPaymentKalemSAPTable bllAccountPaymentKalemSAPTabledel = new BLLActions.AccountPaymentKalemSAPTable(_configuration, _env, _mapper, _server);
                        List<Data.Models.AccountPaymentKalemSAPTable> listAccountPaymentKalemSAPTable = bllAccountPaymentKalemSAPTabledel.GetByOENUM(accountPaymentSAPTableVarMi.oenum);
                        foreach (var item in listAccountPaymentKalemSAPTable)
                        {
                            if (item.currentStateId == 1)
                            {
                                bllAccountPaymentKalemSAPTabledel.Delete(item.Id);
                                List<ActivePaymentDetail> listActivePaymentDetail = bllActivePaymentDetails.GetByAccountPaymentId(item.Id);
                                foreach (var item2 in listActivePaymentDetail)
                                {
                                    bllActivePaymentDetails.Delete(item2.Id);
                                }
                            }

                        }

                        foreach (var item in odemeEmri.AccountPaymentKalemSAPTable)
                        {

                            BLLActions.AccountPaymentKalemSAPTable bllAccountPaymentKalemSAPTable = new BLLActions.AccountPaymentKalemSAPTable(_configuration, _env, _mapper, _server);

                            Data.Models.AccountPaymentKalemSAPTable accountPaymentKalemSAPTable = item;
                            accountPaymentKalemSAPTable.currentUserId = userId;
                            accountPaymentKalemSAPTable.currentStateId = 1;
                            await bllAccountPaymentKalemSAPTable.Add(accountPaymentKalemSAPTable);
                            ActivePaymentDetail activePaymentDetail = new ActivePaymentDetail()
                            {
                                activePaymentId = item.Id,
                                approved = null,
                                createdDate = DateTime.Now,
                                guid = Guid.NewGuid(),
                                isReplied = false,
                                replyDate = null,
                                userId = bllApprovalProcessDetails.GetFirstUser(approvalProcess.Id)!.Value
                            };
                            await bllActivePaymentDetails.Add(activePaymentDetail);

                        }

                        BLLActions.AdminUsers bllAdminUsers = new BLLActions.AdminUsers(_configuration, _env, _mapper);
                        AdminUser? adminUser = bllAdminUsers.GetByID(bllApprovalProcessDetails.GetFirstUser(approvalProcess.Id)!.Value);
                        BLLActions.EmailMessages bllEmailMessages = new BLLActions.EmailMessages(_configuration, _env);
                        EmailMessage emailMessage = new EmailMessage();

                        emailMessage.subject = accountPaymentSAPTableVarMi.oenum.ToString() + " Nolu ödeme onayı hk.";
                        emailMessage.toAddress = adminUser?.email;
                        emailMessage.emailText = getPaymentMailstring(accountPaymentSAPTableVarMi, userId);

                        //emailMessage.emailText = CreatePaymentMail(item.AccountPaymentSAPTable.USNAM, item.AccountPaymentSAPTable.AENAM, item.OENUM, item.POSNR, item.AccountPaymentSAPTable.BUKRS, item.AccountPaymentSAPTable.CPUDT, item.LIFNR, item.NAME1, item.WRBTR, item.IBAN, item.BANKA, item.BRNCH, item.BANKN, adminUser.name, "" + adminUser.imageUrl, adminUser.Company.VTEXT);

                        emailMessage.isSent = false;
                        emailMessage.plannedDate = DateTime.Now;
                        emailMessage.mailTuru = 1;


                        await bllEmailMessages.Add(emailMessage);
                        return "SuccessUpdate";
                    }

                }
                else
                {
                    return "Basarisiz";
                }

            }

            public Data.Models.AccountPaymentSAPTable? GetByOENUM(string oENUM)
            {
                return dal.Get(u => u.enabled && u.oenum == oENUM).FirstOrDefault();
            }

            public string? getPaymentMailstring(Data.Models.AccountPaymentSAPTable accountPaymentSAPTable, int userId)
            {
                var okLink = "";
                //CommonConstants.OUT_URL + "/it-portal/accountpayment/replyfromout?answer=1&guid=" + accountPaymentSAPTable.oenum + "&userid=" + userId.ToString();
                var noLink = "";
                //CommonConstants.OUT_URL + "/it-portal/accountpayment/replyfromout?answer=0&guid=" + accountPaymentSAPTable.oenum + "&userid=" + userId.ToString();


                string mailstring = "<link rel='stylesheet' href='https://cdn.jsdelivr.net/npm/bootstrap@4.0.0/dist/css/bootstrap.min.css' integrity='sha384-Gn5384xqQ1aoWXA+058RXPxPg6fy4IWvTNh0E263XmFcJlSAwiGgFAW/dAiS6JXm' crossorigin='anonymous'>" +
                "<h3>Sayın Yetkili</h3><br/><br/>" +
                "<div>Aşağıdaki ödemeler onayınızı beklemektedir.</div>" +
                    "<table class='table table-striped table-bordered table-hover' style='margin-bottom:0px !important;' id='tableMain' >" +
                                        "<thead>" +
                                            "<tr>" +
                                                "<th style ='text-align: left;border: 1px solid black;'><a class='btn btn-primary' href=\"" + okLink + "\">TÜMÜNÜ ONAYLA</a></hr>" +
                                                "<th style ='text-align: left;border: 1px solid black;'><a class='btn btn-danger' href=\"" + noLink + "\">TÜMÜNÜ REDDET</a></th>" +
                                                "<th style ='text-align: left;border: 1px solid black;'>ID</th>" +


                                                "<th style ='text-align: left;border: 1px solid black;'>Sap Belge No</th>" +
                                                "<th style ='text-align: left;border: 1px solid black;'>Kalem</th>" +

                                                "<th style ='text-align: left;border: 1px solid black;'>Satıcı No</th>" +
                                                "<th style ='text-align: left;border: 1px solid black;'>Satıcı Adı</th>" +
                                                "<th style ='text-align: right;border: 1px solid black;'>Tutar</th>" +
                                                "<th style ='text-align: center;border: 1px solid black;'>Oluşturan Kişi SAP</th>" +


                                            "</tr>" +
                                        "</thead>" +
                                        "<tbody>";
                BLLActions.AccountPaymentKalemSAPTable bllAccountPaymentKalemSAPTable = new BLLActions.AccountPaymentKalemSAPTable(_configuration, _env, _mapper, _server);
                List<Data.Models.AccountPaymentKalemSAPTable> listAccountPaymentKalemSAPTable = bllAccountPaymentKalemSAPTable.GetByOENUMWithUserID(accountPaymentSAPTable.oenum, userId);
                foreach (var item in listAccountPaymentKalemSAPTable.Where(u => u.enabled == true && u.currentStateId == 1))
                {
                    mailstring += "<tr class='datarow'>" +

                                                "<td style ='text-align: left;border: 1px solid black;'><a class='btn btn-primary' href=\"" + okLink + "&posnr=" + @item.posnr + "\">ONAYLA</a></td>" +
                                                "<td style ='text-align: left;border: 1px solid black;'><a class='btn btn-danger' href=\"" + noLink + "&posnr=" + @item.posnr + "\">REDDET</a></td>" +
                                                "<td style ='text-align: left;border: 1px solid black;'>" + @item.Id + "</td>" +
                                               "<td style ='text-align: left;border: 1px solid black;'>" + @item.oenum + "</td>" +
                                               "<td style ='text-align: left;border: 1px solid black;'>" + @item.posnr + "</td>" +
                                               "<td style ='text-align: left;border: 1px solid black;'>" + @item.lifnr + "</td>" +
                                               "<td style ='text-align: left;border: 1px solid black;'>" + @item.name1 + "</td>" +
                                               "<td style ='text-align: right;border: 1px solid black;'>" + @Convert.ToDouble(Convert.ToDouble(@item.wrbtr) / 100).ToString("N2") + "</td>" +
                                               "<td style ='text-align: center;border: 1px solid black;' >" + @item.oenumNavigation.usnam.ToString() + "</td>" +


                                                  "</tr>";
                }



                mailstring += "</tbody>" +
            "</table>";

                mailstring += "<br /><br /> ----------------------------------------- <br /><br /> " +
                                    " Saygılarımızla.";
                return mailstring;
            }

            [AllowAnonymous]
            public async Task<string> GetTransferPayment(string apiKey, Data.Models.TransferPaymentSAPTable havaleEmri)
            {
                if (apiKey == "Ba4nV`=tuPps}^+}py6xVTh,2;]p7zUE")
                {
                    BLLActions.ApprovalProcessDetails bllApprovalProcessDetails = new BLLActions.ApprovalProcessDetails(_configuration, _env, _mapper);
                    BLLActions.TransferPaymentSAPTable bllTransferPaymentSAPTable = new BLLActions.TransferPaymentSAPTable(_configuration, _env);
                    Data.Models.TransferPaymentSAPTable transferPaymentSAPTableVarMi = bllTransferPaymentSAPTable.GetByHENUM(havaleEmri.henum);
                    BLLActions.ApprovalProcesses bllApprovalProcesses = new BLLActions.ApprovalProcesses(_configuration, _env, _mapper);
                    BLLActions.Companies bllCompanies = new BLLActions.Companies(_configuration, _env, _mapper);
                    Company? company = bllCompanies.GetCompany(havaleEmri.bukrs);
                    int processTypeId = (int)Constants.CommonConstants.APPROVAL_PROCESSES.ACCOUNT_PAYMENT;
                    ApprovalProcess approvalProcess = bllApprovalProcesses.GetRelatedProcess(companyID: company!.Id, processTypeId, "A1");
                    BLLActions.ActiveTransferDetails bllActiveTransferDetails = new BLLActions.ActiveTransferDetails(_configuration, _env);
                    int userId = 0;
                    if (bllApprovalProcessDetails.GetFirstUser(approvalProcess.Id).HasValue)
                    {
                        userId = bllApprovalProcessDetails.GetFirstUser(approvalProcess.Id)!.Value;
                    }
                    if (transferPaymentSAPTableVarMi == null)
                    {

                        Data.Models.TransferPaymentSAPTable transferPaymentSAPTable = new Data.Models.TransferPaymentSAPTable()
                        {
                            aedat = havaleEmri.aedat,
                            aenam = havaleEmri.aenam,
                            aeuhr = havaleEmri.aeuhr,
                            hetar = havaleEmri.hetar,
                            iban = havaleEmri.iban,
                            unva1 = havaleEmri.unva1,
                            unva2 = havaleEmri.unva2,
                            henum = havaleEmri.henum,
                            bukrs = havaleEmri.bukrs,
                            name1 = havaleEmri.name1,
                            name2 = havaleEmri.name2,
                            usnam = havaleEmri.usnam,
                            zsayino = havaleEmri.zsayino,
                            cpudt = havaleEmri.cpudt,
                            cputm = havaleEmri.cputm,
                            hkont = havaleEmri.hkont,
                            kurumKodu = havaleEmri.kurumKodu,
                            SubeKodu = havaleEmri.SubeKodu,
                            znot = havaleEmri.znot,
                            bankl = havaleEmri.bankl,
                            bankn = havaleEmri.bankn

                        };
                        await bllTransferPaymentSAPTable.Add(transferPaymentSAPTable);

                        foreach (var item in havaleEmri.TransferPaymentKalemSAPTable)
                        {
                            BLLActions.TransferPaymentKalemSAPTable bllTransferPaymentKalemSAPTable = new BLLActions.TransferPaymentKalemSAPTable(_configuration, _env, _mapper, _server);
                            Data.Models.TransferPaymentKalemSAPTable transferPaymentKalemSAPTable = item;
                            transferPaymentKalemSAPTable.currentUserId = userId;
                            transferPaymentKalemSAPTable.currentStateId = 1;
                            await bllTransferPaymentKalemSAPTable.Add(transferPaymentKalemSAPTable);

                            ActiveTransferDetail activeTransferDetail = new ActiveTransferDetail()
                            {
                                activeTransferId = item.Id,
                                approved = null,
                                createdDate = DateTime.Now,
                                guid = Guid.NewGuid(),
                                isReplied = false,
                                replyDate = null,
                                userId = bllApprovalProcessDetails.GetFirstUser(approvalProcess.Id)!.Value
                            };
                            await bllActiveTransferDetails.Add(activeTransferDetail);
                        }
                        BLLActions.AdminUsers bllAdminUsers = new BLLActions.AdminUsers(_configuration, _env, _mapper);
                        AdminUser? adminUser = bllAdminUsers.GetByID(bllApprovalProcessDetails.GetFirstUser(approvalProcess.Id)!.Value);
                        BLLActions.EmailMessages bllEmailMessages = new BLLActions.EmailMessages(_configuration, _env);
                        Data.Models.TransferPaymentSAPTable savedTransferPaymentSAPTable = bllTransferPaymentSAPTable.GetByHENUM(transferPaymentSAPTable.henum);
                        Data.Models.EmailMessage emailMessage = new Data.Models.EmailMessage();

                        emailMessage.subject = savedTransferPaymentSAPTable.henum.ToString() + " Nolu ödeme onayı hk.";
                        emailMessage.toAddress = adminUser?.email;
                        emailMessage.emailText = getTransferMailstring(savedTransferPaymentSAPTable, userId);

                        //emailMessage.emailText = CreatePaymentMail(item.AccountPaymentSAPTable.USNAM, item.AccountPaymentSAPTable.AENAM, item.OENUM, item.POSNR, item.AccountPaymentSAPTable.BUKRS, item.AccountPaymentSAPTable.CPUDT, item.LIFNR, item.NAME1, item.WRBTR, item.IBAN, item.BANKA, item.BRNCH, item.BANKN, adminUser.name, "" + adminUser.imageUrl, adminUser.Company.VTEXT);

                        emailMessage.isSent = false;
                        emailMessage.plannedDate = DateTime.Now;
                        emailMessage.mailTuru = 1;
                        await bllEmailMessages.Add(emailMessage);
                        return "SuccessAdd";
                    }
                    else
                    {

                        transferPaymentSAPTableVarMi.aedat = havaleEmri.aedat;
                        transferPaymentSAPTableVarMi.aenam = havaleEmri.aenam;
                        transferPaymentSAPTableVarMi.aeuhr = havaleEmri.aeuhr;
                        transferPaymentSAPTableVarMi.hetar = havaleEmri.hetar;
                        transferPaymentSAPTableVarMi.iban = havaleEmri.iban;
                        transferPaymentSAPTableVarMi.unva1 = havaleEmri.unva1;
                        transferPaymentSAPTableVarMi.unva2 = havaleEmri.unva2;
                        transferPaymentSAPTableVarMi.henum = havaleEmri.henum;
                        transferPaymentSAPTableVarMi.bukrs = havaleEmri.bukrs;
                        transferPaymentSAPTableVarMi.name1 = havaleEmri.name1;
                        transferPaymentSAPTableVarMi.name2 = havaleEmri.name2;
                        transferPaymentSAPTableVarMi.usnam = havaleEmri.usnam;
                        transferPaymentSAPTableVarMi.zsayino = havaleEmri.zsayino;
                        transferPaymentSAPTableVarMi.cpudt = havaleEmri.cpudt;
                        transferPaymentSAPTableVarMi.kurumKodu = havaleEmri.kurumKodu;
                        transferPaymentSAPTableVarMi.SubeKodu = havaleEmri.SubeKodu;
                        transferPaymentSAPTableVarMi.cputm = havaleEmri.cputm;
                        transferPaymentSAPTableVarMi.hkont = havaleEmri.hkont;
                        transferPaymentSAPTableVarMi.znot = havaleEmri.znot;
                        transferPaymentSAPTableVarMi.bankl = havaleEmri.bankl;
                        transferPaymentSAPTableVarMi.bankn = havaleEmri.bankn;


                        await bllTransferPaymentSAPTable.Update(transferPaymentSAPTableVarMi);

                        BLLActions.TransferPaymentKalemSAPTable bllTransferPaymentKalemSAPTabledel = new BLLActions.TransferPaymentKalemSAPTable(_configuration, _env, _mapper, _server);
                        List<Data.Models.TransferPaymentKalemSAPTable> listTransferPaymentKalemSAPTable = bllTransferPaymentKalemSAPTabledel.GetByHENUM(transferPaymentSAPTableVarMi.henum);

                        foreach (var item in listTransferPaymentKalemSAPTable)
                        {
                            if (item.currentStateId == 1)
                            {
                                bllTransferPaymentKalemSAPTabledel.Delete(item.Id);
                                List<ActiveTransferDetail> listActiveTransferDetail = bllActiveTransferDetails.GetByAccountTransferId(item.Id);
                                foreach (var item2 in listActiveTransferDetail)
                                {
                                    bllActiveTransferDetails.Delete(item2.Id);
                                }
                            }
                        }
                        foreach (var item in havaleEmri.TransferPaymentKalemSAPTable)
                        {
                            BLLActions.TransferPaymentKalemSAPTable bllTransferPaymentKalemSAPTable = new BLLActions.TransferPaymentKalemSAPTable(_configuration, _env, _mapper, _server);
                            Data.Models.TransferPaymentKalemSAPTable transferPaymentKalemSAPTable = item;
                            transferPaymentKalemSAPTable.currentUserId = userId;
                            transferPaymentKalemSAPTable.currentStateId = 1;
                            await bllTransferPaymentKalemSAPTable.Add(transferPaymentKalemSAPTable);
                            ActiveTransferDetail activeTransferDetail = new ActiveTransferDetail()
                            {
                                activeTransferId = item.Id,
                                approved = null,
                                createdDate = DateTime.Now,
                                guid = Guid.NewGuid(),
                                isReplied = false,
                                replyDate = null,
                                userId = bllApprovalProcessDetails.GetFirstUser(approvalProcess.Id)!.Value
                            };
                            await bllActiveTransferDetails.Add(activeTransferDetail);
                        }
                        BLLActions.AdminUsers bllAdminUsers = new BLLActions.AdminUsers(_configuration, _env, _mapper);
                        AdminUser? adminUser = bllAdminUsers.GetByID(bllApprovalProcessDetails.GetFirstUser(approvalProcess.Id)!.Value);
                        BLLActions.EmailMessages bllEmailMessages = new BLLActions.EmailMessages(_configuration, _env);
                        EmailMessage emailMessage = new EmailMessage();

                        emailMessage.subject = transferPaymentSAPTableVarMi.henum.ToString() + " Nolu ödeme onayı hk.";
                        emailMessage.toAddress = adminUser?.email;
                        emailMessage.emailText = getTransferMailstring(transferPaymentSAPTableVarMi, userId);

                        //emailMessage.emailText = CreatePaymentMail(item.AccountPaymentSAPTable.USNAM, item.AccountPaymentSAPTable.AENAM, item.OENUM, item.POSNR, item.AccountPaymentSAPTable.BUKRS, item.AccountPaymentSAPTable.CPUDT, item.LIFNR, item.NAME1, item.WRBTR, item.IBAN, item.BANKA, item.BRNCH, item.BANKN, adminUser.name, "" + adminUser.imageUrl, adminUser.Company.VTEXT);

                        emailMessage.isSent = false;
                        emailMessage.plannedDate = DateTime.Now;
                        emailMessage.mailTuru = 1;
                        await bllEmailMessages.Add(emailMessage);
                        return "SuccessUpdate";
                    }

                }
                else
                {
                    return "Basarisiz";
                }

            }



            public string? getTransferMailstring(Data.Models.TransferPaymentSAPTable transferPaymentSAPTable, int userId)
            {
                var okLink = "/it-portal/accountpayment/replyfromouttransfer?answer=1&guid=" + transferPaymentSAPTable.henum + "&userid=" + userId.ToString();
                var noLink = "/it-portal/accountpayment/replyfromouttransfer?answer=0&guid=" + transferPaymentSAPTable.henum + "&userid=" + userId.ToString();

                string mailstring = "<link rel='stylesheet' href='https://cdn.jsdelivr.net/npm/bootstrap@4.0.0/dist/css/bootstrap.min.css' integrity='sha384-Gn5384xqQ1aoWXA+058RXPxPg6fy4IWvTNh0E263XmFcJlSAwiGgFAW/dAiS6JXm' crossorigin='anonymous'>" +
                "<div>Aşağıdaki ödemeler onayınızı beklemektedir.</div>" +
                    "<table class='table table-striped table-bordered table-hover' style='margin-bottom:0px !important;' id='tableMain'>" +
                                        "<thead>" +
                                            "<tr>" +
                                            "<th style ='text-align: left;border: 1px solid black;'><a class='btn btn-primary' href=\"" + okLink + "\">TÜMÜNÜ ONAYLA</a></th>" +
                                            "<th style ='text-align: left;border: 1px solid black;'><a class='btn btn-danger' href=\"" + noLink + "\">TÜMÜNÜ REDDET</a></th>" +
                                                "<th style ='text-align: left;border: 1px solid black;'>ID</th>" +

                                                "<th style ='text-align: left;border: 1px solid black;'>Sap Belge No</th>" +
                                                "<th style ='text-align: left;border: 1px solid black;'>Kalem</th>" +

                                                "<th style ='text-align: left;border: 1px solid black;'>Haval Kişi No</th>" +
                                                "<th style ='text-align: left;border: 1px solid black;'>Havale Kişisi</th>" +
                                                "<th style ='text-align: right;border: 1px solid black;'>Tutar</th>" +
                                                "<th style ='text-align: center;border: 1px solid black;'>Oluşturan Kişi SAP</th>" +


                                            "</tr>" +
                                        "</thead>" +
                                        "<tbody>";
                BLLActions.TransferPaymentKalemSAPTable bllTransferPaymentKalemSAPTable = new BLLActions.TransferPaymentKalemSAPTable(_configuration, _env, _mapper, _server);
                List<Data.Models.TransferPaymentKalemSAPTable> listTransferPaymentKalemSAPTables = bllTransferPaymentKalemSAPTable.GetByHENUMByUserId(transferPaymentSAPTable.henum, userId);
                foreach (var item in listTransferPaymentKalemSAPTables.Where(u => u.enabled == true && u.currentStateId == 1 && u.currentUserId == userId))
                {
                    mailstring += "<tr class='datarow'>" +
                        "<td  style ='text-align: left;border: 1px solid black;'><a class='btn btn-primary' href=\"" + okLink + "&posnr=" + @item.posnr + "\">ONAYLA</a></td>" +
                        "<td style ='text-align: left;border: 1px solid black;'><a class='btn btn-danger' href=\"" + noLink + "&posnr=" + @item.posnr + "\">REDDET</a></td>" +
                                               "<td  style ='text-align: left;border: 1px solid black;'>" + @item.Id + "</td>" +
                                               "<td style ='text-align: left;border: 1px solid black;'>" + @item.henum + "</td>" +
                                               "<td style ='text-align: left;border: 1px solid black;'>" + @item.posnr + "</td>" +
                                               "<td style ='text-align: left;border: 1px solid black;'>" + @item.lifnr + "</td>" +
                                               "<td style ='text-align: left;border: 1px solid black;'>" + @item.firma + "</td>" +
                                               "<td style ='text-align: right;border: 1px solid black;'>" + @Convert.ToDouble(Convert.ToDouble(@item.wrbtr) / 100).ToString("N2") + "</td>" +
                                               "<td  style = 'text-align:center;border: 1px solid black;' >" + @item.henumNavigation.usnam.ToString() + "</td>" +


                                                  "</tr>";
                }



                mailstring += "</tbody>" +
            "</table>";

                mailstring += "<br /><br /> ----------------------------- <br /><br /> " +
                                    " Saygılarımızla.";
                return mailstring;
            }

        }
    }

}
