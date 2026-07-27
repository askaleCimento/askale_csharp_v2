using AskalePortal.BLL;
using AskalePortal.Data.Models;
using AskalePortal.Data.RequestParams;
using AskalePortal.Data.ResponseModels;
using AskalePortal.Data.ResponseParams;
using AutoMapper;
using Azure;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AskalePortal.API.Controllers
{

    [Route("api/[controller]")]
    [ApiController]

    public class CustomerComplaintCategoryController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;
        public CustomerComplaintCategoryController(IConfiguration configuration, IWebHostEnvironment env, IMapper mapper)
        {
            _configuration = configuration;
            _env = env;
            _mapper = mapper;
        }
        //    #region filterByPageable
        //    [HttpPost("filterByPageable")]
        //public ActionResult<PageReturn<CustomerComplaintCategoryDto>> filterPageable(
        //        [FromForm] FilterPageParam filterPageParam)
        //    {

        //        BLLActions.CustomerComplaintCategory bllMusteriSikayetCategory = new BLLActions.CustomerComplaintCategory(_configuration,_env);
        //        PageReturn<CustomerComplaintCategoryDto> page = bllMusteriSikayetCategory.listByPageable(filterPageParam);
        //        return Ok(page);
        //    }
        //    #endregion

        #region Save
        [HttpPost("save")]
        public async Task<ActionResult<MusteriSikayetCategorySaveDto?>> save([FromForm] MusteriSikayetCategorySaveDto entity)
        {
            

            if (entity != null)
            {
                int userId = 0;
                if (HttpContext.User.Identity is ClaimsIdentity claimsIdentity)
                {
                    userId = int.Parse(claimsIdentity?.FindFirst("userId")?.Value ?? "0");

                }
                BLLActions.MusteriSikayetCategory bllMusteriSikayetCategory = new BLLActions.MusteriSikayetCategory(_configuration, _env);

                if (entity?.id != null)
                {

                    entity.updateDate = DateTime.Now.ToString();
                    entity.updatedUserId = userId == 0 ? null : userId;
                    await bllMusteriSikayetCategory.Update(_mapper.Map<MusteriSikayetCategory>(entity));

                    return Ok(entity);
                }
                else
                {

                    entity!.createdDate = DateTime.Now.ToString();
                    entity.createdUserId = userId == 0 ? null : userId; ;
                    entity.enabled = true;
                    var saveData = await bllMusteriSikayetCategory.Add(_mapper.Map<MusteriSikayetCategory>(entity));
                    return Ok(saveData);
                }
            }
            return Ok(null);
        }
        #endregion

        #region delete
        [HttpPost("delete")]

        public ActionResult<int> delete([FromForm] int id)
        {
            try
            {
                BLLActions.MusteriSikayetCategory bllMusteriSikayetCategory = new BLLActions.MusteriSikayetCategory(_configuration, _env);
                bllMusteriSikayetCategory.Delete(id);
                return Ok(1);
            }
            catch
            {
                return Ok(0);
            }

        }
        #endregion

        #region getById
        [HttpPost("getById")]

        public ActionResult<object> getById([FromForm] int id)
        {
            BLLActions.MusteriSikayetCategory bllMusteriSikayetCategory = new BLLActions.MusteriSikayetCategory(_configuration, _env);

            MusteriSikayetCategory? category = bllMusteriSikayetCategory.GetByID(id);
            if (category == null)
            {
                return NotFound();
            }
            return Ok(category);


        }
        #endregion


        #region getAll
        [HttpPost("getAll")]

        public ActionResult<object> getAll()
        {
            BLLActions.MusteriSikayetCategory bllMusteriSikayetCategory = new BLLActions.MusteriSikayetCategory(_configuration, _env);

            List<MusteriSikayetCategory>? listCustomerComplaintCategory = bllMusteriSikayetCategory.GetAll();
            return Ok(listCustomerComplaintCategory);

        }
        #endregion

    }
}
