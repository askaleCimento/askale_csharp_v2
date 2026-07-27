using AskalePortal.Data.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AskalePortal.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HRIslemTuruTableController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;
        public HRIslemTuruTableController(IConfiguration configuration, IWebHostEnvironment env, IMapper mapper)
        {
            _configuration = configuration;
            _env = env;
            _mapper = mapper;
        }

        #region getAll
        [HttpPost("getAll")]

        public ActionResult<object> getAll()
        {
            BLL.BLLActions.HRIslemTuruTable bllHRIslemTuruTable = new BLL.BLLActions.HRIslemTuruTable(_configuration, _env);

            List<HRIslemTuruTable>? listHRIslemTuruTable = bllHRIslemTuruTable.GetAll();
            return Ok(listHRIslemTuruTable);

        }
        #endregion

    }
}
