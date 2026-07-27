using AskalePortal.BLL;
using AskalePortal.Data.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AskalePortal.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HRExpenseTripDetailController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        public HRExpenseTripDetailController(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _env = env;
        }

        #region getByTripId
        [HttpPost("getByTripId")]
        public ActionResult<object> getByTripId([FromForm] int tripId)
        {
            BLLActions.HRExpenseTripDetail bllHRExpenseTripDetail = new BLLActions.HRExpenseTripDetail(_configuration, _env);

            List<HRExpenseTripDetail> list = bllHRExpenseTripDetail.getByTripId(tripId);
            
            return Ok(list);

        }
        #endregion

    }
}
