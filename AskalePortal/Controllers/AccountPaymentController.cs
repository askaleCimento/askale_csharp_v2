
using AskalePortal.BLL;
using AskalePortal.Data.SAP.InputParams;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Reflection.PortableExecutable;
using System.Security.Claims;

namespace AskalePortal.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class AccountPaymentController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;
        private readonly ISftpServer _server;
        public AccountPaymentController(IConfiguration configuration, IWebHostEnvironment env, IMapper mapper, ISftpServer server)
        {
            _configuration = configuration;
            _env = env;
            _mapper = mapper;
            _server = server;
        }

        #region save 

        [HttpPost("save")]
        public async Task<ActionResult<object>> save([FromForm] Data.RequestModel.AccountPaymentSAPTableRequest entityReq)
        {

            if (entityReq != null)
            {
                int? userId = 0;
                if (HttpContext.User.Identity is ClaimsIdentity identity)
                {
                    userId = int.Parse(identity?.FindFirst("userId")?.Value ?? "0");
                }

                BLL.BLLActions.AccountPaymentSAPTable bllAccountPaymentSAPTable = new BLL.BLLActions.AccountPaymentSAPTable(_configuration, _env, _mapper, _server);
                Data.Models.AccountPaymentSAPTable? accountPaymentSAPTable = entityReq.oenum != null ? bllAccountPaymentSAPTable.GetByOENUM(entityReq.oenum) : null;
                Data.Models.AccountPaymentSAPTable saveAccountPaymentSAPTable;
                if (accountPaymentSAPTable != null)
                {
                    saveAccountPaymentSAPTable = accountPaymentSAPTable;
                    saveAccountPaymentSAPTable.updateDate = DateTime.Now;
                    saveAccountPaymentSAPTable.updatedUserId = userId == 0 ? null : userId;
                    saveAccountPaymentSAPTable.enabled = entityReq.enabled ?? true;
                    saveAccountPaymentSAPTable.createdDate = entityReq.createdDate;
                    saveAccountPaymentSAPTable.createdUserId = entityReq.createdUserId;
                    saveAccountPaymentSAPTable.SubeKodu = entityReq.SubeKodu;
                    saveAccountPaymentSAPTable.bukrs = entityReq.bukrs;
                    saveAccountPaymentSAPTable.kurumKodu = entityReq.kurumKodu;
                    saveAccountPaymentSAPTable.usnam = entityReq.usnam;
                    saveAccountPaymentSAPTable.bankl = entityReq.bankl;
                    saveAccountPaymentSAPTable.aedat = entityReq.aedat;
                    saveAccountPaymentSAPTable.aenam = entityReq.aenam;
                    saveAccountPaymentSAPTable.aeuhr = entityReq.aeuhr;
                    saveAccountPaymentSAPTable.bankn = entityReq.bankn;
                    saveAccountPaymentSAPTable.belnr = entityReq.belnr;
                    saveAccountPaymentSAPTable.bstat = entityReq.bstat;
                    saveAccountPaymentSAPTable.cpudt = entityReq.cpudt;
                    saveAccountPaymentSAPTable.cputm = entityReq.cputm;
                    saveAccountPaymentSAPTable.gjahr = entityReq.gjahr;
                    saveAccountPaymentSAPTable.hkont = entityReq.hkont;
                    saveAccountPaymentSAPTable.iban = entityReq.iban;
                    saveAccountPaymentSAPTable.name1 = entityReq.name1;
                    saveAccountPaymentSAPTable.name2 = entityReq.name2;
                    saveAccountPaymentSAPTable.oenum = entityReq.oenum;
                    saveAccountPaymentSAPTable.SubeKodu = entityReq.SubeKodu;
                    saveAccountPaymentSAPTable.unva1 = entityReq.unva1;
                    saveAccountPaymentSAPTable.unva2 = entityReq.unva2;
                    saveAccountPaymentSAPTable.usnam = entityReq.usnam;
                    saveAccountPaymentSAPTable.znot = entityReq.znot;
                    saveAccountPaymentSAPTable.zsayino = entityReq.zsayino;

                    await bllAccountPaymentSAPTable.Update(saveAccountPaymentSAPTable);
                    return Ok(saveAccountPaymentSAPTable);
                }
                else
                {
                    saveAccountPaymentSAPTable = new Data.Models.AccountPaymentSAPTable();
                    saveAccountPaymentSAPTable.enabled = entityReq.enabled ?? true;
                    saveAccountPaymentSAPTable.createdDate = entityReq.createdDate;
                    saveAccountPaymentSAPTable.createdUserId = entityReq.createdUserId;
                    saveAccountPaymentSAPTable.SubeKodu = entityReq.SubeKodu;
                    saveAccountPaymentSAPTable.bukrs = entityReq.bukrs;
                    saveAccountPaymentSAPTable.kurumKodu = entityReq.kurumKodu;
                    saveAccountPaymentSAPTable.usnam = entityReq.usnam;
                    saveAccountPaymentSAPTable.bankl = entityReq.bankl;
                    saveAccountPaymentSAPTable.aedat = entityReq.aedat;
                    saveAccountPaymentSAPTable.aenam = entityReq.aenam;
                    saveAccountPaymentSAPTable.aeuhr = entityReq.aeuhr;
                    saveAccountPaymentSAPTable.bankn = entityReq.bankn;
                    saveAccountPaymentSAPTable.belnr = entityReq.belnr;
                    saveAccountPaymentSAPTable.bstat = entityReq.bstat;
                    saveAccountPaymentSAPTable.cpudt = entityReq.cpudt;
                    saveAccountPaymentSAPTable.cputm = entityReq.cputm;
                    saveAccountPaymentSAPTable.gjahr = entityReq.gjahr;
                    saveAccountPaymentSAPTable.hkont = entityReq.hkont;
                    saveAccountPaymentSAPTable.iban = entityReq.iban;
                    saveAccountPaymentSAPTable.name1 = entityReq.name1;
                    saveAccountPaymentSAPTable.name2 = entityReq.name2;
                    saveAccountPaymentSAPTable.oenum = entityReq.oenum;
                    saveAccountPaymentSAPTable.SubeKodu = entityReq.SubeKodu;
                    saveAccountPaymentSAPTable.unva1 = entityReq.unva1;
                    saveAccountPaymentSAPTable.unva2 = entityReq.unva2;
                    saveAccountPaymentSAPTable.usnam = entityReq.usnam;
                    saveAccountPaymentSAPTable.znot = entityReq.znot;
                    saveAccountPaymentSAPTable.zsayino = entityReq.zsayino;


                    await bllAccountPaymentSAPTable.Add(saveAccountPaymentSAPTable);
                    return Ok(saveAccountPaymentSAPTable);
                }
            }
            return Ok(null);


        }

        #endregion

        #region delete 
        [HttpPost("delete")]
        public ActionResult<int> delete([FromForm] int id)
        {
            try
            {
                BLL.BLLActions.AccountPaymentSAPTable bllAccountPaymentSAPTable = new BLL.BLLActions.AccountPaymentSAPTable(_configuration, _env, _mapper, _server);
                bllAccountPaymentSAPTable.Delete(id);
                return Ok(1);
            }
            catch
            {
                return Ok(0);
            }

        }

        #endregion

        #region getById
        [HttpPost("getById")]

        public ActionResult<object> getById([FromForm] int id)
        {
            BLL.BLLActions.AccountPaymentSAPTable bllAccountPaymentSAPTable = new BLL.BLLActions.AccountPaymentSAPTable(_configuration, _env, _mapper, _server);

            Data.Models.AccountPaymentSAPTable? accountPaymentSAPTable = bllAccountPaymentSAPTable.GetByID(id);
            if (accountPaymentSAPTable == null)
            {
                return NotFound();
            }
            return Ok(accountPaymentSAPTable);


        }
        #endregion



        #region getById
        [HttpPost("GetAccountPayment")]
        [Authorize(Roles = "ROLE_74_SEE")]
        public async Task<ActionResult<string>> getaccountpayment([FromForm] AccontPaymentApi api)
        {
            BLL.BLLActions.AccountPaymentSAPTable bllAccountPaymentSAPTable = new BLL.BLLActions.AccountPaymentSAPTable(_configuration, _env, _mapper, _server);

            string? deger = await bllAccountPaymentSAPTable.GetAccountPayment(api.apiKey!, api.odemeEmri!);
            if (deger == null)
            {
                return NotFound();
            }
            return Ok(deger);


        }
        #endregion

        [HttpPost("GetTransferPayment")]
        [Authorize(Roles = "ROLE_74_SEE")]
        public async Task<ActionResult<string>> getTransferPayment([FromForm] TransferPaymentApi api)
        {
            BLL.BLLActions.AccountPaymentSAPTable bllAccountPaymentSAPTable = new BLL.BLLActions.AccountPaymentSAPTable(_configuration, _env, _mapper, _server);
            string? deger = await bllAccountPaymentSAPTable.GetTransferPayment(api.apiKey!, api.havaleEmri);

            return Ok(deger);
        }
    }
}
