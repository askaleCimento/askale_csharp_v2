
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
using Microsoft.Extensions.Primitives;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using SapNwRfc;
using System;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AskalePortal.BLL.BLLActions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;


namespace AskalePortal.BLL
{

    public partial class BLLActions
    {
        public class AccountPaymentKalemSAPTable : BaseBLL<AskalePortal.Data.Models.AccountPaymentKalemSAPTable>
        {
            private readonly IConfiguration _configuration;
            private readonly IWebHostEnvironment _env;
            private readonly IMapper _mapper;
            private readonly ISftpServer _server;
            public AccountPaymentKalemSAPTable(IConfiguration configuration, IWebHostEnvironment env, IMapper mapper, ISftpServer server) : base(configuration, env)
            {
                _configuration = configuration;
                _env = env;
                _mapper = mapper;
                _server = server;
            }
            public List<AskalePortal.Data.Models.AccountPaymentKalemSAPTable> GetByUserId(int userId)
            {
                return dal.Get(u => u.enabled == true && u.currentStateId == 1 && u.currentUserId == userId).ToList();
            }

            public List<AskalePortal.Data.Models.AccountPaymentKalemSAPTable> GetByOENUM(string oENUM)
            {
                return dal.Get(u => u.enabled == true && u.oenum == oENUM && u.currentStateId == 1).ToList();
            }
            public List<AskalePortal.Data.Models.AccountPaymentKalemSAPTable> GetByOENUMFinished(string oENUM)
            {
                return dal.Get(u => u.enabled == true && u.oenum == oENUM).ToList();
            }
            public List<AskalePortal.Data.Models.AccountPaymentKalemSAPTable> GetByUserId(int userId, string name, int activePage, int pageSize)
            {
                return dal.Get(u => u.enabled == true && u.currentStateId == 1 && u.currentUserId == userId && u.name1.Contains(name)).OrderByDescending(u => u.oenum).Skip(activePage * pageSize).Take(pageSize).ToList();
            }

            public List<AskalePortal.Data.Models.AccountPaymentKalemSAPTable> GetByActive(int activePage, int pageSize)
            {
                return dal.Get(u => u.currentStateId == 1 && u.enabled == true).Skip(activePage * pageSize).Take(pageSize).ToList();
            }

            public List<AskalePortal.Data.Models.AccountPaymentKalemSAPTable> GetByFinished(string name, int activePage, int pageSize)
            {
                return dal.Get(u => u.enabled == true && u.currentStateId != 1 && u.name1.Contains(name)).OrderByDescending(u => u.oenum).Skip(activePage * pageSize).Take(pageSize).ToList();
            }

            public object GetByActive(int activePage, int pageSize, string name)
            {
                return dal.Get(u => u.currentStateId == 1 && u.enabled == true && u.oenumNavigation.name1.Contains(name)).OrderBy(u => u.oenum).Skip(activePage * pageSize).Take(pageSize).ToList();
            }

            public List<AskalePortal.Data.Models.AccountPaymentKalemSAPTable> GetByOENUMWithUserID(string guid, int userId)
            {
                return dal.Get(u => u.enabled == true && u.oenum == guid && u.currentStateId == 1 && u.currentUserId == userId).ToList();
            }

            public object GetByFinishedByFinansDanismani(int userId, string name, int activePage, int pageSize)
            {
                return dal.Get(u => u.enabled == true && u.currentStateId != 1 && u.name1.Contains(name) && u.ActivePaymentDetail.Any(y => y.userId == userId)).OrderByDescending(u => u.oenum).Skip(activePage * pageSize).Take(pageSize).ToList();

            }

