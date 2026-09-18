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
    public class FactoryManagerMeetingPerformanceUserController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;
        public FactoryManagerMeetingPerformanceUserController(IConfiguration configuration, IWebHostEnvironment env, IMapper mapper)
        {
            _configuration = configuration;
            _env = env;
            _mapper = mapper;
        }
        #region getById
        [HttpPost("getById")]

        public ActionResult<object> getById([FromForm] int id)
        {
            BLLActions.FactoryManagerMeetingPerformanceUser bllFactoryManagerMeetingPerformanceUser = new BLLActions.FactoryManagerMeetingPerformanceUser(_configuration, _env);

            FactoryManagerMeetingPerformanceUser? factoryManagerMeetingPerformanceUser = bllFactoryManagerMeetingPerformanceUser.GetByID(id);
            if (factoryManagerMeetingPerformanceUser == null)
            {
                return NotFound();
            }
            return Ok(factoryManagerMeetingPerformanceUser);


        }
        #endregion

        #region Save
        [HttpPost("save")]

        public async Task<ActionResult<object>> save([FromForm] FactoryManagerMeetingPerformanceUserSaveDto entity)
        {

            if (entity != null)
            {
                int userId = 0;
                if (HttpContext.User.Identity is ClaimsIdentity claimsIdentity)
                {
                    userId = int.Parse(claimsIdentity?.FindFirst("userId")?.Value ?? "0");

                }
                BLLActions.FactoryManagerMeetingPerformanceUser bllFactoryManagerMeetingDetails = new BLLActions.FactoryManagerMeetingPerformanceUser(_configuration, _env);

                if (entity?.id != null)
                {

                    entity!.updateDate = DateTime.Now.ToString();
                    entity.updatedUserId = userId == 0 ? null : userId;
                    await bllFactoryManagerMeetingDetails.Update(_mapper.Map<Data.Models.FactoryManagerMeetingPerformanceUser>(entity));
                    return Ok(entity);
                }
                else
                {

                    entity.createdDate = DateTime.Now.ToString();
                    entity.createdUserId = userId;
                    entity.enabled = true;
                    await bllFactoryManagerMeetingDetails.Add(_mapper.Map<Data.Models.FactoryManagerMeetingPerformanceUser>(entity));
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
            BLLActions.FactoryManagerMeetingPerformanceUser bllFactoryManagerMeetingDetails = new BLLActions.FactoryManagerMeetingPerformanceUser(_configuration, _env);

            List<FactoryManagerMeetingPerformanceUserSaveDto> list = bllFactoryManagerMeetingDetails.GetAllDto();
            return Ok(list);

        }
        #endregion

        #region delete
        [HttpPost("delete")]
        public ActionResult<int> delete([FromForm] int id)
        {
            try
            {
                BLLActions.FactoryManagerMeetingPerformanceUser bllFactoryManagerMeetingDetails = new BLLActions.FactoryManagerMeetingPerformanceUser(_configuration, _env);
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
