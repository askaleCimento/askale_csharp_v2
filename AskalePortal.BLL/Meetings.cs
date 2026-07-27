using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AskalePortal.BLL
{
    public partial class BLLActions
    {
        public class Meetings : BaseBLL<AskalePortal.Data.Models.Meeting>
        {
            public Meetings(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
            {
            }
            #region GetAll

            public override List<AskalePortal.Data.Models.Meeting> GetAll()
            {
                var q = dal.Get(k =>  k.enabled == true)
                                     .OrderByDescending(k=>k.meetingDate);
                return q.ToList();
            }

            #endregion GetAll

            #region GetByDate

            public List<AskalePortal.Data.Models.Meeting> GetByDate(DateTime dt)
            {
                var q = dal.Get(k => k.meetingDate.Value.Date == dt.Date && k.enabled == true)
                                     .OrderByDescending(k => k.meetingDate);
                return q.ToList();
            }

            #endregion GetByDate
        }

    
    }
}