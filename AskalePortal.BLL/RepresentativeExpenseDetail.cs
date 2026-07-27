using AskalePortal.Data.ReportDataset;
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
        public class RepresentativeExpenseDetail : BaseBLL<AskalePortal.Data.Models.RepresentativeExpenseDetail>
        {
            public RepresentativeExpenseDetail(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
            {
            }
            public Data.Models.RepresentativeExpenseDetail GetByUserIdAndNotApproved(int userId, int repId)
            {
                return dal.Get(u => u.userId == userId && u.enabled == true && u.repId == repId && u.approved == null).FirstOrDefault() ?? new AskalePortal.Data.Models.RepresentativeExpenseDetail();
            }

            public AskalePortal.Data.Models.RepresentativeExpenseDetail GetMyDetail(int id, int userId)
            {
                return dal.Get(u => u.userId == userId && u.repId == id).FirstOrDefault() ?? new AskalePortal.Data.Models.RepresentativeExpenseDetail();
            }

            public List<Data.Models.RepresentativeExpenseDetail> findAllByUserIdActive(int? currentUserId, int repUserId)
            {
                List<Data.Models.RepresentativeExpenseDetail> liste = dal.Get(u => u.enabled && u.rep.userId == repUserId && u.approved == null && u.userId == currentUserId).ToList();
                return liste;
            }

            public List<Data.Models.RepresentativeExpenseDetail> getByActive(int? repId, int? userId)
            {
                List<Data.Models.RepresentativeExpenseDetail> liste = dal.Get(u => u.enabled && u.repId == repId && u.userId == userId).ToList();
                return liste;
            }

            public List<AracTalepDataSource> getByReport(int repId)
            {
                List<AracTalepDataSource> liste = dal.Get(u => u.repId == repId && u.enabled).Select(u => new AracTalepDataSource
                {
                    approved = u.approved == null
            ? "Onay Bekliyor"
            : (u.approved == true
                ? "Onaylandı - " + (u.replyDate.HasValue ? u.replyDate.Value.ToString("MM.dd.yyyy HH:mm") : "")
                : "Reddedildi - " + (u.replyDate.HasValue ? u.replyDate.Value.ToString("MM.dd.yyyy HH:mm") : "")),

                    shortDescription = u.user.shortDescription,
                    username = u.user.name,
                }).ToList();
                return liste;
            }


            public List<Data.Models.RepresentativeExpenseDetail> getByTripId(int? repId)
            {
                List<Data.Models.RepresentativeExpenseDetail> liste = dal.Get(u => u.enabled && u.repId ==repId ).ToList();
                return liste;
            }

        }
    }
}