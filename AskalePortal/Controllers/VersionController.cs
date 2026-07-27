
using AskalePortal.API.Security;
using AskalePortal.Data.ResponseModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AskalePortal.API.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class VersionController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        public VersionController(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _env = env;
        }
      
        [HttpPost("getVersion")]

        public ActionResult<ForceUpdateModel?> getVersion([FromForm]string version, [FromForm] int platform)
        {
        BLL.BLLActions.VersionTable bllVersionTable = new BLL.BLLActions.VersionTable(_configuration, _env);
            return Ok(bllVersionTable.getVersion(version,platform));
        }
    }
}
