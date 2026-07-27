using AskalePortal.BLL;
using AskalePortal.Data.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace AskalePortal.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DieselPriceDetailController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        public DieselPriceDetailController(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _env = env;
        }
        #region listByDieselId
        [HttpPost("listByDieselId")]
        public ActionResult<object> getByTripId([FromForm] int dieselId)
        {
            BLLActions.DieselPriceDetail bllDieselPriceDetail = new BLLActions.DieselPriceDetail(_configuration, _env);
            List<DieselPriceDetail> liste = bllDieselPriceDetail.getByDieselId(true, dieselId);
            return Ok(liste);
        }
        #endregion
    }
}
