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
		public class MeetingPerformanceUser : BaseBLL<AskalePortal.Data.Models.MeetingPerformanceUser>
		{
            public MeetingPerformanceUser(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
            {
            }
            #region GetAll

            public override List<AskalePortal.Data.Models.MeetingPerformanceUser> GetAll()
			{

				var q = dal.Get(k=>k.enabled ==true)
									 .OrderBy(k => k.dataOrder);
				return q.ToList();
			
			}
			#endregion

		}
	}
}
