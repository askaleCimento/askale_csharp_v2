using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AskalePortal.BLL
{
    public partial class BLLActions
    {
        public class FactoryManagerMeetings : BaseBLL<AskalePortal.Data.Models.FactoryManagerMeeting>
        {
            public FactoryManagerMeetings(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
            {
            }
            #region GetAll

            public override List<AskalePortal.Data.Models.FactoryManagerMeeting> GetAll()
            {
                var q = dal.Get(k =>  k.enabled == true)
                                     .OrderByDescending(k=>k.meetingDate);
                return q.ToList();
            }

            #endregion GetAll

            #region GetByDate

            public List<AskalePortal.Data.Models.FactoryManagerMeeting> GetByDate(DateTime dt)
            {
                var q = dal.Get(k => k.meetingDate!.Value.Date == dt.Date && k.enabled == true)
                                     .OrderByDescending(k => k.meetingDate);
                return q.ToList();
            }

            #endregion GetByDate
        }

		
	}
}