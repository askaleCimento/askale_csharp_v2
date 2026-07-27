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
        public class HRAnnouncements : BaseBLL<AskalePortal.Data.Models.HRAnnouncement>
        {
            public HRAnnouncements(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
            {
            }
            #region GetAll

            public List<AskalePortal.Data.Models.HRAnnouncement> GetAll(string title)
            {
                var q = dal.Get(k => (k.title.Contains(title) || string.IsNullOrEmpty(title)) && k.enabled == true).OrderByDescending(k => k.createdDate);
                return q.ToList();
            }

            public override List<AskalePortal.Data.Models.HRAnnouncement> GetAll()
            {
                var q = dal.Get(k => k.enabled == true).OrderByDescending(k => k.createdDate);

                return q.ToList();
            }

            public List<AskalePortal.Data.Models.HRAnnouncement> GetAllTake(int take)
            {
                var q = dal.Get(k => k.enabled == true).OrderByDescending(k => k.createdDate).Take(take);

                return q.ToList();
            }


            #endregion GetAll

            #region GetAllWithPage

            public List<AskalePortal.Data.Models.HRAnnouncement> GetAllWithPage(string searchQuery, int activePage, out double totalPages, out int totalRecords, int recordsPerPage = 10)
            {
                var q = dal.Get(k => (k.title.Contains(searchQuery) || string.IsNullOrEmpty(searchQuery)) &&
                                     k.enabled == true)
                                     .OrderBy(k => k.title)
                                     .GetPage(activePage, recordsPerPage, out totalPages, out totalRecords);
                return q.ToList();
            }

            #endregion GetAllWithPage
        }
    }    
}