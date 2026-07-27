using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AskalePortal.BLL
{


    public partial class BLLActions
    {
        public class IcYazismalarDetayTable : BaseBLL<AskalePortal.Data.Models.IcYazismalarDetayTable>
        {
            public IcYazismalarDetayTable(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
            {

            }

            internal int approvalCount(int userId)
            {
                var count = (
    from a in dal.dB.IcYazismalarDetayTable
    join b in dal.dB.IcYazismalarTable
        on a.icYazismaId equals b.Id
    where
        a.approved == null &&
        a.userId == userId &&
        a.enabled == true &&
        b.onaylandiMi == false &&
        b.bittiMi == false &&
        b.enabled == true
    select a.icYazismaId
)
.Distinct()
.Count();
                return count;
            }

            internal List<Data.Models.IcYazismalarDetayTable> findAllByEnabledAndIcYazismaId(bool enabled, int icYazismaId)
            {
                return dal.Get(u => u.enabled == enabled && u.icYazismaId == icYazismaId).ToList();
            }

            internal List<Data.Models.IcYazismalarDetayTable> findAllByIcYazismaIdAndApprovedAndUserIdAndEnabled(int icYazismaId,
            bool? approved, int userId, bool enabled)
            {
                return dal.Get(u => u.icYazismaId == icYazismaId && u.approved == approved && u.userId == userId && u.enabled == enabled).ToList();
            }
        }
    }
}
