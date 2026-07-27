using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AskalePortal.Data.SAP.InputParams;
using AskalePortal.Data.SAP.OutputParams;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

using SapNwRfc;

namespace AskalePortal.BLL
{
    public partial class BLLActions
    {
        public class RaporTipleri : BaseBLL<AskalePortal.Data.Models.RaporTipleri>
        {
            private readonly IConfiguration _configuration;
            private readonly IWebHostEnvironment _env;
            public RaporTipleri(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
            {
                _configuration = configuration;
                _env = env;
            }
            #region GetAll

            public override List<AskalePortal.Data.Models.RaporTipleri> GetAll()
            {
                var q = dal.Get(k => k.enabled == true).OrderBy(k => k.raporTipi);
                return q.ToList();
            }

            public List<AskalePortal.Data.Models.RaporTipleri> GetAllFromSAP()
            {
                List<AskalePortal.Data.Models.RaporTipleri> lstRaporTipleri = new List<AskalePortal.Data.Models.RaporTipleri>();
                try
                {
                    BLLActions.SAPConnectionData bllSapConnection = new BLLActions.SAPConnectionData(_configuration, _env);
                    SapConnection? sapConnection = bllSapConnection.sapConnection(_configuration, _env);

                    if (sapConnection == null)
                    {

                    }
                    else
                    {
                        ISapFunction sapFunction = sapConnection.CreateFunction("ZWEBI003");

                        RaporTipleriSapOutput list = sapFunction.Invoke<RaporTipleriSapOutput>();

                        for (int i=0;i<list.raporTipleriList?.Length;i++)
                        {
                            Data.Models.RaporTipleri c = new Data.Models.RaporTipleri();
                            c.Id = i + 1;
                            c.raporAdi = list.raporTipleriList?[i].RAPORADI;
                            c.raporTipi = list.raporTipleriList?[i].RAPORTIPI;
                            c.enabled = true;

                            lstRaporTipleri.Add(c);
                        }

                    }
                    
                }
                catch (Exception ex)
                {
                    LogError(ex);
                }

                return lstRaporTipleri;
            }

            #endregion GetAll
        }
    }
}
