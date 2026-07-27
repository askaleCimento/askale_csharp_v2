using AskalePortal.Data.Models;
using AskalePortal.Data.SAP.InputParams;
using AskalePortal.Data.SAP.Models;
using AskalePortal.Data.SAP.OutputParams;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using SapNwRfc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.BLL
{
    public partial class BLLActions
    {
        public class ProfitValues : BaseBLL<AskalePortal.Data.SAP.Models.ProfitValues>
        {
            private readonly IConfiguration _configuration; 
            private readonly IWebHostEnvironment _env;
            public ProfitValues(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
            {
                _configuration = configuration;
                _env = env;
            }

            public Data.SAP.Models.ProfitReturnValues? GetAllByDate(string date)
            {
                List<Data.SAP.Models.ProfitValues> listProfitValues = new List<Data.SAP.Models.ProfitValues>();
                BLLActions.SAPConnectionData bllSapConnection = new BLLActions.SAPConnectionData(_configuration, _env);
                SapConnection? sapConn = bllSapConnection.sapConnection(_configuration, _env);
                SelectOption[] sirket = new SelectOption[1];
              

                SelectOption selectOptions = new SelectOption()
                {
                    SIGN = "I",
                    OPTION = "BT",
                    LOW = "AC10",
                    HIGH = "AC60"
                };
                sirket[0]=selectOptions;

              
                try
                {
                    if (sapConn != null)
                    {
                        sapConn.Connect();
                        ISapFunction sapFunction = sapConn.CreateFunction("ZWEBI040");

                        Data.SAP.Models.ProfitReturnValues? profitReturnValues = sapFunction.Invoke<Data.SAP.Models.ProfitReturnValues>(input: new ProfitValueSelectOptionInput { 
                       selectOptions =sirket,date=date}

                                  );
                        sapConn.Disconnect();
                        return profitReturnValues ?? null;

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

           

            public List<Data.SAP.Models.MasrafOgeleri> GetMasrafOgeleri(string date)
            {
                DateTime date1 = Convert.ToDateTime(date);
                List<Data.SAP.Models.ProfitValues> listProfitValues = new List<Data.SAP.Models.ProfitValues>();
                BLLActions.SAPConnectionData bllSapConnection = new BLLActions.SAPConnectionData(_configuration, _env);
                SapConnection? sapConn = bllSapConnection.sapConnection(_configuration, _env);
                SelectOption[] sirket = new SelectOption[1];
                SelectOption[] lvptyp = new SelectOption[1];
                SelectOption[] lvkkzst = new SelectOption[1];
                SelectOption[] lvprtyp = new SelectOption[1];
                SelectOption[] lvcateg = new SelectOption[1];
                SelectOption[] lvmatkl = new SelectOption[2];
                SelectOption[] lvbdatj = new SelectOption[1];
                SelectOption[] lvpoper;
                if (date1.Month == 1)
                {
                    lvpoper = new SelectOption[2];
                }
                else
                {
                    lvpoper = new SelectOption[1];
                }


                    SelectOption selectOptions = new SelectOption()
                {
                    SIGN = "I",
                    OPTION = "BT",
                    LOW = "AC10",
                    HIGH = "AC60"
                };
                sirket[0] = selectOptions;
                SelectOption ptyp = new SelectOption()
                {
                    SIGN = "I",
                    OPTION = "EQ",
                    LOW = "BF",
                    HIGH = ""
                };
                lvptyp[0] = ptyp;

                SelectOption kkzst = new SelectOption()
                {
                    SIGN = "I",
                    OPTION = "EQ",
                    LOW = "",
                    HIGH = ""
                };
                lvkkzst[0]=kkzst;

                SelectOption PRTYP = new SelectOption()
                {
                    SIGN = "I",
                    OPTION = "EQ",
                    LOW = "S",
                    HIGH = ""
                };
                lvprtyp[0]=PRTYP;

                SelectOption CATEG = new SelectOption()
                {
                    SIGN = "I",
                    OPTION = "EQ",
                    LOW = "ZU",
                    HIGH = ""
                };
                lvcateg[0] = CATEG;

                SelectOption MATKL1 = new SelectOption()
                {
                    SIGN = "I",
                    OPTION = "EQ",
                    LOW = "AC1006",
                    HIGH = ""
                };
                lvmatkl[0]=MATKL1;
                SelectOption MATKL2 = new SelectOption()
                {
                    SIGN = "I",
                    OPTION = "EQ",
                    LOW = "AC1009",
                    HIGH = ""
                };
                lvmatkl[1] = MATKL2;
               
                if (date1.Month == 1)
                {
                    SelectOption BDATJ = new SelectOption()
                    {
                        SIGN = "I",
                        OPTION = "BT",
                        LOW = (date1.Year - 1).ToString(),
                        HIGH = date1.Year.ToString()
                    };
                   lvbdatj[0] = BDATJ;
                }
                else
                {
                    SelectOption BDATJ = new SelectOption()
                    {
                        SIGN = "I",
                        OPTION = "EQ",
                        LOW = date1.Year .ToString(),
                        HIGH = ""
                    };
                    lvbdatj[0] = BDATJ;
                   
                }
                if (date1.Month == 1)
                {
                    SelectOption POPER1 = new SelectOption()
                    {
                        SIGN = "I",
                        OPTION = "EQ",
                        LOW = "001",
                        HIGH = ""
                    };
                    lvpoper[0]=POPER1;
                    SelectOption POPER2 = new SelectOption()
                    {
                        SIGN = "I",
                        OPTION = "EQ",
                        LOW = "012",
                        HIGH = ""
                    };
                    lvpoper[1] = POPER2;

                   
                }
                else if (date1.Month <= 9)
                {
                    SelectOption POPER1 = new SelectOption()
                    {
                        SIGN = "I",
                        OPTION = "BT",
                        LOW = "00" + (date1.Month - 1).ToString(),
                        HIGH = "00" + (date1.Month).ToString()
                    };
                    lvpoper[0] = POPER1;
                }
                else if (date1.Month == 10)
                {
                    SelectOption POPER1 = new SelectOption()
                    {
                        SIGN = "I",
                        OPTION = "BT",
                        LOW = "00" + (date1.Month - 1).ToString(),
                        HIGH = "0" + (date1.Month).ToString()
                    };
                    lvpoper[0] = POPER1;
                   
                }
                else if (date1.Month > 10)
                {
                    SelectOption POPER1 = new SelectOption()
                    {
                        SIGN = "I",
                        OPTION = "BT",
                        LOW = "0" + (date1.Month - 1).ToString(),
                        HIGH = "0" + (date1.Month).ToString()
                    };
                    lvpoper[0] = POPER1;
                   
                }
                try
                {
                    if (sapConn != null)
                    {
                        sapConn.Connect();
                        ISapFunction sapFunction = sapConn.CreateFunction("ZWEBI041");

                        SapMasrafOgeleriOutput? profitReturnValues = sapFunction.Invoke<SapMasrafOgeleriOutput>(input: new SapMasrafOgeleriInput
                        {
                          lvbwkey=sirket,
                          lvprtyp=lvptyp,
                          lvkkzst=lvkkzst,
                          lvptyp=lvptyp,
                          lvcateg=lvcateg,
                          lvmatkl=lvmatkl,
                          lvbdatj= lvbdatj,
                          lvpoper=lvpoper,
                          lvcurtp ="10",
                        }

                                  );
                        sapConn.Disconnect();
                        return profitReturnValues.masrafOgeleri?.ToList() ?? [];

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

            public List<Data.SAP.Models.CemClinkerRate> GetCemClinkerRate(string date)
            {
                BLLActions.SAPConnectionData bllSapConnection = new BLLActions.SAPConnectionData(_configuration, _env);
                SapConnection? sapConn = bllSapConnection.sapConnection(_configuration, _env);
                DateTime tarih = Convert.ToDateTime(date);
                DateTime tarih1 = new DateTime(tarih.AddMonths(-1).Year, tarih.AddMonths(-1).Month, 1);
                DateTime tarih2 = new DateTime(tarih.Year, tarih.Month, DateTime.DaysInMonth(tarih.Year, tarih.Month));
                try
                {
                    if (sapConn != null)
                    {
                        sapConn.Connect();
                        ISapFunction sapFunction = sapConn.CreateFunction("ZWEBI042");

                        SapCemClinkerRateOutput? sapCemClinkerRateOutput = sapFunction.Invoke<SapCemClinkerRateOutput>(input: new SapMalzemeTwoDateInput
                        {
                            tarih1 = tarih1,
                            tarih2 = tarih2
                        }

                                  );
                        sapConn.Disconnect();
                        return sapCemClinkerRateOutput?.cemClinkerRates?.ToList() ?? [];

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
        }
    }
}
