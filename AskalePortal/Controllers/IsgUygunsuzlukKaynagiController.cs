using AskalePortal.BLL;
using AskalePortal.Data.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AskalePortal.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IsgUygunsuzlukKaynagiController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;
        public IsgUygunsuzlukKaynagiController(IConfiguration configuration, IWebHostEnvironment env, IMapper mapper)
        {
            _configuration = configuration;
            _env = env;
            _mapper = mapper;
        }
        #region getuygunsuzlukkaynagi
        [HttpPost("getuygunsuzlukkaynagi")]
        public ActionResult<List<ISGUygunsuzlukKaynagiTable>> getuygunsuzlukkaynagi()
        {
            BLLActions.ISGUygunsuzlukKaynagiTable bllISGUygunsuzlukKaynagiTable = new BLLActions.ISGUygunsuzlukKaynagiTable(_configuration,_env,_mapper);
            int userId = 0;
            if (HttpContext.User.Identity is ClaimsIdentity claimsIdentity)
            {
                userId = int.Parse(claimsIdentity?.FindFirst("userId")?.Value ?? "0");

            }
            List<ISGUygunsuzlukKaynagiTable> listUygunsuzlukKaynagi = bllISGUygunsuzlukKaynagiTable.findAllByEnabled(true, userId);
            return Ok(listUygunsuzlukKaynagi);
        }
        #endregion
    }
}
