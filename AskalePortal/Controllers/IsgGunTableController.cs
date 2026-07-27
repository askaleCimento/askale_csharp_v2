using AskalePortal.Data.Models;
using AskalePortal.Data.ResponseModels;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AskalePortal.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IsgGunTableController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;
        public IsgGunTableController(IConfiguration configuration, IWebHostEnvironment env, IMapper mapper)
        {
            _configuration = configuration;
            _env = env;
            _mapper = mapper;
        }
        #region Save
        [HttpPost("save")]

        public async Task<ActionResult<object>> save([FromForm] ISGGunTableSaveDto entity)
        {

            if (entity != null)
            {
                int userId = 0;
                if (HttpContext.User.Identity is ClaimsIdentity claimsIdentity)
                {
                    userId = int.Parse(claimsIdentity?.FindFirst("userId")?.Value ?? "0");

                }
                BLL.BLLActions.ISGGunTable bllISGGunTable = new BLL.BLLActions.ISGGunTable(_configuration, _env);

                if (entity?.id != null)
                {

                    entity.updateDate = DateTime.Now;
                    entity.updatedUserId = userId == 0 ? null : userId;
                    await bllISGGunTable.Update(_mapper.Map<ISGGunTable>(entity));
                    return Ok(entity);
                }
                else
                {

                    entity!.createdDate = DateTime.Now;
                    entity.createdUserId = userId;
                    entity.enabled = true;
                    await bllISGGunTable.Add(_mapper.Map<ISGGunTable>(entity));
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
                BLL.BLLActions.ISGGunTable bllISGGunTable = new BLL.BLLActions.ISGGunTable(_configuration, _env);
                bllISGGunTable.Delete(id);
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
            BLL.BLLActions.ISGGunTable bllISGGunTable = new BLL.BLLActions.ISGGunTable(_configuration, _env);

            ISGGunTable? isgGunTable = bllISGGunTable.GetByID(id);
            if (isgGunTable == null)
            {
                return NotFound();
            }
            return Ok(isgGunTable);


        }
        #endregion


        #region getAll
        [HttpPost("getAll")]

        public ActionResult<object> getAll()
        {
            BLL.BLLActions.ISGGunTable bllISGGunTable = new BLL.BLLActions.ISGGunTable(_configuration, _env);

            List<ISGGunTable>? listISGGunTable = bllISGGunTable.GetAll();
            return Ok(listISGGunTable);

        }
        #endregion


        #region getAll
        [HttpPost("numberOfAccidentFreeDays")]
        public ActionResult<List<ISGGunTableGraphDto>> numberOfAccidentFreeDays()
        {
            BLL.BLLActions.ISGGunTable bllISGGunTable = new BLL.BLLActions.ISGGunTable(_configuration, _env);

            List<ISGGunTableGraphDto>? listISGGunTable = bllISGGunTable.NumberOfAccidentFreeDays();
            return Ok(listISGGunTable);
        }
        #endregion
    }
}
