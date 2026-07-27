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
        public class IncomingDocumentSources : BaseBLL<AskalePortal.Data.Models.IncomingDocumentSource>
        {
            public IncomingDocumentSources(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
            {
            }
            #region GetAll

            public List<AskalePortal.Data.Models.IncomingDocumentSource> GetAll(string title)
            {
                var q = dal.Get(k => (k.title.Contains(title) || string.IsNullOrEmpty(title)) && k.enabled == true).OrderBy(k => k.title);

                return q.ToList();
            }

            public override List<AskalePortal.Data.Models.IncomingDocumentSource> GetAll()
            {
                var q = dal.Get(k => k.enabled == true).OrderBy(k => k.title);

                return q.ToList();
            }

            #endregion GetAll

            #region GetAllWithPage

            public List<AskalePortal.Data.Models.IncomingDocumentSource> GetAllWithPage(string searchQuery, int activePage, out double totalPages, out int totalRecords, int recordsPerPage = 10)
            {
                var q = dal.Get(k => (k.title.Contains(searchQuery) || string.IsNullOrEmpty(searchQuery)) &&
                                     k.enabled == true)
                                     .OrderBy(k => k.title)
                                     .GetPage(activePage, recordsPerPage, out totalPages, out totalRecords);
                return q.ToList();
            }
            #endregion GetAllWithPage
            public PageReturn<ComingDocumentSourceDto> listByPageable(FilterPageParam<PressAnnouncementDtoParameter> filterPageParam)
            {

                PageReturn<ComingDocumentSourceDto>? result = new PageReturn<ComingDocumentSourceDto>();
                int pageSize = filterPageParam.size ?? 20;
                int pageNumber = filterPageParam.page ?? 0;

                string? title = filterPageParam.liste?.title;

                IQueryable<IncomingDocumentSource> query = dal.Get(u => u.enabled &&
                 (title == null || title == "" ? true : u.title.Contains(title))).OrderByDescending(u => u.Id);
                result.content = query
                  .Skip(pageSize * pageNumber).Take(pageSize)

                    .Select(u => new ComingDocumentSourceDto()
                    {
                        fax = u.fax,
                        id = u.Id,
                        title = u.title,
                        phone = u.phone,
                        subject = u.subject,
                        subTitle = u.subTitle


                    }).ToList();
                result.totalElements = query.Count();
                result.number = result.content.Count();
                result.size = pageSize;

                return result;
            }


        }
    }


}
