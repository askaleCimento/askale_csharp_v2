using AskalePortal.Data.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AskalePortal.BLL
{
    public partial class BLLActions
    {
        public class RatingQuestions : BaseBLL<AskalePortal.Data.Models.RatingQuestion>
        {
            public RatingQuestions(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
            {
            }
            #region GetAll

            public List<AskalePortal.Data.Models.RatingQuestion> GetAll(int ratingID, bool? approval, string title)
            {
                var q = dal.Get(k => k.ratingId == ratingID && (k.approval == approval || approval == null) && (k.title.Contains(title) || string.IsNullOrEmpty(title)) && k.enabled == true).OrderBy(k => k.Id);

                return q.ToList();
            }

            public List<AskalePortal.Data.Models.RatingQuestion> GetByRatingID( int ratingID)
            {
                var q = dal.Get(k => k.ratingId == ratingID  && k.enabled == true).OrderBy(k => k.Id);
                
                return q.ToList();
            }
            public List<AskalePortal.Data.Models.RatingQuestion> GetByRatingIDNotComment(bool? approval, int ratingID)
            {
                var q = dal.Get(k => k.ratingId == ratingID && (k.approval == approval || approval == null) && k.RatingQuestionVote.Where(u=>u.ratingValue>0).Any() && k.enabled == true).OrderBy(k => k.Id);

                return q.ToList();
            }

            public List<RatingQuestion> findByEnabledAndRatingId(bool enabled, int ratingId)
            {
                return dal.Get(u=>u.enabled==enabled && u.ratingId==ratingId).ToList();
            }
            #endregion GetAll
        }
    }    
}