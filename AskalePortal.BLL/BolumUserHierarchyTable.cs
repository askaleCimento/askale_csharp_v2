using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.BLL
{
	
	public partial class BLLActions
	{
        public class BolumUserHierarchyTable : BaseBLL<AskalePortal.Data.Models.BolumUserHierarchyTable>
        {
            public BolumUserHierarchyTable(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
            {
            }
            public List<AskalePortal.Data.Models.BolumUserHierarchyTable> GetByCeoID(int userId)
            {
                return dal.Get(u=>u.enabled==true && u.userId==userId).ToList();
            }

            public List<Data.Models.BolumUserHierarchyTable> getbymanagerid(bool enabled, int managerId)
            {
                return dal.Get(u => u.enabled == enabled && u.managerId == managerId).ToList();
            }

            public List<AskalePortal.Data.Models.BolumUserHierarchyTable> GetByUserId(int userId)
            {
                return dal.Get(u => u.enabled == true && u.managerId == userId).ToList();
            }

            internal Data.Models.BolumUserHierarchyTable findByBolumAdi(string servisi)
            {
               return dal.Get(u=> u.bolumAdi.Contains(servisi)).FirstOrDefault() ?? new Data.Models.BolumUserHierarchyTable();
            }
        }
    }

}
