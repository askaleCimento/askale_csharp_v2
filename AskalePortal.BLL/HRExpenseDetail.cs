using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AskalePortal.Data.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace AskalePortal.BLL
{
    public partial class BLLActions
    {
        public class HRExpenseDetail : BaseBLL<AskalePortal.Data.Models.HRExpenseDetail>
        {
            public HRExpenseDetail(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
            {
            }
            public AskalePortal.Data.Models.HRExpenseDetail GetUser(int tripId,int userId)
            {
                return dal.Get(u => u.approved == null && u.isReplied == false && u.userId == userId && u.tripId == tripId && u.enabled == true).FirstOrDefault() ?? new AskalePortal.Data.Models.HRExpenseDetail();
            }

            public List<AskalePortal.Data.Models.HRExpenseDetail> GetByUserNotApproved(List<int> listTripId)
            {
                return dal.Get(u =>listTripId.Contains(u.tripId) && u.approved==null && u.isReplied==false && u.enabled == true).ToList();
            }

            public List<AskalePortal.Data.Models.HRExpenseDetail> GetByUserIdNotApproved(int vekaletverenId)
            {
                return dal.Get(u => u.userId == vekaletverenId && u.approved == null && u.isReplied == false && u.enabled == true).ToList();
            }
            public List<AskalePortal.Data.Models.HRExpenseDetail> GetByTripId(int tripId)
            {
                return dal.Get(u => u.tripId==tripId && u.enabled == true).ToList();
            }

            public List<AskalePortal.Data.Models.HRExpenseDetail> GetAllActiveByUser(int ownerId,int userId)
            {
                return dal.Get(u =>  u.trip.HRExpenseTable.Any(y=>y.currentStateId == 1) && u.trip.enabled==true && u.enabled == true && u.userId == ownerId &&u.trip.userId==userId && u.approved == null).ToList();
            }

            public List<Data.Models.HRExpenseDetail> findAllByUserIdActive(int? currentUserId, int tripUserId)
            {
                List<Data.Models.HRExpenseDetail> liste = dal.Get(u=>u.enabled && u.trip.userId==tripUserId && u.approved==null && u.userId==currentUserId).ToList();
                return liste;
            }

            public Data.Models.HRExpenseDetail? getByActive(int tripId, int userId)
            {
                Data.Models.HRExpenseDetail? detail = dal.Get(u => u.tripId == tripId && u.userId == userId && u.enabled && u.approved == null).FirstOrDefault();
                return detail;
            }

        }
    }
}
