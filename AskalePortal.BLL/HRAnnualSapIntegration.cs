using AskalePortal.Data.SAP.InputParams;
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
        public class HRAnnualSapIntegration : BaseBLL<AskalePortal.Data.Models.HRAnnualSapIntegration>
        {
            private readonly IWebHostEnvironment _env;
            private readonly IConfiguration _configuration;
            public HRAnnualSapIntegration(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
            {
                _configuration = configuration;
                _env = env;
            }
            public List<AskalePortal.Data.Models.HRAnnualSapIntegration> getNotGoSap()
            {
                return dal.Get(u => u.enabled == true && u.approval == false).ToList();
            }
            public string? sentToSap(int id)
            {
                BLLActions.SAPConnectionData bllSapConnection = new BLLActions.SAPConnectionData(_configuration, _env);
                SapConnection? sapConn = bllSapConnection.sapConnection(_configuration, _env);
                string result = string.Empty;
                BLLActions.HRAnnualSapIntegration bllHRAnnualSapIntegration = new BLLActions.HRAnnualSapIntegration(_configuration, _env);
                AskalePortal.Data.Models.HRAnnualSapIntegration hRAnnualSapIntegration = bllHRAnnualSapIntegration.GetByID(id);
                if (sapConn != null)
                {
                    sapConn.Connect();
                    ISapFunction sapFunction = sapConn.CreateFunction("ZWEBI048");
                    result = sapFunction.Invoke<string>(input: new HRAnnualSapIntegrationInputParams
                    {
                        DUZELTME= hRAnnualSapIntegration.duzeltme.Replace('.', ',').ToString().Split(',')[1] == "5" ? hRAnnualSapIntegration.duzeltme.Replace('.', ',').ToString() : "",
                        IZINBASLANGICI= hRAnnualSapIntegration.izinbaslangici!.Value.Date.ToShortDateString(),
                        IZINBITISI= hRAnnualSapIntegration.izinbitisi!.Value.Date.ToShortDateString(),
                        IZINTURU= hRAnnualSapIntegration.izinturu,
                        PERNR= hRAnnualSapIntegration.perno,
                        SAAT= hRAnnualSapIntegration.saat.Replace('.', ',').ToString()

                    }


                    );
                    sapConn.Disconnect();
                    return result;
                }
                else
                {
                    return null;
                }




            }
        }
    }
}
