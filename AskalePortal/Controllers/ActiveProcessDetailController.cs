using AskalePortal.BLL;
using AskalePortal.Data.Models;
using AutoMapper;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace AskalePortal.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActiveProcessDetailController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;
        public ActiveProcessDetailController(IConfiguration configuration, IWebHostEnvironment env, IMapper mapper)
        {
            _configuration = configuration;
            _env = env;
            _mapper = mapper;
        }

        #region getbyactiveprocess
        [HttpPost("getbyactiveprocess")]
        public ActionResult<List<ActiveProcessDetail>> getbyactiveprocess([FromForm] int processId)
        {
            BLLActions.ActiveProcessDetails bllActiveProcessDetails = new BLLActions.ActiveProcessDetails(_configuration, _env);
            List<ActiveProcessDetail> liste = bllActiveProcessDetails.findAllByActiveProcessIdAndEnabled(processId, true);
            return Ok(liste);
        }
        #endregion


        #region getGuid
        [HttpPost("getGuid")]
        public ActionResult<string> getGuid([FromForm] int activeProcessId, [FromForm] int userId)
        {
            BLLActions.ActiveProcessDetails bllActiveProcessDetails = new BLLActions.ActiveProcessDetails(_configuration, _env);
            string deger = bllActiveProcessDetails.getGuid(activeProcessId, userId, true);
            return Ok(deger);
        }
        #endregion

    }
}
