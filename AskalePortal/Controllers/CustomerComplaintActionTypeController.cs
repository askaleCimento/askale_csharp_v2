using AskalePortal.BLL;
using AskalePortal.Data.Models;
using AskalePortal.Data.ResponseModels;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AskalePortal.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerComplaintActionTypeController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;
        public CustomerComplaintActionTypeController(IConfiguration configuration, IWebHostEnvironment env, IMapper mapper)
        {
            _configuration = configuration;
            _env = env;
            _mapper = mapper;
        }
        #region getAll
        [HttpPost("getAll")]

        public ActionResult<object> getAll()
        {
            BLLActions.MusteriSikayetAksiyonTipi bllMusteriSikayetAksiyonTipi = new BLLActions.MusteriSikayetAksiyonTipi(_configuration, _env);

            List<MusteriSikayetAksiyonTipi>? listCustomerComplaintAksiyonTipi = bllMusteriSikayetAksiyonTipi.GetAll();
            return Ok(listCustomerComplaintAksiyonTipi);

        }
        #endregion
    }
}
