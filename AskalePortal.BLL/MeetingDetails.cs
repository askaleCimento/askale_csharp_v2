using AskalePortal.Data.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AskalePortal.BLL
{
    public partial class BLLActions
    {
        public class MeetingDetails : BaseBLL<AskalePortal.Data.Models.MeetingDetail>
        {
            public MeetingDetails(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
            {
            }
            #region GetAll

            public override List<AskalePortal.Data.Models.MeetingDetail> GetAll()
            {
                var q = dal.Get(k =>  k.enabled == true)
                                     .OrderByDescending(k => k.Id);
                return q.ToList();
            }

            public List<AskalePortal.Data.Models.MeetingDetail> GetAll(DateTime? meetingDate, int? itemStatus, string detailNumber)
            {
                var q = dal.Get(k => k.enabled == true && 
                ((k.meetingDate.Value.Year == meetingDate.Value.Year && k.meetingDate.Value.Month == meetingDate.Value.Month && k.meetingDate.Value.Day == meetingDate.Value.Day) || meetingDate == null) &&
                                    (k.itemStatus == itemStatus || itemStatus == null || itemStatus == 0) &&
                                    (k.detailNumber == detailNumber || string.IsNullOrEmpty(detailNumber)))
                                     .OrderByDescending(k => k.Id);
                return q.ToList();
            }

            #endregion GetAll

            #region GetAll

            public List<AskalePortal.Data.Models.MeetingDetail> GetByMeetingID(int meetingID)
            {
                var q = dal.Get(k => k.meetingId == meetingID && k.enabled == true)
                                     .OrderByDescending(k => k.Id);
                return q.ToList();
            }

            public List<MeetingDetail> listByMeetingId(int meetingId)
            {
                List<MeetingDetail> liste = dal.Get(u => u.enabled && u.meetingId == meetingId).ToList();
                return liste;
            }

            #endregion GetAll

        }
    }
}