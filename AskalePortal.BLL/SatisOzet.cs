using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AskalePortal.Data.Models;
using System.Web;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using SapNwRfc;
using AskalePortal.Data.SAP.OutputParams;
using AskalePortal.Data.SAP.InputParams;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using AskalePortal.Constants;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Azure.Core;
using AutoMapper;

namespace AskalePortal.BLL
{
    public partial class BLLActions
    {
        public class SatisOzet : BaseBLL<AskalePortal.Data.Models.SatisOzet>
        {
            private readonly IConfiguration _configuration;
            private readonly IWebHostEnvironment _env;
            private readonly IMapper _mapper;
            public SatisOzet(IConfiguration configuration, IWebHostEnvironment env,IMapper mapper) : base(configuration, env)
            {
                _configuration = configuration;
                _env = env;
                _mapper = mapper;
            }
            #region GetAll

            public override List<AskalePortal.Data.Models.SatisOzet> GetAll()
            {
                var q = dal.Get(k => k.enabled == true).OrderBy(k => k.satorg);
                return q.ToList();
            }

            #endregion GetAll

            #region GetAllWithParameters

            public List<AskalePortal.Data.Models.SatisOzet> GetAllFromSAP(string RAPORTIPI, DateTime? TARIH)
            {
                List<AskalePortal.Data.Models.SatisOzet> lstSatisOzet = new List<AskalePortal.Data.Models.SatisOzet>();
                try
                {

                    BLLActions.SAPConnectionData bllSapConnection = new BLLActions.SAPConnectionData(_configuration, _env);
                    SapConnection? sapConnection = bllSapConnection.sapConnection(_configuration, _env);

                    if (sapConnection==null)
                    {

                    }
                    else
                    {
                        ISapFunction sapFunction = sapConnection.CreateFunction("ZWEBI001");
                        SatisOzetSapOutput satisOzetSapOutput = sapFunction.Invoke<SatisOzetSapOutput>(input: new IVTarih { 
                        IV_TARIH= TARIH==null?null:TARIH.Value.ToString("dd.MM.yyyy")
                        });
                        BLLActions.AdminUsers bllAdminUsers = new BLLActions.AdminUsers(_configuration, _env, _mapper);
                      
                        //string companies = user.Role.companies;
                        BLLActions.Companies bllcompany = new BLLActions.Companies(_configuration, _env, _mapper);
                        List<AskalePortal.Data.Models.Company> listcompany = bllcompany.GetAll();
                        for (int cuIndex = 0; cuIndex < satisOzetSapOutput.SatisOzet?.Length; cuIndex++)
                        {
                          

                            Data.Models.SatisOzet c = new Data.Models.SatisOzet();
                            c.satorg = satisOzetSapOutput.SatisOzet[cuIndex].SATORG;
                           
                                c.Id = cuIndex + 1;
                                c.cayGun = satisOzetSapOutput.SatisOzet[cuIndex].CAY_GUN;
                                c.cYil = satisOzetSapOutput.SatisOzet[cuIndex].CYIL;
                                c.cyilGun = satisOzetSapOutput.SatisOzet[cuIndex].CYIL_GUN;
                                c.oayGun = satisOzetSapOutput.SatisOzet[cuIndex].OAY_GUN;
                                c.oYil = satisOzetSapOutput.SatisOzet[cuIndex].OYIL;
                                c.oyilGun = satisOzetSapOutput.SatisOzet[cuIndex].OYIL_GUN;
                                c.raporTipi = satisOzetSapOutput.SatisOzet[cuIndex].RAPORTIPI;

                                c.tarih = DataReader.GetDateTime(satisOzetSapOutput.SatisOzet[cuIndex].TARIH!);
                                c.enabled = true;
                                c.satorgAdi = listcompany.Where(u => u.vkorg == c.satorg).FirstOrDefault()?.vtext;

                                    lstSatisOzet.Add(c);
                            

                        }

                    }

                    
                }
                catch (Exception ex)
                {
                    LogError(ex);
                }

                if (string.IsNullOrEmpty(RAPORTIPI))
                    return lstSatisOzet;
                else
                    return lstSatisOzet.Where(q => q.raporTipi == RAPORTIPI).ToList();
            }

            public List<AskalePortal.Data.Models.SatisOzet> GetAll(string RAPORTIPI, DateTime? TARIH)
            {
                var q = dal.Get(k => (k.raporTipi.Equals(RAPORTIPI) || string.IsNullOrEmpty(RAPORTIPI)) &&
                                     (k.tarih == TARIH || TARIH == null) &&
                                      k.enabled == true).OrderBy(k => k.satorg);
                return q.ToList();
            }

            #endregion GetAll
        }
    }
}
