using AskalePortal.BLL;
using AskalePortal.Data.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace AskalePortal.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FuelPriceDifferenceDetailController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        public FuelPriceDifferenceDetailController(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _env = env;
        }
        #region listByFuelId
        [HttpPost("listByFuelId")]
        public ActionResult<object> listByFuelId([FromForm] int fuelId)
        {
            BLLActions.FuelPriceDifferenceDetail bllFuelPriceDifferenceDetail = new BLLActions.FuelPriceDifferenceDetail(_configuration, _env);
            List<FuelPriceDifferenceDetail> liste = bllFuelPriceDifferenceDetail.listByFuelId(fuelId);
            return Ok(liste);
        }
        #endregion

    }
}
