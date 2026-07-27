using AskalePortal.Data.Models;
using AskalePortal.Data.RequestModel;
using AskalePortal.Data.RequestParams;
using AskalePortal.Data.ResponseModels;
using AskalePortal.Data.ResponseParams;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AskalePortal.BLL
{
    public partial class BLLActions
    {
        public class IncomingDocumentTypes : BaseBLL<AskalePortal.Data.Models.IncomingDocumentType>
        {
            public IncomingDocumentTypes(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
            {
            }
            #region GetAll

            public List<AskalePortal.Data.Models.IncomingDocumentType> GetAll(string title)
            {
                var q = dal.Get(k => (k.title.Contains(title) || string.IsNullOrEmpty(title)) && k.enabled == true).OrderBy(k => k.title);

                return q.ToList();
            }

            public override List<AskalePortal.Data.Models.IncomingDocumentType> GetAll()
            {
                var q = dal.Get(k => k.enabled == true).OrderBy(k => k.title);

                return q.ToList();
            }

            #endregion GetAll

            #region GetAllWithPage

            public List<AskalePortal.Data.Models.IncomingDocumentType> GetAllWithPage(string searchQuery, int activePage, out double totalPages, out int totalRecords, int recordsPerPage = 10)
            {
                var q = dal.Get(k => (k.title.Contains(searchQuery) || string.IsNullOrEmpty(searchQuery)) &&
                                     k.enabled == true)
                                     .OrderBy(k => k.title)
                                     .GetPage(activePage, recordsPerPage, out totalPages, out totalRecords);
                return q.ToList();
            }

            public PageReturn<ComingDocumentTypeDto> listByPageable(FilterPageParam<PressAnnouncementDtoParameter> filterPageParam)
            {
                PageReturn<ComingDocumentTypeDto>? result = new PageReturn<ComingDocumentTypeDto>();
                int pageSize = filterPageParam.size ?? 20;
                int pageNumber = filterPageParam.page ?? 0;

                string? title = filterPageParam.liste?.title;
                var query = from u in dal.Get(u => u.enabled &&
                       (string.IsNullOrEmpty(title) || u.title.Contains(title)))
                            join admin in dal.dB.AdminUser on u.createdUserId equals admin.Id into adminJoin
                            from admin in adminJoin.DefaultIfEmpty()
                            orderby u.Id descending
                            select new ComingDocumentTypeDto()
                            {
                                enabled = u.enabled,
                                olusturanKisi = admin != null ? admin.name : "",
                                olusturmaTarihi = u.createdDate,
                                id = u.Id,
                                title = u.title
                            };

                //IQueryable<IncomingDocumentType> query = dal.Get(u => u.enabled &&
                // (title == null || title == "" ? true : u.title.Contains(title)));
                result.content = query
    .Skip(pageSize * pageNumber)
    .Take(pageSize)
    .ToList();

                result.totalElements = query.Count();
                result.number = result.content.Count();
                result.size = pageSize;

                return result;
            }

            #endregion GetAllWithPage
        }
    }


}
