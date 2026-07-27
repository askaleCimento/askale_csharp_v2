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
        public class AnnualLeaveType : BaseBLL<AskalePortal.Data.Models.AnnualLeaveType>
        {
            public AnnualLeaveType(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
            {
            }
        }
    }

}
