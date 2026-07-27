using AskalePortal.Constants;
using AskalePortal.Data.Models;
using AskalePortal.Data.RequestModel;
using AskalePortal.Data.RequestParams;
using AskalePortal.Data.ResponseModels;
using AskalePortal.Data.ResponseParams;
using AskalePortal.Data.SAP.InputParams;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using SapNwRfc;
using System;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AskalePortal.BLL.BLLActions;

namespace AskalePortal.BLL
{

    public partial class BLLActions
    {
        public class TransferPaymentKalemSAPTable : BaseBLL<AskalePortal.Data.Models.TransferPaymentKalemSAPTable>
        {
            private readonly IConfiguration _configuration;
            private readonly IWebHostEnvironment _env;
            private readonly IMapper _mapper;

            private readonly ISftpServer _server;
            public TransferPaymentKalemSAPTable(IConfiguration configuration, IWebHostEnvironment env, IMapper mapper, ISftpServer server) : base(configuration, env)
            {
                _configuration = configuration;
                _env = env;
                _mapper = mapper;
                _server = server;
            }
            public List<AskalePortal.Data.Models.TransferPaymentKalemSAPTable> GetByHENUM(string hENUM)
            {
                return dal.Get(u => u.enabled == true && u.henum == hENUM).ToList();
            }

            public List<AskalePortal.Data.Models.TransferPaymentKalemSAPTable> GetByUserId(int userId, string name, int activePage, int pageSize)
            {
                return dal.Get(u => u.enabled == true && u.currentStateId == 1 && u.currentUserId == userId && u.firma.Contains(name)).OrderBy(u => u.henum).Skip(activePage * pageSize).Take(pageSize).ToList();
            }

            public List<AskalePortal.Data.Models.TransferPaymentKalemSAPTable> GetByFinished(string name, int activePage, int pageSize)
            {
                return dal.Get(u => u.enabled == true && u.currentStateId != 1 && u.firma.Contains(name)).OrderByDescending(u => u.henum).Skip(activePage * pageSize).Take(pageSize).ToList();
            }

            public object GetByActive(int activePage, int pageSize, string name)
            {
                return dal.Get(u => u.enabled == true && u.currentStateId == 1 && u.firma.Contains(name)).OrderBy(u => u.henum).Skip(activePage * pageSize).Take(pageSize).ToList();
            }

            public List<AskalePortal.Data.Models.TransferPaymentKalemSAPTable> GetByHENUMByUserId(string guid, int userid)
            {
                return dal.Get(u => u.enabled == true && u.henum == guid && u.currentStateId == 1 && u.currentUserId == userid).ToList();
            }

            public object GetByFinishedByFinansDanismani(int userId, string name, int activePage, int pageSize)
            {
                return dal.Get(u => u.enabled == true && u.currentStateId != 1 && u.firma.Contains(name) && u.ActiveTransferDetail.Any(y => y.userId == userId)).OrderByDescending(u => u.henum).Skip(activePage * pageSize).Take(pageSize).ToList();

            }

            public List<TransferPaymentKalemActiveDto> listFilterByCompanyIdAndVendorCode(FilterParam<TransferPaymentKalemActiveDtoParameter> filterParam)
            {
                string firma = filterParam?.liste?.firma ?? "";

                var query =
       from a in dal.dB.TransferPaymentKalemSAPTable
       join b in dal.dB.TransferPaymentSAPTable
           on a.henum equals b.henum
       join c in dal.dB.ActiveTransferDetail
           on a.Id equals c.activeTransferId
       join d in dal.dB.AdminUser
           on c.userId equals d.Id
       where
           a.currentStateId == 1 &&
           a.enabled == true &&
           b.enabled == true &&
           c.approved == null &&
           c.enabled == true &&
           (
               a.firma.Contains(firma) ||
               (firma == "" && a.firma == null)
           )
       orderby a.henum descending
       select new TransferPaymentKalemActiveDto()
       {
           id = a.Id,
           henum = a.henum,
           firma = a.firma,
           currentStateId = a.currentStateId,
           lifnr = a.lifnr,
           onayKimde = d.name,
           posnr = a.posnr,
           usnam = b.usnam,
           wrbtr = a.wrbtr,
           znot = b.znot
       };

                return query.ToList();
            }

            public List<TransferPaymentKalemActiveDto> mylist(FilterParam<TransferPaymentKalemMyListDtoParameter> filterParam)
            {
                string firma = filterParam?.liste?.firma ?? "";
                int? userId = filterParam?.liste?.userId;

                var query =
       from a in dal.dB.TransferPaymentKalemSAPTable
       join b in dal.dB.TransferPaymentSAPTable
           on a.henum equals b.henum
       join c in dal.dB.ActiveTransferDetail
           on a.Id equals c.activeTransferId
       join d in dal.dB.AdminUser
           on c.userId equals d.Id
       where
           a.currentStateId == 1 &&
           a.enabled == true &&
           b.enabled == true &&
           c.approved == null &&
           c.enabled == true &&
           a.currentUserId == userId &&
           (
               a.firma.Contains(firma) ||
               (firma == "" && a.firma == null)
           )
       orderby a.henum descending
       select new TransferPaymentKalemActiveDto
       {
           id = a.Id,
           henum = a.henum,
           posnr = a.posnr,
           lifnr = a.lifnr,
           firma = a.firma,
           wrbtr = a.wrbtr,
           usnam = b.usnam,
           currentStateId = a.currentStateId,
           onayKimde = d.name,
           znot = b.znot
       };
                return query.ToList();
            }

