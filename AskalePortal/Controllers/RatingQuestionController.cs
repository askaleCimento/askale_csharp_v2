using AskalePortal.Data.Models;
using AskalePortal.Data.ResponseModels;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using static AskalePortal.BLL.BLLActions;

namespace AskalePortal.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RatingQuestionController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;
        public RatingQuestionController(IConfiguration configuration, IWebHostEnvironment env, IMapper mapper)
        {
            _configuration = configuration;
            _env = env;
            _mapper = mapper;
        }


        #region Save
        [HttpPost("save")]
        public async Task<ActionResult<object>> save([FromForm] RatingQuestionDto entity)
        {
            if (entity != null)
            {
                int userId = 0;
                if (HttpContext.User.Identity is ClaimsIdentity claimsIdentity)
                {
                    userId = int.Parse(claimsIdentity?.FindFirst("userId")?.Value ?? "0");

                }
                BLL.BLLActions.RatingQuestions bllRatingQuestion = new BLL.BLLActions.RatingQuestions(_configuration, _env);
                RatingQuestion ratingQuestion = _mapper.Map<RatingQuestion>(entity);

                if (ratingQuestion?.Id != 0)
                {

                    ratingQuestion!.updatedDate = DateTime.Now;
                    ratingQuestion.updatedUserId = userId == 0 ? null : userId;
                    await bllRatingQuestion.Update(ratingQuestion);
                    return Ok(ratingQuestion);
                }
                else
                {

                    ratingQuestion!.createdDate = DateTime.Now;
                    ratingQuestion.createdUserId = userId == 0 ? null : userId; ;
                    ratingQuestion.enabled = true;
                    await bllRatingQuestion.Add(ratingQuestion);
                    return Ok(ratingQuestion);
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
                BLL.BLLActions.RatingQuestions bllRatingQuestion = new BLL.BLLActions.RatingQuestions(_configuration, _env);
                bllRatingQuestion.Delete(id);
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
            BLL.BLLActions.RatingQuestions bllRatingQuestion = new BLL.BLLActions.RatingQuestions(_configuration, _env);

            RatingQuestion? ratingQuestion = bllRatingQuestion.GetByID(id);
            if (ratingQuestion == null)
            {
                return NotFound();
            }
            return Ok(ratingQuestion);


        }
        #endregion


        #region getAll
        [HttpPost("getAll")]

        public ActionResult<object> getAll()
        {
            BLL.BLLActions.RatingQuestions bllRatingQuestion = new BLL.BLLActions.RatingQuestions(_configuration, _env);

            List<RatingQuestion>? listRatingQuestion = bllRatingQuestion.GetAll();
            return Ok(listRatingQuestion);

        }
        #endregion

        #region listRatingId
        [HttpPost("listRatingId")]

        public ActionResult<object> listByRatingId([FromForm] int ratingId)
        {
            BLL.BLLActions.RatingQuestions bllRatingQuestion = new BLL.BLLActions.RatingQuestions(_configuration, _env);

            List<RatingQuestion> liste = bllRatingQuestion.GetByRatingID(ratingID: ratingId);
            return Ok(liste);
        }

        #endregion

    }
}
