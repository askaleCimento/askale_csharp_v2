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
    public class RoleDetailController : ControllerBase
    {
        private readonly IConfiguration _configuration; private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;

        public RoleDetailController(IConfiguration configuration, IWebHostEnvironment env, IMapper mapper)
        {
            _configuration = configuration;
            _env = env;
            _mapper = mapper;
        }


        #region getByRoleId
        [HttpPost("getByRoleId")]
        public ActionResult<List<RoleDetailSaveDto>> getByRoleId([FromForm] int roleId)
        {
            BLLActions.RoleDetails bllRoleDetails = new BLLActions.RoleDetails(_configuration, _env, _mapper);
            List<RoleDetailSaveDto> list = bllRoleDetails.getByRoleId(roleId);
            return Ok(list);
        }
        #endregion
        #region getByRoleDetailRoleId
        [HttpPost("getByRoleDetailRoleId")]

        public ActionResult<List<RoleDetailSaveDto>> getByRoleDetailRoleId([FromForm] int roleId)
        {
            BLLActions.RoleDetails bllRoleDetails = new BLLActions.RoleDetails(_configuration, _env, _mapper);
            List<RoleDetailSaveDto> list = bllRoleDetails.getByRoleDetailRoleId(roleId);

            return Ok(list);
        }
        #endregion
        #region addRoleDetails
        [HttpPost("addRoleDetails")]
        public async Task<ActionResult<string>> addRoleDetails([FromBody] List<RoleDetailSaveDto> listRoleDetail)
        {
            try
            {


                int userId = 0;
                if (HttpContext.User.Identity is ClaimsIdentity claimsIdentity)
                {
                    userId = int.Parse(claimsIdentity?.FindFirst("userId")?.Value ?? "0");

                }
                BLLActions.RoleDetails bllRoleDetails = new BLLActions.RoleDetails(_configuration, _env, _mapper);

                foreach (RoleDetailSaveDto item in listRoleDetail)
                {
                    item.enabled = true;
                    if (item.id == null)
                    {
                        item.createdUserId = userId;
                        item.createdDate = DateTime.Now.ToString();
                        await bllRoleDetails.Add(_mapper.Map<RoleDetail>(item));
                    }
                    else
                    {
                        item.updatedUserId = userId;
                        item.updateDate = DateTime.Now.ToString();
                        await bllRoleDetails.Update(_mapper.Map<RoleDetail>(item));
                    }
                }
                return "OK";
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
        #endregion
        #region getByModuleIdAndRoleId
        [HttpPost("getByModuleIdAndRoleId")]
        public ActionResult<object> getByModuleIdAndRoleId([FromForm] int moduleId,
            [FromForm] int roleId)
        {
            BLLActions.RoleDetails bllRoleDetails = new BLLActions.RoleDetails(_configuration, _env,_mapper);
            RoleDetail? kayit = bllRoleDetails.GetByRoleIDAndModuleID(roleId, moduleId);
            return Ok(kayit);
        }
        #endregion
        #region delete
        [HttpPost("delete")]
        public async Task<ActionResult<int>> delete([FromForm] int moduleId, [FromForm] int roleId)
        {
            BLLActions.RoleDetails bllRoleDetails = new BLLActions.RoleDetails(_configuration, _env,_mapper);
            return await bllRoleDetails.delete(moduleId, roleId);
        }
        #endregion
    }
}
