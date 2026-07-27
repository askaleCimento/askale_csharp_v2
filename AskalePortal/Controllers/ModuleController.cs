using AskalePortal.BLL;
using AskalePortal.Data.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AskalePortal.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ModuleController : ControllerBase
    {
        private readonly IWebHostEnvironment _env; private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        public ModuleController(IWebHostEnvironment env, IConfiguration configuration, IMapper mapper)
        {
            _configuration = configuration;
            _env = env;
            _mapper = mapper;
        }
        [HttpPost("getAll")]
        public ActionResult<object> getAll()
        {
            BLLActions.Module bllModule = new BLLActions.Module(_configuration, _env);

            List<Module> list = bllModule.GetAll();
            return Ok(list);

        }
    }
}
