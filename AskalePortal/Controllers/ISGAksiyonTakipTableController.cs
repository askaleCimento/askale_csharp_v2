using AskalePortal.BLL;
using AskalePortal.Data.Models;
using AskalePortal.Data.ResponseModels;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AskalePortal.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ISGAksiyonTakipTableController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;
        public ISGAksiyonTakipTableController(IConfiguration configuration, IWebHostEnvironment env, IMapper mapper)
        {
            _configuration = configuration;
            _env = env;
            _mapper = mapper;
        }
        #region listbyaksiyonid
        [HttpPost("listbyaksiyonid")]
        public ActionResult<List<ISGAksiyonTakipTable>> listbyaksiyonid([FromForm] int aksiyonId)
        {
            BLLActions.ISGAksiyonTakipTable bllISGAksiyonTakipTable = new BLLActions.ISGAksiyonTakipTable(_configuration, _env,_mapper);
            List<ISGAksiyonTakipTable> liste = bllISGAksiyonTakipTable.listAllByAksiyonIdAndEnabled(aksiyonId, true);
            return Ok(liste);

        }
        #endregion

        #region saveTakip
        [HttpPost("saveTakip")]
        public async Task<ActionResult<ISGAksiyonTakipTableSaveDto>> saveTakip([FromForm] ISGAksiyonTakipTableDto isgAksiyonDto)
        {
            BLLActions.ISGAksiyonTakipTable bllISGAksiyonTakipTable = new BLLActions.ISGAksiyonTakipTable(_configuration, _env, _mapper);
            int userId = 0;
            if (HttpContext.User.Identity is ClaimsIdentity claimsIdentity)
            {
                userId = int.Parse(claimsIdentity?.FindFirst("userId")?.Value ?? "0");

            }
            ISGAksiyonTakipTableSaveDto saved = await bllISGAksiyonTakipTable.saveTakip(isgAksiyonDto.isgAksiyonTakipTable, isgAksiyonDto.sapBildirim, userId);
            return Ok(saved);

        }
        #endregion

    }
}
