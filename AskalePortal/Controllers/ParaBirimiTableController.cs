using AskalePortal.BLL;
using AskalePortal.Data.Models;
using Microsoft.AspNetCore.Mvc;

namespace AskalePortal.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ParaBirimiTableController : ControllerBase
    {
        private readonly IConfiguration _configuration; private readonly IWebHostEnvironment _env;

        public ParaBirimiTableController(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _env = env;
        }
        [HttpPost("getAll")]
        public ActionResult<object> getAll()
        {
            BLLActions.ParaBirimiTable bllParaBirimiTable = new BLLActions.ParaBirimiTable(_configuration, _env);

            List<ParaBirimiTable> list = bllParaBirimiTable.GetAll();
            return Ok(list);

        }
    }
}
