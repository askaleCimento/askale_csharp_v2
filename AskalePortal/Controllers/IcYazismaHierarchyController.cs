using AskalePortal.BLL;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace AskalePortal.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IcYazismaHierarchyController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        public IcYazismaHierarchyController(IConfiguration configuration, IWebHostEnvironment env, IMapper mapper)
        {
            _configuration = configuration;
            _env = env;
        }
        #region getbyuserId
        [HttpPost("getbyuserId")]
        public ActionResult<object> getbyuserId()
        {
            BLLActions.IcYazismaHierarchyTable bllIcYazismaHierarchyTable = new BLLActions.IcYazismaHierarchyTable(_configuration, _env);
            List<Data.Models.IcYazismaHierarchyTable> liste = bllIcYazismaHierarchyTable.getbyuserId(true);
            return Ok(liste);
        }
        #endregion

        #region getbymanagerid
        [HttpPost("getbymanagerid")]
        public ActionResult<object> getbymanagerid([FromForm] int managerId)
        {
            BLLActions.IcYazismaHierarchyTable bllIcYazismaHierarchyTable = new BLLActions.IcYazismaHierarchyTable(_configuration, _env);
            List<Data.Models.IcYazismaHierarchyTable> liste = bllIcYazismaHierarchyTable.getbymanagerid(true, managerId);
            return Ok(liste);
        }
        #endregion

    }
}
