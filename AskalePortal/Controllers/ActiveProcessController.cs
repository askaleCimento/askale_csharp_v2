using AskalePortal.BLL;
using AskalePortal.Data.Models;
using AskalePortal.Data.RequestModel;
using AskalePortal.Data.RequestParams;
using AskalePortal.Data.ResponseModels;
using AskalePortal.Data.ResponseParams;
using AskalePortal.Data.SAP.InputParams;
using AutoMapper;
using Azure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using System;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Security.Claims;

namespace AskalePortal.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActiveProcessController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;
        public ActiveProcessController(IConfiguration configuration, IWebHostEnvironment env, IMapper mapper)
        {
            _configuration = configuration;
            _env = env;
            _mapper = mapper;
        }
        #region hasactiveprocess
        [HttpPost("hasactiveprocess")]
        public ActionResult<bool> hasactiveprocess([FromForm] int processType, [FromForm] string relatedDataId, [FromForm] string relatedDataDesc)
        {
            BLLActions.ActiveProcesses bllActiveProcesses = new BLLActions.ActiveProcesses(_configuration, _env, _mapper);
            bool sonuc = bllActiveProcesses.hasActiveProcess(processType, relatedDataId, relatedDataDesc);
            return Ok(sonuc);
        }
        #endregion

        #region changelimit
        [HttpPost("changelimit")]
        public async Task<ActionResult<bool>> changelimit([FromForm] string name1, [FromForm] string kunnr,
            [FromForm] string klimk, [FromForm] string dagitimKanali, [FromForm] decimal amount,
            [FromForm] string description, [FromForm] int userId, [FromForm] int processId)
        {
            BLLActions.ActiveProcesses bllActiveProcesses = new BLLActions.ActiveProcesses(_configuration, _env, _mapper);
            bool deger = await bllActiveProcesses.changeLimit(name1, kunnr, klimk, dagitimKanali, amount, description, userId, processId);
            return Ok(deger);
        }
        #endregion

        #region changealllimit
        [HttpPost("changealllimit")]

        public async Task<ActionResult<bool>> changealllimit([FromForm] bool approved, [FromForm] List<int> listInt,
            [FromForm] int userId)
        {
            BLLActions.ActiveProcesses bllActiveProcesses = new BLLActions.ActiveProcesses(_configuration, _env, _mapper);
            bool deger = await bllActiveProcesses.changeAllLimit(approved, listInt, userId);
            return Ok(deger);
        }
        #endregion

        #region changealldate
        [HttpPost("changealldate")]
        public async Task<ActionResult<bool>> changealldate([FromForm] bool approved, [FromForm] List<int> listInt,
            [FromForm] int userId)
        {
            BLLActions.ActiveProcesses bllActiveProcesses = new BLLActions.ActiveProcesses(_configuration, _env, _mapper);
            bool deger = await bllActiveProcesses.changeAllDate(approved, listInt, userId);

            return Ok(deger);
        }
        #endregion

        #region changedate
        [HttpPost("changedate")]
        public async Task<ActionResult<bool>> changedate([FromForm] string bukrs, [FromForm] int gjahr,
            [FromForm] string name1, [FromForm] string kunnr, [FromForm] string faedt,
            [FromForm] string belnr, [FromForm] string zfbdt, [FromForm] string dagitimKanali,
            [FromForm] int newValue, [FromForm] string description, [FromForm] int userId, [FromForm] string belgeTutari)
        {
            BLLActions.ActiveProcesses bllActiveProcesses = new BLLActions.ActiveProcesses(_configuration, _env, _mapper);
            bool deger = await bllActiveProcesses.changedate(bukrs, gjahr, name1, kunnr, faedt, belnr, zfbdt,
                    dagitimKanali, newValue, description, userId, belgeTutari);

            return Ok(deger);
        }
        #endregion

        #region listFilterByStateIdAndTypeId
        [HttpPost("listFilterByStateIdAndTypeId")]
        public ActionResult<PageReturn<ActiveProcessDto>> listFilterByStateIdAndTypeId(
            [FromForm] FilterPageParam<ActiveProcessListParameter> filterPageParam)
        {
            BLLActions.ActiveProcesses bllActiveProcesses = new BLLActions.ActiveProcesses(_configuration, _env, _mapper);
            PageReturn<ActiveProcessDto> page = bllActiveProcesses.listFilterByStateIdAndTypeId(filterPageParam);
            return Ok(page);
        }
        #endregion

        #region mylist
        [HttpPost("mylist")]
        public ActionResult<PageReturn<ActiveProcessDto>> myList([FromForm] FilterPageParam<ActiveProsessMyListDtoParameter> filterPageParam)
        {
            BLLActions.ActiveProcesses bllActiveProcesses = new BLLActions.ActiveProcesses(_configuration, _env, _mapper);

            return Ok(bllActiveProcesses.mylist(filterPageParam));
        }
        #endregion

        #region approved
        [HttpPost("approved")]

        public ActionResult<string> approved([FromForm] string guid)
        {
            int userId = 0;
            if (HttpContext.User.Identity is ClaimsIdentity claimsIdentity)
            {
                userId = int.Parse(claimsIdentity?.FindFirst("userId")?.Value ?? "0");

            }
            BLLActions.AdminUsers bllAdminUsers = new BLLActions.AdminUsers(_configuration, _env, _mapper);
            AdminUser? user = bllAdminUsers.GetByID(userId);
            BLLActions.ActiveProcesses bllActiveProcesses = new BLLActions.ActiveProcesses(_configuration, _env, _mapper);
            string deger = bllActiveProcesses.approved(guid, user);
            return Ok(deger);
        }
        #endregion

        #region reject
        [HttpPost("reject")]
        public ActionResult<string> reject([FromForm] string guid)
        {

            int userId = 0;
            if (HttpContext.User.Identity is ClaimsIdentity claimsIdentity)
            {
                userId = int.Parse(claimsIdentity?.FindFirst("userId")?.Value ?? "0");

            }
            BLLActions.AdminUsers bllAdminUsers = new BLLActions.AdminUsers(_configuration, _env, _mapper);
            AdminUser user = bllAdminUsers.GetByID(userId);
            BLLActions.ActiveProcesses bllActiveProcesses = new BLLActions.ActiveProcesses(_configuration, _env, _mapper);
            string deger = bllActiveProcesses.reject(guid, user);

            return Ok(deger);
        }
        #endregion

        #region setCustomerSanalLimit
        [HttpPost("setCustomerSanalLimit")]
        public ActionResult<string> setCustomerSanalLimit([FromForm] string kunnr, [FromForm] double dmbtr,
            [FromForm] string yeniMusteriMi)
        {
            int userId = 0;
            if (HttpContext.User.Identity is ClaimsIdentity claimsIdentity)
            {
                userId = int.Parse(claimsIdentity?.FindFirst("userId")?.Value ?? "0");

            }
            BLLActions.AdminUsers bllAdminUsers = new BLLActions.AdminUsers(_configuration, _env, _mapper);
            AdminUser? username = bllAdminUsers.GetByID(userId);
            string nameString = username?.username.ToUpper() ??"";
            nameString = nameString.Replace('İ', 'I').Replace('Ü', 'U').Replace('Ğ', 'G').Replace('Ö', 'O')
                    .Replace('Ç', 'C').Replace('Ş', 'S');
            BLLActions.ActiveProcesses bllActiveProcesses = new BLLActions.ActiveProcesses(_configuration, _env, _mapper);

            string deger = bllActiveProcesses.setCustomerSanalLimit(kunnr, dmbtr, yeniMusteriMi, nameString);

            return Ok(deger);
        }
        #endregion
        #region getAvgVadeDays
        [HttpPost("getAvgVadeDays")]
        public ActionResult<AvgVadeDaysDto> getAvgVadeDays([FromForm] string kunnr)
        {
            BLLActions.ActiveProcesses bllActiveProcesses = new BLLActions.ActiveProcesses(_configuration, _env, _mapper);
            AvgVadeDaysDto dto = bllActiveProcesses.getAvgVadeDays(kunnr);
            return Ok(dto);
        }
        #endregion

    }
}
