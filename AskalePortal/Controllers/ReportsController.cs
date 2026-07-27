using AskalePortal.BLL;
using AskalePortal.Data.Models;
using AskalePortal.Data.SAP.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AskalePortal.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly IWebHostEnvironment _env; private readonly IConfiguration _configuration; private readonly IMapper _mapper;
        public ReportsController(IWebHostEnvironment env, IConfiguration configuration, IMapper mapper)
        {
            _configuration = configuration;
            _env = env;
            _mapper = mapper;
        }
        [HttpPost("personel")]
        public ActionResult<List<EmployeeSap>> getPersonels([FromForm] int? pernr)
        {
            BLLActions.Personel bllPersonel = new BLLActions.Personel(_configuration, _env);
            var liste = bllPersonel.GetAllFromSAP(pernr.ToString());

            return Ok(liste);

        }
    }
}
