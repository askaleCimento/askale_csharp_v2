using AskalePortal.BLL;
using AskalePortal.Data.Models;
using AskalePortal.Data.RequestModel;
using AskalePortal.Data.RequestParams;
using AskalePortal.Data.ResponseModels;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AskalePortal.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HelpDeskRoleController : ControllerBase
    {
        private readonly IWebHostEnvironment _env; private readonly IConfiguration _configuration; private readonly IMapper _mapper;
        public HelpDeskRoleController(IWebHostEnvironment env, IConfiguration configuration, IMapper mapper)
        {
            _configuration = configuration;
            _env = env;
            _mapper = mapper;
        }
        [HttpPost("getAll")]
        public ActionResult<object> getAll()
        {
            BLLActions.HelpDeskRoles bllHelpDeskRoles = new BLLActions.HelpDeskRoles(_configuration, _env);

            List<HelpDeskRole> roles = bllHelpDeskRoles.GetAll();
            return Ok(roles);

        }
        [HttpPost("getAllNameAndId")]
        public ActionResult<List<IdandText>> getAllNameAndId()
        {
            BLLActions.HelpDeskRoles bllHelpDeskRoles = new BLLActions.HelpDeskRoles(_configuration, _env);

            List<IdandText> list = bllHelpDeskRoles.GetIdandText();
            return Ok(list);

        }


        #region getAllFilter
        [HttpPost("getAllFilter")]
        public ActionResult<List<HelpDeskRoleSaveDto>> getAllFilter([FromForm] FilterParam<HelpDeskStatusListDtoParameter> filterParam)
        {
            BLLActions.HelpDeskRoles bllHelpDeskRoles = new BLLActions.HelpDeskRoles(_configuration, _env);

            List<HelpDeskRoleSaveDto>? listHelpDeskRole = bllHelpDeskRoles.GetAllFilter(filterParam);
            return Ok(listHelpDeskRole);

        }
        #endregion


        #region Save
        [HttpPost("save")]
        public async Task<ActionResult<HelpDeskRoleSaveDto?>> save([FromForm] HelpDeskRoleSaveDto entity)
        {

            if (entity != null)
            {
                int userId = 0;
                if (HttpContext.User.Identity is ClaimsIdentity claimsIdentity)
                {
                    userId = int.Parse(claimsIdentity?.FindFirst("userId")?.Value ?? "0");

                }
                BLLActions.HelpDeskRoles bllHelpDeskRoles = new BLLActions.HelpDeskRoles(_configuration, _env);

                if (entity?.id != null)
                {

                    entity.updateDate = DateTime.Now.ToString();
                    entity.updatedUserId = userId == 0 ? null : userId;
                    await bllHelpDeskRoles.Update(_mapper.Map<HelpDeskRole>(entity));
                    return Ok(entity);
                }
                else
                {

                    entity!.createdDate = DateTime.Now.ToString();
                    entity.createdUserId = userId == 0 ? null : userId; ;
                    entity.enabled = true;
                    HelpDeskRole? kayit = await bllHelpDeskRoles.Add(_mapper.Map<HelpDeskRole>(entity));
                    return Ok(_mapper.Map<HelpDeskRole>(kayit));
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
                BLLActions.HelpDeskRoles bllHelpDeskRoles = new BLLActions.HelpDeskRoles(_configuration, _env);
                bllHelpDeskRoles.Delete(id);
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
            BLLActions.HelpDeskRoles bllHelpDeskRoles = new BLLActions.HelpDeskRoles(_configuration, _env);

            HelpDeskRole? helpDeskRole = bllHelpDeskRoles.GetByID(id);
            if (helpDeskRole == null)
            {
                return NotFound();
            }
            return Ok(helpDeskRole);


        }
        #endregion

    }
}
