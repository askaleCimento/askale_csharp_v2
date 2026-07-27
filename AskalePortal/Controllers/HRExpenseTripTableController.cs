using AskalePortal.BLL;
using AskalePortal.Data.Models;
using AskalePortal.Data.RequestModel;
using AskalePortal.Data.RequestParams;
using AskalePortal.Data.ResponseModels;
using AskalePortal.Data.ResponseParams;
using AutoMapper;
using Azure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using System.Collections.Generic;
using System.Security.Claims;

namespace AskalePortal.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HRExpenseTripTableController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;
        public HRExpenseTripTableController(IConfiguration configuration, IWebHostEnvironment env, IMapper mapper)
        {
            _configuration = configuration;
            _env = env;
            _mapper = mapper;
        }

        #region getById
        [HttpPost("getById")]
        public ActionResult<object> getById([FromForm] int id)
        {
            BLLActions.HRExpenseTripTable bllHRExpenseTripTable = new BLLActions.HRExpenseTripTable(_configuration, _env, _mapper);

            HRExpenseTripTable? hrExpenseTripTable = bllHRExpenseTripTable.GetByID(id);
            if (hrExpenseTripTable == null)
            {
                return NotFound();
            }
            return Ok(hrExpenseTripTable);

        }
        #endregion

        #region approvalCount
        [HttpPost("approvalCount")]
        public ActionResult<int> myApprovalCount([FromForm] int userId)
        {
            BLLActions.HRExpenseTripTable bllHRExpenseTripTable = new BLLActions.HRExpenseTripTable(_configuration, _env, _mapper);

            int deger = bllHRExpenseTripTable.approvalCount(userId);

            return Ok(deger);
        }
        #endregion

        #region save
        [HttpPost("save")]
        public async Task<ActionResult<object>> save([FromForm] HRExpenseTripTableSaveDto entity)
        {

            BLLActions.HRExpenseTripTable bllHRExpenseTripTable = new BLLActions.HRExpenseTripTable(_configuration, _env, _mapper);
            int userId = 0;
            if (HttpContext.User.Identity is ClaimsIdentity claimsIdentity)
            {
                userId = int.Parse(claimsIdentity?.FindFirst("userId")?.Value ?? "0");

            }
            HRExpenseTripTable triptable = await bllHRExpenseTripTable.save(entity, userId);

            return Ok(triptable);
        }
        #endregion
        #region active
        [HttpPost("active")]
        public ActionResult<PageReturn<HRExpenseTripTableSaveDto>> active([FromForm] FilterPageParam<HRExpenseTripTableActiveListDtoParameter> filterPageParam)
        {
            BLLActions.HRExpenseTripTable bllHRExpenseTripTable = new BLLActions.HRExpenseTripTable(_configuration, _env, _mapper);
            PageReturn<HRExpenseTripTableSaveDto> page = bllHRExpenseTripTable.listByUserIdActive(filterPageParam);
            return Ok(page);
        }
        #endregion
        #region mylist
        [HttpPost("mylist")]
        public ActionResult<PageReturn<HRExpenseTripTableSaveDto>> mylist([FromForm] FilterPageParam<HRExpenseTripTableMyListDtoParameter> filterPageParam)
        {
            BLLActions.HRExpenseTripTable bllHRExpenseTripTable = new BLLActions.HRExpenseTripTable(_configuration, _env, _mapper);
            PageReturn<HRExpenseTripTableSaveDto> page = bllHRExpenseTripTable.listByUserIdMyList(filterPageParam);

            return Ok(page);
        }
        #endregion


        #region approve
        [HttpPost("approve")]
        public async Task<ActionResult<int>> approve([FromForm] int userId, [FromForm] int tripId, [FromForm] bool approved)
        {
            BLLActions.HRExpenseTripTable bllHRExpenseTripTable = new BLLActions.HRExpenseTripTable(_configuration, _env, _mapper);
            int deger = await bllHRExpenseTripTable.approve(userId, tripId, approved);

            return Ok(deger);

        }
        #endregion
        #region completed
        [HttpPost("completed")]
        public ActionResult<PageReturn<HRExpenseTripTableSaveDto>> listCompleted([FromForm] FilterPageParam<HRExpenseTripTableCompletedListDtoParameter> filterPageParam)
        {
            BLLActions.HRExpenseTripTable bllHRExpenseTripTable = new BLLActions.HRExpenseTripTable(_configuration, _env, _mapper);
            PageReturn<HRExpenseTripTableSaveDto> page = bllHRExpenseTripTable.listCompleted(filterPageParam);
            return Ok(page);
        }
        #endregion

        #region finishedforexpense
        [HttpPost("finishedforexpense")]
        public ActionResult<object> finishedforexpense([FromForm] int userId)
        {
            BLLActions.HRExpenseTripTable bllHRExpenseTripTable = new BLLActions.HRExpenseTripTable(_configuration, _env, _mapper);
            List<HRExpenseTripTable> list = bllHRExpenseTripTable.getFinishedForExpense(userId);
            return Ok(list);
        }
        #endregion

        #region completedExpense
        [HttpPost("completedExpense")]
        public ActionResult<PageReturn<HRExpenseTripTableSaveDto>> listCompletedExpense([FromForm] FilterPageParam<HRExpenseTripTableCompletedListDtoParameter> filterPageParam)
        {
            BLLActions.HRExpenseTripTable bllHRExpenseTripTable = new BLLActions.HRExpenseTripTable(_configuration, _env, _mapper);
            PageReturn<HRExpenseTripTableSaveDto> page = bllHRExpenseTripTable.listCompletedExpense(filterPageParam);

            return Ok(page);
        }
        #endregion
        #region getAll
        [HttpPost("getAll")]
        public ActionResult<List<HRExpenseTripTableSaveDto>> listByEnabled()
        {
            BLLActions.HRExpenseTripTable bllHRExpenseTripTable = new BLLActions.HRExpenseTripTable(_configuration, _env, _mapper);
            List<HRExpenseTripTableSaveDto> liste = bllHRExpenseTripTable.listAllByEnabled(true);
            return Ok(liste);
        }
        #endregion

    }
}