            public PageReturn<AccountPaymentKalemActiveDto> completed(FilterPageParam<AccountPaymentKalemCompletedDtoParameter> filterPageParam, int userId)
            {
                PageReturn<AccountPaymentKalemActiveDto>? result = new PageReturn<AccountPaymentKalemActiveDto>();
                int pageSize = filterPageParam.size ?? 20;
                int pageNumber = filterPageParam.page ?? 0;

                string name1 = filterPageParam?.liste?.name1 ?? "";
                int? filterUserId = filterPageParam?.liste?.userId;
                BLLActions.AdminUsers bllAdminUsers = new BLLActions.AdminUsers(_configuration, _env, _mapper);
                AdminUser? user = bllAdminUsers.GetByID(userId);

                if (user != null && user.username.Contains("omer.baktir"))
                {
                    var query =
                        from a in dal.dB.AccountPaymentKalemSAPTable
                        join b in dal.dB.AccountPaymentSAPTable
                            on a.oenum equals b.oenum
                        join c in dal.dB.ActivePaymentDetail
                            on a.Id equals c.activePaymentId
                        join d in dal.dB.AdminUser
                            on c.userId equals d.Id
                        where a.currentStateId != 1
                              && a.enabled
                              && b.enabled
                              && c.userId == userId
                              && (
                                    (a.name1 != null && a.name1.Contains(name1)) ||
                                    (name1 == "" && a.name1 == null)
                                 )
                        orderby a.oenum descending
                        select new AccountPaymentKalemActiveDto
                        {
                            id = a.Id,
                            oenum = a.oenum,
                            posnr = a.posnr,
                            lifnr = a.lifnr,
                            name1 = a.name1,
                            wrbtr = a.wrbtr,
                            usnam = b.usnam,
                            currentStateId = a.currentStateId,
                            onayKimde = d.name,
                            znot = b.znot

                        };

                    result.content = query
                     .Skip(pageSize * pageNumber).Take(pageSize).ToList();
                    result.totalElements = query.Count();
                    result.number = result.content.Count();
                    result.size = pageSize;

                    return result;
                }
                else
                {
                    var query =
         from a in dal.dB.AccountPaymentKalemSAPTable
         join b in dal.dB.AccountPaymentSAPTable
             on a.oenum equals b.oenum
         where a.currentStateId != 1
               && a.enabled == true
               && b.enabled == true
               && (
                     (a.name1 != null && a.name1.Contains(name1)) || (name1 == "" && a.name1 == null)
                  )
         orderby a.oenum descending
         select new AccountPaymentKalemActiveDto
         {
             id = a.Id,
             oenum = a.oenum,
             posnr = a.posnr,
             lifnr = a.lifnr,
             name1 = a.name1,
             wrbtr = a.wrbtr,
             usnam = b.usnam,
             currentStateId = a.currentStateId,
             onayKimde = a.name1,
             znot = b.znot
         };

                    result.content = query
                      .Skip(pageSize * pageNumber).Take(pageSize).ToList();
                    result.totalElements = query.Count();
                    result.number = result.content.Count();
                    result.size = pageSize;

                    return result;
                }
            }

            public AccountPaymentKalemMyListDetailDto mylistdetail(int id)
            {
                AccountPaymentKalemMyListDetailDto? dto = MyListDetail(id);
                if (dto != null)
                {
                    BLLActions.ActivePaymentDetails bllActivePaymentDetails = new BLLActions.ActivePaymentDetails(_configuration, _env);
                    List<ActivePaymentDetail> activePaymentDetails = bllActivePaymentDetails.GetByAccountPaymentId(id);

                    List<ApprovedPerson> approvedPersons = new List<ApprovedPerson>();
                    BLLActions.Companies bllCompanies = new BLLActions.Companies(_configuration, _env, _mapper);
                    foreach (ActivePaymentDetail activePaymentDetail in activePaymentDetails)
                    {
                        ApprovedPerson approvedPerson = new ApprovedPerson
                        {
                            companyName =
                                bllCompanies.getByUserId(activePaymentDetail.userId),

                            dateTime =
                                activePaymentDetail.replyDate.HasValue
                                    ? activePaymentDetail.replyDate.Value
                                        .ToString("yyyy-MM-ddTHH:mm:ss")
                                    : DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"),

                            process = activePaymentDetail.approved,
                            userId = activePaymentDetail.userId
                        };

                        approvedPersons.Add(approvedPerson);
                    }

                    dto.listApprovedPerson = approvedPersons;
                    return dto;
                }
                else
                {
                    return new AccountPaymentKalemMyListDetailDto();
                }
            }

