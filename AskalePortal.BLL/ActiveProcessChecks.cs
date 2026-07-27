using AskalePortal.Data.SAP.InputParams;
using AskalePortal.Data.SAP.OutputParams;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using SapNwRfc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.BLL
{
    public partial class BLLActions
    {

        public class ActiveProcessChecks : BaseBLL<AskalePortal.Data.Models.ActiveProcessChecks>
        {
            private IConfiguration _configuration;
            private IWebHostEnvironment _env;
            private IMapper _mapper;
            public ActiveProcessChecks(IConfiguration configuration, IWebHostEnvironment env, IMapper mapper) : base(configuration, env)
            {
                _configuration = configuration;
                _env = env;
                _mapper = mapper;
            }

            public List<Data.Models.ActiveProcessChecks> getByActiveProcessId(int activeProcessId)
            {
                List<Data.Models.ActiveProcessChecks> liste = dal.Get(u => u.enabled == true && u.activeProcessId == activeProcessId).ToList();
                return liste;
            }
            public async Task<bool> saveCheckList(List<Data.RequestModel.Checks> listActiveProcessChecks, int activeProcessId, int createdUserId)
            {
                try
                {

                    foreach (Data.RequestModel.Checks entity in listActiveProcessChecks)
                    {
                        entity.activeProcessId = activeProcessId;
                        if (entity.id == null)
                        {
                            entity.createdUserId = createdUserId;
                            entity.createdDate = DateTime.Now;
                            entity.enabled = true;
                            await Add(_mapper.Map<Data.Models.ActiveProcessChecks>(entity));
                        }
                        else
                        {

                            entity.updatedUserId = createdUserId;
                            entity.updateDate = DateTime.Now;
                            entity.enabled = true;
                            await Update(_mapper.Map<Data.Models.ActiveProcessChecks>(entity));
                        }
                    }
                    return true;
                }
                catch 
                {
                    return false;
                }
            }
            public List<Data.Models.ActiveProcessChecks> getCheckList(string bukrs, string kunnr, string portfo)
            {
                try
                {
                    List<Data.Models.ActiveProcessChecks> liste = new List<Data.Models.ActiveProcessChecks>();
                    BLLActions.SAPConnectionData bllSapConnection = new BLLActions.SAPConnectionData(_configuration, _env);
                    SapConnection? sapConn = bllSapConnection.sapConnection(_configuration, _env);


                    if (sapConn != null)
                    {
                        sapConn.Connect();
                        ISapFunction sapFunction = sapConn.CreateFunction("ZWEBI070");


                        ActiveProcessChecksOutput? output = sapFunction.Invoke<ActiveProcessChecksOutput>(
                            input: new ChecksParams
                            {
                                bukrs = bukrs,
                                kunnr = kunnr,
                                portfo = portfo

                            });
                        sapConn.Disconnect();
                        int numRows = (output.activeProcessCheckList ?? []).Count();
                        for (int iRow = 0; iRow < numRows; iRow++)
                        {
                            Data.Models.ActiveProcessChecks activeProcessChecks = new Data.Models.ActiveProcessChecks();
                            ActiveProcessCheckList table = (output.activeProcessCheckList ?? [])[iRow];
                            activeProcessChecks.belnr=table.belnr;
                            activeProcessChecks.kunnr = table.kunnr;
                            activeProcessChecks.name1 = table.name1;
                            activeProcessChecks.netdt = table.netdt;
                            activeProcessChecks.wrbtr = double.Parse(table.wrbtr ?? "0") ;

                            liste.Add(activeProcessChecks);
                        }
                    }
                    return liste;
                }
                catch (Exception)
                {
                    // TODO: handle exception
                }
                return [];
            }
        }
    }
}
