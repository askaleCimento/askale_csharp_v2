
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
        public class PressAnnouncements : BaseBLL<AskalePortal.Data.Models.PressAnnouncement>
        {

            public PressAnnouncements(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
            {
            }
            #region GetAll

            public List<AskalePortal.Data.Models.PressAnnouncement> GetAll(string title)
            {
                var q = dal.Get(k => (k.title.Contains(title) || string.IsNullOrEmpty(title)) && k.enabled == true).OrderByDescending(k => k.createdDate);

                return q.ToList();
            }

            public List<AskalePortal.Data.Models.PressAnnouncement> GetAll(string title, int pageNumber, int pageSize)
            {
                var q = dal.Get(k => (k.title.Contains(title) || string.IsNullOrEmpty(title)) && k.enabled == true)
                    .OrderByDescending(k => k.createdDate).Skip(pageNumber * pageSize).Take(pageSize).ToList();

                return q;
            }

            public override List<AskalePortal.Data.Models.PressAnnouncement> GetAll()
            {
                var q = dal.Get(k => k.enabled == true).OrderByDescending(k => k.createdDate);

                return q.ToList();
            }

            public List<AskalePortal.Data.Models.PressAnnouncement> GetAllTake(int take)
            {
                var q = dal.Get(k => k.enabled == true).OrderByDescending(k => k.createdDate).Take(take);

                return q.ToList();
            }


            #endregion GetAll

            #region GetAllWithPage

            public List<AskalePortal.Data.Models.PressAnnouncement> GetAllWithPage(string searchQuery, int activePage, out double totalPages, out int totalRecords, int recordsPerPage = 10)
            {
                var q = dal.Get(k => (k.title.Contains(searchQuery) || string.IsNullOrEmpty(searchQuery)) &&
                                     k.enabled == true)
                                     .OrderBy(k => k.title)
                                     .GetPage(activePage, recordsPerPage, out totalPages, out totalRecords);
                return q.ToList();
            }

            #endregion GetAllWithPage


            public PageReturn<PressAnnouncementDto>? FilterPageableDto(FilterPageParam<PressAnnouncementDtoParameter> filterPageParam)
            {
                PageReturn<PressAnnouncementDto>? result = new PageReturn<PressAnnouncementDto>();
                int pageSize = filterPageParam.size ?? 10;
                int pageNumber = filterPageParam.page ?? 0;



                IQueryable<PressAnnouncement> query = dal.Get(k => k.enabled == true);
                result.content = query
                  .Skip(pageSize * pageNumber).Take(pageSize)

                    .Select(u => new PressAnnouncementDto()
                    {
                        id = u.Id,
                        createdByUserName = u.createdByUserName,
                        description = u.description,
                        imageUrl = u.imageUrl,
                        newsDate = u.newsDate,
                        title = u.title,



                    }).ToList();
                result.totalElements = query.Count();
                result.number = result.content.Count();
                result.size = pageSize;

                return result;
            }

            public List<PressAnnouncement>? ListTop8Picture()
            {
                
                List<PressAnnouncement>? list = dal.Get(k => k.enabled == true).OrderByDescending(k => k.newsDate).Take(8).ToList();
                return list;
            }
        }
    }
}