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
    public class FactoryManagerMeetingUserController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;
        public FactoryManagerMeetingUserController(IConfiguration configuration, IWebHostEnvironment env, IMapper mapper)
        {
            _configuration = configuration;
            _env = env;
            _mapper = mapper;
        }
        #region getAll
        [HttpPost("getAll")]

        public ActionResult<object> getAll()
        {
            BLL.BLLActions.FactoryManagerMeetingUsers bllFactoryManagerMeetingUsers = new BLL.BLLActions.FactoryManagerMeetingUsers(_configuration, _env);

            List<FactoryManagerMeetingUser>? listMeeting = bllFactoryManagerMeetingUsers.GetAll();
            return Ok(listMeeting);

        }
        #endregion

        #region listAllMeetingUser
        [HttpPost("listAllMeetingUser")]
        public ActionResult<object> getListAllMeetingUser()
        {
            BLLActions.FactoryManagerMeetingUsers bllFactoryManagerMetingUsers = new BLLActions.FactoryManagerMeetingUsers(_configuration, _env);
            List<FactoryManagerMeetingUser> list = bllFactoryManagerMetingUsers.listAllMeetingUser();
            return Ok(list);
        }
        #endregion

        #region getById
        [HttpPost("getById")]
        public ActionResult<object> getById([FromForm] int id)
        {
            BLLActions.FactoryManagerMeetingUsers bllFactoryManagerMeetingUsers = new BLLActions.FactoryManagerMeetingUsers(_configuration, _env);

            FactoryManagerMeetingUser? factoryManagerMeetingPerformanceUser = bllFactoryManagerMeetingUsers.GetByID(id);
            if (factoryManagerMeetingPerformanceUser == null)
            {
                return NotFound();
            }
            return Ok(factoryManagerMeetingPerformanceUser);


        }
        #endregion

        #region Save
        [HttpPost("save")]

        public async Task<ActionResult<object>> save([FromForm] FactoryManagerMeetingUserSaveDto entity)
        {

            if (entity != null)
            {
                int userId = 0;
                if (HttpContext.User.Identity is ClaimsIdentity claimsIdentity)
                {
                    userId = int.Parse(claimsIdentity?.FindFirst("userId")?.Value ?? "0");

                }
                BLLActions.FactoryManagerMeetingUsers bllFactoryManagerMeetingUsers = new BLLActions.FactoryManagerMeetingUsers(_configuration, _env);

                if (entity?.id != null)
                {

                    entity!.updateDate = DateTime.Now.ToString();
                    entity.updatedUserId = userId == 0 ? null : userId;
                    await bllFactoryManagerMeetingUsers.Update(_mapper.Map<Data.Models.FactoryManagerMeetingUser>(entity));
                    return Ok(entity);
                }
                else
                {

                    entity.createdDate = DateTime.Now.ToString();
                    entity.createdUserId = userId;
                    entity.enabled = true;
                    await bllFactoryManagerMeetingUsers.Add(_mapper.Map<Data.Models.FactoryManagerMeetingUser>(entity));
                    return Ok(entity);
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
                BLLActions.FactoryManagerMeetingUsers bllFactoryManagerMeetingUsers = new BLLActions.FactoryManagerMeetingUsers(_configuration, _env);
                bllFactoryManagerMeetingUsers.Delete(id);
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
