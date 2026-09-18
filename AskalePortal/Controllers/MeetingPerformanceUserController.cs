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
    public class MeetingPerformanceUserController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;
        public MeetingPerformanceUserController(IConfiguration configuration, IWebHostEnvironment env, IMapper mapper)
        {
            _configuration = configuration;
            _env = env;
            _mapper = mapper;
        }

        [HttpPost("getAll")]
        public ActionResult<object> getAll()
        {
            BLLActions.MeetingPerformanceUser bllMeetingPerformanceUser = new BLLActions.MeetingPerformanceUser(_configuration, _env);

            List<MeetingPerformanceUser> roles = bllMeetingPerformanceUser.GetAll();
            return Ok(roles);

        }


        #region Save
        [HttpPost("save")]
        public async Task<ActionResult<MeetingPerformanceUserSaveDto?>> save([FromForm] MeetingPerformanceUserSaveDto entity)
        {

            if (entity != null)
            {
                int userId = 0;
                if (HttpContext.User.Identity is ClaimsIdentity claimsIdentity)
                {
                    userId = int.Parse(claimsIdentity?.FindFirst("userId")?.Value ?? "0");

                }
                BLLActions.MeetingPerformanceUser bllMeetingPerformanceUser = new BLLActions.MeetingPerformanceUser(_configuration, _env);

                if (entity?.id != null)
                {

                    entity.updateDate = DateTime.Now.ToString();
                    entity.updatedUserId = userId == 0 ? null : userId;
                    await bllMeetingPerformanceUser.Update(_mapper.Map<MeetingPerformanceUser>(entity));
                    return Ok(entity);
                }
                else
                {

                    entity!.createdDate = DateTime.Now.ToString();
                    entity.createdUserId = userId == 0 ? null : userId; ;
                    entity.enabled = true;
                    MeetingPerformanceUser? kayit = await bllMeetingPerformanceUser.Add(_mapper.Map<MeetingPerformanceUser>(entity));
                    return Ok(_mapper.Map<MeetingPerformanceUser>(kayit));
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
                BLLActions.MeetingPerformanceUser bllMeetingPerformanceUser = new BLLActions.MeetingPerformanceUser(_configuration, _env);
                bllMeetingPerformanceUser.Delete(id);
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
            BLLActions.MeetingPerformanceUser bllMeetingPerformanceUser = new BLLActions.MeetingPerformanceUser(_configuration, _env);

            MeetingPerformanceUser? meetingPerformanceUser = bllMeetingPerformanceUser.GetByID(id);
            if (meetingPerformanceUser == null)
            {
                return NotFound();
            }
            return Ok(meetingPerformanceUser);


        }
        #endregion

    }
}
