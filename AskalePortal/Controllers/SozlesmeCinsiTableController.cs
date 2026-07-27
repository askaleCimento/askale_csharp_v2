using AskalePortal.BLL;
using AskalePortal.Data.Models;
using AskalePortal.Data.ResponseModels;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AskalePortal.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SozlesmeCinsiTableController : ControllerBase
    {
        private readonly IConfiguration _configuration; private readonly IWebHostEnvironment _env;

        public SozlesmeCinsiTableController(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _env = env;
        }


        [HttpPost("getAll")]
        public ActionResult<object> getAll()
        {
            BLLActions.SozlesmeCinsiTable bllSozlesmeCinsiTable = new BLLActions.SozlesmeCinsiTable(_configuration, _env);

            List<SozlesmeCinsiTable> list = bllSozlesmeCinsiTable.GetAll();
            return Ok(list);

        }
    }
}
