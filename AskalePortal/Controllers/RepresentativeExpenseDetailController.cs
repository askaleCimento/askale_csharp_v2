using AskalePortal.BLL;
using AskalePortal.Data.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AskalePortal.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RepresentativeExpenseDetailController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;
        public RepresentativeExpenseDetailController(IConfiguration configuration, IWebHostEnvironment env, IMapper mapper)
        {
            _configuration = configuration;
            _env = env;
            _mapper = mapper;
        }

        #region listByRepId
        [HttpPost("listByRepId")]
        public ActionResult<object> getByTripId([FromForm] int repId)
        {
            BLLActions.RepresentativeExpenseDetail bllRepresentativeExpenseDetail = new BLLActions.RepresentativeExpenseDetail(_configuration,_env);
            List<RepresentativeExpenseDetail>? liste = bllRepresentativeExpenseDetail.getByTripId(repId);
            return Ok(liste ?? []);



        }
        #endregion
    }
}
