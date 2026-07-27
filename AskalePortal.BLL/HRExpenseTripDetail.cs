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

        public class HRExpenseTripDetail : BaseBLL<AskalePortal.Data.Models.HRExpenseTripDetail>
        {
            public HRExpenseTripDetail(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
            {
            }
            public AskalePortal.Data.Models.HRExpenseTripDetail GetByGuid(string guid)
            {
                return dal.Get(u => u.guid.ToString() == guid && u.enabled == true).FirstOrDefault() ?? new AskalePortal.Data.Models.HRExpenseTripDetail();
            }

            public AskalePortal.Data.Models.HRExpenseTripDetail GetUser(int tripId, int userId)
            {
                return dal.Get(u => u.enabled == true && u.trip.Id == tripId && u.userId == userId).FirstOrDefault() ?? new AskalePortal.Data.Models.HRExpenseTripDetail();
            }

            public List<AskalePortal.Data.Models.HRExpenseTripDetail> GetAllActiveByUser(int ownerId,int userId)
            {
                return dal.Get(u =>u.trip.currentStateId==1 && u.trip.enabled==true && u.enabled == true && u.userId == ownerId && u.trip.userId==userId && u.approved == null).ToList();
            }

            public AskalePortal.Data.Models.HRExpenseTripDetail GetMyDetail(int id, int userId)
            {
                return dal.Get(u => u.userId == userId && u.tripId == id).FirstOrDefault() ?? new AskalePortal.Data.Models.HRExpenseTripDetail();
            }

            public List<Data.Models.HRExpenseTripDetail> findByUserIdActive(int? currentUserId, int tripUserId)
            {
                List<Data.Models.HRExpenseTripDetail> liste = dal.Get(u => u.enabled && u.trip.userId == tripUserId && u.approved == null && u.userId == currentUserId).ToList();
                return liste;
            }

            public List<Data.Models.HRExpenseTripDetail> getByTripId(int tripId)
            {
                List<Data.Models.HRExpenseTripDetail>? liste = dal.Get(u => u.enabled && u.tripId == tripId).ToList();
                return liste ?? [];
            }

            public Data.Models.HRExpenseTripDetail getByActive(int tripId, int userId)
            {
                Data.Models.HRExpenseTripDetail? detail = dal.Get(u => u.tripId == tripId && u.userId == userId && u.enabled && u.approved == null).FirstOrDefault();
                return detail ?? new Data.Models.HRExpenseTripDetail();
            }
        }
    }
}
