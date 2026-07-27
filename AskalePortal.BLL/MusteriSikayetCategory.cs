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
		public class MusteriSikayetCategory : BaseBLL<AskalePortal.Data.Models.MusteriSikayetCategory>
		{

            public MusteriSikayetCategory(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
            {
            }
            #region GetAll

            public List<AskalePortal.Data.Models.MusteriSikayetCategory> GetAllCategory()
			{
				var q = dal.Get(k => k.enabled == true).OrderBy(k => k.Id);
				return q.ToList();
			}

			
			public AskalePortal.Data.Models.MusteriSikayetCategory GetByName(string name)
			{
				var q = dal.Get(k => k.categoryName == name).FirstOrDefault();
				return q;
			}
			public List<AskalePortal.Data.Models.MusteriSikayetCategory> GetAll(int? Id, string CategoryName, int? createdUserId, DateTime? createdDate, bool seeLog, int pageNumber, int pageSize)
			{
				var q = dal.Get(k => (k.Id == Id) || (Id == null) || (Id == 0)
				&& (k.categoryName == CategoryName || CategoryName == null || string.IsNullOrEmpty(CategoryName))
				&& k.enabled == true)
				.OrderByDescending(k => k.createdDate);
				if (seeLog != true)
					q = q.Where(u => u.createdUserId == createdUserId).OrderByDescending(k => k.createdDate);
				return q.Skip(pageNumber * pageSize).Take(pageSize).ToList();
			}
			#endregion

		}
	}
	
}
