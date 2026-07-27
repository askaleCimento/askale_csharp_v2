
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
        public class SAPUSERS : BaseBLL<AskalePortal.Data.Models.Employee>
        {
            private readonly IConfiguration _configuration;
            private readonly IWebHostEnvironment _env;
            public SAPUSERS(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
            {
                _configuration = configuration;
                _env = env;
            }

            
            public string? ChangeUserPassword(string userName, string password, string islock)
            {

                BLLActions.SAPConnectionData bllSapConnection = new BLLActions.SAPConnectionData(_configuration, _env);
                SapConnection? sapConn = bllSapConnection.sapConnection(_configuration, _env);
                string result = string.Empty;
                if (sapConn != null)
                {
                    sapConn.Connect();
                    ISapFunction sapFunction = sapConn.CreateFunction("ZWEBI017");
                    result = sapFunction.Invoke<string>(input: new ChangeUserPasswordInputParams
                    {
                        IV_LOCK = islock,
                        IV_PASSWORD = password,
                        IV_USERNAME = userName,
                    }


                    );
                    sapConn.Disconnect();
                    return result;
                }
                return null;


            }

        }

    }
}
