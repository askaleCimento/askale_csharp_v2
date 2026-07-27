using AskalePortal.BLL;
using AskalePortal.Data.Models;
using AskalePortal.Data.ResponseModels;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AskalePortal.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HRDepartmanTableController : ControllerBase
    {
        private readonly IWebHostEnvironment _env; private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        public HRDepartmanTableController(IWebHostEnvironment env, IConfiguration configuration, IMapper mapper)
        {
            _configuration = configuration;
            _env = env;
            _mapper = mapper;
        }
        [HttpPost("getAll")]
        public ActionResult<object> getAll()
        {
            BLLActions.HRDepartmanTable bllHRDepartmanTable = new BLLActions.HRDepartmanTable(_configuration, _env);

            List<HRDepartmanTable> list = bllHRDepartmanTable.GetAll();
            return Ok(list);

        }

        [HttpPost("getAllNameAndId")]
        public ActionResult<List<IdandText>> getDepartmanIdAndName()
        {
            BLLActions.HRDepartmanTable bllHRDepartmanTable = new BLLActions.HRDepartmanTable(_configuration, _env);

            List<IdandText> list = bllHRDepartmanTable.GetDepartmanIdAndName();
            return Ok(list);
        }

        #region Save
        [HttpPost("save")]
        public async Task<ActionResult<object>> save([FromForm] HRDepartmanTableSaveDto entity)
        {

            if (entity != null)
            {
                int userId = 0;
                if (HttpContext.User.Identity is ClaimsIdentity claimsIdentity)
                {
                    userId = int.Parse(claimsIdentity?.FindFirst("userId")?.Value ?? "0");

                }
                BLLActions.HRDepartmanTable bllHRDepartmanTable = new BLLActions.HRDepartmanTable(_configuration, _env);

                if (entity?.id != null)
                {

                    entity.updateDate = DateTime.Now.ToString();
                    entity.updatedUserId = userId == 0 ? null : userId;
                    await bllHRDepartmanTable.Update(_mapper.Map<HRDepartmanTable>(entity));
                    return Ok(entity);
                }
                else
                {

                    entity!.createdDate = DateTime.Now.ToString();
                    entity.createdUserId = userId == 0 ? null : userId; ;
                    entity.enabled = true;
                    await bllHRDepartmanTable.Add(_mapper.Map<HRDepartmanTable>(entity));
                    return Ok(entity);
                }
            }
            return Ok(null);
        }
        #endregion

        #region getById
        [HttpPost("getById")]
        public ActionResult<object> getById([FromForm] int id)
        {
            BLLActions.HRDepartmanTable bllHRDepartmanTable = new BLLActions.HRDepartmanTable(_configuration, _env);

            HRDepartmanTable? hrDepartmanTable = bllHRDepartmanTable.GetByID(id);
            if (hrDepartmanTable == null)
            {
                return NotFound();
            }
            return Ok(hrDepartmanTable);


        }
        #endregion

        #region delete
        [HttpPost("delete")]

        public ActionResult<int> delete([FromForm] int id)
        {
            try
            {
                BLLActions.HRDepartmanTable bllHRDepartmanTable = new BLLActions.HRDepartmanTable(_configuration, _env);
                bllHRDepartmanTable.Delete(id);
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
