using AskalePortal.BLL;
using AskalePortal.Data.Models;
using AskalePortal.Data.ResponseModels;
using Microsoft.AspNetCore.Mvc;

namespace AskalePortal.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApprovalProcessTypeController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        public ApprovalProcessTypeController(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _env = env;
        }
        [HttpPost("getAll")]
        public ActionResult<object> listByEnabled()
        {
            BLLActions.ApprovalProcessTypes bllApprovalProcessTypes = new BLLActions.ApprovalProcessTypes(_configuration, _env);
            List<ApprovalProcessType> liste = bllApprovalProcessTypes.GetAll();
            return Ok(liste);
        }
    }
}
