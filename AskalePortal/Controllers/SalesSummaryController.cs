using AskalePortal.BLL;
using AskalePortal.Data.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Globalization;

namespace AskalePortal.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SalesSummaryController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;
        public SalesSummaryController(IConfiguration configuration, IWebHostEnvironment env, IMapper mapper)
        {
            _configuration = configuration;
            _env = env;
            _mapper = mapper;
        }
        #region getsales
        [HttpPost("getsales")]
        public ActionResult<object> getsales([FromForm] string raporTipleri,
            [FromForm] string tarih)
        {
            DateTime dateTime = DateTime.ParseExact(
     tarih,
     "dd.MM.yyyy",
     CultureInfo.InvariantCulture
 );
            BLLActions.SatisOzet bllSatisOzet = new BLLActions.SatisOzet(_configuration, _env, _mapper);
            //List<SatisOzet> liste = bllSatisOzet.getList(raporTipleri, dateTime);
            List<SatisOzet> lliste = bllSatisOzet.GetAllFromSAP(raporTipleri, dateTime);

            return Ok(lliste);
        }
        #endregion

    }
}
