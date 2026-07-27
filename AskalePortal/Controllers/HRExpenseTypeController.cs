using AskalePortal.Data.Models;
using AskalePortal.Data.ResponseModels;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AskalePortal.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HRExpenseTypeController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;
        public HRExpenseTypeController(IConfiguration configuration, IWebHostEnvironment env, IMapper mapper)
        {
            _configuration = configuration;
            _env = env;
            _mapper = mapper;
        }

        #region getAll
        [HttpPost("getAll")]
        public ActionResult<object> getAll()
        {
            BLL.BLLActions.HRExpenseTypeTable bllHRExpenseTypeTable = new BLL.BLLActions.HRExpenseTypeTable(_configuration, _env);

            List<HRExpenseTypeTable>? listHRExpenseTypeTable = bllHRExpenseTypeTable.GetAll().OrderByDescending(u=>u.Id).ToList();
            return Ok(listHRExpenseTypeTable);

        }
        #endregion

        #region getById
        [HttpPost("getById")]

        public ActionResult<object> getById([FromForm] int id)
        {
            BLL.BLLActions.HRExpenseTypeTable bllHRExpenseTypeTable = new BLL.BLLActions.HRExpenseTypeTable(_configuration, _env);

            HRExpenseTypeTable? hrExpenseTypeTable = bllHRExpenseTypeTable.GetByID(id);
            if (hrExpenseTypeTable == null)
            {
                return NotFound();
            }
            return Ok(hrExpenseTypeTable);


        }
        #endregion

        #region delete
        [HttpPost("delete")]

        public ActionResult<int> delete([FromForm] int id)
        {
            try
            {
                BLL.BLLActions.HRExpenseTypeTable bllHRExpenseTypeTable = new BLL.BLLActions.HRExpenseTypeTable(_configuration, _env);
                bllHRExpenseTypeTable.Delete(id);
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
        public async Task<ActionResult<object>> save([FromForm] HRExpenseTypeTableSaveDto entity)
        {

            if (entity != null)
            {
                int userId = 0;
                if (HttpContext.User.Identity is ClaimsIdentity claimsIdentity)
                {
                    userId = int.Parse(claimsIdentity?.FindFirst("userId")?.Value ?? "0");

                }
                BLL.BLLActions.HRExpenseTypeTable bllHRExpenseTypeTable = new BLL.BLLActions.HRExpenseTypeTable(_configuration, _env);

                if (entity?.id != 0)
                {

                    entity!.updateDate = DateTime.Now.ToString();
                    entity.updatedUserId = userId == 0 ? null : userId;
                    await bllHRExpenseTypeTable.Update(_mapper.Map<HRExpenseTypeTable>(entity));
                    return Ok(entity);
                }
                else
                {

                    entity!.createdDate = DateTime.Now.ToString();
                    entity.createdUserId = userId == 0 ? null : userId; ;
                    entity.enabled = true;
                    await bllHRExpenseTypeTable.Add(_mapper.Map<HRExpenseTypeTable>(entity));
                    return Ok(entity);
                }
            }
            return Ok(null);
        }
        #endregion

    }
}
