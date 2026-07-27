using AskalePortal.Data.Models;
using AskalePortal.Data.ResponseModels;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AskalePortal.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HRDestinationLocationController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;
        public HRDestinationLocationController(IConfiguration configuration, IWebHostEnvironment env,IMapper mapper)
        {
            _configuration = configuration;
            _env = env;
            _mapper = mapper;
        }

        #region Save
        [HttpPost("save")]
        public async Task<ActionResult<object>> save([FromForm] HRDestinationLocationTableSaveDto entity)
        {

            if (entity != null)
            {
                int userId = 0;
                if (HttpContext.User.Identity is ClaimsIdentity claimsIdentity)
                {
                    userId = int.Parse(claimsIdentity?.FindFirst("userId")?.Value ?? "0");

                }
                BLL.BLLActions.HRDestinationLocation bllHRDestinationLocationTable = new BLL.BLLActions.HRDestinationLocation(_configuration, _env);

                if (entity?.id != 0)
                {

                    entity!.updateDate = DateTime.Now.ToString();
                    entity.updatedUserId = userId == 0 ? null : userId;
                    await bllHRDestinationLocationTable.Update(_mapper.Map<HRDestinationLocationTable>(entity));
                    return Ok(entity);
                }
                else
                {

                    entity!.createdDate = DateTime.Now.ToString();
                    entity.createdUserId = userId == 0 ? null : userId; ;
                    entity.enabled = true;
                    await bllHRDestinationLocationTable.Add(_mapper.Map<HRDestinationLocationTable>(entity));
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
                BLL.BLLActions.HRDestinationLocation bllHRDestinationLocationTable = new BLL.BLLActions.HRDestinationLocation(_configuration, _env);
                bllHRDestinationLocationTable.Delete(id);
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
            BLL.BLLActions.HRDestinationLocation bllHRDestinationLocationTable = new BLL.BLLActions.HRDestinationLocation(_configuration, _env);

            HRDestinationLocationTable? hrDestinationLocation = bllHRDestinationLocationTable.GetByID(id);
            if (hrDestinationLocation == null)
            {
                return NotFound();
            }
            return Ok(hrDestinationLocation);


        }
        #endregion


        #region getAll
        [HttpPost("getAll")]

        public ActionResult<object> getAll()
        {
            BLL.BLLActions.HRDestinationLocation bllHRDestinationLocation = new BLL.BLLActions.HRDestinationLocation(_configuration, _env);

            List<HRDestinationLocationTable>? listHRDestinationLocation = bllHRDestinationLocation.GetAll().OrderByDescending(u => u.Id).ToList();
            return Ok(listHRDestinationLocation);

        }
        #endregion


    }
}
