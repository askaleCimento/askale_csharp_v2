using AskalePortal.BLL;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AskalePortal.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ISGAksiyonTableController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;

        public ISGAksiyonTableController(IConfiguration configuration, IWebHostEnvironment env, IMapper mapper)
        {
            _configuration = configuration;
            _env = env;
            _mapper = mapper;
        }
        #region approvalCount
        [HttpPost("approvalCount")]
        public ActionResult<int> approvalCount([FromForm] int userId)
        {
            
            BLLActions.ISGAksiyonTable bllISGAksiyonTable = new BLLActions.ISGAksiyonTable(_configuration, _env);
            int count = bllISGAksiyonTable.approvalCount(userId);
            return Ok(count);
        }
        #endregion
    }
}
