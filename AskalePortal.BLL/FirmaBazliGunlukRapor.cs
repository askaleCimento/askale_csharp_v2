using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AskalePortal.Data.Models;
using System.Web;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using SapNwRfc;
using AskalePortal.Data.SAP.InputParams;
using AskalePortal.Data.SAP.OutputParams;
using Microsoft.AspNetCore.Http;
using AutoMapper;

namespace AskalePortal.BLL
{
    public partial class BLLActions
    {
        public class FirmaBazliGunlukRapor : BaseBLL<Data.SAP.Models.FirmaBazliGunlukRapor>
        {
            private readonly IConfiguration _configuration;
            private readonly IWebHostEnvironment _env;
            private readonly IHttpContextAccessor _httpContextAccessor;
            private readonly IMapper _mapper;
            public FirmaBazliGunlukRapor(IConfiguration configuration, IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor,IMapper mapper) : base(configuration, env)
            {
                _configuration = configuration;
                _env = env;
                _httpContextAccessor = httpContextAccessor; 
                _mapper = mapper;   
            }
            #region GetAllWithParameters

            public List<Data.SAP.Models.FirmaBazliGunlukRapor> GetAllFromSAP(DateTime? TARIH)
            {
                List<Data.SAP.Models.FirmaBazliGunlukRapor> lstData = new List<Data.SAP.Models.FirmaBazliGunlukRapor>();
                try
                {
                    BLLActions.SAPConnectionData bllSapConnection = new BLLActions.SAPConnectionData(_configuration, _env);
                    SapConnection? sapConnection = bllSapConnection.sapConnection(_configuration, _env);
                
                    if (sapConnection != null)
                    {
                      ISapFunction sapFunction=  sapConnection.CreateFunction("ZWEBI004");

                        FirmaBazliGunlikRaporOutput listFirmaBazliGunlikRapor = sapFunction.Invoke<FirmaBazliGunlikRaporOutput>(input: new IVTarih
                        {
                            IV_TARIH= TARIH?.ToString("dd.MM.yyyy")
                        });

                        string? userId = _httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(u => u.Type == "userId")?.Value;

                        if (userId != null)
                        {
                            AdminUsers bllAdminUser = new AdminUsers(_configuration, _env, _mapper);

                            AdminUser? adminUser = bllAdminUser.GetByID(Convert.ToInt32(userId));
                            if (adminUser != null)
                            {
                                string companies = adminUser.role.companies;

                                foreach (var item in listFirmaBazliGunlikRapor.liste ?? [])
                                {
                                    if (companies.Contains(string.Format("[{0}]", item.SATORG)))
                                        lstData.Add(item);
                                }

                            }
                        }

                        }

                    
                }
                catch (Exception ex)
                {
                    LogError(ex);
                }

                    return lstData;
              
            }

            #endregion GetAll
        }
    }
}
