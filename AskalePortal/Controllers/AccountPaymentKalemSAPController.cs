using AskalePortal.BLL;
using AskalePortal.Data.RequestModel;
using AskalePortal.Data.RequestParams;
using AskalePortal.Data.ResponseModels;
using AskalePortal.Data.ResponseParams;
using AutoMapper;
using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Security.Claims;

namespace AskalePortal.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class AccountPaymentKalemSAPController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;
        private readonly ISftpServer _server;

        public AccountPaymentKalemSAPController(IConfiguration configuration, IWebHostEnvironment env, IMapper mapper, ISftpServer server)
        {
            _configuration = configuration;
            _env = env;
            _mapper = mapper;
            _server = server;
        }

        private int GetCurrentUserId()
        {
            string? claimValue =
                User.FindFirst("userId")?.Value ??
                User.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
                User.FindFirst("sub")?.Value;

            return int.TryParse(claimValue, out int userId) ? userId : 0;
        }
        #region completed 
        [HttpPost("completed")]
        //[Authorize(Roles = "ROLE_74_SEE")]
        public ActionResult<PageReturn<AccountPaymentKalemActiveDto>> completed([FromForm] FilterPageParam<AccountPaymentKalemCompletedDtoParameter> filterPageParam)
        {
            BLLActions.AccountPaymentKalemSAPTable bllAccountPaymentKalemSAPTable = new BLLActions.AccountPaymentKalemSAPTable(_configuration, _env, _mapper, _server);
            int userId = GetCurrentUserId();
            PageReturn<AccountPaymentKalemActiveDto> page = bllAccountPaymentKalemSAPTable.completed(filterPageParam, userId);
            return Ok(page);
        }
        #endregion

        #region mylistdetail 
        [HttpPost("mylistdetail/{id:int}")]
        //[Authorize(Roles = "ROLE_74_SEE")]
        public ActionResult<AccountPaymentKalemMyListDetailDto> mylistdetail([FromRoute] int id)
        {
            BLLActions.AccountPaymentKalemSAPTable bllAccountPaymentKalemSAPTable = new BLLActions.AccountPaymentKalemSAPTable(_configuration, _env, _mapper, _server);

            AccountPaymentKalemMyListDetailDto page = bllAccountPaymentKalemSAPTable.mylistdetail(id);
            return Ok(page);
        }
        #endregion
        #region mylist 
        [HttpPost("mylist")]
        [Authorize(Roles = "ROLE_74_SEE")]
        public ActionResult<List<AccountPaymentKalemActiveDto>> mylist([FromForm] FilterParam<AccountPaymentKalemCompletedDtoParameter> filterParam)
        {
            BLLActions.AccountPaymentKalemSAPTable bllAccountPaymentKalemSAPTable = new BLLActions.AccountPaymentKalemSAPTable(_configuration, _env, _mapper, _server);

            List<AccountPaymentKalemActiveDto> list = bllAccountPaymentKalemSAPTable.mylist(filterParam);
            return Ok(list);
        }
        #endregion

        #region approved 
        [HttpPost("approved")]
        public async Task<ActionResult<bool>> approved([FromForm] bool approved, [FromForm] List<int> list, [FromForm] int userId)
        {
            BLLActions.AccountPaymentKalemSAPTable bllAccountPaymentKalemSAPTable = new BLLActions.AccountPaymentKalemSAPTable(_configuration, _env, _mapper, _server);
            bool deger = await bllAccountPaymentKalemSAPTable.approved(approved, list, userId);
            return Ok(deger);
        }
        #endregion

        #region listFilterByName1 
        [HttpPost("listFilterByName1")]
        public ActionResult<List<AccountPaymentKalemActiveDto>> listFilterByCompanyIdAndVendorCode([FromForm] FilterParam<AccountPaymentKalemActiveDtoParameter> filterParam)
        {
            BLLActions.AccountPaymentKalemSAPTable bllAccountPaymentKalemSAPTable = new BLLActions.AccountPaymentKalemSAPTable(_configuration, _env, _mapper, _server);
            List<AccountPaymentKalemActiveDto> liste = bllAccountPaymentKalemSAPTable.listFilterByCompanyIdAndVendorCode(filterParam);
            return Ok(liste);
        }
        #endregion

    }

}