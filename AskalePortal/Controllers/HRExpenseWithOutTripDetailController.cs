using AskalePortal.BLL;
using AskalePortal.Data.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using System.Collections.Generic;

namespace AskalePortal.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HRExpenseWithOutTripDetailController
    : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        public HRExpenseWithOutTripDetailController(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _env = env;
        }

        #region getByTripId
        [HttpPost("getByTripId")]
        public ActionResult<object> getByTripId([FromForm] int tripId, [FromForm] int userId)
        {
            BLLActions.HRExpenseWithOutDetail bllHRExpenseWithOutDetail = new BLLActions.HRExpenseWithOutDetail(_configuration, _env);
            HRExpenseWithOutDetail hRExpenseWithOutDetail = bllHRExpenseWithOutDetail.getByTripIdFinished(tripId, userId);
            return Ok(hRExpenseWithOutDetail);
        }
        #endregion

        #region listByTripId
        [HttpPost("listByTripId")]
        public ActionResult<object> getByTripId([FromForm] int tripId)
        {
            BLLActions.HRExpenseWithOutDetail bllHRExpenseWithOutDetail = new BLLActions.HRExpenseWithOutDetail(_configuration, _env);
            List<HRExpenseWithOutDetail> liste = bllHRExpenseWithOutDetail.GetByTripId(tripId);
            return Ok(liste);
        }
        #endregion

    }
}