            public PageReturn<TransferPaymentKalemActiveDto> completed(FilterPageParam<TransferPaymentKalemMyListDtoParameter> filterPageParam, int userId)
            {
                PageReturn<TransferPaymentKalemActiveDto>? result = new PageReturn<TransferPaymentKalemActiveDto>();
                int pageSize = filterPageParam.size ?? 20;
                int pageNumber = filterPageParam.page ?? 0;

                string? firma = filterPageParam.liste?.firma;
                int? filterUserId = filterPageParam.liste?.userId;

                var query = from a in dal.dB.TransferPaymentKalemSAPTable
                            join b in dal.dB.TransferPaymentSAPTable
                                on a.henum equals b.henum
                            where
                           a.enabled == true &&
        b.enabled == true &&
        a.currentStateId != 1 &&
        (
            string.IsNullOrEmpty(firma)
                ? a.firma == null
                : a.firma.Contains(firma)
        )
                            orderby a.Id descending
                            select new TransferPaymentKalemActiveDto
                            {
                                firma = a.firma,
                                currentStateId = a.currentStateId,
                                henum = a.henum,
                                id = a.Id,
                                lifnr = a.lifnr,
                                onayKimde = a.firma,
                                posnr = a.posnr,
                                usnam = b.usnam,
                                wrbtr = a.wrbtr,
                                znot = b.znot
                            };
                result.content = query
                  .Skip(pageSize * pageNumber).Take(pageSize).ToList();
                result.totalElements = query.Count();
                result.number = result.content.Count();
                result.size = pageSize;

                return result;
            }

