using AskalePortal.BLL;
using AskalePortal.Data.Models;
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
    public class EArsivFaturaController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;

        public EArsivFaturaController(IConfiguration configuration, IWebHostEnvironment env, IMapper mapper)
        {
            _configuration = configuration;
            _env = env;
            _mapper = mapper;
        }
        #region listMyIncoices 
        [HttpPost("listMyIncoices")]
        public ActionResult<List<EArsivFaturaResponseDto>> listMyIncoices([FromForm] int userId)
        {
            BLLActions.EArsivFatura bllEArsivFatura = new BLLActions.EArsivFatura(_configuration, _env, _mapper);
            List<EArsivFaturaResponseDto> listDto = bllEArsivFatura.listMyIncoices(userId, true, false);
            return Ok(listDto);
        }
        #endregion

        #region save 
        [HttpPost("save")]
        public async Task<ActionResult<EArsivFaturaSaveDto>> saveAsync([FromForm] EArsivFaturaSaveDto entity)
        {
            BLLActions.EArsivFatura bllEArsivFatura = new BLLActions.EArsivFatura(_configuration, _env, _mapper);
            EArsivFaturaSaveDto saveDto = await bllEArsivFatura.save(entity);
            return Ok(saveDto);
        }
        #endregion

        #region finished 
        [HttpPost("finished")]
        public async Task<ActionResult<int>> finished([FromForm] string ettn)
        {

            BLLActions.EArsivFatura bllEArsivFatura = new BLLActions.EArsivFatura(_configuration, _env, _mapper);
            int deger =await bllEArsivFatura.finished(ettn);
            return Ok(deger);
        }
        #endregion

        #region listFinishedIncoices 
        [HttpPost("listFinishedIncoices")]
        public ActionResult<List<EArsivFaturaResponseDto>> listFinishedIncoices([FromForm] int userId)
        {
            BLLActions.EArsivFatura bllEArsivFatura = new BLLActions.EArsivFatura(_configuration, _env, _mapper);
            List<EArsivFaturaResponseDto> liste = bllEArsivFatura.listMyIncoices(userId, true, true);
            return Ok(liste);
        }
        #endregion

    }
}
