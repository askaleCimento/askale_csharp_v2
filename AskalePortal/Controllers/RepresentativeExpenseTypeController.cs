using AskalePortal.BLL;
using AskalePortal.Data.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AskalePortal.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RepresentativeExpenseTypeController : ControllerBase
    {
        private readonly IConfiguration _configuration; private readonly IWebHostEnvironment _env;
       
        public RepresentativeExpenseTypeController(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _env = env;
        }

        #region getAll
        [HttpPost("getAll")]
        public ActionResult<object> getAll()
        {
            BLLActions.RepresentativeExpenseType bllRepresentativeExpenseType = new BLLActions.RepresentativeExpenseType(_configuration, _env);

            List<RepresentativeExpenseType>? listRepresentativeExpenseType = bllRepresentativeExpenseType.GetAll().OrderByDescending(u => u.Id).ToList();
            return Ok(listRepresentativeExpenseType);

        }
        #endregion
    }
}
