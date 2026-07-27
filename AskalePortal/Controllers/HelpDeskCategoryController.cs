using AskalePortal.BLL;
using AskalePortal.Data.Models;
using AskalePortal.Data.RequestModel;
using AskalePortal.Data.RequestParams;
using AskalePortal.Data.ResponseModels;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AskalePortal.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HelpDeskCategoryController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        public HelpDeskCategoryController(IWebHostEnvironment env, IConfiguration configuration, IMapper mapper)
        {
            _configuration = configuration;
            _env = env;
            _mapper = mapper;
        }


        #region Save
        [HttpPost("save")]
        public async Task<ActionResult<HelpDeskCategorySaveDto?>> save([FromForm] HelpDeskCategorySaveDto entity)
        {

            if (entity != null)
            {
                int userId = 0;
                if (HttpContext.User.Identity is ClaimsIdentity claimsIdentity)
                {
                    userId = int.Parse(claimsIdentity?.FindFirst("userId")?.Value ?? "0");

                }
                BLLActions.HelpDeskCategories bllHelpDeskCategory = new BLLActions.HelpDeskCategories(_configuration, _env);

                if (entity?.id != null)
                {

                    entity.updateDate = DateTime.Now;
                    entity.updatedUserId = userId == 0 ? null : userId;
                    await bllHelpDeskCategory.Update(_mapper.Map< HelpDeskCategory >(entity));
                    return Ok(entity);
                }
                else
                {

                    entity!.createdDate = DateTime.Now;
                    entity.createdUserId = userId == 0 ? null : userId; ;
                    entity.enabled = true;
                    HelpDeskCategory? kayit = await bllHelpDeskCategory.Add(_mapper.Map<HelpDeskCategory>(entity));
                    return Ok(_mapper.Map<HelpDeskCategory>(kayit));
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
                BLLActions.HelpDeskCategories bllHelpDeskCategory = new BLLActions.HelpDeskCategories(_configuration, _env);
                bllHelpDeskCategory.Delete(id);
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
            BLLActions.HelpDeskCategories bllHelpDeskCategory = new BLLActions.HelpDeskCategories(_configuration, _env);

            HelpDeskCategory? helpDeskCategory = bllHelpDeskCategory.GetByID(id);
            if (helpDeskCategory == null)
            {
                return NotFound();
            }
            return Ok(helpDeskCategory);


        }
        #endregion


        #region getAll
        [HttpPost("getAll")]
        public ActionResult<object> getAll()
        {
            BLLActions.HelpDeskCategories bllHelpDeskCategory = new BLLActions.HelpDeskCategories(_configuration, _env);

            List<HelpDeskCategory>? listHelpDeskCategory = bllHelpDeskCategory.GetAll();
            return Ok(listHelpDeskCategory);

        }
        #endregion

        #region getAllFilter
        [HttpPost("getAllFilter")]
        public ActionResult<List<HelpDeskCategorySaveDto>> getAllFilter([FromForm] FilterParam<HelpDeskStatusListDtoParameter> filterParam)
        {
            BLLActions.HelpDeskCategories bllHelpDeskCategory = new BLLActions.HelpDeskCategories(_configuration, _env);

            List<HelpDeskCategorySaveDto>? listHelpDeskCategory = bllHelpDeskCategory.GetAllFilter(filterParam);
            return Ok(listHelpDeskCategory);

        }
        #endregion

        #region getAllCategoryName
       
         [HttpGet("getAllCategoryName")]
        public ActionResult<object> getAllNameAndId()
        {
            BLLActions.HelpDeskCategories bllHelpDeskCategory = new BLLActions.HelpDeskCategories(_configuration, _env);

            List<HelpDeskCategory> listCategory = bllHelpDeskCategory.GetByTopID(-1);
            return Ok(listCategory);

        }
        #endregion
    }
}