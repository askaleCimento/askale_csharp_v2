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
		public class FactoryManagerMeetingPerformanceUser : BaseBLL<AskalePortal.Data.Models.FactoryManagerMeetingPerformanceUser>
		{
            public FactoryManagerMeetingPerformanceUser(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
            {
            }
            #region GetAll

            public override List<AskalePortal.Data.Models.FactoryManagerMeetingPerformanceUser> GetAll()
			{

				var q = dal.Get(k=>k.enabled==true)
									 .OrderBy(k => k.dataOrder);
				return q.ToList();
			
			}
			#endregion

		}
	}
}
