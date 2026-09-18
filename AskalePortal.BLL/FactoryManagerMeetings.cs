using AskalePortal.Data.ResponseModels;
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

            public List<FactoryManagerMeetingSaveDto>? GetAllDto()
            {
                var q = dal.Get(k => k.enabled == true)
                                     .OrderByDescending(k => k.Id).Select(u=> new FactoryManagerMeetingSaveDto
                                     {
                                         createdDate= u.createdDate.HasValue ? u.createdDate.Value.ToString("dd.MM.yyyy"):"",
                                         createdUserId=u.createdUserId,
                                         enabled=u.enabled,
                                         id=u.Id,
                                         meetingDate=u.meetingDate.HasValue ? u.meetingDate.Value.ToString("dd.MM.yyyy"):"",
                                         meetingPlace= u.meetingPlace,
                                         updateDate=u.updatedDate.ToString(),
                                         updatedUserId= u.updatedUserId,
                                         users = u.users
                                     });
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