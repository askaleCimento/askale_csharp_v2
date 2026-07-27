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
        public class CeoTable : BaseBLL<AskalePortal.Data.Models.CeoTable>
        {
            public CeoTable(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
            {
            }
            public AskalePortal.Data.Models.CeoTable? GetCeo()
            {
                return dal.Get(u => u.enabled == true).FirstOrDefault()  ;
            }
        }
    }

}
