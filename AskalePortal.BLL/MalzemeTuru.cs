using AskalePortal.Data.Models;
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
using System.Web;

namespace AskalePortal.BLL
{
    public partial class BLLActions
    {
        public class MalzemeTuru : BaseBLL<Data.SAP.Models.MalzemeTuru>
        {
            private readonly IConfiguration _configuration;
            private readonly IWebHostEnvironment _env;
            private readonly IMapper _mapper;
            public MalzemeTuru(IConfiguration configuration, IWebHostEnvironment env, IMapper mapper) : base(configuration, env)
            {
                _configuration = configuration;
                _env = env;
                _mapper = mapper;
            }

            public List<Data.SAP.Models.MalzemeTuru>? GetAllFromSAP(string WERKS_S)
            {
                List<Data.SAP.Models.MalzemeTuru>? lstmalzemeTurleri = new List<Data.SAP.Models.MalzemeTuru>();


                try
                {
                    BLLActions.SAPConnectionData bllSapConnection = new BLLActions.SAPConnectionData(_configuration, _env);
                    SapConnection? con = bllSapConnection.sapConnection(_configuration, _env);

                    if (con != null)
                    {
                        con.Connect();
                        ISapFunction sapFunction = con.CreateFunction("ZWEBI015");

                        MalzemeOutputParams list = sapFunction.Invoke<MalzemeOutputParams>(input: new BukrsParams
                        {
                            IV_SIRKET = WERKS_S,
                        });
                        lstmalzemeTurleri = list.listMalzeme?.ToList();


                    }





                }
                catch (Exception ex)
                {
                    LogError(ex);
                }



                return lstmalzemeTurleri;

            }
            public List<Data.SAP.Models.MalzemeTuru>? GetAllCementFromSAP(string WERKS_S)
            {
                List<Data.SAP.Models.MalzemeTuru>? lstmalzemeTurleri = new List<Data.SAP.Models.MalzemeTuru>();



                try
                {
                    BLLActions.SAPConnectionData bllSapConnection = new BLLActions.SAPConnectionData(_configuration, _env);
                    SapConnection? con = bllSapConnection.sapConnection(_configuration, _env);

                    if (con != null)
                    {
                        con.Connect();
                        ISapFunction sapFunction = con.CreateFunction("ZWEBI050");

                        MalzemeOutputParams list = sapFunction.Invoke<MalzemeOutputParams>(input: new BukrsParams
                        {
                            IV_SIRKET = WERKS_S,
                        });
                        lstmalzemeTurleri = list.listMalzeme?.ToList();


                    }
                }
                catch (Exception ex)
                {
                    LogError(ex);
                }



                return lstmalzemeTurleri;

            }
            public List<Data.SAP.Models.MalzemeTuru>? GetAllFromSAPMalzemeTuru(string WERKS_S)
            {
                List<Data.SAP.Models.MalzemeTuru>? lstmalzemeTurleri = new List<Data.SAP.Models.MalzemeTuru>();



                try
                {
                    BLLActions.SAPConnectionData bllSapConnection = new BLLActions.SAPConnectionData(_configuration, _env);
                    SapConnection? con = bllSapConnection.sapConnection(_configuration, _env);

                    BLLActions.Companies bllCompanies = new BLLActions.Companies(_configuration, _env, _mapper);

                    Company? company = bllCompanies.GetByID(int.Parse(WERKS_S));
                    if (company != null)
                    {
                        if (con != null)
                        {
                            con.Connect();
                            ISapFunction sapFunction = con.CreateFunction("ZWEBI015");

                            MalzemeOutputParams list = sapFunction.Invoke<MalzemeOutputParams>(input: new MalzemeTurleriWithMalGrubuInputParams
                            {
                                IV_SIRKET = company.vkorg,
                                IV_MALGRUP = "X"
                            });
                            lstmalzemeTurleri = list.listMalzeme?.ToList();

                            return lstmalzemeTurleri;

                        }
                        return lstmalzemeTurleri;
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
