using AskalePortal.Data.Models;
using AskalePortal.Data.RequestModel;
using AskalePortal.Data.RequestParams;
using AskalePortal.Data.ResponseModels;
using AskalePortal.Data.ResponseParams;
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
        public class Ratings : BaseBLL<AskalePortal.Data.Models.Rating>
        {
            public Ratings(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
            {
            }
            #region GetAll

            public List<AskalePortal.Data.Models.Rating> GetAll(string title)
            {
                var q = dal.Get(k => (k.title.Contains(title) || string.IsNullOrEmpty(title)) && k.enabled == true).OrderByDescending(k => k.createdDate);

                return q.ToList();
            }

            public override List<AskalePortal.Data.Models.Rating> GetAll()
            {
                var q = dal.Get(k => k.enabled == true).OrderByDescending(k => k.createdDate);

                return q.ToList();
            }

            #endregion GetAll


            public PageReturn<RatingListDto>? FilterPageableDto(FilterPageParam<RatingDtoRequest> filterPageParam)
            {
                PageReturn<RatingListDto>? result = new PageReturn<RatingListDto>();
                int pageSize = filterPageParam.size ?? 20;
                int pageNumber = filterPageParam.page ?? 0;

                string? baslik = filterPageParam.liste?.filterbaslik;
               
                IQueryable<Rating> query = dal.Get(u => u.enabled &&
                baslik == null ? true :
                u.title.Contains(baslik!));
                result.content = query
                    .OrderByDescending(u=>u.Id)
                  .Skip(pageSize * pageNumber).Take(pageSize)

                    .Select(u => new RatingListDto()
                    {
                        createdDate=u.createdDate.ToString("dd.MM.yyyy"),
                        title =u.title,  
                        id=u.Id,
                        

                    }).ToList();
                result.totalElements = query.Count();
                result.number = result.content.Count();
                result.size = pageSize;

                return result;
            }
        }
    }    
}