            public AccountPaymentKalemMyListDetailDto? MyListDetail(int id)
            {
                return
                    (from a in dal.dB.AccountPaymentKalemSAPTable
                     from b in dal.dB.AccountPaymentSAPTable
                     where a.enabled
                           && b.enabled
                           && a.oenum == b.oenum
                           && a.Id == id
                     orderby a.oenum descending
                     select new AccountPaymentKalemMyListDetailDto
                     {
                         id = a.Id,
                         oenum = a.oenum,
                         posnr = a.posnr,
                         lifnr = a.lifnr,
                         name1 = a.name1,
                         wrbtr = a.wrbtr,
                         usnam = b.usnam,
                         currentStateId = a.currentStateId,
                         aenam = b.aenam,
                         cpudt = b.cpudt,
                         iban = a.iban,
                         banka = a.banka,
                         brnch = a.brnch,
                         bankn = a.bankn
                     }).FirstOrDefault();
            }

            public List<AccountPaymentKalemActiveDto> mylist(FilterParam<AccountPaymentKalemCompletedDtoParameter> filterParam)
            {
                string? name1 = filterParam?.liste?.name1;
                int? userId = filterParam?.liste?.userId;
                List<AccountPaymentKalemActiveDto> list = (from a in dal.dB.AccountPaymentKalemSAPTable
                                                           join b in dal.dB.AccountPaymentSAPTable
                                                               on a.oenum equals b.oenum
                                                           join c in dal.dB.ActivePaymentDetail
                                                               on a.Id equals c.activePaymentId
                                                           join d in dal.dB.AdminUser
                                                               on c.userId equals d.Id
                                                           where a.currentStateId == 1
                                                                 && a.enabled
                                                                 && b.enabled
                                                                 && c.approved == null
                                                                 && a.currentUserId == userId
                                                                 && (
                                                                      a.name1.Contains(name1 ?? "") ||
                                                                      (name1 == "" && a.name1 == null)
                                                                    )
                                                           orderby a.oenum descending
                                                           select new AccountPaymentKalemActiveDto
                                                           {
                                                               id = a.Id,
                                                               oenum = a.oenum,
                                                               posnr = a.posnr,
                                                               lifnr = a.lifnr,
                                                               name1 = a.name1,
                                                               wrbtr = a.wrbtr,
                                                               usnam = b.usnam,
                                                               currentStateId = a.currentStateId,
                                                               onayKimde = d.name,
                                                               znot = b.znot
                                                           }).ToList();

                return list;
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
                    List<Data.Models.AccountPaymentKalemSAPTable> liste = new List<Data.Models.AccountPaymentKalemSAPTable>();
                    foreach (int id in list)
                    {

                        Data.Models.AccountPaymentKalemSAPTable? accountPaymentKalemSAPTable = GetByID(id);
                        if (accountPaymentKalemSAPTable != null)
                        {
                            liste.Add(accountPaymentKalemSAPTable);
                        }
                    }

                    double sayi = Math.Ceiling((((double)liste.Count) / 10.0));
                    int dongu = (int)sayi;
                    for (int k = 0; k < dongu; k++)
                    {
                        int skipSayasi = k * 10;
                        Dictionary<string, List<Data.Models.AccountPaymentKalemSAPTable>> listedGroup = liste.Skip(skipSayasi).Take(10)
                                .GroupBy(u => u.oenum).ToDictionary(g => g.Key, g => g.ToList());
                        foreach (List<Data.Models.AccountPaymentKalemSAPTable> item in listedGroup.Values)
                        {
                            BLLActions.AccountPaymentSAPTable bllAccountPaymentSAPTable = new BLLActions.AccountPaymentSAPTable(_configuration, _env, _mapper, _server);
                            Data.Models.AccountPaymentSAPTable? accountPaymentSAPTable = bllAccountPaymentSAPTable.GetByOENUM(item[0].oenum);
                            string companyName = accountPaymentSAPTable?.bukrs ?? "";
                            BLLActions.Companies bllCompanies = new BLLActions.Companies(_configuration, _env, _mapper);
                            Company company = bllCompanies.getByVkorgCompany(companyName);
                            StringBuilder stringBuilder = new StringBuilder();
                            bool onaylandiMi = false;
                            int donguSayisi = 0;
                            decimal araToplam = 0;
                            int processTypeId = (int)CommonConstants.APPROVAL_PROCESSES.ACCOUNT_PAYMENT;
                            BLLActions.ApprovalProcesses bllApprovalProcesses = new BLLActions.ApprovalProcesses(_configuration, _env, _mapper);
                            ApprovalProcess? approvalProcess = bllApprovalProcesses
                                    .findByCompanyIdAndTypeIdAndDagitimKanaliAndEnabled(company.Id, processTypeId, "A1",
                                            true);
                            stringBuilder.Append("H");
                            stringBuilder.Append(accountPaymentSAPTable?.cpudt);
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
                            AdminUser? nextUser = null;
                            foreach (Data.Models.AccountPaymentKalemSAPTable item2 in item)
                            {
                                try
                                {
                                    BLLActions.ApprovalProcessDetails bllApprovalProcessDetails = new BLLActions.ApprovalProcessDetails(_configuration, _env, _mapper);


                                    nextUser = bllApprovalProcessDetails.GetNextUser(item2.currentUserId,
                                            approvalProcess?.Id, true);
                                    if (nextUser != null)
                                    {
                                        BLLActions.ActivePaymentDetails bllActivePaymentDetails = new BLLActions.ActivePaymentDetails(_configuration, _env);
                                        ActivePaymentDetail activePaymentDetail = bllActivePaymentDetails
                                                .findAllByActivePaymentIdAndApprovedAndUserId(item2.Id, null, userId);
                                        activePaymentDetail.approved = true;
                                        activePaymentDetail.isReplied = true;
                                        activePaymentDetail.replyDate = DateTime.Now;
                                        await bllActivePaymentDetails.Update(activePaymentDetail);
                                        ActivePaymentDetail nextActivePaymenDetail = new ActivePaymentDetail();
                                        nextActivePaymenDetail.approved = null;
                                        nextActivePaymenDetail.replyDate = null;
                                        nextActivePaymenDetail.isReplied = false;
                                        nextActivePaymenDetail.activePaymentId = item2.Id;
                                        nextActivePaymenDetail.guid = Guid.NewGuid();
                                        nextActivePaymenDetail.userId = nextUser.Id;
                                        await bllActivePaymentDetails.Add(nextActivePaymenDetail);
                                        item2.currentUserId = nextUser.Id;
                                        await Update(item2);

                                        BLLActions.SAPConnectionData bllSapConnection = new BLLActions.SAPConnectionData(_configuration, _env);
                                        SapConnection? sapConn = bllSapConnection.sapConnection(_configuration, _env);
                                        if (sapConn != null)
                                        {

                                            sapConn.Connect();
                                            ISapFunction sapFunction = sapConn.CreateFunction("ZWEBI043");
                                            AccountPaymentKalemSAPTableApproveParams inputparams = new AccountPaymentKalemSAPTableApproveParams
                                            {
                                                apikey = "Ba4nV`=tuPps}^+}py6xVTh,2;]p7zUE",
                                                oenum = item2.oenum,
                                                onaylayan = user?.name ?? "",
                                                onaysekli = "O",
                                                posnr = item2.posnr,
                                                saat = DateTime.Now.ToString("HH:mm:ss"),
                                                tarih = DateTime.Now.ToString("yyyyMMdd")
                                            };


                                            sapFunction.Invoke<string>(input: inputparams);
                                            sapConn.Disconnect();

                                        }
                                    }
                                    else
                                    {
                                        BLLActions.SAPConnectionData bllSapConnection = new BLLActions.SAPConnectionData(_configuration, _env);
                                        SapConnection? sapConn = bllSapConnection.sapConnection(_configuration, _env);
                                        if (sapConn != null)
                                        {

                                            sapConn.Connect();
                                            ISapFunction sapFunction = sapConn.CreateFunction("ZWEBI043");
                                            AccountPaymentKalemSAPTableApproveParams inputparams = new AccountPaymentKalemSAPTableApproveParams
                                            {
                                                apikey = "Ba4nV`=tuPps}^+}py6xVTh,2;]p7zUE",
                                                oenum = item2.oenum,
                                                onaylayan = user?.name ?? "",
                                                onaysekli = "O",
                                                posnr = item2.posnr,
                                                saat = DateTime.Now.ToString("HH:mm:ss"),
                                                tarih = DateTime.Now.ToString("yyyyMMdd")
                                            };


                                            sapFunction.Invoke<string>(input: inputparams);
                                            sapConn.Disconnect();

                                        }

                                        item2.approval = true;
                                        item2.currentStateId = 4;
                                        await Update(item2);
                                        BLLActions.ActivePaymentDetails bllActivePaymentDetails = new BLLActions.ActivePaymentDetails(_configuration, _env);
                                        ActivePaymentDetail activePaymentDetail = bllActivePaymentDetails
                                                .findAllByActivePaymentIdAndApprovedAndUserId(item2.Id, null, userId);
                                        activePaymentDetail.approved = true;
                                        activePaymentDetail.isReplied = true;
                                        activePaymentDetail.replyDate = DateTime.Now;
                                        await bllActivePaymentDetails.Update(activePaymentDetail);
                                        onaylandiMi = true;
                                        StringBuilder stringBuilderDeger = new StringBuilder();
                                        int basamak = item2.wrbtr.Length - 1;
                                        for (int i = 0; i < 21 - basamak; i++)
                                        {
                                            stringBuilderDeger.Append("0");
                                        }
                                        stringBuilderDeger.Append(item2.wrbtr);

                                        StringBuilder? stringBuilderAciklama = null;
                                        if (accountPaymentSAPTable?.znot == null)
                                        {
                                            stringBuilderAciklama = new StringBuilder();
                                            int basamakaciklama = stringBuilderAciklama.Length;
                                            for (int i = 0; i < 100 - basamakaciklama; i++)
                                            {
                                                stringBuilderAciklama.Insert(0, " ");

                                            }
                                        }
                                        else
                                        {
                                            stringBuilderAciklama = new StringBuilder(accountPaymentSAPTable.znot);
                                            int basamakaciklama = stringBuilderAciklama.Length;
                                            for (int i = 0; i < 100 - basamakaciklama; i++)
                                            {
                                                stringBuilderAciklama.Insert(0, " ");

                                            }
                                        }

                                        StringBuilder stringBuilderAdsoyad = new StringBuilder(item2.name1);
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

                                        StringBuilder stringBuilderSaticiNo = new StringBuilder(item2.lifnr);
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
                                        string rezerv1 = accountPaymentSAPTable?.iban ?? "";

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
                                        stringBuilder.Append(accountPaymentSAPTable?.cpudt);
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
                                catch (Exception e)
                                {
                                    Console.WriteLine(item2.Id.ToString()
                                            + " id'li EFT onaylanmadı. Hata: " + e.Message);
                                    return false;
                                }

                            }

                            if (nextUser != null)
                            {

                                // BURAYA SMS EKLENECEK
                                BLLActions.CeoTable bllCeoTable = new BLLActions.CeoTable(_configuration, _env);
                                Data.Models.CeoTable? ceoTable = bllCeoTable.GetByID(1);
                                if (nextUser.Id == (ceoTable?.userId ?? 0))
                                {
                                    SMSMessage smsMessage = new SMSMessage();
                                    smsMessage.plannedDate = DateTime.Now;
                                    smsMessage.isSent = false;
                                    smsMessage.smsText = (
                                            item[0].oenum + "Id'li " + item[0].name1 + " firmasına "
                                                    + item[0].wrbtr + " tutarındaki ödeme onayınızı beklemektedir.");
                                    smsMessage.toNumbers = nextUser.mobile;
                                    BLLActions.SMSMessages bllSMSMessages = new BLLActions.SMSMessages(_configuration, _env);
                                    await bllSMSMessages.Add(smsMessage);

                                }
                                else
                                {
                                    EmailMessage emailMessage = new EmailMessage();
                                    AdminUser? userMail = bllAdminUsers.GetByID(nextUser.Id);

                                    emailMessage.subject = item[0].oenum + " Nolu ödeme onayı hk.";
                                    emailMessage.toAddress = userMail?.email;
                                    emailMessage.emailText = getPaymentMailString(accountPaymentSAPTable, userMail);
                                    emailMessage.isSent = false;
                                    emailMessage.plannedDate = DateTime.Now;
                                    emailMessage.mailTuru = 1;
                                    BLLActions.EmailMessages bllEmailMessages = new BLLActions.EmailMessages(_configuration, _env);
                                    await bllEmailMessages.Add(emailMessage);
                                }
                            }
                            else
                            {

                                EmailMessage emailMessage = new EmailMessage();

                                emailMessage.subject = item[0].oenum + " Nolu ödeme onayı hk.";
                                emailMessage.toAddress = "finans@askalecimento.com.tr";
                                emailMessage.emailText = getPaymentMailStringOnaylandi(accountPaymentSAPTable);
                                emailMessage.isSent = false;
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
                                //DateTimeFormatter dateTimeFormatterFile = DateTimeFormatter
                                //        .ofPattern("yyyy_MM_dd_HH__mm_ss_SSS");
                                //DateTimeFormatter dateTimeFormatterFileName = DateTimeFormatter.ofPattern("dd.MM.yyyy");
                                string filename = "odeme-" + accountPaymentSAPTable?.oenum + "_Nolu_"
                                        + DateTime.Now.ToString("yyyy_MM_dd_HH__mm_ss_SSS") + ".txt";

                                string directoryPath = Path.Combine(
     _env.IsDevelopment() ? _configuration["spring:sevlet:fileLocation:local"]! :
     _env.IsProduction() ? _configuration["spring:sevlet:fileLocation:server"]! :
                            _configuration["spring:sevlet:fileLocation:test"]!,
     "banka",
     "odeme",
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




                                if (listcompanykavcim.Contains(companyName.ToUpperInvariant()))
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
                    List<Data.Models.AccountPaymentKalemSAPTable> liste = new List<Data.Models.AccountPaymentKalemSAPTable>();
                    foreach (int id in list)
                    {
                        try
                        {
                          
                            Data.Models.AccountPaymentKalemSAPTable? accountPaymentKalemSAPTable = GetByID(id);
                            if (accountPaymentKalemSAPTable != null)
                            {
                                liste.Add(accountPaymentKalemSAPTable ?? new Data.Models.AccountPaymentKalemSAPTable());
                                accountPaymentKalemSAPTable!.approval = false;
                                accountPaymentKalemSAPTable.currentStateId = 2;


                                Data.Models.AccountPaymentKalemSAPTable accountPaymentKalemSAPTableSaved = await Update(accountPaymentKalemSAPTable);

                                BLLActions.ActivePaymentDetails bllActivePaymentDetails = new BLLActions.ActivePaymentDetails(_configuration, _env);
                                ActivePaymentDetail activePaymentDetail = bllActivePaymentDetails
                                        .findAllByActivePaymentIdAndApprovedAndUserId(id, null, userId);
                                if (activePaymentDetail != null)
                                {
                                    activePaymentDetail.approved = false;
                                    activePaymentDetail.isReplied = true;
                                    activePaymentDetail.replyDate = DateTime.Now;
                                    await bllActivePaymentDetails.Update(activePaymentDetail);


                                    BLLActions.SAPConnectionData bllSapConnection = new BLLActions.SAPConnectionData(_configuration, _env);
                                    SapConnection? sapConn = bllSapConnection.sapConnection(_configuration, _env);
                                    if (sapConn != null)
                                    {

                                        sapConn.Connect();
                                        ISapFunction sapFunction = sapConn.CreateFunction("ZWEBI043");
                                        AccountPaymentKalemSAPTableApproveParams inputparams = new AccountPaymentKalemSAPTableApproveParams
                                        {
                                            apikey = "Ba4nV`=tuPps}^+}py6xVTh,2;]p7zUE",
                                            oenum = accountPaymentKalemSAPTableSaved.oenum,
                                            onaylayan = user?.name ?? "",
                                            onaysekli = "R",
                                            posnr = accountPaymentKalemSAPTableSaved.posnr,
                                            saat = DateTime.Now.ToString("HH:mm:ss"),
                                            tarih = DateTime.Now.ToString("yyyyMMdd"),
                                            bittimi = "X"
                                        };


                                        sapFunction.Invoke<string>(input: inputparams);
                                        sapConn.Disconnect();

                                    }




                                    EmailMessage emailMessage = new EmailMessage();
                                    BLLActions.AccountPaymentSAPTable bllAccountPaymentSAPTable = new BLLActions.AccountPaymentSAPTable(_configuration, _env, _mapper,_server );
                                    Data.Models.AccountPaymentSAPTable? accountPaymentSAPTable = bllAccountPaymentSAPTable.GetByOENUM(accountPaymentKalemSAPTable.oenum);
                                    emailMessage.subject = accountPaymentSAPTable?.oenum + " Nolu ödeme onayı hk.";
                                    emailMessage.toAddress = "finans@askalecimento.com.tr";
                                    emailMessage.emailText = getPaymentMailStringOnaylandi(accountPaymentSAPTable);
                                    emailMessage.isSent = false;
                                    emailMessage.plannedDate = DateTime.Now;
                                    emailMessage.mailTuru = 1;
                                    BLLActions.EmailMessages bllEmailMessages = new BLLActions.EmailMessages(_configuration, _env);
                                    await bllEmailMessages.Add(emailMessage);

                                }
                            }
                        }
                        catch (Exception e)
                        {

                            Console.WriteLine(id + " id'li EFT onaylanamadı. Hata: "
                                    + e.Message);
                        }

                    }

                    return false;

                }

            }

            public string? getPaymentMailStringOnaylandi(Data.Models.AccountPaymentSAPTable? accountPaymentSAPTable)
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

                List<Data.Models.AccountPaymentKalemSAPTable> listAccountPaymentKalemSAPTable = GetByOENUM(accountPaymentSAPTable?.oenum ?? "");
                foreach (Data.Models.AccountPaymentKalemSAPTable item in listAccountPaymentKalemSAPTable)
                {
                    mailString += "<tr class='datarow'>" +

                            "<td style ='text-align: left;border: 1px solid black;'>" + item.Id + "</td>"
                            + "<td style ='text-align: left;border: 1px solid black;'>" + item.oenum + "</td>"
                            + "<td style ='text-align: left;border: 1px solid black;'>" + item.posnr + "</td>"
                            + "<td style ='text-align: left;border: 1px solid black;'>" + item.lifnr + "</td>"
                            + "<td style ='text-align: left;border: 1px solid black;'>" + item.name1 + "</td>"
                            + "<td style ='text-align: right;border: 1px solid black;'>" + item.wrbtr + "</td>"
                            + "<td style ='text-align: center;border: 1px solid black;' >" + accountPaymentSAPTable?.usnam
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

            public string? getPaymentMailString(Data.Models.AccountPaymentSAPTable? accountPaymentSAPTable, AdminUser? user)
            {


                var okLink = CommonConstants.OkNoLinks.OK_LINK + "/it-portal/accountpayment/replyfromout?answer=1&guid="
                        + accountPaymentSAPTable?.oenum + "&userid=" + user?.Id.ToString();
                var noLink = CommonConstants.OkNoLinks.OK_LINK + "/it-portal/accountpayment/replyfromout?answer=0&guid="
                        + accountPaymentSAPTable?.oenum + "&userid=" + user?.Id.ToString();

                string mailString = "<link rel='stylesheet' href='https://cdn.jsdelivr.net/npm/bootstrap@4.0.0/dist/css/bootstrap.min.css' integrity='sha384-Gn5384xqQ1aoWXA+058RXPxPg6fy4IWvTNh0E263XmFcJlSAwiGgFAW/dAiS6JXm' crossorigin='anonymous'>"
                        + "<h3>Sayın Yetkili</h3><br/><br/>" + "<div>Aşağıdaki ödemeler onayınızı beklemektedir.</div>"
                        + "<table class='table table-striped table-bordered table-hover' style='margin-bottom:0px !important;' id='tableMain' >"
                        + "<thead>" + "<tr>"
                        + "<th style ='text-align: left;border: 1px solid black;'><a class='btn btn-primary' href=\"" + okLink
                        + "\">TÜMÜNÜ ONAYLA</a></hr>"
                        + "<th style ='text-align: left;border: 1px solid black;'><a class='btn btn-danger' href=\"" + noLink
                        + "\">TÜMÜNÜ REDDET</a></th>" + "<th style ='text-align: left;border: 1px solid black;'>ID</th>" +

                        "<th style ='text-align: left;border: 1px solid black;'>Sap Belge No</th>"
                        + "<th style ='text-align: left;border: 1px solid black;'>Kalem</th>" +

                        "<th style ='text-align: left;border: 1px solid black;'>Satıcı No</th>"
                        + "<th style ='text-align: left;border: 1px solid black;'>Satıcı Adı</th>"
                        + "<th style ='text-align: right;border: 1px solid black;'>Tutar</th>"
                        + "<th style ='text-align: center;border: 1px solid black;'>Oluşturan Kişi SAP</th>" +

                        "</tr>" + "</thead>" + "<tbody>";

                List<Data.Models.AccountPaymentKalemSAPTable> listAccountPaymentKalemSAPTable = GetByOENUM(accountPaymentSAPTable?.oenum ?? "");
                foreach (Data.Models.AccountPaymentKalemSAPTable item in listAccountPaymentKalemSAPTable)
                {
                    mailString += "<tr class='datarow'>" +

                            "<td style ='text-align: left;border: 1px solid black;'>" + item.Id + "</td>"
                            + "<td style ='text-align: left;border: 1px solid black;'>" + item.oenum + "</td>"
                            + "<td style ='text-align: left;border: 1px solid black;'>" + item.posnr + "</td>"
                            + "<td style ='text-align: left;border: 1px solid black;'>" + item.lifnr + "</td>"
                            + "<td style ='text-align: left;border: 1px solid black;'>" + item.name1 + "</td>"
                            + "<td style ='text-align: right;border: 1px solid black;'>" + item.wrbtr + "</td>"
                            + "<td style ='text-align: center;border: 1px solid black;' >" + accountPaymentSAPTable?.usnam
                            + "</td>";
                }

                mailString += "</tbody>" + "</table>";

                mailString += "<br /><br /> ----------------------------------------- <br /><br /> " + " Saygılarımızla.";
                return mailString;

            }

            public List<AccountPaymentKalemActiveDto> listFilterByCompanyIdAndVendorCode(FilterParam<AccountPaymentKalemActiveDtoParameter> filterParam)
            {
                string name1 = filterParam?.liste?.name1 ??"";
                var query = from a in dal.dB.AccountPaymentKalemSAPTable
                            join b in dal.dB.AccountPaymentSAPTable
                                on a.oenum equals b.oenum
                            join c in dal.dB.ActivePaymentDetail
                                on a.Id equals c.activePaymentId
                            join d in dal.dB.AdminUser
                                on c.userId equals d.Id
                            where a.currentStateId == 1
                                  && a.enabled == true
                                  && b.enabled == true
                                  && c.approved == null
                                  && (
                                        a.name1.Contains(name1) ||
                                        (name1 == "" && a.name1 == null)
                                     )
                            orderby a.oenum descending
                            select new AccountPaymentKalemActiveDto
                            {
                                id = a.Id,
                                oenum = a.oenum,
                                posnr = a.posnr,
                                lifnr = a.lifnr,
                                name1 = a.name1,
                                wrbtr = a.wrbtr,
                                usnam = b.usnam,
                                currentStateId = a.currentStateId,
                                onayKimde = d.name,
                                znot = b.znot
                            };

                return query.ToList();


            }
        }


    }

}
