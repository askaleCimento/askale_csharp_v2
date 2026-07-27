using AskalePortal.Data.ReportDataset;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.BLL
{
    public partial class BLLActions
    {
        public class AracTalepTableDetail : BaseBLL<AskalePortal.Data.Models.AracTalepTableDetail>
        {
            private readonly IConfiguration _configuration; 
            private readonly IWebHostEnvironment _env;
            

            public AracTalepTableDetail(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
            {
                _configuration = configuration;
                _env = env;
            }

            public Data.Models.AracTalepTableDetail getByActiveNull(int talepId, int userId)
            {
                Data.Models.AracTalepTableDetail? aracTalepTable = dal.Get(u => u.enabled == true && u.talepId == talepId && u.userId == userId && u.approved == null).FirstOrDefault();
                return aracTalepTable ?? new Data.Models.AracTalepTableDetail();
            }

            public List<AracTalepDataSource> getByReport(int talepId)
            {
                List<AracTalepDataSource> liste = dal.Get(u => u.talepId == talepId &&u.enabled).Select(u => new AracTalepDataSource
                {
                    approved = u.approved==true ? " Onaylandı - "+u.replyDate : " Reddedildi - "+u.replyDate,
                    shortDescription = u.user.shortDescription,
                    username = u.user.name,
                }).ToList();
                return liste;
            }

            public List<Data.Models.AracTalepTableDetail> getByTalepId(int talepId)
            {
                List<Data.Models.AracTalepTableDetail> liste = dal.Get(k => k.enabled && k.talepId == talepId).ToList();
                return liste;
            }

            public List<Data.Models.AracTalepTableDetail> findAllByUserIdActive(int? currentUserId, int acanUser)
            {
                //bak
                //List<Data.Models.AracTalepTableDetail> liste = dal.Get(u=> u.enabled).ToList();
                var liste = (from d in dal.dB.AracTalepTableDetail
                             join t in dal.dB.AracTalepTable
                                 on d.talepId equals t.Id
                             where d.enabled && t.createdUserId == acanUser && d.approved==null &&d.userId==currentUserId
                             select d).ToList();
                return liste;
            }
        }
    }
}
