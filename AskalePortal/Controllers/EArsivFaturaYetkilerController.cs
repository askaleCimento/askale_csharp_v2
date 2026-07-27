using AskalePortal.BLL;
using AskalePortal.Data.ResponseModels;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AskalePortal.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EArsivFaturaYetkilerController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;

        public EArsivFaturaYetkilerController(IConfiguration configuration, IWebHostEnvironment env, IMapper mapper)
        {
            _configuration = configuration;
            _env = env;
            _mapper = mapper;
        }
        #region saveTotal 
        [HttpPost("saveTotal")]

        public async Task<ActionResult<string>> saveTotal([FromForm] int userId, [FromForm] int selectedUserId,
            [FromForm] List<int> selectedCompanyIds)
        {
            BLLActions.EArsivFaturaYetkiler bllEArsivFaturaYetkiler = new BLLActions.EArsivFaturaYetkiler(_configuration, _env, _mapper);
            string deger = await bllEArsivFaturaYetkiler.saveTotal(userId, selectedUserId, selectedCompanyIds);
            return Ok(deger);
        }
        #endregion

        #region listDtoByEnabled 
        [HttpPost("listDtoByEnabled")]
        public ActionResult<List<EArsivFaturaYetkilerResponseDto>> listDtoByEnabled()
        {
            BLLActions.EArsivFaturaYetkiler bllEArsivFaturaYetkiler = new BLLActions.EArsivFaturaYetkiler(_configuration, _env, _mapper);
            List<EArsivFaturaYetkilerResponseDto> liste = bllEArsivFaturaYetkiler.listDtoByEnabled(true);
            return Ok(liste);
        }
        #endregion

        #region deletebyuserid 
        [HttpPost("deletebyuserid")]
        public async Task<ActionResult<int>> deletebyuserid([FromForm] int userId)
        {
            BLLActions.EArsivFaturaYetkiler bllEArsivFaturaYetkiler = new BLLActions.EArsivFaturaYetkiler(_configuration, _env, _mapper);
            int deger =await bllEArsivFaturaYetkiler.deletebyuserid(userId);
            return Ok(deger);
        }
        #endregion

        #region getByUserId 
        [HttpPost("getByUserId")]
        public ActionResult<EArsivFaturaYetkilerResponseDto> getByUserId([FromForm] int userId)
        {
            BLLActions.EArsivFaturaYetkiler bllEArsivFaturaYetkiler = new BLLActions.EArsivFaturaYetkiler(_configuration, _env, _mapper);
            EArsivFaturaYetkilerResponseDto dto = bllEArsivFaturaYetkiler.getByUserId(userId);
            return Ok(dto);
        }
        #endregion

    }
}
