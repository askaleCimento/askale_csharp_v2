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
        public class Announcements : BaseBLL<AskalePortal.Data.Models.Announcement>
        {
            public Announcements(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
            {
            }
            #region GetAll

            public List<AskalePortal.Data.Models.Announcement> GetAll(string title)
            {
                var q = dal.Get(k => (k.title.Contains(title) || string.IsNullOrWhiteSpace(title)) && k.enabled == true).OrderBy(k => k.title);

                return q.ToList();
            }

            public override List<AskalePortal.Data.Models.Announcement> GetAll()
            {
                var q = dal.Get(k => k.enabled == true).OrderByDescending(k => k.createdDate);

                return q.ToList();
            }

            public List<AskalePortal.Data.Models.Announcement> GetAllTake(int take)
            {
                var q = dal.Get(k => k.enabled == true).OrderByDescending(k => k.createdDate).Take(take);

                return q.ToList();
            }


            #endregion GetAll
        }
    }    
}