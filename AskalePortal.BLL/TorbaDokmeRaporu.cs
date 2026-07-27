using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AskalePortal.Data.Models;
using System.Web;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using AskalePortal.Data.SAP.InputParams;
using AskalePortal.Data.SAP.OutputParams;
using SapNwRfc;
using System.Net.Sockets;
using AskalePortal.Data.SAP.Models;

namespace AskalePortal.BLL
{
    public partial class BLLActions
    {

        public class TorbaDokmeRaporu : BaseBLL<TorbaDokmeReport>
        {
            public readonly IConfiguration _configuration; 
            public readonly IWebHostEnvironment _env;
            public TorbaDokmeRaporu(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
            {
                _env = env;
                _configuration = configuration;
            }
            #region GetAllWithParameters

            public List<TorbaDokmeReport> GetAllFromSAP(DateTime? TARIH)
            {
                
                try
                {
                    BLLActions.SAPConnectionData bllSapConnection = new BLLActions.SAPConnectionData(_configuration, _env);
                    SapConnection? sapConn = bllSapConnection.sapConnection(_configuration, _env);
                    if (sapConn != null)
                    {
                        sapConn.Connect();
                        ISapFunction sapFunction = sapConn.CreateFunction("ZWEBI008");

                        SapTorbaDokmeReportOutput? sapTorbaDokmeReportOutput = sapFunction.Invoke<SapTorbaDokmeReportOutput>(input: new IVTarih
                        {
                          IV_TARIH=TARIH?.ToString()
                        }

                                  );
                        sapConn.Disconnect();
                        return sapTorbaDokmeReportOutput.torbaDokmeReports?.ToList() ?? [];

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

            #endregion GetAll
        }
    }
}
