using AskalePortal.BLL;
using AskalePortal.Data.Models;
using AskalePortal.Data.ResponseModels;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static AskalePortal.BLL.BLLActions;

namespace AskalePortal.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MeetingUserController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;
        
        public MeetingUserController(IConfiguration configuration, IWebHostEnvironment env, IMapper mapper)
        {
            _configuration = configuration;
            _env = env;
            _mapper = mapper;
        }

        #region getAll
        [HttpPost("getAll")]

        public ActionResult<object> getAll()
        {
            BLL.BLLActions.MeetingUsers bllMeetingUsers = new BLL.BLLActions.MeetingUsers(_configuration, _env);

            List<MeetingUser>? listMeeting = bllMeetingUsers.GetAll();
            return Ok(listMeeting);

        }
        #endregion
       
        #region listAllMeetingUser
        [HttpPost("listAllMeetingUser")]
        public ActionResult<object> getListAllMeetingUser()
        {
            BLLActions.MeetingUsers bllMetingUsers = new BLLActions.MeetingUsers(_configuration, _env);
            List<MeetingUser> list = bllMetingUsers.listAllMeetingUser();
            return Ok(list);
        }
        #endregion

        #region Save
        [HttpPost("save")]

        public async Task<ActionResult<object>> save([FromForm] MeetingUserSaveDto entity)
        {

            if (entity != null)
            {
                int userId = 0;
                if (HttpContext.User.Identity is ClaimsIdentity claimsIdentity)
                {
                    userId = int.Parse(claimsIdentity?.FindFirst("userId")?.Value ?? "0");

                }
                BLL.BLLActions.MeetingUsers bllMeetingUsers = new BLL.BLLActions.MeetingUsers(_configuration, _env);

                if (entity?.id != null)
                {

                    entity!.updateDate = DateTime.Now.ToString();
                    entity.updatedUserId = userId == 0 ? null : userId;
                    await bllMeetingUsers.Update(_mapper.Map<Data.Models.MeetingUser>(entity));
                    return Ok(entity);
                }
                else
                {

                    entity.createdDate = DateTime.Now.ToString();
                    entity.createdUserId = userId;
                    entity.enabled = true;
                    await bllMeetingUsers.Add(_mapper.Map<Data.Models.MeetingUser>(entity));
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
                BLLActions.MeetingUsers bllMeetingUsers = new BLLActions.MeetingUsers(_configuration, _env);
                bllMeetingUsers.Delete(id);
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
            BLL.BLLActions.MeetingUsers bllMeetingUsers = new BLL.BLLActions.MeetingUsers(_configuration, _env);

            MeetingUser? meetingUsers = bllMeetingUsers.GetByID(id);
            if (meetingUsers == null)
            {
                return NotFound();
            }
            return Ok(meetingUsers);


        }
        #endregion

    }
}
