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
    public class HelpDeskTypeController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;
        public HelpDeskTypeController(IConfiguration configuration, IWebHostEnvironment env, IMapper mapper)
        {
            _configuration = configuration;
            _env = env;
            _mapper = mapper;
        }


        #region getById
        [HttpPost("getById")]

        public ActionResult<object> getById([FromForm] int id)
        {
            BLL.BLLActions.HelpDeskTypes bllHelpDeskTypes = new BLL.BLLActions.HelpDeskTypes(_configuration, _env);

            HelpDeskType? helpDeskType = bllHelpDeskTypes.GetByID(id);
            if (helpDeskType == null)
            {
                return NotFound();
            }
            return Ok(helpDeskType);

        }
        #endregion
        
        #region getAll
        [HttpPost("getAll")]
        public ActionResult<object> getAll()
        {
            BLL.BLLActions.HelpDeskTypes bllHelpDeskType = new BLL.BLLActions.HelpDeskTypes(_configuration, _env);

            List<HelpDeskType>? listHelpDeskType = bllHelpDeskType.GetAll();
            return Ok(listHelpDeskType);

        }
        #endregion

        #region getAllFilter
        [HttpPost("getAllFilter")]
        public ActionResult<object> getAllFilter([FromForm] FilterParam<HelpDeskStatusListDtoParameter> filterParam)
        {
            BLL.BLLActions.HelpDeskTypes bllHelpDeskType = new BLL.BLLActions.HelpDeskTypes(_configuration, _env);

            List<HelpDeskType> listHelpDeskType = bllHelpDeskType.getAllFilter(filterParam);
            return Ok(listHelpDeskType);

        }
        #endregion



        #region Save
        [HttpPost("save")]

        public async Task<ActionResult<HelpDeskTypeSaveDto?>> save([FromForm] HelpDeskTypeSaveDto entity)
        {

            if (entity != null)
            {
                int userId = 0;
                if (HttpContext.User.Identity is ClaimsIdentity claimsIdentity)
                {
                    userId = int.Parse(claimsIdentity?.FindFirst("userId")?.Value ?? "0");

                }
                BLLActions.HelpDeskTypes bllHelpDeskTypes= new BLLActions.HelpDeskTypes(_configuration, _env);

                if (entity?.id != null)
                {

                    entity!.updateDate = DateTime.Now.ToString();
                    entity.updatedUserId = userId == 0 ? null : userId;
                    await bllHelpDeskTypes.Update(_mapper.Map<Data.Models.HelpDeskType>(entity));
                    return Ok(entity);
                }
                else
                {

                    entity!.createdDate = DateTime.Now.ToString();
                    entity.createdUserId = userId;
                    entity.enabled = true;
                    HelpDeskType? type = await bllHelpDeskTypes.Add(_mapper.Map<Data.Models.HelpDeskType>(entity));
                    return Ok(_mapper.Map<HelpDeskType>(type));
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
                BLLActions.HelpDeskTypes bllHelpDeskTypes = new BLLActions.HelpDeskTypes(_configuration, _env);
                bllHelpDeskTypes.Delete(id);
                return Ok(1);
            }
            catch
            {
                return Ok(0);
            }

        }
        #endregion
    }
}
