using AskalePortal.BLL;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AskalePortal.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SapıslemleriController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        public SapıslemleriController(IConfiguration configuration, IWebHostEnvironment env)
        {

            _env = env;
            _configuration = configuration;
        }

        [HttpPost("changePassword")]
        public ActionResult<string?> changePassword([FromForm] string username, [FromBody] string password, [FromForm] string islock)
        {
            BLLActions.SAPUSERS bllSapUser = new BLLActions.SAPUSERS(_configuration, _env);
            return bllSapUser.ChangeUserPassword(username, password,islock);
        }
    }
}
