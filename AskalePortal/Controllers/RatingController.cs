using AskalePortal.BLL;
using AskalePortal.Data.Models;
using AskalePortal.Data.RequestModel;
using AskalePortal.Data.RequestParams;
using AskalePortal.Data.ResponseModels;
using AskalePortal.Data.ResponseParams;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AskalePortal.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RatingController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;
        public RatingController(IConfiguration configuration, IWebHostEnvironment env, IMapper mapper)
        {
            _configuration = configuration;
            _env = env;
            _mapper = mapper;
        }

        #region Save
        [HttpPost("save")]
        public async Task<ActionResult<object>> save([FromForm] RatingDto entity)
        {

            int userId = 0;
            if (HttpContext.User.Identity is ClaimsIdentity claimsIdentity)
            {
                userId = int.Parse(claimsIdentity?.FindFirst("userId")?.Value ?? "0");

            }
            BLLActions.Ratings bllRating = new BLLActions.Ratings(_configuration, _env);
            if (entity != null)
            {

               

                if (entity.id != null)
                {
                    entity!.updateDate = DateTime.Now.ToString();
                    entity.updatedUserId = userId == 0 ? null : userId;
                    Rating rating = _mapper.Map<Rating>(entity);

                    await bllRating.Update(rating);
                    return Ok(rating);
                }
                else
                {

                    entity.createdDate = DateTime.Now.ToString();
                    entity.createdUserId = userId == 0 ? null : userId; ;
                    entity.enabled = true;
                    Rating? rating = await bllRating.Add(_mapper.Map<Rating>(entity));

                    return Ok(rating);
                }
            }
            else
            {
                return Ok(null);
            }

        }
        #endregion

        #region delete
        [HttpPost("delete")]

        public ActionResult<int> delete([FromForm] int id)
        {
            try
            {
                BLLActions.Ratings bllRating = new BLLActions.Ratings(_configuration, _env);
                bllRating.Delete(id);
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
            BLLActions.Ratings bllRating = new BLLActions.Ratings(_configuration, _env);

            Rating? rating = bllRating.GetByID(id);
            if (rating == null)
            {
                return NotFound();
            }
            return Ok(rating);


        }
        #endregion


        #region getAll
        [HttpPost("getAll")]

        public ActionResult<object> getAll()
        {
            BLLActions.Ratings bllRating = new BLLActions.Ratings(_configuration, _env);

            List<Rating>? listRating = bllRating.GetAll();
            return Ok(listRating);

        }
        #endregion
        #region filterPageable
        [HttpPost("filterPageable")]

        public ActionResult<PageReturn<Data.ResponseModels.RatingListDto>?> filterPageableDto([FromForm] FilterPageParam<RatingDtoRequest> filterPageParam)
        {

            BLLActions.Ratings bllRatings = new BLLActions.Ratings(_configuration, _env);
            PageReturn<Data.ResponseModels.RatingListDto>? liste = bllRatings.FilterPageableDto(filterPageParam);
            return Ok(liste);
        }
        #endregion
    }
}
