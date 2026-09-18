using AskalePortal.BLL;
using AskalePortal.Data.Models;
using AskalePortal.Data.RequestModel;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AskalePortal.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActiveProcessInvoiceController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;
        public ActiveProcessInvoiceController(IConfiguration configuration, IWebHostEnvironment env, IMapper mapper)
        {
            _configuration = configuration;
            _env = env;
            _mapper = mapper;
        }
        #region getByActiveProcessId
        [HttpPost("getByActiveProcessId")]
        public ActionResult<List<ActiveProcessInvoice>> getByActiveProcessId([FromForm] int activeProcessId)
        {
            BLLActions.ActiveProcessInvoices bllActiveProcessInvoices = new BLLActions.ActiveProcessInvoices(_configuration, _env, _mapper);
            List<ActiveProcessInvoice> liste = bllActiveProcessInvoices.getByActiveProcessId(activeProcessId);
            return Ok(liste);
        }
        #endregion

        #region saveActiveProcessInvoice
        [HttpPost("saveActiveProcessInvoice")]
        public async Task<ActionResult<bool>> saveActiveProcessInvoice([FromForm] ActiveProcessInvoiceSaveDto request)
        {
            int userId = 0;
            if (HttpContext.User.Identity is ClaimsIdentity claimsIdentity)
            {
                userId = int.Parse(claimsIdentity?.FindFirst("userId")?.Value ?? "0");

            }
            BLLActions.ActiveProcessInvoices bllActiveProcessInvoices = new BLLActions.ActiveProcessInvoices(_configuration, _env, _mapper);
            bool deger = await bllActiveProcessInvoices.saveActiveProcessInvoice(request.listCustomerDocumentSap ?? [], request.activeProcessId, userId);
            return Ok(deger);
        }
        #endregion


    }
}
