using AskalePortal.Data.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace AskalePortal.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AracTalepTableDetailController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        public AracTalepTableDetailController(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _env = env;
        }
        #region getById
        [HttpPost("getById")]

        public ActionResult<object> getById([FromForm] int id)
        {
            BLL.BLLActions.AracTalepTableDetail bllAracTalepTableDetail = new BLL.BLLActions.AracTalepTableDetail(_configuration, _env);

            AracTalepTableDetail? aracTalepTableDetail = bllAracTalepTableDetail.GetByID(id);
            if (aracTalepTableDetail == null)
            {
                return NotFound();
            }
            return Ok(aracTalepTableDetail);


        }
        #endregion
        #region listByTalepId
        [HttpPost("listByTalepId")]

        public ActionResult<object> getByTripId([FromForm] int talepId)
        {
            BLL.BLLActions.AracTalepTableDetail bllAracTalepTableDetail = new BLL.BLLActions.AracTalepTableDetail(_configuration, _env);
            List<AracTalepTableDetail> liste = bllAracTalepTableDetail.getByTalepId(talepId);
            return Ok(liste);
        }
        #endregion

    }
}
