using AskalePortal.BLL;
using AskalePortal.Data.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AskalePortal.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CeoTableController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;
        public CeoTableController(IConfiguration configuration, IWebHostEnvironment env, IMapper mapper)
        {
            _configuration = configuration;
            _env = env;
            _mapper = mapper;
        }
        #region getById
        [HttpPost("getById")]
        public ActionResult<object> getById([FromForm] int id)
        {
            BLLActions.CeoTable bllCeoTable = new BLLActions.CeoTable(_configuration, _env);

            CeoTable? ceoTable = bllCeoTable.GetByID(id);
            if (ceoTable == null)
            {
                return Ok(null);
            }
            else
            {
                return Ok(ceoTable);
            }


        
        }
        #endregion
    }
}
