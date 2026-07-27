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

namespace AskalePortal.BLL
{
    public partial class BLLActions
    {
        public class Personel : BaseBLL<AskalePortal.Data.Models.Employee>
        {
            private readonly IConfiguration _configuration;
            private readonly IWebHostEnvironment _env;
            public Personel(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
            {
                _configuration = configuration;
                _env = env;
            }
            #region GetAllFromSAP

            public List<Data.SAP.Models.EmployeeSap>? GetAllFromSAP(string? PERNR)
            {
                List<Data.SAP.Models.EmployeeSap>? lstData = new List<Data.SAP.Models.EmployeeSap>();
                try
                {
                    BLLActions.SAPConnectionData bllSapConnection = new BLLActions.SAPConnectionData(_configuration, _env);
                    SapConnection? sapConnection = bllSapConnection.sapConnection(_configuration, _env);

                    if (sapConnection == null)
                    {

                    }
                    else
                    {
                        ISapFunction sapFunction = sapConnection.CreateFunction("ZWEBI009");

                        PersonelNumberOutputs list = sapFunction.Invoke<PersonelNumberOutputs>(input: new PersonelNoInputs
                        {
                            PersonelNo = PERNR
                        });
                        lstData=list.personelNo?.ToList();
                     
                    }

                }
                catch (Exception ex)
                {
                    LogError(ex);
                }
                lstData = lstData?.Where(u => u.PLANS != "99999999").ToList();
                return lstData;
            }

			#endregion GetAllFromSAP


			public string? GetSAPID(string PERNR)
			{
				
				try
				{
                    BLLActions.SAPConnectionData bllSapConnection = new BLLActions.SAPConnectionData(_configuration, _env);
                    SapConnection? sapConnection = bllSapConnection.sapConnection(_configuration, _env);

                    if (sapConnection == null)
                    {

                    }
                    else
                    {
                        ISapFunction sapFunction = sapConnection.CreateFunction("ZWEBI009");

                        PersonelNumberOutputs list = sapFunction.Invoke<PersonelNumberOutputs>(input: new PersonelNoInputs
                        {
                            PersonelNo = PERNR
                        });
                        return list.personelNo?[0].SYSUNAME;
                    }

					
				}
				catch (Exception ex)
				{
					LogError(ex);
					return "0";
				}
                return null;
				
			}
		}
    }
}
