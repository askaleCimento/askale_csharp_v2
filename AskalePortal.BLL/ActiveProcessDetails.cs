

using AskalePortal.Data.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AskalePortal.BLL
{
    public partial class BLLActions
    {
        public class ActiveProcessDetails : BaseBLL<AskalePortal.Data.Models.ActiveProcessDetail>
        {
            public ActiveProcessDetails(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
            {
            }

            public Data.Models.ActiveProcessDetail GetMyDetail(int activeProcessId, int userId)
            {
                var q = dal.Get(x => x.activeProcessId == activeProcessId && x.userId == userId && x.enabled == true).FirstOrDefault();
                return q ?? new Data.Models.ActiveProcessDetail();
            }

            public Data.Models.ActiveProcessDetail GetMyDetailWaiting(int activeProcessId, int userId)
            {
                var q = dal.Get(x => x.activeProcessId == activeProcessId && x.userId == userId && x.isReplied == false && x.enabled == true).FirstOrDefault();
                return q ?? new Data.Models.ActiveProcessDetail();
            }

            public Data.Models.ActiveProcessDetail GetByGuid(string guid, int userId)
            {
                var q = dal.Get(x => x.guid.ToString() == guid && x.userId == userId && x.enabled == true).FirstOrDefault();
                return q ?? new Data.Models.ActiveProcessDetail();
            }
            public List<Data.Models.ActiveProcessDetail> GetByActiveId(int Id)
            {
                return dal.Get(x => x.activeProcessId == Id && x.enabled == true).ToList();

            }
            public List<Data.Models.ActiveProcess> GetAboutMe(int userId, int typeId)
            {
                var q = dal.Get(x => (x.userId == userId || x.activeProcess.createdUserId == userId) && x.activeProcess.approvalProcess.typeId == typeId && x.approved != true && x.approved != false && x.activeProcess.enabled == true).OrderByDescending(c => c.createdDate).Select(v => v.activeProcess).ToList();
                return q;
            }

            public List<Data.Models.ActiveProcess> GetAboutMe(int? currentStateId, int userId, int[] typeId)
            {
                List<int> activeprocessId = dal.Get(u => (u.userId == userId || u.activeProcess.createdUserId == userId) && u.activeProcess.currentStateId == currentStateId && typeId.Contains(u.activeProcess.approvalProcess.typeId) && u.activeProcess.enabled == true).Select(u => u.activeProcessId).ToList();

                var q = dal.Get(x => (x.userId == userId || x.activeProcess.currentUserId == userId || x.activeProcess.createdUserId == userId) && x.activeProcess.currentStateId == currentStateId && activeprocessId.Contains(x.activeProcessId)).OrderByDescending(c => c.createdDate).Select(v => v.activeProcess).Distinct().ToList();

                return q;
            }

            public int GetUser(int companyId, int yENI_MUSTERI)
            {
                throw new NotImplementedException();
            }

            public List<Data.Models.ActiveProcessDetail> GetByUserIdNotApproved(int vekaletverenId)
            {
                return dal.Get(u => u.userId == vekaletverenId && u.approved == null && u.isReplied == false && u.replyDate == null && u.enabled == true).ToList();
            }

            public ActiveProcessDetail? findByActiveProcessIdAndUserIdAndApprovedAndEnabled(int activeProcessId, int userId, bool? approved, bool enabled)
            {
                return dal.Get(u => u.activeProcessId == activeProcessId && u.userId == userId && u.approved == approved && u.enabled == enabled).FirstOrDefault();
            }

            public List<ActiveProcessDetail> findAllByListActiveProcessIdAndEnabled(List<int> listActiveProcessId, bool enabled)
            {
                return dal.Get(u => u.enabled == enabled && listActiveProcessId.Contains(u.activeProcessId)).ToList();
            }

        }
    }
}