using AskalePortal.BLL;
using AskalePortal.Data.Models;
using AskalePortal.Data.RequestParams;
using AskalePortal.Data.ResponseModels;
using AskalePortal.Data.SAP.InputParams;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AskalePortal.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IWebHostEnvironment _env; 
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        public RoleController(IWebHostEnvironment env, IConfiguration configuration, IMapper mapper)
        {
            _configuration = configuration;
            _env = env;
            _mapper = mapper;
        }
        [HttpPost("getById")]
        public ActionResult<RoleDto> getById([FromForm] int id)
        {
            BLLActions.Roles bllRoles = new BLLActions.Roles(_configuration, _env, _mapper);

            RoleDto? role = _mapper.Map<RoleDto>(bllRoles.GetByID(id));
            return Ok(role);

        }

        [HttpPost("getAll")]
        public ActionResult<List<RoleDto>> getAll()
        {
            BLLActions.Roles bllRoles = new BLLActions.Roles(_configuration, _env, _mapper);

            List<RoleDto> roles = _mapper.Map<List<Role>, List<RoleDto>>(bllRoles.GetAll());
            return Ok(roles);

        }
        [HttpPost("getAllNameAndId")]
        public ActionResult<List<IdandText>> getAllNameAndId([FromForm] int userId)
        {
            BLLActions.Roles bllRoles = new BLLActions.Roles(_configuration, _env, _mapper);

            List<IdandText> list = bllRoles.GetIdandText(userId);
            return Ok(list);

        }
        [HttpPost("getAllFilter")]
        public ActionResult<List<RoleDto>> getAllFilter([FromForm] FilterParam<RoleListParameter> filterParam)
        {
            BLLActions.Roles bllRoles = new BLLActions.Roles(_configuration, _env, _mapper);

            List<RoleDto> list = bllRoles.getAllFilter(filterParam);
            return Ok(list);

        }


        #region Save
        [HttpPost("save")]

        public async Task<ActionResult<RoleDto?>> save([FromForm] RoleDto entity)
        {

            if (entity != null)
            {
                int userId = 0;
                if (HttpContext.User.Identity is ClaimsIdentity claimsIdentity)
                {
                    userId = int.Parse(claimsIdentity?.FindFirst("userId")?.Value ?? "0");

                }
                BLLActions.Roles bllRoles = new BLLActions.Roles(_configuration, _env, _mapper);

                if (entity?.Id != null)
                {

                    entity!.updatedDate = DateTime.Now;
                    entity.updatedUserId = userId == 0 ? null : userId;
                    await bllRoles.Update(_mapper.Map<Data.Models.Role>(entity));
                    return Ok(entity);
                }
                else
                {

                    entity!.createdDate = DateTime.Now;
                    entity.createdUserId = userId;
                    entity.enabled = true;
                    Role? role = await bllRoles.Add(_mapper.Map<Data.Models.Role>(entity));
                    return Ok(_mapper.Map<RoleDto>(role));
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
                BLLActions.Roles bllRoles = new BLLActions.Roles(_configuration, _env,_mapper);
                bllRoles.Delete(id);
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
