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
    public class FinansUserTableController : ControllerBase
    {
        private readonly IConfiguration _configuration; private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;


        public FinansUserTableController(IConfiguration configuration, IWebHostEnvironment env, IMapper mapper)
        {
            _configuration = configuration;
            _env = env;
            _mapper = mapper;
        }

        #region listFinansUser
        [HttpPost("listFinansUser")]
        public ActionResult<List<FinansUserDto>> listFinansUserTable()
        {
            BLLActions.FinansUserTable bllFinansUserTable = new BLLActions.FinansUserTable(_configuration, _env);

            List<FinansUserDto> list = bllFinansUserTable.listFinansUser();
            return Ok(list);

        }
        #endregion

        #region getById
        [HttpPost("getById")]
        public ActionResult<object> getById([FromForm] int id)
        {

            BLLActions.FinansUserTable bllFinansUserTable = new BLLActions.FinansUserTable(_configuration, _env);
            FinansUserTable? finansUserTable = bllFinansUserTable.GetByID(id);

            return Ok(finansUserTable);
        }
        #endregion


        #region delete
        [HttpPost("delete")]

        public ActionResult<int> delete([FromForm] int id)
        {
            try
            {
                BLL.BLLActions.FinansUserTable bllFinansUserTable = new BLL.BLLActions.FinansUserTable(_configuration, _env);
                bllFinansUserTable.Delete(id);
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
        public async Task<ActionResult<object>> save([FromForm] FinansUserTableSaveDto entity)
        {

            if (entity != null)
            {
                int userId = 0;
                if (HttpContext.User.Identity is ClaimsIdentity claimsIdentity)
                {
                    userId = int.Parse(claimsIdentity?.FindFirst("userId")?.Value ?? "0");

                }
                BLL.BLLActions.FinansUserTable bllFinansUserTable = new BLL.BLLActions.FinansUserTable(_configuration, _env);

                if (entity?.id != 0)
                {

                    entity!.updateDate = DateTime.Now.ToString();
                    entity.updatedUserId = userId == 0 ? null : userId;
                    await bllFinansUserTable.Update(_mapper.Map<FinansUserTable>(entity));
                    return Ok(entity);
                }
                else
                {

                    entity!.createdDate = DateTime.Now.ToString();
                    entity.createdUserId = userId == 0 ? null : userId; ;
                    entity.enabled = true;
                    await bllFinansUserTable.Add(_mapper.Map<FinansUserTable>(entity));
                    return Ok(entity);
                }
            }
            return Ok(null);
        }
        #endregion
    }
}
