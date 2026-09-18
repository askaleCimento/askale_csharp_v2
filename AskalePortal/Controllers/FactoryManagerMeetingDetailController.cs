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
    public class FactoryManagerMeetingDetailController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;
        public FactoryManagerMeetingDetailController(IConfiguration configuration, IWebHostEnvironment env, IMapper mapper)
        {
            _configuration = configuration;
            _env = env;
            _mapper = mapper;
        }

        #region listByMeetingId
        [HttpPost("listByMeetingId")]
        public ActionResult<object> getListMeetingId([FromForm] int meetingId)
        {
            BLLActions.FactoryManagerMeetingDetails bllFactoryManagerMeetingDetails = new BLLActions.FactoryManagerMeetingDetails(_configuration, _env);
            List<FactoryManagerMeetingDetail> list = bllFactoryManagerMeetingDetails.listByMeetingId(meetingId);
            return Ok(list);
        }
        #endregion

        #region getById
        [HttpPost("getById")]

        public ActionResult<object> getById([FromForm] int id)
        {
            BLLActions.FactoryManagerMeetingDetails bllFactoryManagerMeetingDetail = new BLLActions.FactoryManagerMeetingDetails(_configuration, _env);

            FactoryManagerMeetingDetail? meetingDetail = bllFactoryManagerMeetingDetail.GetByID(id);
            if (meetingDetail == null)
            {
                return NotFound();
            }
            return Ok(meetingDetail);


        }
        #endregion

        #region Save
        [HttpPost("save")]

        public async Task<ActionResult<object>> save([FromForm] FactoryManagerMeetingDetailSaveDto entity)
        {

            if (entity != null)
            {
                int userId = 0;
                if (HttpContext.User.Identity is ClaimsIdentity claimsIdentity)
                {
                    userId = int.Parse(claimsIdentity?.FindFirst("userId")?.Value ?? "0");

                }
                BLLActions.FactoryManagerMeetingDetails bllFactoryManagerMeetingDetails = new BLLActions.FactoryManagerMeetingDetails(_configuration, _env);

                if (entity?.id != null)
                {

                    entity!.updateDate = DateTime.Now.ToString();
                    entity.updatedUserId = userId == 0 ? null : userId;
                    await bllFactoryManagerMeetingDetails.Update(_mapper.Map<Data.Models.FactoryManagerMeetingDetail>(entity));
                    return Ok(entity);
                }
                else
                {

                    entity.createdDate = DateTime.Now.ToString();
                    entity.createdUserId = userId;
                    entity.enabled = true;
                    await bllFactoryManagerMeetingDetails.Add(_mapper.Map<Data.Models.FactoryManagerMeetingDetail>(entity));
                    return Ok(entity);
                }
            }
            return Ok(null);
        }
        #endregion

        #region getAll
        [HttpPost("getAll")]
        public ActionResult<object> getAll()
        {
            BLLActions.FactoryManagerMeetingDetails bllFactoryManagerMeetingDetails = new BLLActions.FactoryManagerMeetingDetails(_configuration, _env);

            List<FactoryManagerMeetingDetailSaveDto> list = bllFactoryManagerMeetingDetails.GetAllDto();
            return Ok(list);

        }
        #endregion

        #region delete
        [HttpPost("delete")]
        public ActionResult<int> delete([FromForm] int id)
        {
            try
            {
                BLLActions.FactoryManagerMeetingDetails bllFactoryManagerMeetingDetails = new BLLActions.FactoryManagerMeetingDetails(_configuration, _env);
                bllFactoryManagerMeetingDetails.Delete(id);
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
