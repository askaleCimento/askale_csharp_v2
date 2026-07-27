using AskalePortal.BLL;
using AskalePortal.Data.Models;
using AskalePortal.Data.RequestModel;
using AskalePortal.Data.RequestParams;
using AskalePortal.Data.ResponseModels;
using AskalePortal.Data.ResponseParams;
using AskalePortal.Data.SAP.OutputParams;
using AutoMapper;
using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using System.Runtime.CompilerServices;
using System.Security.Claims;

namespace AskalePortal.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnnualLeaveTableController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;
        public AnnualLeaveTableController(IConfiguration configuration, IWebHostEnvironment env, IMapper mapper)
        {
            _configuration = configuration;
            _env = env;
            _mapper = mapper;
        }

        [HttpPost("getById")]
        public ActionResult getById([FromForm] int id)
        {
            BLLActions.AnnualLeaveTable bllAnnualLeaveTable = new BLLActions.AnnualLeaveTable(_configuration, _env, _mapper);
            return Ok(bllAnnualLeaveTable.GetByID(id));
        }
        [HttpPost("getbyannualleaveId")]
        public ActionResult getbyannualleaveId([FromForm] int annualId)
        {
            BLLActions.AnnualLeaveTable bllAnnualLeaveTable = new BLLActions.AnnualLeaveTable(_configuration, _env, _mapper);
            return Ok(bllAnnualLeaveTable.getByAnnualLeaveId(annualId));
        }
        [HttpPost("save")]
        public async Task<ActionResult<AnnualLeaveTableSaveDto>> save([FromForm] AnnualLeaveTableSaveDto entity)
        {
            BLLActions.AnnualLeaveTable bllAnnualLeaveTable = new BLLActions.AnnualLeaveTable(_configuration, _env, _mapper);
            int userId = GetCurrentUserId();
            if (userId <= 0)
            {
                return Unauthorized();
            }
            AnnualLeaveTableSaveDto savedData = await bllAnnualLeaveTable.save(entity, userId);
            return Ok(savedData);


        }

        [HttpPost("delete")]
        public ActionResult<int> delete([FromForm] int id)
        {
            BLLActions.AnnualLeaveTable bllAnnualLeaveTable = new BLLActions.AnnualLeaveTable(_configuration, _env, _mapper);
            try
            {
                bllAnnualLeaveTable.Delete(id);
                return Ok(1);
            }
            catch
            {
                return Ok(0);
            }
        }
        [HttpPost("getAnnualLeaveSap")]
        public ActionResult<AnnualLeaveSapModel?> getAnnualLeaveSap([FromForm] string perNo)
        {
            BLLActions.AnnualLeaveTable bllAnnualLeaveTable = new BLLActions.AnnualLeaveTable(_configuration, _env, _mapper);
            return Ok(bllAnnualLeaveTable.getAnnualLeaveSap(perNo));
        }
        [HttpPost("mylist")]
        public ActionResult<PageReturn<AnnualLeaveTableResponseDto?>> mylist([FromForm] FilterPageParam<AnnualTableFilterDtoRequest> filterPageParam)
        {
            BLLActions.AnnualLeaveTable bllAnnualLeaveTable = new BLLActions.AnnualLeaveTable(_configuration, _env, _mapper);
            return Ok(bllAnnualLeaveTable.mylist(filterPageParam));
        }

        #region approvalCount
        [HttpPost("approvalCount")]
        public ActionResult<int> myApprovalCount([FromForm] int userId)
        {
            BLLActions.AnnualLeaveTable bllAnnualLeaveTable = new BLLActions.AnnualLeaveTable(_configuration, _env, _mapper);
            int count = bllAnnualLeaveTable.approvalCount(userId);
            return Ok(count);
        }
        #endregion

        #region approvalCountIk
        [HttpPost("approvalCountIk")]
        public ActionResult<int> myApprovalCountIk([FromForm] int userId)
        {
            BLLActions.AnnualLeaveTable bllAnnualLeaveTable = new BLLActions.AnnualLeaveTable(_configuration, _env, _mapper);
            int count = bllAnnualLeaveTable.approvalCountIk(userId);
            return Ok(count);
        }
        #endregion

        #region getAllByUserId
        [HttpPost("getAllByUserId")]
        public ActionResult<List<Data.Models.AnnualLeaveTable>?> getAllByUserId([FromForm] int userId)
        {
            BLLActions.AnnualLeaveTable bllAnnualLeaveTable = new BLLActions.AnnualLeaveTable(_configuration, _env, _mapper);
            List<Data.Models.AnnualLeaveTable>? liste = bllAnnualLeaveTable.getAllByUserId(userId);
            return Ok(liste);
        }
        #endregion

        #region list
        [HttpPost("list")]
        public ActionResult<PageReturn<AnnualLeaveTableResponseDto>?> list([FromForm] FilterPageParam<AnnualLeaveFilterDtoRequest> filterPageParam)
        {
            int userId = GetCurrentUserId();
            if (userId <= 0)
            {
                return Unauthorized();
            }
            BLLActions.AnnualLeaveTable bllAnnualLeaveTable = new BLLActions.AnnualLeaveTable(_configuration, _env, _mapper);
            PageReturn<AnnualLeaveTableResponseDto>? liste = bllAnnualLeaveTable.list(filterPageParam, userId);
            return Ok(liste);
        }
        #endregion
        #region iklist
        [HttpPost("iklist")]
        public ActionResult<PageReturn<AnnualLeaveTableDto>> iklist([FromForm] FilterPageParam<AnnualLeaveFilterDtoRequest> filterPageParam)
        {
            BLLActions.AnnualLeaveTable bllAnnualLeaveTable = new BLLActions.AnnualLeaveTable(_configuration, _env, _mapper);
            PageReturn<AnnualLeaveTableDto>? liste = bllAnnualLeaveTable.iklist(filterPageParam);

            return Ok(liste);
        }
        #endregion
        #region listFinished
        [HttpPost("listFinished")]
        public ActionResult<PageReturn<AnnualLeaveTableDto>> listFinished([FromForm] FilterPageParam<AnnualLeaveFilterDtoRequest> filterPageParam)
        {
            BLLActions.AnnualLeaveTable bllAnnualLeaveTable = new BLLActions.AnnualLeaveTable(_configuration, _env, _mapper);
            int userId = GetCurrentUserId();
            if (userId <= 0)
            {
                return Unauthorized();
            }
            PageReturn<AnnualLeaveTableDto>? liste = bllAnnualLeaveTable.listFinished(filterPageParam, userId);

            return Ok(liste);
        }
        #endregion

        #region list
        [HttpPost("showPdf")]
        public ActionResult<ResponseByteArray?> showPdf([FromForm] int izinTalepId)
        {

            BLLActions.AnnualLeaveTable bllAnnualLeaveTable = new BLLActions.AnnualLeaveTable(_configuration, _env, _mapper);
            ResponseByteArray? liste = bllAnnualLeaveTable.showPdf(izinTalepId);
            return Ok(liste);
        }
        #endregion

        #region onayla
        [HttpPost("onayla")]
        public async Task<ActionResult<int>> onayla([FromForm] int id)
        {
            BLLActions.AnnualLeaveTable bllAnnualLeaveTable = new BLLActions.AnnualLeaveTable(_configuration, _env, _mapper);
            int userId = GetCurrentUserId();
            if (userId <= 0)
            {
                return Unauthorized();
            }
            int deger = await bllAnnualLeaveTable.approve(id, userId);
            return Ok(deger);
        }
        #endregion

        #region reject
        [HttpPost("reject")]
        public async Task<ActionResult<int>> reject([FromForm] int id)
        {
            BLLActions.AnnualLeaveTable bllAnnualLeaveTable = new BLLActions.AnnualLeaveTable(_configuration, _env, _mapper);
            int userId = GetCurrentUserId();
            if (userId <= 0)
            {
                return Unauthorized();
            }
            int deger = await bllAnnualLeaveTable.reject(id, userId);
            return Ok(deger);
        }
        #endregion

        #region onaylaIK
        [HttpPost("onaylaIK")]
        public async Task<ActionResult<int>> onaylaIK([FromForm] int id)
        {
            BLLActions.AnnualLeaveTable bllAnnualLeaveTable = new BLLActions.AnnualLeaveTable(_configuration, _env, _mapper);
            int userId = GetCurrentUserId();
            if (userId <= 0)
            {
                return Unauthorized();
            }
            int deger = await bllAnnualLeaveTable.onaylaIK(id, userId);
            return Ok(deger);
        }
        #endregion

        private int GetCurrentUserId()
        {
            string? value =
                User.FindFirst("userId")?.Value ??
                User.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
                User.FindFirst("sub")?.Value;

            return int.TryParse(value, out int userId) ? userId : 0;
        }

    }
}
