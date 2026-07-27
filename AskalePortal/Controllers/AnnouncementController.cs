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
    public class AnnouncementController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;
        public AnnouncementController(IConfiguration configuration, IWebHostEnvironment env, IMapper mapper)
        {
            _configuration = configuration;
            _env = env;
            _mapper = mapper;
        }


        #region Save
        [HttpPost("save")]

        public async Task<ActionResult<object>> save([FromForm] AnnouncementSaveDto entity)
        {

            if (entity != null)
            {
                int userId = 0;
                if (HttpContext.User.Identity is ClaimsIdentity claimsIdentity)
                {
                    userId = int.Parse(claimsIdentity?.FindFirst("userId")?.Value ?? "0");

                }
               BLLActions.Announcements bllAnnouncement = new BLLActions.Announcements(_configuration, _env);

                if (entity?.id != 0)
                {

                    entity!.updateDate = DateTime.Now.ToString();
                    entity.updatedUserId = userId == 0 ? null : userId;
                    await bllAnnouncement.Update(_mapper.Map<Data.Models.Announcement>(entity));
                    return Ok(entity);
                }
                else
                {

                    entity.createdDate = DateTime.Now.ToString();
                    entity.createdUserId =  userId;
                    entity.enabled = true;
                    await bllAnnouncement.Add(_mapper.Map<Data.Models.Announcement>(entity));
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
               BLLActions.Announcements bllAnnouncement = new BLLActions.Announcements(_configuration, _env);
                bllAnnouncement.Delete(id);
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
           BLLActions.Announcements bllAnnouncement = new BLLActions.Announcements(_configuration, _env);

            Announcement? announcement = bllAnnouncement.GetByID(id);
            if (announcement == null)
            {
                return NotFound();
            }
            return Ok(announcement);


        }
        #endregion


        #region getAll
        [HttpPost("getAll")]

        public ActionResult<object> getAll()
        {
           BLLActions.Announcements bllAnnouncement = new BLLActions.Announcements(_configuration, _env);

            List<Announcement>? listAnnouncement = bllAnnouncement.GetAll();
            return Ok(listAnnouncement);

        }
        #endregion


    }
}
