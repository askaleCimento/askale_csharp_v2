using AskalePortal.Data.ResponseModels;
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

            public List<FactoryManagerMeetingPerformanceUserSaveDto> GetAllDto()
            {
                var q = dal.Get(k => k.enabled == true)
                                     .OrderBy(k => k.Id).Select(u=> new FactoryManagerMeetingPerformanceUserSaveDto { 
                                     createdDate=u.createdDate.HasValue ? u.createdDate.Value.ToString("dd.MM.yyyy") :"",
                                     createdUserId=u.createdUserId,
                                     dataOrder=u.dataOrder,
                                     enabled=u.enabled,
                                     id=u.Id,
                                     updateDate=u.updatedDate.ToString(),
                                     updatedUserId=u.updatedUserId,
                                     userId = u.userId
                                     });
                return q.ToList();
            }
            #endregion

        }
	}
}
