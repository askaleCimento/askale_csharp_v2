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
    public class MeetingController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;
        public MeetingController(IConfiguration configuration, IWebHostEnvironment env,IMapper mapper)
        {
            _configuration = configuration;
            _env = env;
            _mapper = mapper;
        }

        #region getAll
        [HttpPost("getAll")]

        public ActionResult<object> getAll()
        {
            BLL.BLLActions.Meetings bllMeetings = new BLL.BLLActions.Meetings(_configuration, _env);

            List<Meeting>? listMeeting = bllMeetings.GetAll();
            return Ok(listMeeting);

        }
        #endregion


        #region getById
        [HttpPost("getById")]

        public ActionResult<object> getById([FromForm] int id)
        {
            BLL.BLLActions.Meetings bllMeeting = new BLL.BLLActions.Meetings(_configuration, _env);

            Meeting? meeting = bllMeeting.GetByID(id);
            if (meeting == null)
            {
                return NotFound();
            }
            return Ok(meeting);


        }
        #endregion


        #region Save
        [HttpPost("save")]

        public async Task<ActionResult<object>> save([FromForm] MeetingSaveDto entity)
        {

            if (entity != null)
            {
                int userId = 0;
                if (HttpContext.User.Identity is ClaimsIdentity claimsIdentity)
                {
                    userId = int.Parse(claimsIdentity?.FindFirst("userId")?.Value ?? "0");

                }
                BLL.BLLActions.Meetings bllMeetings = new BLL.BLLActions.Meetings(_configuration, _env);

                if (entity?.id != 0)
                {

                    entity!.updateDate = DateTime.Now.ToString();
                    entity.updatedUserId = userId == 0 ? null : userId;
                    await bllMeetings.Update(_mapper.Map<Data.Models.Meeting>(entity));
                    return Ok(entity);
                }
                else
                {

                    entity.createdDate = DateTime.Now.ToString();
                    entity.createdUserId = userId;
                    entity.enabled = true;
                    await bllMeetings.Add(_mapper.Map<Data.Models.Meeting>(entity));
                    return Ok(entity);
                }
            }
            return Ok(null);
        }
        #endregion
    }
}
