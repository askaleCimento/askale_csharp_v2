using AskalePortal.Data.Models;
using AskalePortal.Data.ResponseModels;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AskalePortal.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HelpDeskMessageController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;
        public HelpDeskMessageController(IConfiguration configuration, IWebHostEnvironment env, IMapper mapper)
        {
            _configuration = configuration;
            _env = env;
            _mapper = mapper;
        }
        #region Save
        [HttpPost("save")]

        public async Task<ActionResult<object>> save([FromForm] HelpDeskMessageSaveDto entity)
        {

            if (entity != null)
            {
                int userId = 0;
                if (HttpContext.User.Identity is ClaimsIdentity claimsIdentity)
                {
                    userId = int.Parse(claimsIdentity?.FindFirst("userId")?.Value ?? "0");

                }
                BLL.BLLActions.HelpDeskMessages bllHelpDeskMessage = new BLL.BLLActions.HelpDeskMessages(_configuration, _env);

                if (entity?.id != null)
                {

                    entity.updateDate = DateTime.Now;
                    entity.updatedUserId = userId == 0 ? null : userId;
                   await bllHelpDeskMessage.Update(_mapper.Map< HelpDeskMessage > (entity));
                    return Ok(entity);
                }
                else
                {

                    entity!.createdDate = DateTime.Now;
                    entity.createdUserId = userId;
                    entity.enabled = true;
                    await bllHelpDeskMessage.Add(_mapper.Map< HelpDeskMessage >(entity));
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
                BLL.BLLActions.HelpDeskMessages bllHelpDeskMessage = new BLL.BLLActions.HelpDeskMessages(_configuration, _env);
                bllHelpDeskMessage.Delete(id);
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
            BLL.BLLActions.HelpDeskMessages bllHelpDeskMessage = new BLL.BLLActions.HelpDeskMessages(_configuration, _env);

            HelpDeskMessage? helpDeskMessage = bllHelpDeskMessage.GetByID(id);
            if (helpDeskMessage == null)
            {
                return NotFound();
            }
            return Ok(helpDeskMessage);


        }
        #endregion


        #region getAll
        [HttpPost("getAll")]

        public ActionResult<object> getAll()
        {
            BLL.BLLActions.HelpDeskMessages bllHelpDeskMessage = new BLL.BLLActions.HelpDeskMessages(_configuration, _env);

            List<HelpDeskMessage>? listHelpDeskMessage = bllHelpDeskMessage.GetAll();
            return Ok(listHelpDeskMessage);

        }
        #endregion
        #region getAll
        [HttpPost("listDemandId")]
        public ActionResult<List<HelpDeskMessageDto>> listDemandId([FromForm] int demandId)
        {
            BLL.BLLActions.HelpDeskMessages bllHelpDeskMessage = new BLL.BLLActions.HelpDeskMessages(_configuration, _env);

            List<HelpDeskMessageDto>? listHelpDeskMessage = bllHelpDeskMessage.listDemandId(demandId);
            return Ok(listHelpDeskMessage);
        }
        #endregion
    }
}
