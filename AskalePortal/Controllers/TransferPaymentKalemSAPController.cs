using AskalePortal.BLL;
using AskalePortal.Data.RequestModel;
using AskalePortal.Data.RequestParams;
using AskalePortal.Data.ResponseModels;
using AskalePortal.Data.ResponseParams;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AskalePortal.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransferPaymentKalemSAPController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;
        private readonly ISftpServer _server;

        public TransferPaymentKalemSAPController(IConfiguration configuration, IWebHostEnvironment env, IMapper mapper, ISftpServer server)
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

        #region listFilterByName1 
        [HttpPost("listFilterByName1")]
        public ActionResult<List<TransferPaymentKalemActiveDto>> listFilterByCompanyIdAndVendorCode([FromForm] FilterParam<TransferPaymentKalemActiveDtoParameter> filterParam)
        {
            BLLActions.TransferPaymentKalemSAPTable bllTransferPaymentKalemSAPTable = new BLLActions.TransferPaymentKalemSAPTable(_configuration, _env,_mapper, _server);
            List<TransferPaymentKalemActiveDto> liste = bllTransferPaymentKalemSAPTable.listFilterByCompanyIdAndVendorCode(filterParam);
            return Ok(liste);
        }
        #endregion

        #region mylist 
        [HttpPost("mylist")]
        public ActionResult<List<TransferPaymentKalemActiveDto>> mylist([FromForm] FilterParam<TransferPaymentKalemMyListDtoParameter> filterParam)
        {
            BLLActions.TransferPaymentKalemSAPTable bllTransferPaymentKalemSAPTable = new BLLActions.TransferPaymentKalemSAPTable(_configuration, _env, _mapper, _server);
            List<TransferPaymentKalemActiveDto> liste = bllTransferPaymentKalemSAPTable.mylist(filterParam);
            return Ok(liste);
        }
        #endregion

        #region completed 
        [HttpPost("completed")]
        public ActionResult<PageReturn<TransferPaymentKalemActiveDto>> completed([FromForm] FilterPageParam<TransferPaymentKalemMyListDtoParameter> filterPageParam)
        {
            int userId = GetCurrentUserId();
            BLLActions.TransferPaymentKalemSAPTable bllTransferPaymentKalemSAPTable = new BLLActions.TransferPaymentKalemSAPTable(_configuration, _env, _mapper, _server);
            PageReturn<TransferPaymentKalemActiveDto> page = bllTransferPaymentKalemSAPTable.completed(filterPageParam, userId);
            return Ok(page);
        }
        #endregion

        #region mylistdetail 
        [HttpPost("mylistdetail/{id:int}")]
        public ActionResult<TransferPaymentKalemMyListDetailDto> mylistdetail([FromRoute] int id)
        {
            BLLActions.TransferPaymentKalemSAPTable bllTransferPaymentKalemSAPTable = new BLLActions.TransferPaymentKalemSAPTable(_configuration, _env, _mapper, _server);
            TransferPaymentKalemMyListDetailDto dto = bllTransferPaymentKalemSAPTable.mylistdetail(id);
            return Ok(dto);
        }
        #endregion

        #region approved 
        [HttpPost("approved")]
        public async Task<ActionResult<bool>> approved([FromForm] bool approved, [FromForm] List<int> list, [FromForm] int userId)
        {
            BLLActions.TransferPaymentKalemSAPTable bllTransferPaymentKalemSAPTable = new BLLActions.TransferPaymentKalemSAPTable(_configuration, _env, _mapper, _server);

            bool sonuc =await bllTransferPaymentKalemSAPTable.approved(approved, list, userId);
            return Ok(sonuc);
        }
        #endregion

    }
}
