using AskalePortal.Data.Models;
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
        public class FactoryManagerMeetingDetails : BaseBLL<AskalePortal.Data.Models.FactoryManagerMeetingDetail>
        {
            public FactoryManagerMeetingDetails(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
            {
            }
            #region GetAll

            public override List<AskalePortal.Data.Models.FactoryManagerMeetingDetail> GetAll()
            {
                var q = dal.Get(k =>  k.enabled == true)
                                     .OrderByDescending(k => k.Id);
                return q.ToList();
            }

            public List<AskalePortal.Data.Models.FactoryManagerMeetingDetail> GetAll(DateTime? meetingDate, int? itemStatus, string detailNumber)
            {
                var q = dal.Get(k => k.enabled == true && 
                ((k.meetingDate!.Value.Year == meetingDate!.Value.Year && k.meetingDate.Value.Month == meetingDate.Value.Month && k.meetingDate.Value.Day == meetingDate.Value.Day) || meetingDate == null) &&
                                    (k.itemStatus == itemStatus || itemStatus == null || itemStatus == 0) &&
                                    (k.detailNumber == detailNumber || string.IsNullOrEmpty(detailNumber)))
                                     .OrderByDescending(k => k.Id);
                return q.ToList();
            }

            public List<FactoryManagerMeetingDetailSaveDto> GetAllDto()
            {
                var q = dal.Get(k => k.enabled == true)
                                     .OrderByDescending(k => k.Id).Select(u => new FactoryManagerMeetingDetailSaveDto
                                     {
                                         users = u.users,
                                         updatedUserId = u.updatedUserId,
                                         completedDate = u.completedDate.HasValue
    ? u.completedDate.Value.ToString("dd.MM.yyyy")
    : "",
                                         completedNote = u.completedNote,
                                         copyFromMeetingDetailId = u.copyFromMeetingDetailId,
                                         copyFromMeetingId = u.copyFromMeetingId,
                                         createdDate = u.createdDate.HasValue ? u.createdDate.Value.ToString("dd.MM.yyyy"):"",
                                         createdUserId = u.createdUserId,
                                         description = u.description,
                                         detailNumber = u.detailNumber,
                                         enabled = u.enabled,
                                         id = u.Id,
                                         itemStatus = u.itemStatus,
                                         meetingDate = u.meetingDate.HasValue ? u.meetingDate.Value.ToString("dd.MM.yyyy") : "",
                                         meetingId = u.meetingId,
                                         plannedDate = u.plannedDate.HasValue ? u.plannedDate.Value.ToString("dd.MM.yyyy") : "",
                                         title = u.title,
                                         updateDate =  u.updatedDate.ToString(),
                                     });
                return q.ToList();
            }

            #endregion GetAll

            #region GetAll

            public List<AskalePortal.Data.Models.FactoryManagerMeetingDetail> GetByMeetingID(int meetingID)
            {
                var q = dal.Get(k => k.meetingId == meetingID && k.enabled == true)
                                     .OrderByDescending(k => k.Id);
                return q.ToList();
            }

            #endregion GetAll


            public List<FactoryManagerMeetingDetail> listByMeetingId(int meetingId)
            {
                List<FactoryManagerMeetingDetail> liste = dal.Get(u => u.enabled && u.meetingId == meetingId).ToList();
                return liste;
            }

        }
    }
}