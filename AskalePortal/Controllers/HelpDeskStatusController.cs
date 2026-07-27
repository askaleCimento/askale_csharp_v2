using AskalePortal.Data.Models;
using AskalePortal.Data.RequestModel;
using AskalePortal.Data.RequestParams;
using AskalePortal.Data.ResponseModels;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AskalePortal.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class HelpDeskStatusController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;
        public HelpDeskStatusController(IConfiguration configuration, IWebHostEnvironment env, IMapper mapper)
        {
            _configuration = configuration;
            _env = env;
            _mapper = mapper;
        }


        #region Save
        [HttpPost("save")]

        public async Task<ActionResult<object>> save([FromForm] HelpDeskStatusSaveDto entity)
        {

            if (entity != null)
            {
                int userId = 0;
                if (HttpContext.User.Identity is ClaimsIdentity claimsIdentity)
                {
                    userId = int.Parse(claimsIdentity?.FindFirst("userId")?.Value ?? "0");

                }
                BLL.BLLActions.HelpDeskStatuses bllHelpDeskStatuses = new BLL.BLLActions.HelpDeskStatuses(_configuration, _env);

                if (entity?.id != 0)
                {

                    entity!.updateDate = DateTime.Now;
                    entity.updatedUserId = userId == 0 ? null : userId;
                    await bllHelpDeskStatuses.Update(_mapper.Map< HelpDeskStatus >(entity));
                    return Ok(entity);
                }
                else
                {

                    entity!.createdDate = DateTime.Now;
                    entity.createdUserId = userId == 0 ? null : userId; ;
                    entity.enabled = true;
                    await bllHelpDeskStatuses.Add(_mapper.Map<HelpDeskStatus>(entity));
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
                BLL.BLLActions.HelpDeskStatuses bllHelpDeskStatuses = new BLL.BLLActions.HelpDeskStatuses(_configuration, _env);
                bllHelpDeskStatuses.Delete(id);
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
            BLL.BLLActions.HelpDeskStatuses bllHelpDeskStatuses = new BLL.BLLActions.HelpDeskStatuses(_configuration, _env);

            HelpDeskStatus? helpDeskStatus = bllHelpDeskStatuses.GetByID(id);
            if (helpDeskStatus == null)
            {
                return NotFound();
            }
            return Ok(helpDeskStatus);


        }
        #endregion


        #region getAll
        [HttpPost("getAll")]

        public ActionResult<object> getAll()
        {
            BLL.BLLActions.HelpDeskStatuses bllHelpDeskStatuses = new BLL.BLLActions.HelpDeskStatuses(_configuration, _env);

            List<HelpDeskStatus>? listHelpDeskStatuses = bllHelpDeskStatuses.GetAll();
            return Ok(listHelpDeskStatuses);

        }
        #endregion

        #region getAllFilter
        [HttpPost("getAllFilter")]

        public ActionResult<object> getAllFilter([FromForm] FilterParam<HelpDeskStatusListDtoParameter> filterParam)
        {
            BLL.BLLActions.HelpDeskStatuses bllHelpDeskStatuses = new BLL.BLLActions.HelpDeskStatuses(_configuration, _env);

            List<HelpDeskStatus>? listHelpDeskStatuses = bllHelpDeskStatuses.GetAllFilter(filterParam);
            return Ok(listHelpDeskStatuses);

        }
        #endregion


    }
}
