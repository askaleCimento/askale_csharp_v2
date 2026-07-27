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
        public class AuditorTable : BaseBLL<AskalePortal.Data.Models.AuditorTable>
        {
            public AuditorTable(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
            {
            }

            public List<Data.Models.AuditorTable> listAllByEnabled(bool enabled)
            {
                return dal.Get(u=> u.enabled==enabled).ToList();
            }
        }
    }

}
