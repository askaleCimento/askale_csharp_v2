using AskalePortal.BLL;
using AskalePortal.Data.Models;
using AskalePortal.Data.RequestModel;
using AskalePortal.Data.RequestParams;
using AskalePortal.Data.ResponseModels;
using AskalePortal.Data.ResponseParams;
using AskalePortal.Data.SAP.Models;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static AskalePortal.BLL.BLLActions;

namespace AskalePortal.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnnualCalenderTableController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;
        public AnnualCalenderTableController(IConfiguration configuration, IWebHostEnvironment env, IMapper mapper)
        {
            _configuration = configuration;
            _env = env;
            _mapper = mapper;
        }
        [HttpPost("filterPageable")]
        public ActionResult<object> filterPageable([FromForm] FilterPageParam<AnnualCalenderDtoRequest> filterPageParam)
        {

            BLLActions.AnnualCalendarTable bllAnnualCalendarTable = new BLLActions.AnnualCalendarTable(_configuration, _env);
            PageReturn<AnnualCalenderTable> pageReturn = bllAnnualCalendarTable.filterPageable(filterPageParam);
            return pageReturn;
        }

        [HttpPost("save")]
        public async Task<ActionResult<AnnualCalenderTableSaveDto>> save([FromForm] AnnualCalenderTableSaveDto entity)
        {
            if (entity != null)
            {
                int userId = 0;
                if (HttpContext.User.Identity is ClaimsIdentity identity)
                {
                    userId = int.Parse(identity?.FindFirst("userId")?.Value ?? "0");
                }
                BLLActions.AnnualCalendarTable bllAnnualCalendarTable = new BLLActions.AnnualCalendarTable(_configuration, _env);
                if (entity?.id == null)
                {

                    entity.createdDate = DateTime.Now.ToString();
                    
                   entity.createdUserId =userId;
                    Data.Models.AnnualCalenderTable? annualCalenderTable = await bllAnnualCalendarTable.Add(_mapper.Map< Data.Models.AnnualCalenderTable > (entity));
                    return Ok(_mapper.Map< AnnualCalenderTableSaveDto > (annualCalenderTable));
                }
                else
                {
                    entity!.updateDate = DateTime.Now.ToString();
                    entity.updatedUserId = userId;
                    await bllAnnualCalendarTable.Update(_mapper.Map<Data.Models.AnnualCalenderTable>(entity));
                    return Ok(entity);
                }
            }
            return Ok(null);

        }
        #region delete
        [HttpPost("delete")]

        public ActionResult<int> delete([FromForm] int id)
        {
            try
            {
                BLLActions.AnnualCalendarTable bllAnnualCalendarTable = new BLLActions.AnnualCalendarTable(_configuration, _env);
                bllAnnualCalendarTable.Delete(id);
                return Ok(1);
            }
            catch
            {
                return Ok(0);
            }

        }
        #endregion

        [HttpPost("getById")]
        public ActionResult<object> getById([FromForm] int id)
        {

            BLLActions.AnnualCalendarTable bllAnnualCalendarTable = new BLLActions.AnnualCalendarTable(_configuration, _env);
            AnnualCalenderTable? annualLeaveCalender = bllAnnualCalendarTable.GetByID(id);
            return annualLeaveCalender;
        }
        [HttpPost("getAll")]
        public ActionResult<object> getAll()
        {

            BLLActions.AnnualCalendarTable bllAnnualCalendarTable = new BLLActions.AnnualCalendarTable(_configuration, _env);
            List<AnnualCalenderTable> listAnnualLeave = bllAnnualCalendarTable.GetAll();
            return listAnnualLeave;
        }
    }
}
