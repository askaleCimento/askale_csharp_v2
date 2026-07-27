using AskalePortal.Data.SAP.InputParams;
using AskalePortal.Data.SAP.OutputParams;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using SapNwRfc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.BLL
{
    public partial class BLLActions
    {
        public class ChangeCreditOrVadeLimitSap
        {
            private readonly IConfiguration _configuration;
            private readonly IWebHostEnvironment _env;
            private readonly IMapper _mapper;
            public ChangeCreditOrVadeLimitSap(IConfiguration configuration, IWebHostEnvironment env, IMapper mapper)
            {
                _configuration = configuration;
                _env = env;
                _mapper = mapper;
            }
            public string changeCreditLimitSap(string kunnr, Double dmbtr)
            {
                string returnString = "";
                BLLActions.SAPConnectionData bllSapConnection = new BLLActions.SAPConnectionData(_configuration, _env);
                SapConnection? sapConn = bllSapConnection.sapConnection(_configuration, _env);

                try
                {
                    if (sapConn != null)
                    {
                        sapConn.Connect();
                        ISapFunction sapFunction = sapConn.CreateFunction("ZWEBI011");


                        ChangeCrediLimitOutput? output = sapFunction.Invoke<ChangeCrediLimitOutput>(
                            input: new ChangeCreditLimitSapParams
                            {
                                IV_ADD_LIMIT = dmbtr.ToString(),
                                IV_KUNNR = kunnr,
                            }
                                 );
                        sapConn.Disconnect();
                        string evMessage = output?.EV_MESSAGE ?? "";
                        string evReturn = output?.EV_RETURN ?? "";
                        returnString = evMessage + evReturn;

                    }




                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                return returnString;
            }

            public string changeVadeSap(string BUKRS, string BELNR, string GJAHR, int DAY)
            {


                string returnString = "";
                BLLActions.SAPConnectionData bllSapConnection = new BLLActions.SAPConnectionData(_configuration, _env);
                SapConnection? sapConn = bllSapConnection.sapConnection(_configuration, _env);

                try
                {
                    if (sapConn != null)
                    {
                        sapConn.Connect();
                        ISapFunction sapFunction = sapConn.CreateFunction("ZWEBI014");


                        ChangeCrediLimitOutput? output = sapFunction.Invoke<ChangeCrediLimitOutput>(
                            input: new ChangeVadeSapParams
                            {
                                IV_BELNR = BELNR,
                                IV_BUKRS = BUKRS,
                                IV_GJAHR = GJAHR,
                                IV_DAY = DAY.ToString()
                            });
                        sapConn.Disconnect();
                        string evMessage = output?.EV_MESSAGE ?? "";
                        string evReturn = output?.EV_RETURN ?? "";
                        returnString = evMessage + evReturn;

                    }


                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                return returnString;


            }
        }
    }
}
