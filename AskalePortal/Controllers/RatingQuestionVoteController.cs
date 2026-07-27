using AskalePortal.Data.Models;
using AskalePortal.Data.ResponseModels;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using System.Collections.Generic;
using System.Security.Claims;
using static AskalePortal.BLL.BLLActions;

namespace AskalePortal.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RatingQuestionVoteController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;
        public RatingQuestionVoteController(IConfiguration configuration, IWebHostEnvironment env, IMapper mapper)
        {
            _configuration = configuration;
            _env = env;
            _mapper = mapper;
        }
        #region listRatingId
        [HttpPost("listRatingId")]
        public ActionResult<object> listByRatingId([FromForm] int ratingId,
            [FromForm] int userId)
        {
            BLL.BLLActions.RatingQuestionVotes bllRatingQuestionVotes = new BLL.BLLActions.RatingQuestionVotes(_configuration, _env, _mapper);

            List<RatingQuestionVote> liste = bllRatingQuestionVotes.GetByRatingId(userID: userId, ratingID: ratingId);
            return Ok(liste);
        }
        #endregion

        #region Save
        [HttpPost("save")]
        public async Task<ActionResult<RatingQuestionVoteDto?>> save([FromForm] RatingQuestionVoteDto entity)
        {

            if (entity != null)
            {
                int userId = 0;
                if (HttpContext.User.Identity is ClaimsIdentity claimsIdentity)
                {
                    userId = int.Parse(claimsIdentity?.FindFirst("userId")?.Value ?? "0");

                }
                BLL.BLLActions.RatingQuestionVotes bllRatingQuestionVote = new BLL.BLLActions.RatingQuestionVotes(_configuration, _env, _mapper);

                RatingQuestionVote ratingQuestionVote = new RatingQuestionVote();
                ratingQuestionVote.comment = entity.comment;
                ratingQuestionVote.ratingValue = entity.ratingValue??0;
                ratingQuestionVote.Id = entity.id??0;
                ratingQuestionVote.ratingId = entity.ratingId??0;
                ratingQuestionVote.userId = entity.userId??0;
                ratingQuestionVote.questionId = entity.questionId??0;

                if (ratingQuestionVote?.Id != 0)
                {

                    ratingQuestionVote!.updatedDate = DateTime.Now;
                    ratingQuestionVote.updatedUserId = userId == 0 ? null : userId;
                    await bllRatingQuestionVote.Update(ratingQuestionVote);
                    return Ok(ratingQuestionVote);
                }
                else
                {

                    ratingQuestionVote!.createdDate = DateTime.Now;
                    ratingQuestionVote.createdUserId = userId == 0 ? null : userId; ;
                    ratingQuestionVote.enabled = true;
                    await bllRatingQuestionVote.Add(ratingQuestionVote);
                    return Ok(ratingQuestionVote);
                }
            }
            return Ok(null);
        }
        #endregion


        #region listChartData
        [HttpPost("listChartData")]
         public ActionResult<List<DoubleAndStringDto>> listChartData([FromForm] int ratingId,
            [FromForm] int filterCompanyId)
        {
            BLL.BLLActions.RatingQuestionVotes bllRatingQuestionVotes = new BLL.BLLActions.RatingQuestionVotes(_configuration, _env, _mapper);

            List<DoubleAndStringDto>? liste = bllRatingQuestionVotes.getChartData(ratingId, filterCompanyId);
            return Ok(liste ?? []);
        }
        #endregion

        #region listChartData
        [HttpPost("listAnswers")]
        public ActionResult<List<Data.ResponseModels.ResultDetailDto>> listByAnswers([FromForm] int ratingId)
        {
            BLL.BLLActions.RatingQuestionVotes bllRatingQuestionVotes = new BLL.BLLActions.RatingQuestionVotes(_configuration, _env, _mapper);
            List<Data.ResponseModels.ResultDetailDto> liste = bllRatingQuestionVotes.listAnswers(ratingId);
            return Ok(liste);
        }
        #endregion
    }
}
