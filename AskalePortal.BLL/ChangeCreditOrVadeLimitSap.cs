using System;
using System.Globalization;
using AskalePortal.Data.SAP.InputParams;
using AskalePortal.Data.SAP.OutputParams;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using SapNwRfc;

namespace AskalePortal.BLL
{
    public partial class BLLActions
    {
        public class ChangeCreditOrVadeLimitSap
        {
            private readonly IConfiguration _configuration;
            private readonly IWebHostEnvironment _env;
            public ChangeCreditOrVadeLimitSap(IConfiguration configuration, IWebHostEnvironment env, IMapper mapper)
            {
                _configuration = configuration;
                _env = env;
            }
            public string changeCreditLimitSap(string kunnr, double dmbtr)
            {
                return Invoke("ZWEBI011", new ChangeCreditLimitSapParams
                {
                    IV_KUNNR = kunnr,
                    IV_ADD_LIMIT = dmbtr.ToString(CultureInfo.InvariantCulture)
                });
            }
            public string changeVadeSap(string BUKRS, string BELNR, string GJAHR, int DAY)
            {
                return Invoke("ZWEBI014", new ChangeVadeSapParams
                {
                    IV_BUKRS = BUKRS,
                    IV_BELNR = BELNR,
                    IV_GJAHR = GJAHR,
                    IV_DAY = DAY.ToString(CultureInfo.InvariantCulture)
                });
            }
            private string Invoke(string functionName, object input)
            {
                var connectionFactory = new SAPConnectionData(_configuration, _env);
                using SapConnection? connection = connectionFactory.sapConnection(_configuration, _env);
                if (connection == null) return "";
                try
                {
                    connection.Connect();
                    var output = connection.CreateFunction(functionName)
                        .Invoke<ChangeCrediLimitOutput>(input: input);
                    return (output?.EV_MESSAGE ?? "") + (output?.EV_RETURN ?? "");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return "";
                }
            }
        }
    }
}