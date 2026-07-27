using AskalePortal.Data.SAP.InputParams;
using AskalePortal.Data.SAP.OutputParams;
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
        public class ChequeStatus : BaseBLL<AskalePortal.Data.SAP.OutputParams.ChequeStatus>
        {

            private readonly IConfiguration _configuration;
            private readonly IWebHostEnvironment _env;
            public ChequeStatus(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
            {
                _configuration = configuration;
                _env = env;
            }

            public Data.SAP.OutputParams.ChequeStatus? getCheques(string tarih)
            {
                BLLActions.SAPConnectionData bllSapConnection = new BLLActions.SAPConnectionData(_configuration, _env);
                SapConnection? sapConn = bllSapConnection.sapConnection(_configuration, _env);
                if (sapConn != null)
                {

                    sapConn.Connect();

                    string tarihString;

                    if (tarih == null || tarih.Equals(""))
                    {
                        tarihString = DateTime.Now.ToString("yyyyMMdd");
                    }
                    else
                    {
                        DateTime localDate = DateTime.Parse(tarih);
                        tarihString = localDate.ToString("yyyyMMdd");
                    }
                    ISapFunction sapFunction = sapConn.CreateFunction("ZWEBI047");
                    ChequeStatusImputParams inputparams = new ChequeStatusImputParams { tarihP = tarihString };

                    Data.SAP.OutputParams.ChequeStatus? setOrderDto = sapFunction.Invoke<Data.SAP.OutputParams.ChequeStatus>(input: inputparams);
                    sapConn.Disconnect();
                    return setOrderDto;

                }
                return null;
            }
        }
    }
}
