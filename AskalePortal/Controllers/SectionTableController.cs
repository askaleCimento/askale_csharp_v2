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
    public class SectionTableController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;
        public SectionTableController(IConfiguration configuration, IWebHostEnvironment env, IMapper mapper)
        {
            _configuration = configuration;
            _env = env;
            _mapper = mapper;
        }

        #region Save
        [HttpPost("save")]

        public async Task<ActionResult<object>> save([FromForm] SectionTableSaveDto entity)
        {

            if (entity != null)
            {
                int userId = 0;
                if (HttpContext.User.Identity is ClaimsIdentity claimsIdentity)
                {
                    userId = int.Parse(claimsIdentity?.FindFirst("userId")?.Value ?? "0");

                }
                BLLActions.SectionTable bllSectionTable = new BLLActions.SectionTable(_configuration, _env);

                if (entity?.id != null)
                {

                    entity!.updateDate = DateTime.Now.ToString();
                    entity.updatedUserId = userId == 0 ? null : userId;
                    await bllSectionTable.Update(_mapper.Map<Data.Models.SectionTable>(entity));
                    return Ok(entity);
                }
                else
                {

                    entity.createdDate = DateTime.Now.ToString();
                    entity.createdUserId = userId;
                    entity.enabled = true;
                    await bllSectionTable.Add(_mapper.Map<Data.Models.SectionTable>(entity));
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
                BLLActions.SectionTable bllSectionTable = new BLLActions.SectionTable(_configuration, _env);
                bllSectionTable.Delete(id);
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
            BLLActions.SectionTable bllSectionTable = new BLLActions.SectionTable(_configuration, _env);

            SectionTable? announcement = bllSectionTable.GetByID(id);
            if (announcement == null)
            {
                return NotFound();
            }
            return Ok(announcement);


        }
        #endregion


        #region getAll
        [HttpPost("getAll")]

        public ActionResult<SectionTableSaveDto> getAll()
        {
            BLLActions.SectionTable bllSectionTable = new BLLActions.SectionTable(_configuration, _env);

            List<SectionTableSaveDto>? listSectionTable = bllSectionTable.GetAllDto();
            return Ok(listSectionTable);

        }
        #endregion


    }
}
