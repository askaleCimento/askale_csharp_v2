using AskalePortal.Data.Contracts.Detached;
using AskalePortal.Data.RequestModel;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

using ActiveProcessChecksBll =
    AskalePortal.BLL.BLLActions.ActiveProcessChecks;

using ActiveProcessChecksEntity =
    AskalePortal.Data.Models.ActiveProcessChecks;

namespace AskalePortal.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActiveProcessChecksController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;

        public ActiveProcessChecksController(
            IConfiguration configuration,
            IWebHostEnvironment env,
            IMapper mapper)
        {
            _configuration = configuration;
            _env = env;
            _mapper = mapper;
        }

        private ActiveProcessChecksBll CreateBll()
        {
            return new ActiveProcessChecksBll(
                _configuration,
                _env,
                _mapper);
        }

        private int GetCurrentUserId()
        {
            if (HttpContext.User.Identity is not ClaimsIdentity claimsIdentity)
            {
                return 0;
            }

            return int.TryParse(
                claimsIdentity.FindFirst("userId")?.Value,
                out var userId)
                ? userId
                : 0;
        }

        #region Save

        [HttpPost("save")]
        public async Task<ActionResult<ActiveProcessChecksDto>> Save(
            [FromForm] ActiveProcessChecksDto request)
        {
            var entity =
                request.ToEntity<ActiveProcessChecksEntity>();

            var userId = GetCurrentUserId();
            var bll = CreateBll();

            if (entity.id is not null)
            {
                entity.updateDate = DateTime.Now;
                entity.updatedUserId =
                    userId == 0 ? null : userId;

                await bll.Update(entity);
            }
            else
            {
                entity.createdDate = DateTime.Now;
                entity.createdUserId =
                    userId == 0 ? null : userId;

                entity.enabled = true;

                await bll.Add(entity);
            }

            return Ok(
                DetachedDtoMapper.ToDetached(entity));
        }

        #endregion

        #region Delete

        [HttpPost("delete")]
        public ActionResult<int> Delete([FromForm] int id)
        {
            try
            {
                var bll = CreateBll();
                bll.Delete(id);

                return Ok(1);
            }
            catch
            {
                return Ok(0);
            }
        }

        #endregion

        #region GetById

        [HttpPost("getById")]
        public ActionResult<ActiveProcessChecksDto> GetById(
            [FromForm] int id)
        {
            var bll = CreateBll();

            ActiveProcessChecksEntity? entity =
                bll.GetByID(id);

            if (entity is null)
            {
                return NotFound();
            }

            return Ok(
                DetachedDtoMapper.ToDetached(entity));
        }

        #endregion

        #region GetAll

        [HttpPost("getAll")]
        public ActionResult<List<ActiveProcessChecksDto>> GetAll()
        {
            var bll = CreateBll();

            List<ActiveProcessChecksEntity> entities =
                bll.GetAll() ?? [];

            var response = entities
                .Select(entity =>
                    (ActiveProcessChecksDto)
                    DetachedDtoMapper.ToDetached(entity)!)
                .ToList();

            return Ok(response);
        }

        #endregion

        #region GetCheckList

        [HttpPost("getCheckList")]
        public ActionResult<List<ActiveProcessChecksDto>> GetCheckList(
            [FromForm] string bukrs,
            [FromForm] string kunnr,
            [FromForm] string portfo)
        {
            var bll = CreateBll();

            List<ActiveProcessChecksEntity> entities =
                bll.getCheckList(
                    bukrs,
                    kunnr,
                    portfo) ?? [];

            var response = entities
                .Select(entity =>
                    (ActiveProcessChecksDto)
                    DetachedDtoMapper.ToDetached(entity)!)
                .ToList();

            return Ok(response);
        }

        #endregion

        #region SaveCheckList

        [HttpPost("saveCheckList")]
        public async Task<ActionResult<bool>> SaveCheckList(
            [FromForm] SaveCheckListRequestDto request)
        {
            var bll = CreateBll();
            var userId = GetCurrentUserId();

            var result = await bll.saveCheckList(
                request.listActiveProcessChecks ?? [],
                request.activeProcessId ?? 0,
                userId);

            return Ok(result);
        }

        #endregion

        #region GetByActiveProcessId

        [HttpPost("getByActiveProcessId")]
        public ActionResult<List<ActiveProcessChecksDto>>
            GetByActiveProcessId(
                [FromForm] int activeProcessId)
        {
            var bll = CreateBll();

            List<ActiveProcessChecksEntity> entities =
                bll.getByActiveProcessId(
                    activeProcessId) ?? [];

            var response = entities
                .Select(entity =>
                    (ActiveProcessChecksDto)
                    DetachedDtoMapper.ToDetached(entity)!)
                .ToList();

            return Ok(response);
        }

        #endregion
    }
}