using AskalePortal.Data.Contracts.Detached;
using AskalePortal.BLL;
using AskalePortal.Data.Functions;
using AskalePortal.Data.Models;
using AskalePortal.Data.RequestParams;
using AskalePortal.Data.ResponseModels;
using AskalePortal.Data.ResponseParams;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AskalePortal.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HRAnnouncementController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;

        public HRAnnouncementController(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _env = env;
        }

        [HttpPost("save")]
        public async Task<ActionResult<object>> save([FromForm] HRAnnouncementDto request)
        {
            HRAnnouncement entity = request.ToEntity<HRAnnouncement>();
            int userId = 0;
            if (HttpContext.User.Identity is ClaimsIdentity claimsIdentity)
            {
                userId = int.Parse(claimsIdentity.FindFirst("userId")?.Value ?? "0");
            }

            BLLActions.HRAnnouncements bll = new BLLActions.HRAnnouncements(_configuration, _env);

            if (entity.Id > 0)
            {
                entity.updatedDate = DateTime.Now;
                entity.updatedUserId = userId == 0 ? null : userId;
                await bll.Update(entity);
            }
            else
            {
                entity.createdDate = DateTime.Now;
                entity.createdUserId = userId;
                entity.enabled = true;
                await bll.Add(entity);
            }

            return Ok(entity);
        }

        [HttpPost("delete")]
        public ActionResult<int> delete([FromForm] int id)
        {
            try
            {
                BLLActions.HRAnnouncements bll = new BLLActions.HRAnnouncements(_configuration, _env);
                bll.Delete(id);
                return Ok(1);
            }
            catch
            {
                return Ok(0);
            }
        }

        [HttpPost("getById")]
        public ActionResult<object> getById([FromForm] int id)
        {
            BLLActions.HRAnnouncements bll = new BLLActions.HRAnnouncements(_configuration, _env);
            HRAnnouncement? entity = bll.GetByID(id);
            return entity == null ? NotFound() : Ok(entity);
        }

        [HttpPost("getAll")]
        public ActionResult<object> getAll()
        {
            BLLActions.HRAnnouncements bll = new BLLActions.HRAnnouncements(_configuration, _env);
            return Ok(bll.GetAll());
        }

        [HttpPost("filterPageableList")]
        public ActionResult<object> filterPageableList(
            [FromForm] FilterPageParam<HRAnnouncementFilterParameter> filterPageParam)
        {
            BLLActions.HRAnnouncements bll = new BLLActions.HRAnnouncements(_configuration, _env);

            string searchText = filterPageParam.liste?.filterName?.Trim() ?? string.Empty;
            int page = Math.Max(filterPageParam.page ?? 0, 0);
            int size = Math.Clamp(filterPageParam.size ?? 10, 1, 100);

            IQueryable<HRAnnouncement> query = bll.GetAll(searchText).AsQueryable();
            PageReturn<HRAnnouncement> result = new PageReturn<HRAnnouncement>().GetPage(query, page, size);
            result.numberOfElements = result.content?.Count ?? 0;
            result.empty = result.numberOfElements == 0;
            result.first = page == 0;
            result.last = result.totalPages == 0 || page >= result.totalPages - 1;
            result.pageable = new Pageable
            {
                pageNumber = page,
                pageSize = size,
                offset = page * size,
                paged = true,
                unpaged = false
            };
            result.sort = new Sort { sorted = false, unsorted = true, empty = true };

            return Ok(result);
        }

        [HttpGet("download")]
        public ActionResult<ResponseByteArray> download([FromQuery] string file)
        {
            if (string.IsNullOrWhiteSpace(file))
            {
                return BadRequest();
            }

            string filePath = Path.Combine(
                _env.IsDevelopment() ? _configuration["FilePath:local"]! :
                _env.IsProduction() ? _configuration["FilePath:server"]! :
                _configuration["FilePath:test"]!,
                "uploads\\");

            return Ok(FileConverter.convertByte(filePath, file, file));
        }
    }

    public class HRAnnouncementFilterParameter
    {
        public string? filterName { get; set; }
    }
}
