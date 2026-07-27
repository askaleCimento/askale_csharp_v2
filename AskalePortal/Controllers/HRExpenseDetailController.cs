using AskalePortal.BLL;
using AskalePortal.Data.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AskalePortal.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class HRExpenseDetailController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;
        public HRExpenseDetailController(IConfiguration configuration, IWebHostEnvironment env, IMapper mapper)
        {
            _configuration = configuration;
            _env = env;
            _mapper = mapper;
        }
        #region listByTripId
        [HttpPost("listByTripId")]
        public ActionResult<object> getByTripId([FromForm] int tripId)
        {
            BLLActions.HRExpenseDetail bllHRExpenseDetail = new BLLActions.HRExpenseDetail(_configuration, _env);
            List<HRExpenseDetail> hrExpenseDetail = bllHRExpenseDetail.GetByTripId(tripId);
            return Ok(hrExpenseDetail);
        }
        #endregion
    }
}
