using AskalePortal.Data.Models;
using AskalePortal.Data.RequestModel;
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
    public class UserTelephoneTableController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;
        public UserTelephoneTableController(IConfiguration configuration, IWebHostEnvironment env, IMapper mapper)
        {
            _configuration = configuration;
            _env = env;
            _mapper = mapper;
        }

        #region Save
        [HttpPost("save")]
        public async Task<ActionResult<object>> save([FromForm] UserTelephoneTableSaveDto entity)
        {

            if (entity != null)
            {
                int userId = 0;
                if (HttpContext.User.Identity is ClaimsIdentity claimsIdentity)
                {
                    userId = int.Parse(claimsIdentity?.FindFirst("userId")?.Value ?? "0");

                }
                BLL.BLLActions.UserTelephoneTable bllUserTelephoneTable = new BLL.BLLActions.UserTelephoneTable(_configuration, _env);

                if (entity?.id != null)
                {

                    entity.updateDate = DateTime.Now;
                    entity.updatedUserId = userId == 0 ? null : userId;
                    await bllUserTelephoneTable.Update(_mapper.Map< UserTelephoneTable > (entity));
                    return Ok(entity);
                }
                else
                {

                    entity!.createdDate = DateTime.Now;
                    entity.createdUserId = userId == 0 ? null : userId; ;
                    entity.enabled = true;
                    await bllUserTelephoneTable.Add(_mapper.Map< UserTelephoneTable >(entity));
                    return Ok(entity);
                }
            }
            return Ok(null);
        }
        #endregion

        [HttpPost("filterPageableDto")]
        public ActionResult<PageReturn<UserTelephoneTableDto>?> filterPageableDto([FromForm] FilterPageParam<PressAnnouncementDtoParameter> filterPageParam)
        {

            BLL.BLLActions.UserTelephoneTable bllAdminUsers = new BLL.BLLActions.UserTelephoneTable(_configuration, _env);
            PageReturn<UserTelephoneTableDto>? liste = bllAdminUsers.FilterPageableDto(filterPageParam);
            return Ok(liste);
        }

        [HttpPost("getByUserId")]
        public ActionResult<object> getByUserId([FromForm] int userId)
        {
            BLL.BLLActions.UserTelephoneTable bllAdminUsers = new BLL.BLLActions.UserTelephoneTable(_configuration, _env);

            UserTelephoneTable? userTelephoneTable = bllAdminUsers.getByUserId(userId);
            if (userTelephoneTable == null)
            {
                return Ok(null);
            }
            else
            {
                return Ok(userTelephoneTable);
            }

        }

    }
}
