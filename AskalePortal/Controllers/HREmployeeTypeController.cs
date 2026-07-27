using AskalePortal.BLL;
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
    public class HREmployeeTypeController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;
        public HREmployeeTypeController(IConfiguration configuration, IWebHostEnvironment env, IMapper mapper)
        {
            _configuration = configuration;
            _env = env;
            _mapper = mapper;
        }



        #region getAll
        [HttpPost("getAll")]
        public ActionResult<object> getAll()
        {
            BLLActions.HREmployeeType bllHREmployeeType = new BLLActions.HREmployeeType(_configuration, _env);

            List<HREmployeeType>? listHREmployeeType = bllHREmployeeType.GetAll().OrderByDescending(u => u.Id).ToList();
            return Ok(listHREmployeeType);

        }
        #endregion


        #region getById
        [HttpPost("getById")]

        public ActionResult<object> getById([FromForm] int id)
        {
            BLLActions.HREmployeeType bllHREmployeeType = new BLLActions.HREmployeeType(_configuration, _env);

            HREmployeeType? hrEmployeeType = bllHREmployeeType.GetByID(id);
            if (hrEmployeeType == null)
            {
                return NotFound();
            }
            return Ok(hrEmployeeType);


        }
        #endregion

        #region delete
        [HttpPost("delete")]

        public ActionResult<int> delete([FromForm] int id)
        {
            try
            {
                BLLActions.HREmployeeType bllHREmployeeType = new BLLActions.HREmployeeType(_configuration, _env);
                bllHREmployeeType.Delete(id);
                return Ok(1);
            }
            catch
            {
                return Ok(0);
            }

        }
        #endregion

        #region Save
        [HttpPost("save")]
        public async Task<ActionResult<object>> save([FromForm] HREmployeeTypeSaveDto entity)
        {

            if (entity != null)
            {
                int userId = 0;
                if (HttpContext.User.Identity is ClaimsIdentity claimsIdentity)
                {
                    userId = int.Parse(claimsIdentity?.FindFirst("userId")?.Value ?? "0");

                }
                BLLActions.HREmployeeType bllHREmployeeType = new BLLActions.HREmployeeType(_configuration, _env);

                if (entity?.id != 0)
                {

                    entity!.updateDate = DateTime.Now.ToString();
                    entity.updatedUserId = userId == 0 ? null : userId;
                    await bllHREmployeeType.Update(_mapper.Map<HREmployeeType>(entity));
                    return Ok(entity);
                }
                else
                {

                    entity!.createdDate = DateTime.Now.ToString();
                    entity.createdUserId = userId == 0 ? null : userId; ;
                    entity.enabled = true;
                    await bllHREmployeeType.Add(_mapper.Map<HREmployeeType>(entity));
                    return Ok(entity);
                }
            }
            return Ok(null);
        }
        #endregion

    }
}
