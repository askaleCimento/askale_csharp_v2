using AskalePortal.BLL;
using AskalePortal.Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AskalePortal.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnnualLeaveTypeController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        public AnnualLeaveTypeController(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _env = env;
        }

        [HttpPost("getAll")]
        public ActionResult<object> getAll()
        {
            BLLActions.AnnualLeaveType bllAnnualLeaveType=new BLLActions.AnnualLeaveType(_configuration, _env);
            return Ok(bllAnnualLeaveType.GetAll());
        }
    }
}
