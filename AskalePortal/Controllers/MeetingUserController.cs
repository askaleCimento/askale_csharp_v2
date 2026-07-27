using AskalePortal.BLL;
using AskalePortal.Data.Models;
using Microsoft.AspNetCore.Mvc;

namespace AskalePortal.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MeetingUserController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        public MeetingUserController(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _env = env;
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

    }
}
