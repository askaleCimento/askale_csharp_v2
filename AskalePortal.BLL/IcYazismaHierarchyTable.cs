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
        public class IcYazismaHierarchyTable : BaseBLL<AskalePortal.Data.Models.IcYazismaHierarchyTable>
        {
            public IcYazismaHierarchyTable(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
            {

            }

            public List<Data.Models.IcYazismaHierarchyTable> getbymanagerid(bool enabled, int managerId)
            {
                return dal.Get(u => u.enabled == enabled && u.managerId == managerId).ToList();
            }

            public List<Data.Models.IcYazismaHierarchyTable> getbyuserId(bool enabled)
            {
                return dal.Get(u => u.enabled == enabled).ToList();
            }
        }
    }
}
