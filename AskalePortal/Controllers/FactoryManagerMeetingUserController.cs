using AskalePortal.BLL;
using AskalePortal.Data.Models;
using Microsoft.AspNetCore.Mvc;

namespace AskalePortal.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FactoryManagerMeetingUserController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        public FactoryManagerMeetingUserController(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _env = env;
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
    }
}
