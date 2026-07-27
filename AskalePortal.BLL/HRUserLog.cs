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
        public class HRUserLog : BaseBLL<AskalePortal.Data.Models.HRUserLog>
        {
            public HRUserLog(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
            {
            }
        }
    }

}
