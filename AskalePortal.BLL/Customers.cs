using AskalePortal.Constants;
using AskalePortal.Data.ResponseModels;
using AskalePortal.Data.SAP.InputParams;
using AskalePortal.Data.SAP.Models;
using AskalePortal.Data.SAP.OutputParams;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SapNwRfc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using static AskalePortal.BLL.BLLActions;

namespace AskalePortal.BLL
{
    public partial class BLLActions
    {
        public class Customers : BaseBLL<AskalePortal.Data.Models.Customer>
        {
            public readonly IConfiguration _configuration;
            public readonly IWebHostEnvironment _env;
            public Customers(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
            {
                _configuration = configuration;
                _env = env;
            }
            #region GetAll

            public override List<AskalePortal.Data.Models.Customer> GetAll()
            {
                var q = dal.Get(k => k.enabled == true).OrderBy(k => k.name1);

                return q.ToList();
            }

            #endregion GetAll

            #region GetAllWithPage

            public List<AskalePortal.Data.Models.Customer> GetAllWithPage(string searchQuery, int activePage, int recordsPerPage = 10)
            {
                var q = dal.Get(k => (k.name1.Contains(searchQuery) || string.IsNullOrEmpty(searchQuery)) &&
                                     k.enabled == true)
                                     .OrderBy(k => k.name1)
                                     .Skip(activePage * recordsPerPage).Take(recordsPerPage).ToList();
                return q;
            }

            #endregion GetAllWithPage

            public List<Data.SAP.Models.Customer>? GetAllFromSAP(string searchQuery, int activePage, int recordsPerPage = 10)
            {
                List<Data.SAP.Models.Customer>? lstCustomers = null;
                //return lstCustomers.ToPagedList(activePage, recordsPerPage);

                try
                {
                    BLLActions.SAPConnectionData bllSapConnection = new BLLActions.SAPConnectionData(_configuration, _env);
                    SapConnection? sapConn = bllSapConnection.sapConnection(_configuration, _env);
                    if (sapConn == null)
                    {

                    }
                    else
                    {
                        sapConn.Connect();
                        ISapFunction function = sapConn.CreateFunction("ZWEBI010");

                        CustomerFromSap listCustomer = function.Invoke<CustomerFromSap>();

                        lstCustomers = listCustomer.customer?.ToList();



                    }
                }
                catch (Exception ex)
                {
                    LogError(ex);
                }


                List<Data.SAP.Models.Customer>? returnList;


                returnList = lstCustomers?.Skip(activePage * recordsPerPage).Take(recordsPerPage).ToList();

                return returnList;
            }


            public List<CustomerListDto>? GetAllFromSAP()
            {
                List<CustomerListDto> liste = new List<CustomerListDto>();

                BLLActions.SAPConnectionData bllSapConnection = new BLLActions.SAPConnectionData(_configuration, _env);
                SapConnection? sapConn = bllSapConnection.sapConnection(_configuration, _env);


                if (sapConn != null)
                {
                    sapConn.Connect();
                    ISapFunction sapFunction = sapConn.CreateFunction("ZWEBI010");


                    CustomerOutput? output = sapFunction.Invoke<CustomerOutput>(
                        input: null
                             );
                    sapConn.Disconnect();


                    foreach(ListCustomer i in output.customer ?? [])
                    {
                        CustomerListDto customerListDto = new CustomerListDto();
                        customerListDto.adrnr = i.ADRNR;
                        customerListDto.name1 = i.NAME1;
                        customerListDto.duefl = i.DUEFL;
                        customerListDto.erdat = i.ERDAT.ToString();
                        customerListDto.ernam = i.ERNAM;
                        customerListDto.ktokd = i.KTOKD;
                        customerListDto.kunnr = i.KUNNR;
                        customerListDto.land1 = i.LAND1;
                        customerListDto.lifnr = i.LIFNR;
                        customerListDto.title = i.NAME1;
                        customerListDto.name2 = i.NAME2;
                        customerListDto.ort01 = i.ORT01;
                        customerListDto.pstlz = i.PSTLZ;
                        customerListDto.regio = i.REGIO;
                        customerListDto.sortl = i.SORTL;
                        customerListDto.sperr = i.SPERR;
                        customerListDto.spras = i.SPRAS;
                        customerListDto.stcd1 = i.STCD1;
                        customerListDto.stcd2 = i.STCD2;
                        customerListDto.stras = i.STRAS;
                        customerListDto.telf1 = i.TELF1;
                        customerListDto.telf2 = i.TELF2;
                        customerListDto.telfx = i.TELFX;
                        customerListDto.creditlimit = 0.0;

                        liste.Add(customerListDto);

                    }


                    return liste;


                }
                else
                {
                    return null;
                }

            }

                
                

            public Data.SAP.Models.Customer? GetByKUNNR(string KUNNR)
            {
                List<Data.SAP.Models.Customer>? lstCustomers = null;
                //return lstCustomers.ToPagedList(activePage, recordsPerPage);

                try
                {
                    BLLActions.SAPConnectionData bllSapConnection = new BLLActions.SAPConnectionData(_configuration, _env);
                    SapConnection? sapConn = bllSapConnection.sapConnection(_configuration, _env);
                    if (sapConn == null)
                    {

                    }
                    else
                    {
                        sapConn.Connect();
                        ISapFunction function = sapConn.CreateFunction("ZWEBI010");

                        CustomerFromSap listCustomer = function.Invoke<CustomerFromSap>(input: new KunnrParams
                        {
                            kunnr = KUNNR
                        });

                        lstCustomers = listCustomer.customer?.ToList();



                    }
                }
                catch (Exception ex)
                {
                    LogError(ex);
                }

                return lstCustomers?[0];
            }

            //public string GetSAPMusteriCevap(string customer, string miktar, string username, string yeni)
            //{
            //    string cevap = "";
            //    try
            //    {
            //        username = username.ToUpper().Replace('İ', 'I').Replace('Ç', 'C').Replace('Ğ', 'G').Replace('Ö', 'O').Replace('Ü', 'U').Replace('Ş', 'S');
            //        SAPConnection con = new SAPConnection(new BLLActions.Configs().GetByID(1));
            //        IRfcFunction function = con.Repostory.CreateFunction("ZWEBI029");
            //        function.SetValue("lv_kunnr", customer);
            //        function.SetValue("lv_dmbtr", miktar);
            //        function.SetValue("lv_kullaniciadi", username);
            //        function.SetValue("lv_yeni_musteri", yeni);
            //        function.Invoke(con.Destination);
            //        cevap = function.Getstring ? ("EV_MESSAGE");
            //    }
            //    catch (Exception ex)
            //    {
            //        LogError(ex);
            //    }
            //    return cevap;
            //}

            public Data.SAP.Models.CustomerCredit? GetCustomerCreditByKUNNR(string KUNNR)
            {
                //return new Data.SAP.Models.CustomerCredit { KUNNR = "0000200020",
                //    NAME1 = "ŞAH-İN İNŞAAT NAKLİYAT TAAHHÜT",
                //    TOP_BORC = 123
                //};

                List<Data.SAP.Models.CustomerCredit>? lstCustomerCredits = new List<Data.SAP.Models.CustomerCredit>();

                try
                {

                    BLLActions.SAPConnectionData bllSapConnection = new BLLActions.SAPConnectionData(_configuration, _env);
                    SapConnection? sapConn = bllSapConnection.sapConnection(_configuration, _env);
                    if (sapConn == null)
                    {

                    }
                    else
                    {
                        sapConn.Connect();
                        ISapFunction function = sapConn.CreateFunction("ZWEBI012");

                        CustomerCreditSap? listCustomer = function.Invoke<CustomerCreditSap>(
                            input: new KunnrParams { kunnr = KUNNR }
                            );

                        lstCustomerCredits = listCustomer?.listcCustomerCredit?.ToList();



                    }



                }
                catch (Exception ex)
                {
                    LogError(ex);
                }

                return lstCustomerCredits?.FirstOrDefault();
            }



            public string? ChangeCustomerCreditFromSAP(string KUNNR, decimal ADD_LIMIT)
            {
                try
                {

                    BLLActions.SAPConnectionData bllSapConnection = new BLLActions.SAPConnectionData(_configuration, _env);
                    SapConnection? sapConn = bllSapConnection.sapConnection(_configuration, _env);
                    if (sapConn == null)
                    {
                        return "sapError";
                    }
                    else
                    {
                        sapConn.Connect();
                        ISapFunction function = sapConn.CreateFunction("ZWEBI011");

                        CustomerCreditLimitIncreaseReturn? returnLimit = function.Invoke<CustomerCreditLimitIncreaseReturn>(
                            input: new CustomerCreditIncreaseLimitInput { kunnr = KUNNR, limit = ADD_LIMIT }
                            );


                        var returnText = returnLimit.evreturn;
                        var errorMessage = returnLimit.evmessage;
                        return returnText + errorMessage;


                    }




                }
                catch (Exception ex)
                {
                    LogError(ex);
                    return "sapError";
                }
            }

            public string ChangeCustomerDocumentDateFromSAP(string BUKRS, string BELNR, string GJAHR, int DAY)
            {
                try
                {
                    BLLActions.SAPConnectionData bllSapConnection = new BLLActions.SAPConnectionData(_configuration, _env);
                    SapConnection? sapConn = bllSapConnection.sapConnection(_configuration, _env);
                    if (sapConn == null)
                    {
                        return "sapError";
                    }
                    else
                    {
                        sapConn.Connect();
                        ISapFunction function = sapConn.CreateFunction("ZWEBI014");

                        CustomerCreditLimitIncreaseReturn? returnLimit = function.Invoke<CustomerCreditLimitIncreaseReturn>(
                            input: new ChangeCustomerDocumentDateInput { bukrs = BUKRS, belnr = BELNR, gjahr = GJAHR, ivday = DAY }
                            );


                        var returnText = returnLimit.evreturn;
                        var errorMessage = returnLimit.evmessage;
                        return returnText + errorMessage;


                    }

                }
                catch (Exception ex)
                {
                    LogError(ex);
                    return "sapError";
                }
            }

            public List<Data.SAP.OutputParams.CustomerDocumentDto> GetDocumentList(string KUNNR)
            {

                BLLActions.SAPConnectionData bllSapConnection = new BLLActions.SAPConnectionData(_configuration, _env);
                SapConnection? sapConn = bllSapConnection.sapConnection(_configuration, _env);
                List<Data.SAP.Models.CustomerDocument> lstDocument = new List<Data.SAP.Models.CustomerDocument>();
                if (sapConn != null)
                {
                    sapConn.Connect();
                    ISapFunction sapFunction = sapConn.CreateFunction("ZWEBI013");

                    Data.SAP.OutputParams.CustomerDocumentOutput customerDocumentOutput = sapFunction.Invoke<Data.SAP.OutputParams.CustomerDocumentOutput>(input: new Data.SAP.InputParams.CustomerNoParams { kunnr = KUNNR }

                              );
                    sapConn.Disconnect();
                    List<Data.SAP.OutputParams.CustomerDocumentDto> liste = customerDocumentOutput.OUTPUT?.ToList() ?? [];
                    return liste;
                }
                else
                {
                    return [];
                }



            }
            public List<Data.SAP.OutputParams.CustomerDocumentDto> GetDocumentList(string KUNNR, string BUKRS)
            {

                BLLActions.SAPConnectionData bllSapConnection = new BLLActions.SAPConnectionData(_configuration, _env);
                SapConnection? sapConn = bllSapConnection.sapConnection(_configuration, _env);
                List<Data.SAP.OutputParams.CustomerDocumentDto> lstDocument = new List<Data.SAP.OutputParams.CustomerDocumentDto>();
                try
                {
                    if (sapConn != null)
                    {
                        sapConn.Connect();
                        ISapFunction sapFunction = sapConn.CreateFunction("ZWEBI013");

                        Data.SAP.OutputParams.CustomerDocumentOutput customerDocumentOutput = sapFunction.Invoke<Data.SAP.OutputParams.CustomerDocumentOutput>(input: new Data.SAP.InputParams.CustomerNoParams { kunnr = KUNNR, bukrs = BUKRS }

                                  );
                        sapConn.Disconnect();
                        lstDocument = customerDocumentOutput.OUTPUT?.ToList() ?? [];
                        return lstDocument;
                    }
                    else
                    {
                        return [];
                    }


                }
                catch (Exception ex)
                {
                    LogError(ex);
                }

                return lstDocument;
            }

            public string FiyatOnayi(int WI_ID, int onay)
            {
                BLLActions.SAPConnectionData bllSapConnection = new BLLActions.SAPConnectionData(_configuration, _env);
                SapConnection? sapConn = bllSapConnection.sapConnection(_configuration, _env);

                try
                {
                    if (sapConn != null)
                    {
                        sapConn.Connect();
                        ISapFunction sapFunction = sapConn.CreateFunction("ZWEBI035");

                        EVMESSAGE message = sapFunction.Invoke<EVMESSAGE>(input: new SapFiyatOnayInput { iv_wi_id = WI_ID, onay = onay }

                                  );
                        sapConn.Disconnect();
                        return message.EV_MESSAGE ?? "";

                    }
                    else
                    {
                        return "sapError";
                    }


                }
                catch (Exception ex)
                {
                    LogError(ex);
                    return "sapError";

                }



            }

            public List<FiyatOnayi> GetAllFromSAP(string sapUser)
            {
                BLLActions.SAPConnectionData bllSapConnection = new BLLActions.SAPConnectionData(_configuration, _env);
                SapConnection? sapConn = bllSapConnection.sapConnection(_configuration, _env);

                try
                {
                    if (sapConn != null)
                    {
                        sapConn.Connect();
                        ISapFunction sapFunction = sapConn.CreateFunction("ZWEBI034");

                        SapFiyatOnayiOutput fiyatOnayiOutput = sapFunction.Invoke<SapFiyatOnayiOutput>(input: new SapUsernameInput { username = sapUser }

                                  );
                        sapConn.Disconnect();
                        return fiyatOnayiOutput.fiyatOnayiList?.ToList() ?? [];

                    }
                    else
                    {
                        return [];
                    }


                }
                catch (Exception ex)
                {
                    LogError(ex);
                    return [];

                }


            }

            public string SetCustomerSanal(string KUNNR, string DMBTR, string yeniMusteriMi, string username)
            {
                BLLActions.SAPConnectionData bllSapConnection = new BLLActions.SAPConnectionData(_configuration, _env);
                SapConnection? sapConn = bllSapConnection.sapConnection(_configuration, _env);

                try
                {
                    if (sapConn != null)
                    {
                        sapConn.Connect();
                        ISapFunction sapFunction = sapConn.CreateFunction("ZWEBI019");

                        EVMESSAGE message = sapFunction.Invoke<EVMESSAGE>(input: new SapSanalLimitIncrease { kunnr = KUNNR, dmbtr = DMBTR, yeniMusteri = yeniMusteriMi, kullaniciAdi = username }

                                  );
                        sapConn.Disconnect();
                        return message.EV_MESSAGE ?? "";

                    }
                    else
                    {
                        return "sapError";
                    }


                }
                catch (Exception ex)
                {
                    LogError(ex);
                    return "sapError";

                }


            }

            public CustomerSikayetList[] getCustomerSikayet(string bukrs)
            {

                BLLActions.SAPConnectionData bllSapConnection = new BLLActions.SAPConnectionData(_configuration, _env);
                SapConnection? sapConn = bllSapConnection.sapConnection(_configuration, _env);

                try
                {
                    if (sapConn != null)
                    {
                        sapConn.Connect();
                        ISapFunction sapFunction = sapConn.CreateFunction("ZWEBI051");


                        CustomerSikayetOutput? fiyatOnayiOutput = sapFunction.Invoke<CustomerSikayetOutput>(input: new IVBUKRS { IV_BUKRS = bukrs }

                                 );
                        sapConn.Disconnect();
                        return fiyatOnayiOutput?.customerSikayetList ?? [];


                    }
                    else
                    {
                        return [];
                    }


                }
                catch (Exception ex)
                {
                    LogError(ex);
                    return [];

                }

            }

            public CustomerCredit? getCustomerCredit(string kunnr)
            {


                BLLActions.SAPConnectionData bllSapConnection = new BLLActions.SAPConnectionData(_configuration, _env);
                SapConnection? sapConn = bllSapConnection.sapConnection(_configuration, _env);

                try
                {
                    if (sapConn != null)
                    {
                        sapConn.Connect();
                        ISapFunction sapFunction = sapConn.CreateFunction("ZWEBI012");


                        CustomerCreditSap? customerCreditSap = sapFunction.Invoke<CustomerCreditSap>(
                            input: new KunnrParams { kunnr = kunnr }

                                 );
                        sapConn.Disconnect();
                        //buraya bak olmazsa dto ile yapmam gerekiyor
                        CustomerCredit? customerCredit = customerCreditSap.listcCustomerCredit.FirstOrDefault();
                        Console.WriteLine(customerCredit);
                        return customerCredit ;


                    }
                    else
                    {
                        return null;
                    }


                }
                catch (Exception ex)
                {
                    LogError(ex);
                    return null;

                }
            }

            public CustomerDocumentDto[]? getCustomerDocument(string kunnr)
            {


                BLLActions.SAPConnectionData bllSapConnection = new BLLActions.SAPConnectionData(_configuration, _env);
                SapConnection? sapConn = bllSapConnection.sapConnection(_configuration, _env);

                try
                {
                    if (sapConn != null)
                    {
                        sapConn.Connect();
                        ISapFunction sapFunction = sapConn.CreateFunction("ZWEBI013");

                        CustomerDocumentOutput? output = sapFunction.Invoke<CustomerDocumentOutput>(
                            input: new KunnrParams { kunnr = kunnr }

                                 );
                        sapConn.Disconnect();
                        return output.OUTPUT;


                    }
                    else
                    {
                        return null;
                    }


                }
                catch (Exception ex)
                {
                    LogError(ex);
                    return null;

                }
            }

            public ActionResult<List<FiyatOnayiList>?>? getMyFiyatList(string sapUser)
            {

                BLLActions.SAPConnectionData bllSapConnection = new BLLActions.SAPConnectionData(_configuration, _env);
                SapConnection? sapConn = bllSapConnection.sapConnection(_configuration, _env);

                try
                {
                    if (sapConn != null)
                    {
                        sapConn.Connect();
                        ISapFunction sapFunction = sapConn.CreateFunction("ZWEBI034");


                        FiyatOnayiDtoOutput? output = sapFunction.Invoke<FiyatOnayiDtoOutput>(
                            input: new SUNAME { S_UNAME = sapUser }

                                 );
                        sapConn.Disconnect();
                        return output.fiyatOnayiList;


                    }
                    else
                    {
                        return null;
                    }


                }
                catch (Exception ex)
                {
                    LogError(ex);
                    return null;

                }
            }

            public string? setFiyatOnayi(int wiid, int onay)
            {
                BLLActions.SAPConnectionData bllSapConnection = new BLLActions.SAPConnectionData(_configuration, _env);
                SapConnection? sapConn = bllSapConnection.sapConnection(_configuration, _env);

                try
                {
                    if (sapConn != null)
                    {
                        sapConn.Connect();
                        ISapFunction sapFunction = sapConn.CreateFunction("ZWEBI035");

                        EVMESSAGE message = sapFunction.Invoke<EVMESSAGE>(input: new SapFiyatOnayInput{ iv_wi_id = wiid, onay = onay }  );
                        sapConn.Disconnect();
                        return message.EV_MESSAGE ?? "";

                    }
                    else
                    {
                        return "sapError";
                    }


                }
                catch (Exception ex)
                {
                    LogError(ex);
                    return "sapError";

                }


            }
        }
    }
}