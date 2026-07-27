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
		public class InternalAuditor : BaseBLL<AskalePortal.Data.Models.InternalAuditor>
		{
            public InternalAuditor(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
            {
            }

        }
	}
}
