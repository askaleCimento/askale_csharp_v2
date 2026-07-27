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
        public class IcYazismalarMesajTable : BaseBLL<AskalePortal.Data.Models.IcYazismalarMesajTable>
        {
            public IcYazismalarMesajTable(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
            {

            }

            internal List<Data.Models.IcYazismalarMesajTable> findAllByIcYazismaIdAndEnabledOrderByCreatedDate(int icYazismaId, bool enabled)
            {
                return dal.Get(u=>u.enabled==enabled &&u.icYazismaId==icYazismaId).OrderBy(u=>u.createdDate).ToList();
            }
        }
    }
}
