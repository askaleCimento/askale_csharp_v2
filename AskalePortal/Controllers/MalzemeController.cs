using AskalePortal.BLL;
using AskalePortal.Data.SAP.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AskalePortal.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MalzemeController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;
        public MalzemeController(IConfiguration configuration, IWebHostEnvironment env, IMapper mapper)
        {
            _configuration = configuration;
            _env = env;
            _mapper = mapper;
        }

        [HttpPost("getMalzemeler")]
        public ActionResult<List<MalzemeTuru>> getMalzemeler([FromForm]string werks)
        {
            BLLActions.MalzemeTuru bllMalzemeTuru = new BLLActions.MalzemeTuru(_configuration, _env,_mapper);
            List<MalzemeTuru>? listMalzemeTuru = bllMalzemeTuru.GetAllFromSAPMalzemeTuru(werks);
            return Ok(listMalzemeTuru ?? []);
        }

    }
}
