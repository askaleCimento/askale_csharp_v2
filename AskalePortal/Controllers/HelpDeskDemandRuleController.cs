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
  
    public class HelpDeskDemandRuleController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;
        public HelpDeskDemandRuleController(IConfiguration configuration, IWebHostEnvironment env, IMapper mapper)
        {
            _configuration = configuration;
            _env = env;
            _mapper = mapper;
        }


        #region Save
        [HttpPost("save")]
        public async Task<ActionResult<object>> save([FromForm] HelpDeskDemandRuleSaveDto entity)
        {

            if (entity != null)
            {
                int userId = 0;
                if (HttpContext.User.Identity is ClaimsIdentity claimsIdentity)
                {
                    userId = int.Parse(claimsIdentity?.FindFirst("userId")?.Value ?? "0");

                }
                BLL.BLLActions.HelpDeskDemandRules bllHelpDeskDemandRules = new BLL.BLLActions.HelpDeskDemandRules(_configuration, _env);

                if (entity?.id != null)
                {

                    entity.updateDate = DateTime.Now;
                    entity.updatedUserId = userId == 0 ? null : userId;
                    await bllHelpDeskDemandRules.Update(_mapper.Map< HelpDeskDemandRule >(entity));
                    return Ok(entity);
                }
                else
                {

                    entity!.createdDate = DateTime.Now;
                    entity.createdUserId = userId == 0 ? null : userId; ;
                    entity.enabled = true;
                    await bllHelpDeskDemandRules.Add(_mapper.Map<HelpDeskDemandRule>(entity));
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
                BLL.BLLActions.HelpDeskDemandRules bllHelpDeskDemandRules = new BLL.BLLActions.HelpDeskDemandRules(_configuration, _env);
                bllHelpDeskDemandRules.Delete(id);
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
            BLL.BLLActions.HelpDeskDemandRules bllHelpDeskDemandRules = new BLL.BLLActions.HelpDeskDemandRules(_configuration, _env);

            HelpDeskDemandRule? helpDeskDemandRule = bllHelpDeskDemandRules.GetByID(id);
            if (helpDeskDemandRule == null)
            {
                return NotFound();
            }
            return Ok(helpDeskDemandRule);


        }
        #endregion


        #region getAll
        [HttpPost("getAll")]

        public ActionResult<object> getAll()
        {
            BLL.BLLActions.HelpDeskDemandRules bllHelpDeskDemandRules = new BLL.BLLActions.HelpDeskDemandRules(_configuration, _env);

            List<HelpDeskDemandRule>? listHelpDeskDemandRule = bllHelpDeskDemandRules.GetAll();
            return Ok(listHelpDeskDemandRule);

        }
        #endregion

        #region getAllFilter
        [HttpPost("getAllFilter")]

        public ActionResult<object> getAllFilter([FromForm] FilterParam<HelpDeskStatusListDtoParameter> filterParam)
        {
            BLL.BLLActions.HelpDeskDemandRules bllHelpDeskDemandRules = new BLL.BLLActions.HelpDeskDemandRules(_configuration, _env);

            List<HelpDeskDemandRule>? listHelpDeskDemandRule = bllHelpDeskDemandRules.GetAllFilter(filterParam);
            return Ok(listHelpDeskDemandRule);

        }
        #endregion


    }
}
