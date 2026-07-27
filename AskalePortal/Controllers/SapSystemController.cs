using AskalePortal.Data.Contracts.Detached;
﻿using AskalePortal.BLL;
using AskalePortal.Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AskalePortal.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SapSystemController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        public SapSystemController(IConfiguration configuration, IWebHostEnvironment env)
        {

            _env = env;
            _configuration = configuration;
        }

        [HttpGet("getSystem")]

        public ActionResult<object> getSystem()
        {
            BLLActions.Configs bllCongfigs = new BLLActions.Configs(_configuration, _env);

            return bllCongfigs.GetFirst();
        }

        [HttpPost("save")]

        public async Task<ActionResult<object>> save([FromForm] ConfigDto request, [FromForm] int userId)
        {
            Config config = request.ToEntity<Config>();
            BLLActions.Configs bllCongfigs = new BLLActions.Configs(_configuration, _env);

            return await bllCongfigs.save(config, userId);
        }
    }
}
