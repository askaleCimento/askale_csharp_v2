using AskalePortal.Data.Models;
using AskalePortal.Data.RequestModel;
using AskalePortal.Data.RequestParams;
using AskalePortal.Data.ResponseModels;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using System.Collections.Generic;
using System.Security.Claims;

namespace AskalePortal.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FuelPriceDifferenceController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;
        public FuelPriceDifferenceController(IConfiguration configuration, IWebHostEnvironment env, IMapper mapper)
        {
            _configuration = configuration;
            _env = env;
            _mapper = mapper;
        }

        #region delete
        [HttpPost("delete")]
        public ActionResult<int> delete([FromForm] int id)
        {
            try
            {
                BLL.BLLActions.FuelPriceDifference bllFuelPriceDifference = new BLL.BLLActions.FuelPriceDifference(_configuration, _env, _mapper);
                bllFuelPriceDifference.Delete(id);
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
            BLL.BLLActions.FuelPriceDifference bllFuelPriceDifference = new BLL.BLLActions.FuelPriceDifference(_configuration, _env, _mapper);

            FuelPriceDifference? fuelPriceDifference = bllFuelPriceDifference.GetByID(id);
            if (fuelPriceDifference == null)
            {
                return NotFound();
            }
            return Ok(fuelPriceDifference);


        }
        #endregion

        #region approvalCount
        [HttpPost("approvalCount")]
        public ActionResult<int> myApprovalCount([FromForm] int userId)
        {
            BLL.BLLActions.FuelPriceDifference bllFuelPriceDifference = new BLL.BLLActions.FuelPriceDifference(_configuration, _env, _mapper);
            int count = bllFuelPriceDifference.approvalCount(userId);
            return Ok(count);
        }
        #endregion

        #region active
        [HttpPost("active")]
        public ActionResult<object> listActive([FromForm] int userId)
        {
            BLL.BLLActions.FuelPriceDifference bllFuelPriceDifference = new BLL.BLLActions.FuelPriceDifference(_configuration, _env, _mapper);
            List<FuelPriceDifference> liste = bllFuelPriceDifference.listActive(userId);
            return Ok(liste);
        }
        #endregion

        #region activemyapprovallist
        [HttpPost("activemyapprovallist")]
        public ActionResult<object> activeMyApprovalList([FromForm] FilterParam<FuelPriceDifferenceListDtoParameter> filterParam)
        {
            BLL.BLLActions.FuelPriceDifference bllFuelPriceDifference = new BLL.BLLActions.FuelPriceDifference(_configuration, _env, _mapper);
            List<FuelPriceDifference> list = bllFuelPriceDifference.activeMyApprovalList(filterParam);
            return Ok(list);
        }
        #endregion

        #region completed
        [HttpPost("completed")]
        public ActionResult<object> listCompleted([FromForm] int userId)
        {
            BLL.BLLActions.FuelPriceDifference bllFuelPriceDifference = new BLL.BLLActions.FuelPriceDifference(_configuration, _env, _mapper);
            List<FuelPriceDifference> list = bllFuelPriceDifference.listCompleted(userId);

            return Ok(list);
        }
        #endregion

        #region saveSozlesmeBitis
        [HttpPost("saveSozlesmeBitis")]
        public async Task<ActionResult<object>> saveSozlesmeBitis([FromForm] Data.ResponseModels.FuelPriceDifferenceDto fuelPriceDifference)
        {
            BLL.BLLActions.FuelPriceDifference bllFuelPriceDifference = new BLL.BLLActions.FuelPriceDifference(_configuration, _env, _mapper);
            FuelPriceDifference? priceDifference =await bllFuelPriceDifference.saveSozlesmeBitisTarih(fuelPriceDifference);
            return Ok(priceDifference);
        }
        #endregion
        #region save
        [HttpPost("save")]
        public async Task<ActionResult<object>> save([FromForm] FuelPriceDifferenceDto entity)
        {
            int userId = 0;
            if (HttpContext.User.Identity is ClaimsIdentity claimsIdentity)
            {
                userId = int.Parse(claimsIdentity?.FindFirst("userId")?.Value ?? "0");

            }
            BLL.BLLActions.FuelPriceDifference bllFuelPriceDifference = new BLL.BLLActions.FuelPriceDifference(_configuration, _env, _mapper);
            FuelPriceDifference? priceDifference = await bllFuelPriceDifference.save(entity, userId);
            return Ok(priceDifference);
        }
        #endregion
        #region confirmsave
        [HttpPost("confirmsave")]
        public async Task<ActionResult<int>> confirmSave([FromForm] int id, [FromForm] int userId)
        {
            BLL.BLLActions.FuelPriceDifference bllFuelPriceDifference = new BLL.BLLActions.FuelPriceDifference(_configuration, _env, _mapper);
            int deger = await bllFuelPriceDifference.confirmSave(id, userId);
            return Ok(deger);
        }
        #endregion
    }
}
