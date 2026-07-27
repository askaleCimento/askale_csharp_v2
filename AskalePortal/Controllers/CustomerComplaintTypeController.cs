using AskalePortal.BLL;
using AskalePortal.Data.Models;
using AskalePortal.Data.ResponseModels;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AskalePortal.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerComplaintTypeController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;
        public CustomerComplaintTypeController(IConfiguration configuration, IWebHostEnvironment env, IMapper mapper)
        {
            _configuration = configuration;
            _env = env;
            _mapper = mapper;
        }
        [HttpPost("getByCategoryId")]
        public ActionResult<List<MusteriSikayetTipiSaveDto>> filterPageable([FromForm] int categoryId)
        {
            BLLActions.MusteriSikayetTipi bllMusteriSikayetTipi = new BLLActions.MusteriSikayetTipi(_configuration, _env);
            List<MusteriSikayetTipiSaveDto> liste = bllMusteriSikayetTipi.getByCategoryId(categoryId);
            return Ok(liste);
        }
        #region getAll
        [HttpPost("getAll")]

        public ActionResult<List<MusteriSikayetTipiSaveDto>> getAll()
        {
            BLLActions.MusteriSikayetTipi bllMusteriSikayetTipi = new BLLActions.MusteriSikayetTipi(_configuration, _env);

            List<MusteriSikayetTipiSaveDto>? listCustomerComplaintTipi = bllMusteriSikayetTipi.GetAllDto();
            return Ok(listCustomerComplaintTipi);

        }
        #endregion


        #region Save
        [HttpPost("save")]

        public async Task<ActionResult<MusteriSikayetTipiSaveDto?>> save([FromForm] MusteriSikayetTipiSaveDto entity)
        {

            if (entity != null)
            {
                int userId = 0;
                if (HttpContext.User.Identity is ClaimsIdentity claimsIdentity)
                {
                    userId = int.Parse(claimsIdentity?.FindFirst("userId")?.Value ?? "0");

                }
                BLLActions.MusteriSikayetTipi bllMusteriSikayetTipi = new BLLActions.MusteriSikayetTipi(_configuration, _env);

                if (entity?.id != 0)
                {

                    entity!.updateDate = DateTime.Now.ToString();
                    entity.updatedUserId = userId == 0 ? null : userId;
                    await bllMusteriSikayetTipi.Update(_mapper.Map<Data.Models.MusteriSikayetTipi>(entity));
                    return Ok(entity);
                }
                else
                {

                    entity.createdDate = DateTime.Now.ToString();
                    entity.createdUserId = userId;
                    entity.enabled = true;
                    await bllMusteriSikayetTipi.Add(_mapper.Map<Data.Models.MusteriSikayetTipi>(entity));
                    return Ok(entity);
                }
            }
            return Ok(null);
        }
        #endregion

    }
}
