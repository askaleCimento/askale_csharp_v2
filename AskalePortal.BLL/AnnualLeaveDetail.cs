using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.BLL
{
	
	public partial class BLLActions
	{
        public class AnnualLeaveDetail : BaseBLL<AskalePortal.Data.Models.AnnualLeaveDetail>
        {
            public AnnualLeaveDetail(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
            {
            }

            public Data.Models.AnnualLeaveDetail? GetByUserIdAndId(int id, int userId, int siraNo)
            {
                return dal.Get(u => u.enabled == true && u.userId == userId && u.anuId == id && u.siraNo==siraNo ).OrderByDescending(u => u.createdDate).FirstOrDefault();
            }

            public Data.Models.AnnualLeaveDetail GetMyDetail(int anuId, int userId)
            {
                return dal.Get(u => u.enabled == true && u.anuId == anuId && u.userId == userId).First();
            }

            public List<Data.Models.AnnualLeaveDetail> GetAllByUnApproved(int userId)
            {
                return dal.Get(u => u.userId == userId && u.enabled == true && u.approved == null).ToList();
            }

            public List<Data.Models.AnnualLeaveDetail> getByAnuId(int anuId )
            {
                return dal.Get(u =>  u.enabled && u.anuId == anuId).ToList();
            }

            public List<Data.Models.AnnualLeaveDetail> findAllByUserIdActive(int? currentUserId, int anuUserId)
            {
                List<Data.Models.AnnualLeaveDetail> liste = dal.Get(u => u.enabled && u.anu.userId == anuUserId && u.approved == null && u.userId == currentUserId).ToList();
                return liste;
            }

            public Data.Models.AnnualLeaveDetail findByAnuIdAndUserIdAndSiraNoAndEnabled(int anuId, int userId, int siraNo, bool enabled)
            {
                Data.Models.AnnualLeaveDetail? annualLeaveDetail = dal.Get(u=> u.anuId==anuId && u.userId==userId &&  u.siraNo==siraNo &&u.enabled==enabled).FirstOrDefault();
            return annualLeaveDetail ?? new Data.Models.AnnualLeaveDetail();
            }
        }
    }

}
