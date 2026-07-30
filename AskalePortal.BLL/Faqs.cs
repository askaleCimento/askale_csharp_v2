using AskalePortal.Data.Models;
using AskalePortal.Data.RequestModel;
using AskalePortal.Data.RequestParams;
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
        public class Faqs : BaseBLL<AskalePortal.Data.Models.Faq>
        {
            public Faqs(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
            {
            }
            #region GetAll

            public List<AskalePortal.Data.Models.Faq> GetAll(string title)
            {
                var q = dal.Get(k => (k.title.Contains(title) || string.IsNullOrWhiteSpace(title)) && k.enabled == true).OrderBy(k => k.title);

                return q.ToList();
            }

            public override List<AskalePortal.Data.Models.Faq> GetAll()
            {
                var q = dal.Get(k => k.enabled == true).OrderBy(k => k.title).OrderByDescending(u => u.Id);

                return q.ToList();
            }

            public List<Faq>? GetAllFilter(FilterParam<HelpDeskStatusListDtoParameter> filterParam)
            {
                string? title = filterParam.liste?.title;
                List<Faq> liste = dal.Get(u => u.enabled && (string.IsNullOrEmpty(title) ? true : u.title.ToLower().Contains(title))).OrderByDescending(u=> u.Id).ToList();
                return liste;
            }

            public List<AskalePortal.Data.Models.Faq> GetAllTake(int take)
            {
                var q = dal.Get(k => k.enabled == true).OrderByDescending(k => k.Id).Take(take);

                return q.ToList();
            }
			 
			

            #endregion GetAll

            #region GetAllWithPage

            public List<AskalePortal.Data.Models.Faq> GetAllWithPage(string searchQuery, int activePage, out double totalPages, out int totalRecords, int recordsPerPage = 10)
            {
                var q = dal.Get(k => (k.title.Contains(searchQuery) || string.IsNullOrEmpty(searchQuery)) &&
                                     k.enabled == true)
                                     .OrderBy(k => k.title)
                                     .GetPage(activePage, recordsPerPage, out totalPages, out totalRecords);
                return q.ToList();
            }

            #endregion GetAllWithPage


            public PageReturn<Faq>? FilterPageableDto(FilterPageParam<UserFilterDtoRequest> filterPageParam)
            {
                PageReturn<Faq>? result = new PageReturn<Faq>();
                int pageSize = filterPageParam.size ?? 20;
                int pageNumber = filterPageParam.page ?? 0;

               
                IQueryable<Faq> query = dal.Get(u => u.enabled ).OrderByDescending(u => u.Id);
                result.content = query
                  .Skip(pageSize * pageNumber).Take(pageSize)

                    .ToList();
                result.totalElements = query.Count();
                result.number = result.content.Count();
                result.size = pageSize;

                return result;
            }

        }
    }

    
}
