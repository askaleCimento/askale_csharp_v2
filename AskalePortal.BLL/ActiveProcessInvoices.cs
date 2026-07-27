using AskalePortal.Data.Models;
using AskalePortal.Data.SAP.OutputParams;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
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
        public class ActiveProcessInvoices : BaseBLL<AskalePortal.Data.Models.ActiveProcessInvoice>
        {
            private IConfiguration _configuration;
            private IWebHostEnvironment _env;
            private IMapper _mapper;
            public ActiveProcessInvoices(IConfiguration configuration, IWebHostEnvironment env, IMapper mapper) : base(configuration, env)
            {
                _configuration = configuration;
                _env = env;
                _mapper = mapper;
            }

            public List<ActiveProcessInvoice> getByActiveProcessId(int activeProcessId)
            {
                List<ActiveProcessInvoice> liste = dal.Get(u => u.enabled == true && u.activeProcessId == activeProcessId).ToList();
                return liste;
            }

            public async Task<bool> saveActiveProcessInvoice(List<CustomerDocumentDto> listCustomerDocumentSap, int activeProcessId, int userId)
            {
                try
                {

                    foreach (CustomerDocumentDto customerDocumentDto in listCustomerDocumentSap)
                    {
                        ActiveProcessInvoice activeProcessInvoice = new ActiveProcessInvoice();
                        activeProcessInvoice.activeProcessId = activeProcessId;
                        activeProcessInvoice.belnr = customerDocumentDto.BELNR;
                        activeProcessInvoice.bldat = customerDocumentDto.BLDAT;
                        activeProcessInvoice.bukrs = customerDocumentDto.BUKRS;
                        activeProcessInvoice.createdDate = DateTime.Now;
                        activeProcessInvoice.createdUserId = userId;
                        //buna bak
                        //activeProcessInvoice.dagitimkanali = customerDocumentDto.vtweg;
                        activeProcessInvoice.dmshb = 0;
                        activeProcessInvoice.enabled = true;
                        activeProcessInvoice.faedt = customerDocumentDto.FAEDT;
                        activeProcessInvoice.gjahr = int.Parse(customerDocumentDto.GJAHR??"0");
                        activeProcessInvoice.zterm = customerDocumentDto.ZTERM;
                        await Add(activeProcessInvoice);

                    }
                    return true;
                }
                catch
                {
                    return false;
                }

            }

        }
    }
}