            public TransferPaymentKalemMyListDetailDto mylistdetail(int id)
            {
                TransferPaymentKalemMyListDetailDto transferPaymentKalemMyListDetailDto = mylistdetail(id);

                BLLActions.ActiveTransferDetails bllActiveTransferDetails = new BLLActions.ActiveTransferDetails(_configuration, _env);
                List<ActiveTransferDetail> listActiveTransferDetails = bllActiveTransferDetails.GetByAccountTransferId(id);
                List<ApprovedPerson> listApprovedPerson = new List<ApprovedPerson>();
                foreach (ActiveTransferDetail activePaymentDetail in listActiveTransferDetails)
                {
                    ApprovedPerson approvedPerson = new ApprovedPerson();
                    BLLActions.Companies bllCompanies = new BLLActions.Companies(_configuration, _env, _mapper);
                    approvedPerson.companyName = (bllCompanies.getByUserId(activePaymentDetail.userId));
                    if (activePaymentDetail.replyDate != null)
                    {
                        approvedPerson.dateTime = (activePaymentDetail.replyDate ?? DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss");
                    }
                    else
                    {
                        approvedPerson.dateTime = (DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss");
                    }

                    approvedPerson.process = activePaymentDetail.approved;
                    approvedPerson.userId = activePaymentDetail.userId;
                    listApprovedPerson.Add(approvedPerson);
                }
                transferPaymentKalemMyListDetailDto.listApprovedPerson = listApprovedPerson;
                return transferPaymentKalemMyListDetailDto;

            }

            public async Task<bool> approved(bool approved, List<int> list, int userId)
            {
                BLLActions.AdminUsers bllAdminUsers = new BLLActions.AdminUsers(_configuration, _env, _mapper);
                AdminUser? user = bllAdminUsers.GetByID(userId);
                if (approved)
                {
                    //DateTimeFormatter dateTimeFormatter = DateTimeFormatter.ofPattern("yyyyMMdd");
                    //DateTimeFormatter dateTimeFormatter2 = DateTimeFormatter.ofPattern("HH:mm:ss");

                    List<string> listcompanykavcim = new List<string>();
                    listcompanykavcim.Add("AC60");
                    listcompanykavcim.Add("AC91");
                    listcompanykavcim.Add("AC92");
                    List<Data.Models.TransferPaymentKalemSAPTable> liste = new List<Data.Models.TransferPaymentKalemSAPTable>();
                    foreach (int id in list)
                    {

                        Data.Models.TransferPaymentKalemSAPTable? transferPaymentKalemSAPTable = GetByID(id);
                        if (transferPaymentKalemSAPTable != null)
                        {
                            liste.Add(transferPaymentKalemSAPTable);
                        }
                    }

                    double sayi = Math.Ceiling((double)liste.Count / 10.0);
                    int dongu = (int)sayi;
                    for (int k = 0; k < dongu; k++)
                    {
                        int skipSayasi = k * 10;

                        //Map<String, List<TransferPaymentKalemSAPTable>> listedGroup = liste.stream()
                        //    .skip(skipSayasi).limit(10).collect(Collectors.groupingBy(u->u.getHenum()));
                        Dictionary<string, List<Data.Models.TransferPaymentKalemSAPTable>> listedGroup =
                        liste
                            .Skip(skipSayasi)
                            .Take(10)
                            .GroupBy(u => u.henum)
                            .ToDictionary(g => g.Key, g => g.ToList());

                        foreach (List<Data.Models.TransferPaymentKalemSAPTable> item in listedGroup.Values)
                        {
                            BLLActions.TransferPaymentSAPTable bllTransferPaymentSAPTable = new BLLActions.TransferPaymentSAPTable(_configuration, _env);
                            Data.Models.TransferPaymentSAPTable transferPaymentSAPTable = bllTransferPaymentSAPTable
                                    .GetByHENUM(item[0].henum);
                            string companyName = transferPaymentSAPTable.bukrs;
                            BLLActions.Companies bllCompanies = new BLLActions.Companies(_configuration, _env, _mapper);
                            Company company = bllCompanies.getByVkorgCompany(companyName);
                            StringBuilder stringBuilder = new StringBuilder();
                            bool onaylandiMi = false;
                            int donguSayisi = 0;
                            decimal araToplam = 0;
                            int processTypeId = (int)CommonConstants.APPROVAL_PROCESSES.ACCOUNT_PAYMENT;
                            BLLActions.ApprovalProcesses bllApprovalProcesses = new BLLActions.ApprovalProcesses(_configuration, _env, _mapper);
                            ApprovalProcess? approvalProcess = bllApprovalProcesses
                                    .findByCompanyIdAndTypeIdAndDagitimKanaliAndEnabled(company.Id, processTypeId, "A1", true);
                            stringBuilder.Append("H");
                            stringBuilder.Append(transferPaymentSAPTable.cpudt);
                            stringBuilder.Append("0015");
                            if (listcompanykavcim.Contains(companyName.ToUpper()))
                            {
                                stringBuilder.Append("26431");// beklenecek
                                stringBuilder.Append("TR060001500158007305328436");
                                stringBuilder.Append("00034");
                            }
                            else if (companyName.ToUpper().Equals("ACSG"))
                            {
                                stringBuilder.Append("39906");// beklenecek
                                stringBuilder.Append("TR170001500158007293737708");
                                stringBuilder.Append("00034");
                            }
                            else if (companyName.ToUpper().Equals("ACPZ"))
                            {
                                stringBuilder.Append("39908");// beklenecek
                                stringBuilder.Append("TR780001500158007287959775");
                                stringBuilder.Append("00034");
                            }
                            else if (companyName.ToUpper().Equals("ACPT"))
                            {
                                stringBuilder.Append("39903");// beklenecek
                                stringBuilder.Append("TR580001500158007316828111");
                                stringBuilder.Append("00034");
                            }
                            else if (companyName.ToUpper().Equals("ACAT"))
                            {
                                stringBuilder.Append("39907");// beklenecek
                                stringBuilder.Append("TR750001500158007318826005");
                                stringBuilder.Append("00034");
                            }
                            else if (companyName.ToUpper().Equals("AC68"))
                            {
                                stringBuilder.Append("39898");// beklenecek
                                stringBuilder.Append("TR140001500158007324274332");
                                stringBuilder.Append("00034");
                            }
                            else if (companyName.ToUpper().Equals("ACFC"))
                            {
                                stringBuilder.Append("44583");// beklenecek
                                stringBuilder.Append("TR380001500158007348744187");
                                stringBuilder.Append("00034");
                            }
                            else
                            {
                                stringBuilder.Append("26430");// beklenecek
                                stringBuilder.Append("TR900001500158007265188227");
                                stringBuilder.Append("00034");
                            }
                            stringBuilder.Append("\r\n");

                            foreach (Data.Models.TransferPaymentKalemSAPTable item2 in item)
                            {
                                AdminUser? nextUserr = bllApprovalProcesses
                                        .GetNextUser(item2.currentUserId, approvalProcess?.Id ??0, true);
                                if (nextUserr != null)
                                {
                                    BLLActions.ActiveTransferDetails bllActiveTransferDetails = new BLLActions.ActiveTransferDetails(_configuration, _env);
                                    ActiveTransferDetail? activePaymentDetail = bllActiveTransferDetails
                                            .findAllByActiveTransferIdAndApprovedAndUserIdAndEnabled(item2.Id, null, userId,
                                                    true);
                                    if (activePaymentDetail != null)
                                    {
                                        activePaymentDetail.approved = true;
                                        activePaymentDetail.isReplied = true;
                                        activePaymentDetail.replyDate = DateTime.Now;
                                        await bllActiveTransferDetails.Update(activePaymentDetail);
                                    }

                                    ActiveTransferDetail nextActivePaymenDetail = new ActiveTransferDetail();
                                    nextActivePaymenDetail.approved = null;
                                    nextActivePaymenDetail.replyDate = null;
                                    nextActivePaymenDetail.isReplied = false;
                                    nextActivePaymenDetail.activeTransferId = item2.Id;
                                    nextActivePaymenDetail.guid = Guid.NewGuid();
                                    nextActivePaymenDetail.userId = nextUserr.Id;
                                    await bllActiveTransferDetails.Add(nextActivePaymenDetail);
                                    item2.currentUserId = nextUserr.Id;
                                    await Update(item2);


                                    BLLActions.SAPConnectionData bllSapConnection = new BLLActions.SAPConnectionData(_configuration, _env);
                                    SapConnection? sapConn = bllSapConnection.sapConnection(_configuration, _env);
                                    if (sapConn != null)
                                    {

                                        sapConn.Connect();
                                        ISapFunction sapFunction = sapConn.CreateFunction("ZWEBI044");
                                        TransferPaymentKalemSAPTableApproveParams inputparams = new TransferPaymentKalemSAPTableApproveParams
                                        {
                                            apikey = "Ba4nV`=tuPps}^+}py6xVTh,2;]p7zUE",
                                            henum = item2.henum,
                                            onaylayan = user?.name ?? "",
                                            onaysekli = "O",
                                            posnr = item2.posnr,
                                            bittimi= "X",
                                            saat = DateTime.Now.ToString("HH:mm:ss"),
                                            tarih = DateTime.Now.ToString("yyyyMMdd")
                                        };


                                        sapFunction.Invoke<string>(input: inputparams);
                                        sapConn.Disconnect();

                                    }


                                }
                                else
                                {
                                    BLLActions.ActiveTransferDetails bllActiveTransferDetails = new BLLActions.ActiveTransferDetails(_configuration, _env);
                                    ActiveTransferDetail? activePaymentDetail = bllActiveTransferDetails
                                            .findAllByActiveTransferIdAndApprovedAndUserIdAndEnabled(item2.Id, null, userId,
                                                    true);
                                    if (activePaymentDetail != null)
                                    {
                                        item2.approval = true;
                                        item2.currentStateId = 4;
                                        await Update(item2);

                                        activePaymentDetail.approved = true;
                                        activePaymentDetail.isReplied = true;
                                        activePaymentDetail.replyDate = DateTime.Now;
                                        await bllActiveTransferDetails.Update(activePaymentDetail);



                                        BLLActions.SAPConnectionData bllSapConnection = new BLLActions.SAPConnectionData(_configuration, _env);
                                        SapConnection? sapConn = bllSapConnection.sapConnection(_configuration, _env);
                                        if (sapConn != null)
                                        {

                                            sapConn.Connect();
                                            ISapFunction sapFunction = sapConn.CreateFunction("ZWEBI043");
                                            AccountPaymentKalemSAPTableApproveParams inputparams = new AccountPaymentKalemSAPTableApproveParams
                                            {
                                                apikey = "Ba4nV`=tuPps}^+}py6xVTh,2;]p7zUE",
                                                oenum = item2.henum,
                                                onaylayan = user?.name ?? "",
                                                onaysekli = "B",
                                                posnr = item2.posnr,
                                                bittimi= "X",
                                                saat = DateTime.Now.ToString("HH:mm:ss"),
                                                tarih = DateTime.Now.ToString("yyyyMMdd")
                                            };


                                            sapFunction.Invoke<string>(input: inputparams);
                                            sapConn.Disconnect();

                                        }
                                      
                                        onaylandiMi = true;
                                        StringBuilder stringBuilderDeger = new StringBuilder();
                                        int basamak = item2.wrbtr.Length - 1;
                                        for (int i = 0; i < 21 - basamak; i++)
                                        {
                                            stringBuilderDeger.Append("0");
                                        }
                                        stringBuilderDeger.Append(item2.wrbtr);

                                        StringBuilder stringBuilderAciklama = new StringBuilder(transferPaymentSAPTable.znot);
                                        int basamakaciklama = stringBuilderAciklama.Length;
                                        for (int i = 0; i < 100 - basamakaciklama; i++)
                                        {
                                            stringBuilderAciklama.Insert(0, " ");

                                        }
                                        StringBuilder stringBuilderAdsoyad = new StringBuilder(item2.firma);
                                        int basamakadsoyad = stringBuilderAdsoyad.Length;
                                        for (int i = 0; i < 40 - basamakadsoyad; i++)
                                        {
                                            stringBuilderAdsoyad.Insert(0, " ");
                                        }

                                        StringBuilder stringBuilderAdress = new StringBuilder();
                                        int basamakadresi = stringBuilderAdress.Length;
                                        for (int i = 0; i < 50 - basamakadresi; i++)
                                        {
                                            stringBuilderAdress.Insert(0, " ");
                                        }

                                        StringBuilder stringBuilderTelefonNo = new StringBuilder();
                                        int basamaktelefonNo = stringBuilderTelefonNo.Length;
                                        for (int i = 0; i < 20 - basamaktelefonNo; i++)
                                        {
                                            stringBuilderTelefonNo.Insert(0, " ");
                                        }

                                        StringBuilder stringBuilderVergiNo = new StringBuilder();
                                        int basamakVergiNo = stringBuilderVergiNo.Length;
                                        for (int i = 0; i < 11 - basamakVergiNo; i++)
                                        {
                                            stringBuilderVergiNo.Insert(0, " ");
                                        }

                                        StringBuilder stringBuilderEmail = new StringBuilder();
                                        int basamakEmail = stringBuilderEmail.Length;
                                        for (int i = 0; i < 50 - basamakEmail; i++)
                                        {
                                            stringBuilderEmail.Insert(0, " ");
                                        }

                                        StringBuilder stringBuilderAlacakVergiDairesi = new StringBuilder();
                                        int basamakAlacakVergiDairesi = stringBuilderAlacakVergiDairesi.Length;
                                        for (int i = 0; i < 15 - basamakAlacakVergiDairesi; i++)
                                        {
                                            stringBuilderAlacakVergiDairesi.Insert(0, " ");
                                        }

                                        StringBuilder stringBuilderSaticiNo = new StringBuilder();
                                        if (item2.lifnr != null)
                                        {
                                            stringBuilderSaticiNo.Append(item2.lifnr);
                                        }
                                        int basamakSaticiNo = stringBuilderSaticiNo.Length;
                                        for (int i = 0; i < 10 - basamakSaticiNo; i++)
                                        {
                                            stringBuilderSaticiNo.Append(" ");
                                        }

                                        StringBuilder stringBuilderBabaAdi = new StringBuilder();
                                        int basamakBabaAdi = stringBuilderBabaAdi.Length;
                                        for (int i = 0; i < 20 - basamakBabaAdi; i++)
                                        {
                                            stringBuilderBabaAdi.Insert(0, " ");
                                        }

                                        StringBuilder stringBuilderReferens = new StringBuilder();
                                        int basamakReferens = stringBuilderReferens.Length;
                                        for (int i = 0; i < 16 - basamakReferens; i++)
                                        {
                                            stringBuilderReferens.Insert(0, " ");
                                        }

                                        StringBuilder stringBuilderParametre = new StringBuilder();
                                        int basamakParametre = stringBuilderParametre.Length;
                                        for (int i = 0; i < 40 - basamakParametre; i++)
                                        {
                                            stringBuilderParametre.Insert(0, " ");
                                        }
                                        String rezerv1 = transferPaymentSAPTable.iban;

                                        StringBuilder stringBuilderRezerv2 = new StringBuilder();
                                        int basamakRezerv2 = stringBuilderRezerv2.Length;
                                        for (int i = 0; i < 11 - basamakRezerv2; i++)
                                        {
                                            stringBuilderRezerv2.Insert(0, " ");
                                        }

                                        StringBuilder stringBuilderDurumKodu = new StringBuilder();
                                        int basamakDurumKodu = stringBuilderDurumKodu.Length;
                                        for (int i = 0; i < 2 - basamakDurumKodu; i++)
                                        {
                                            stringBuilderDurumKodu.Insert(0, " ");
                                        }
                                        StringBuilder stringBuilderEftSorguNo = new StringBuilder();
                                        int basamakEftSorguNo = stringBuilderEftSorguNo.Length;
                                        for (int i = 0; i < 30 - basamakEftSorguNo; i++)
                                        {
                                            stringBuilderEftSorguNo.Insert(0, " ");
                                        }

                                        stringBuilder.Append("D");
                                        stringBuilder.Append(transferPaymentSAPTable.cpudt);
                                        stringBuilder.Append(item2.bankl.Substring(0, 4));
                                        stringBuilder.Append(item2.bankl.Substring(5));
                                        stringBuilder.Append(item2.iban);
                                        stringBuilder.Append(stringBuilderDeger.ToString().Trim());
                                        stringBuilder.Append("TRY");
                                        stringBuilder.Append(stringBuilderAciklama.ToString());
                                        stringBuilder.Append(stringBuilderAdsoyad.ToString());
                                        stringBuilder.Append(stringBuilderAdress.ToString());
                                        stringBuilder.Append(stringBuilderTelefonNo.ToString());
                                        stringBuilder.Append(stringBuilderVergiNo.ToString());
                                        stringBuilder.Append(stringBuilderAlacakVergiDairesi.ToString());
                                        stringBuilder.Append(stringBuilderSaticiNo.ToString());
                                        stringBuilder.Append(stringBuilderBabaAdi.ToString());
                                        stringBuilder.Append(stringBuilderEmail.ToString());
                                        stringBuilder.Append(stringBuilderReferens.ToString());
                                        stringBuilder.Append(stringBuilderParametre.ToString());
                                        stringBuilder.Append("00");
                                        stringBuilder.Append(rezerv1);
                                        stringBuilder.Append(stringBuilderRezerv2.ToString());
                                        stringBuilder.Append(stringBuilderDurumKodu.ToString());
                                        stringBuilder.Append(stringBuilderEftSorguNo.ToString());
                                        stringBuilder.Append("\r\n");
                                        donguSayisi += 1;

                                        double doubleSt = double.Parse(item2.wrbtr);
                                        decimal value = Convert.ToDecimal(doubleSt);
                                        araToplam = araToplam + value;
                                    }


                                }
                            }

                            AdminUser? nextUser = bllApprovalProcesses.GetNextUser(userId,
                                    approvalProcess?.Id??0, true);
                            if (nextUser != null)
                            {

                                //BURAYA SMS EKLENECEK
                                BLLActions.CeoTable bllCeoTable = new BLLActions.CeoTable(_configuration, _env);
                                Data.Models.CeoTable? ceoTable = bllCeoTable.GetByID(1);
                                if (ceoTable != null && nextUser.Id == ceoTable.userId)
                                {
                                    SMSMessage smsMessage = new SMSMessage();
                                    smsMessage.plannedDate = DateTime.Now;
                                    smsMessage.isSent = false;
                                    smsMessage.smsText = item[0].henum + "Id'li " + item[0].firma +
                                            " firmasına " + item[0].wrbtr + " tutarındaki ödeme onayınızı beklemektedir.";
                                    smsMessage.toNumbers = nextUser.mobile;

                                    BLLActions.SMSMessages bllSMSMessages = new BLLActions.SMSMessages(_configuration, _env);
                                    await bllSMSMessages.Add(smsMessage);


                                }
                                else
                                {
                                    EmailMessage emailMessage = new EmailMessage();

                                    AdminUser? userMail = bllAdminUsers.GetByID(nextUser.Id);

                                    emailMessage.subject = (item[0].henum + " Nolu ödeme onayı hk.");
                                    emailMessage.toAddress = (userMail?.email);
                                    emailMessage.emailText = (getTransferMailString(transferPaymentSAPTable));
                                    emailMessage.isSent = (false);
                                    emailMessage.plannedDate = (DateTime.Now);
                                    emailMessage.mailTuru = (1);

                                    BLLActions.EmailMessages bllEmailMessages = new BLLActions.EmailMessages(_configuration, _env);
                                    await bllEmailMessages.Add(emailMessage);
                                }
                            }
                            else
                            {
                                //
                                EmailMessage emailMessage = new EmailMessage();

                                emailMessage.subject = (item[0].henum + " Nolu ödeme onayı hk.");
                                emailMessage.toAddress = ("finans@askalecimento.com.tr");
                                emailMessage.emailText = (getPaymentMailStringOnaylandi(transferPaymentSAPTable));
                                emailMessage.isSent = (false);
                                emailMessage.plannedDate = DateTime.Now;
                                emailMessage.mailTuru = 1;
                                BLLActions.EmailMessages bllEmailMessages = new BLLActions.EmailMessages(_configuration, _env);

                                await bllEmailMessages.Add(emailMessage);

                            }

                            if (onaylandiMi)
                            {

                                string islemAdetiStr = "";
                                int islemAdeti = donguSayisi.ToString().Length;
                                for (int i = 0; i < 6 - islemAdeti; i++)
                                {
                                    islemAdetiStr += "0";
                                }
                                islemAdetiStr += donguSayisi.ToString();
                                string strAraToplam = "";
                                int islemAdetiAraToplam = araToplam.ToString().Length;
                                for (int i = 0; i < 21 - islemAdetiAraToplam; i++)
                                {
                                    strAraToplam += "0";
                                }
                                strAraToplam += araToplam.ToString().Replace(',', '.');
                                stringBuilder.Append("F" + "0015" + islemAdetiStr + strAraToplam + "\r\n");
                                //DateTimeFormatter dateTimeFormatterFile = DateTimeFormatter.ofPattern("yyyy_MM_dd_HH__mm_ss_SSS");
                                //DateTimeFormatter dateTimeFormatterFileName = DateTimeFormatter.ofPattern("dd.MM.yyyy");
                                string filename = "havale-" + transferPaymentSAPTable.henum + "_Nolu_"
                                        + DateTime.Now.ToString("yyyy_MM_dd_HH__mm_ss_SSS") + ".txt";


                                string directoryPath = Path.Combine(
    _env.IsDevelopment() ? _configuration["spring:sevlet:fileLocation:local"]! :
    _env.IsProduction() ? _configuration["spring:sevlet:fileLocation:server"]! :
                           _configuration["spring:sevlet:fileLocation:test"]!,
    "banka",
    "havale",
    "yuklenen",
    DateTime.Now.ToString("dd.MM.yyyy")
);
                                if (!Directory.Exists(directoryPath))
                                {
                                    Directory.CreateDirectory(directoryPath);
                                }
                                string pathFile = Path.Combine(directoryPath, filename);

                                try
                                {
                                    using (StreamWriter writer = new StreamWriter(pathFile))
                                    {
                                        writer.Write(stringBuilder.ToString());
                                    }
                                }
                                catch (IOException ex)
                                {
                                    Console.WriteLine(ex.ToString());
                                }
                                if (listcompanykavcim.Contains(companyName.ToUpper()))
                                {
                                    _server.SendFileWithRetry(
                                        _configuration["sftp:host"] ?? "",
                                        _configuration["sftp:kavcimcimento:username"] ?? "",
                                        _configuration["sftp:kavcimcimento:password"] ?? "",
                                        pathFile,
                                        $"kavcimcimentotos/{filename}"
                                    );


                                }
                                else if (companyName.ToUpper().Equals("ACAT"))
                                {
                                    _server.SendFileWithRetry(
                                                                            _configuration["sftp:host"] ?? "",
                                                                            _configuration["sftp:askaleatiktos:username"] ?? "",
                                                                            _configuration["sftp:askaleatiktos:password"] ?? "",
                                                                            pathFile,
                                                                            $"askaleatiktos/{filename}"
                                                                        );

                                }
                                else if (companyName.ToUpper().Equals("ACPT"))
                                {

                                    _server.SendFileWithRetry(
                                     _configuration["sftp:host"] ?? "",
                                     _configuration["sftp:askalepetroltos:username"] ?? "",
                                     _configuration["sftp:askalepetroltos:password"] ?? "",
                                     pathFile,
                                     $"askalepetroltos/{filename}"
                                 );
                                    

                                }
                                else if (companyName.ToUpper().Equals("ACSG"))
                                {
                                    _server.SendFileWithRetry(
                                    _configuration["sftp:host"] ?? "",
                                    _configuration["sftp:askalesigortatos:username"] ?? "",
                                    _configuration["sftp:askalesigortatos:password"] ?? "",
                                    pathFile,
                                    $"askalesigortatos/{filename}"
                                );

                                   

                                }
                                else if (companyName.ToUpper().Equals("ACPZ"))
                                {
                                    _server.SendFileWithRetry(
                                    _configuration["sftp:host"] ?? "",
                                    _configuration["sftp:askalepazarlamatos:username"] ?? "",
                                    _configuration["sftp:askalepazarlamatos:password"] ?? "",
                                    pathFile,
                                    $"askalepazarlamatos/{filename}"
                                );

                                  

                                }
                                else if (companyName.ToUpper().Equals("AC68"))
                                {

                                    _server.SendFileWithRetry(
                                   _configuration["sftp:host"] ?? "",
                                   _configuration["sftp:nuryoltos:username"] ?? "",
                                   _configuration["sftp:nuryoltos:password"] ?? "",
                                   pathFile,
                                   $"nuryoltos/{filename}"
                               );

                                }
                                else if (companyName.ToUpper().Equals("ACFC"))
                                {
                                    _server.SendFileWithRetry(
                                   _configuration["sftp:host"] ?? "",
                                   _configuration["sftp:futurechemtos:username"] ?? "",
                                   _configuration["sftp:futurechemtos:password"] ?? "",
                                   pathFile,
                                   $"futurechemtos/{filename}"
                               );

                                }
                                else
                                {
                                    _server.SendFileWithRetry(
                                _configuration["sftp:host"] ?? "",
                                _configuration["sftp:askalecimento:username"] ?? "",
                                _configuration["sftp:askalecimento:password"] ?? "",
                                pathFile,
                                $"askalecimentotos/{filename}"
                            );

                                }
                            }

                        }
                    }
                    return true;

                }
                else
                {
                    List<Data.Models.TransferPaymentKalemSAPTable> liste = new List<Data.Models.TransferPaymentKalemSAPTable>();
                    foreach (int id in list)
                    {
                        //DateTimeFormatter dateTimeFormatter = DateTimeFormatter.ofPattern("yyyyMMdd");
                        //DateTimeFormatter dateTimeFormatter2 = DateTimeFormatter.ofPattern("HH:mm:ss");
                        Data.Models.TransferPaymentKalemSAPTable? transferPaymentKalemSAPTable = GetByID(id);
                        if (transferPaymentKalemSAPTable != null)
                        {
                            liste.Add(transferPaymentKalemSAPTable!);
                            transferPaymentKalemSAPTable!.approval = false;
                            transferPaymentKalemSAPTable!.currentStateId = 2;
                            Data.Models.TransferPaymentKalemSAPTable transferPaymentKalemSAPTableSaved =
                                  await Update(transferPaymentKalemSAPTable);

                            BLLActions.ActiveTransferDetails bllActiveTransferDetails = new BLLActions.ActiveTransferDetails(_configuration, _env);
                            ActiveTransferDetail? activePaymentDetail = bllActiveTransferDetails
                                    .findAllByActiveTransferIdAndApprovedAndUserIdAndEnabled(id, null, userId, true);
                            if (activePaymentDetail != null)
                            {
                                activePaymentDetail.approved = false;
                                activePaymentDetail.isReplied = true;
                                activePaymentDetail.replyDate = DateTime.Now;
                                await bllActiveTransferDetails.Update(activePaymentDetail);

                                BLLActions.SAPConnectionData bllSapConnection = new BLLActions.SAPConnectionData(_configuration, _env);
                                SapConnection? sapConn = bllSapConnection.sapConnection(_configuration, _env);
                                if (sapConn != null)
                                {
                                    sapConn.Connect();
                                    ISapFunction sapFunction = sapConn.CreateFunction("ZWEBI044");
                                    TransferPaymentKalemSAPTableApproveParams inputparams = new TransferPaymentKalemSAPTableApproveParams
                                    {
                                        apikey = "Ba4nV`=tuPps}^+}py6xVTh,2;]p7zUE",
                                        henum = transferPaymentKalemSAPTableSaved.henum,
                                        onaylayan = user?.name ?? "",
                                        onaysekli = "R",
                                        posnr = transferPaymentKalemSAPTableSaved.posnr,
                                        saat = DateTime.Now.ToString("HH:mm:ss"),
                                        tarih = DateTime.Now.ToString("yyyyMMdd"),
                                        bittimi = "X"
                                    };
                                    sapFunction.Invoke<string>(input: inputparams);
                                    sapConn.Disconnect();
                                }

                            }
                        }
                    }
                    
                    Dictionary<string, List<Data.Models.TransferPaymentKalemSAPTable>> listedGroup = liste.GroupBy(u => u.henum).ToDictionary(g => g.Key, g => g.ToList());
                    foreach (List<Data.Models.TransferPaymentKalemSAPTable> item in listedGroup.Values)
                    {
                        EmailMessage emailMessage = new EmailMessage();

                        BLLActions.TransferPaymentSAPTable bllTransferPaymentSAPTable = new BLLActions.TransferPaymentSAPTable(_configuration,_env);
                        Data.Models.TransferPaymentSAPTable transferPaymentSAPTable = bllTransferPaymentSAPTable
                                .GetByHENUM(item[0].henum);

                        emailMessage.subject=(item[0].henum + " Nolu ödeme onayı hk.");
                        emailMessage.toAddress="finans@askalecimento.com.tr";
                        emailMessage.emailText=getPaymentMailStringOnaylandi(transferPaymentSAPTable);
                        emailMessage.isSent=false;
                        emailMessage.plannedDate=DateTime.Now;
                        emailMessage.mailTuru=1;

                        BLLActions.EmailMessages bllEmailMessages = new BLLActions.EmailMessages(_configuration,_env);
                        await bllEmailMessages.Add(emailMessage);
                    }
                    return false;
                }
            }

            public string? getPaymentMailStringOnaylandi(Data.Models.TransferPaymentSAPTable transferPaymentSAPTable)
            {

                string mailString = "<link rel='stylesheet' href='https://cdn.jsdelivr.net/npm/bootstrap@4.0.0/dist/css/bootstrap.min.css' integrity='sha384-Gn5384xqQ1aoWXA+058RXPxPg6fy4IWvTNh0E263XmFcJlSAwiGgFAW/dAiS6JXm' crossorigin='anonymous'>"
                        + "<h3>Sayın Yetkili</h3><br/><br/>" + "<div>Aşağıdaki ödemelerin onayı aşağıdaki gibidir.</div>"
                        + "<table class='table table-striped table-bordered table-hover' style='margin-bottom:0px !important;' id='tableMain' >"
                        + "<thead>" + "<tr>" + "<th style ='text-align: left;border: 1px solid black;'>ID</th>" +

                        "<th style ='text-align: left;border: 1px solid black;'>Sap Belge No</th>"
                        + "<th style ='text-align: left;border: 1px solid black;'>Kalem</th>" +

                        "<th style ='text-align: left;border: 1px solid black;'>Satıcı No</th>"
                        + "<th style ='text-align: left;border: 1px solid black;'>Satıcı Adı</th>"
                        + "<th style ='text-align: right;border: 1px solid black;'>Tutar</th>"
                        + "<th style ='text-align: center;border: 1px solid black;'>Oluşturan Kişi SAP</th>"
                        + "<th style ='text-align: center;border: 1px solid black;'>Onay Durumu</th>" +

                        "</tr>" + "</thead>" + "<tbody>";

                List<Data.Models.TransferPaymentKalemSAPTable> listTransferPaymentKalemSAPTable = findAllByHenum(transferPaymentSAPTable.henum);
                foreach (Data.Models.TransferPaymentKalemSAPTable item in listTransferPaymentKalemSAPTable)
                {
                    mailString += "<tr class='datarow'>" +

                            "<td style ='text-align: left;border: 1px solid black;'>" + item.Id + "</td>"
                            + "<td style ='text-align: left;border: 1px solid black;'>" + item.henum + "</td>"
                            + "<td style ='text-align: left;border: 1px solid black;'>" + item.posnr + "</td>"
                            + "<td style ='text-align: left;border: 1px solid black;'>" + item.lifnr + "</td>"
                            + "<td style ='text-align: left;border: 1px solid black;'>" + item. firma + "</td>"
                            + "<td style ='text-align: right;border: 1px solid black;'>" + item.wrbtr + "</td>"
                            + "<td style ='text-align: center;border: 1px solid black;' >" + transferPaymentSAPTable.usnam
                            + "</td>";
                    if (item.currentStateId == 4)
                    {
                        mailString += "<td style ='text-align: center;border: 1px solid black;' >Onaylandı</td>";
                    }
                    else if (item.currentStateId == 2)
                    {
                        mailString += "<td style ='text-align: center;border: 1px solid black;' >Red Edildi</td>";
                    }
                    else
                    {
                        mailString += "<td style ='text-align: center;border: 1px solid black;' ></td>";
                    }

                    mailString += "</tr>";
                }

                mailString += "</tbody>" + "</table>";

                mailString += "<br /><br /> ----------------------------------------- <br /><br /> " + " Saygılarımızla.";
                return mailString;
            }

            public string? getTransferMailString(Data.Models.TransferPaymentSAPTable transferPaymentSAPTable)
            {
                var okLink = CommonConstants.OkNoLinks.OK_LINK+ "/accountpayment/replyfromouttransfer?answer=1&guid="
                 + transferPaymentSAPTable.henum;
                var noLink = CommonConstants.OkNoLinks.OK_LINK + "/it-portal/accountpayment/replyfromouttransfer?answer=0&guid="
                        + transferPaymentSAPTable.henum;
                string mailString = "<link rel='stylesheet' href='https://cdn.jsdelivr.net/npm/bootstrap@4.0.0/dist/css/bootstrap.min.css' integrity='sha384-Gn5384xqQ1aoWXA+058RXPxPg6fy4IWvTNh0E263XmFcJlSAwiGgFAW/dAiS6JXm' crossorigin='anonymous'>"
                        + "<div>Aşağıdaki ödemeler onayınızı beklemektedir.</div>"
                        + "<table class='table table-striped table-bordered table-hover' style='margin-bottom:0px !important;' id='tableMain'>"
                        + "<thead>" + "<tr>"
                        + "<th style ='text-align: left;border: 1px solid black;'><a class='btn btn-primary' href=\"" + okLink
                        + "\">TÜMÜNÜ ONAYLA</a></th>"
                        + "<th style ='text-align: left;border: 1px solid black;'><a class='btn btn-danger' href=\"" + noLink
                        + "\">TÜMÜNÜ REDDET</a></th>" + "<th style ='text-align: left;border: 1px solid black;'>ID</th>" +

                        "<th style ='text-align: left;border: 1px solid black;'>Sap Belge No</th>"
                        + "<th style ='text-align: left;border: 1px solid black;'>Kalem</th>" +

                        "<th style ='text-align: left;border: 1px solid black;'>Haval Kişi No</th>"
                        + "<th style ='text-align: left;border: 1px solid black;'>Havale Kişisi</th>"
                        + "<th style ='text-align: right;border: 1px solid black;'>Tutar</th>"
                        + "<th style ='text-align: center;border: 1px solid black;'>Oluşturan Kişi SAP</th>" +

                        "</tr>" + "</thead>" + "<tbody>";
                List<Data.Models.TransferPaymentKalemSAPTable> listTransferPaymentKalemSAPTable = findAllByHenum(transferPaymentSAPTable.henum);
                foreach (Data.Models.TransferPaymentKalemSAPTable item in listTransferPaymentKalemSAPTable)
                {

                    mailString += "<tr class='datarow'>"
                            + "<td  style ='text-align: left;border: 1px solid black;'><a class='btn btn-primary' href=\""
                            + okLink + "&posnr=" + item.posnr + "\">ONAYLA</a></td>"
                            + "<td style ='text-align: left;border: 1px solid black;'><a class='btn btn-danger' href=\""
                            + noLink + "&posnr=" + item.posnr + "\">REDDET</a></td>"
                            + "<td  style ='text-align: left;border: 1px solid black;'>" + item.Id.ToString() + "</td>"
                            + "<td style ='text-align: left;border: 1px solid black;'>" + item.henum + "</td>"
                            + "<td style ='text-align: left;border: 1px solid black;'>" + item.posnr + "</td>"
                            + "<td style ='text-align: left;border: 1px solid black;'>" + item.lifnr + "</td>"
                            + "<td style ='text-align: left;border: 1px solid black;'>" + item.firma + "</td>"
                            + "<td style ='text-align: right;border: 1px solid black;'>" + item.wrbtr + "</td>"
                            + "<td  style = 'text-align:center;border: 1px solid black;' >" + transferPaymentSAPTable.usnam
                            + "</td>" +

                            "</tr>";
                }

                mailString += "</tbody>" + "</table>";

                mailString += "<br /><br /> ----------------------------- <br /><br /> " + " Saygılarımızla.";
                return mailString;
            }

            private List<Data.Models.TransferPaymentKalemSAPTable> findAllByHenum(string henum)
            {
                return dal.Get(u => u.enabled && u.henum == henum).ToList();
            }
        }
    }

}
