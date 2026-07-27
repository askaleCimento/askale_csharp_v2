using AskalePortal.BLL;
using AskalePortal.Data.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace AskalePortal.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BolumUserHierarchyController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;
        public BolumUserHierarchyController(IConfiguration configuration, IWebHostEnvironment env, IMapper mapper)
        {
            _configuration = configuration;
            _env = env;
            _mapper = mapper;
        }
        #region getbyuserId
        [HttpPost("getbyuserId")]
        public ActionResult<object> getbyuserId()
        {
            BLLActions.CeoTable bllCeoTable = new BLLActions.CeoTable(_configuration, _env);
            Data.Models.CeoTable? ceoTable = bllCeoTable.GetByID(1);
            BLLActions.BolumUserHierarchyTable bllBolumUserHierarchyTable = new BLLActions.BolumUserHierarchyTable(_configuration, _env);
            List<BolumUserHierarchyTable> list = [];
            if (ceoTable != null)
            {
                list = bllBolumUserHierarchyTable.GetByUserId(ceoTable!.userId);
            }

            return Ok(list);
        }
        #endregion
        #region getbymanagerid
        [HttpPost("getbymanagerid")]
        public ActionResult<object> getbymanagerid([FromForm] int managerId)
        {
            BLLActions.BolumUserHierarchyTable bllBolumUserHierarchyTable = new BLLActions.BolumUserHierarchyTable(_configuration, _env);
            List<BolumUserHierarchyTable> liste = bllBolumUserHierarchyTable.getbymanagerid(true, managerId);
            return Ok(liste);
        }
        #endregion

    }
}
