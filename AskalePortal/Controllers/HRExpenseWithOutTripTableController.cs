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
    public class HRExpenseWithOutTripTableController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;
        public HRExpenseWithOutTripTableController(IConfiguration configuration, IWebHostEnvironment env, IMapper mapper)
        {
            _configuration = configuration;
            _env = env;
            _mapper = mapper;
        }

        #region getById
        [HttpPost("getById")]

        public ActionResult<object> getById([FromForm] int id)
        {
            BLLActions.HRExpenseWithOutTripTable bllHRExpenseWithOutTripTable = new BLLActions.HRExpenseWithOutTripTable(_configuration, _env, _mapper);

            HRExpenseWithOutTripTable? hrExpenseWithOutTripTable = bllHRExpenseWithOutTripTable.GetByID(id);
            if (hrExpenseWithOutTripTable == null)
            {
                return NotFound();
            }
            return Ok(hrExpenseWithOutTripTable);


        }

        #region Save
        [HttpPost("save")]
        public  async Task<ActionResult<object>> save([FromForm] HRExpenseWithOutTripTableSaveDto entity)
        {

            if (entity != null)
            {
                int userId = 0;
                if (HttpContext.User.Identity is ClaimsIdentity claimsIdentity)
                {
                    userId = int.Parse(claimsIdentity?.FindFirst("userId")?.Value ?? "0");

                }
                BLLActions.HRExpenseWithOutTripTable bllHRExpenseWithOutTripTable = new BLLActions.HRExpenseWithOutTripTable(_configuration, _env, _mapper);

                if (entity?.id != 0)
                {

                    entity!.updateDate = DateTime.Now.ToString();
                    entity.updatedUserId = userId == 0 ? null : userId;
                  await  bllHRExpenseWithOutTripTable.Update((_mapper.Map<HRExpenseWithOutTripTable>(entity)));
                    return Ok(entity);
                }
                else
                {

                    entity!.createdDate = DateTime.Now.ToString();
                    entity.createdUserId = userId == 0 ? null : userId; ;
                    entity.enabled = true;
                   await bllHRExpenseWithOutTripTable.Add((_mapper.Map<HRExpenseWithOutTripTable>(entity)));
                    return Ok(entity);
                }
            }
            return Ok(null);
        }
        #endregion


        #endregion
        #region finishedforexpense
        [HttpPost("finishedforexpense")]
        public ActionResult<object> finishedforexpense([FromForm] int userId)
        {
            BLLActions.HRExpenseWithOutTripTable bllHRExpenseWithOutTripTable = new BLLActions.HRExpenseWithOutTripTable(_configuration, _env, _mapper);
            List<HRExpenseWithOutTripTable> liste = bllHRExpenseWithOutTripTable.getFinishedForExpense(userId);
            return Ok(liste);
        }
        #endregion

        #region completed
        [HttpPost("completed")]
        public ActionResult<object> listCompleted([FromForm] FilterPageParam<HRExpenseWithOutTripTableDtoParameter> filterPageParam)
        {
            BLLActions.HRExpenseWithOutTripTable bllHRExpenseWithOutTripTable = new BLLActions.HRExpenseWithOutTripTable(_configuration, _env, _mapper);

            PageReturn<HRExpenseWithOutTripTable> liste = bllHRExpenseWithOutTripTable.listCompleted(filterPageParam);
            return Ok(liste);
        }
        #endregion

        #region myListdto
        [HttpPost("myListdto")]
        public ActionResult<PageReturn<HRExpenseTripDto>> myList([FromForm] FilterPageParam<DieselPriceListDtoParameter> filterPageParam)
        {
            BLLActions.HRExpenseWithOutTripTable bllHRExpenseWithOutTripTable = new BLLActions.HRExpenseWithOutTripTable(_configuration, _env, _mapper);
            PageReturn<HRExpenseTripDto> liste = bllHRExpenseWithOutTripTable.mylist(filterPageParam);

            return Ok(liste);
        }
        #endregion

        #region myListApprovealStatusdto
        [HttpPost("myListApprovealStatusdto")]
        public ActionResult<PageReturn<HRExpenseTripDto>> myListAprovalStatus([FromForm] FilterPageParam<HRExpenseWitOutTripTableMyListParameter> filterPageParam)
        {
            BLLActions.HRExpenseWithOutTripTable bllHRExpenseWithOutTripTable = new BLLActions.HRExpenseWithOutTripTable(_configuration, _env, _mapper);
            PageReturn<HRExpenseTripDto> liste = bllHRExpenseWithOutTripTable.mylistAprovalStatus(filterPageParam);
            return Ok(liste);
        }
        #endregion

        #region filterPageable
        [HttpPost("filterPageable")]
        public ActionResult<PageReturn<HRExpenseWithOutTripTableSaveDto>> listPageable([FromForm] FilterPageParam<HRExpenseWithOutTripTableFilterDtoRequest> filterPageParam)
        {
            BLLActions.HRExpenseWithOutTripTable bllHRExpenseWithOutTripTable = new BLLActions.HRExpenseWithOutTripTable(_configuration, _env, _mapper);

            PageReturn<HRExpenseWithOutTripTableSaveDto> liste = bllHRExpenseWithOutTripTable.listPageable(filterPageParam);
            return Ok(liste);
        }
        #endregion

        #region activeListdto
        [HttpPost("activeListdto")]
        public ActionResult<PageReturn<HRExpenseTripDto>> activeList([FromForm] FilterPageParam<HRExpenseWithOutTripTableActiveListDtoRequest> filterPageParam)
        {
            BLLActions.HRExpenseWithOutTripTable bllHRExpenseWithOutTripTable = new BLLActions.HRExpenseWithOutTripTable(_configuration, _env, _mapper);
            PageReturn<HRExpenseTripDto> dto = bllHRExpenseWithOutTripTable.activelist(filterPageParam);
            return Ok(dto);
        }
        #endregion
    }
}
