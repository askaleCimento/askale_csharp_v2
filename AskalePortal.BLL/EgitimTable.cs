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
		public class EgitimTable : BaseBLL<AskalePortal.Data.Models.EgitimTable>
		{
            public EgitimTable(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
            {
            }
            public List<AskalePortal.Data.Models.EgitimTable> GetAll(int? Id, int? altBirimId, DateTime? startDate, DateTime? endDate, DateTime? createdDate, bool seeLog, bool see, int pageNumber, int pageSize)
			{

				var q = dal.Get(k => (Id == 0 ? true : k.Id == Id)
				&& (altBirimId.HasValue ? k.egitimBolumId == altBirimId: true )
				&& (startDate.HasValue ? k.startDate==startDate : true)
				&& (endDate.HasValue?k.endDate==endDate:true)
				&& k.enabled == true)
				.OrderByDescending(k => k.createdDate);
				q = q.OrderByDescending(u => u.Id);
				return q.ToList();
			}
			public List<AskalePortal.Data.Models.EgitimTable> GetAll(int pageNumber, int pageSize)
			{

				var q = dal.Get(k => k.enabled == true);
				q = q.OrderByDescending(u => u.Id);
				return q.Skip(pageNumber * pageSize).Take(pageSize).ToList();
            }
		}
	}
}
