
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
		public class MusteriSikayetAksiyonTipi : BaseBLL<AskalePortal.Data.Models.MusteriSikayetAksiyonTipi>
		{

            public MusteriSikayetAksiyonTipi(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
            {
            }

            #region GetAll

            public List<AskalePortal.Data.Models.MusteriSikayetAksiyonTipi> GetAllAksiyonTipi()
			{
				var q = dal.Get(k => k.enabled == true).OrderBy(k => k.Id);
				return q.ToList();
			}

			public AskalePortal.Data.Models.MusteriSikayetAksiyonTipi GetByName(string name)
			{
				var q = dal.Get(k => k.enabled == true && k.aksiyonTipi == name).FirstOrDefault();
				return q;
			}
			
			public List<AskalePortal.Data.Models.MusteriSikayetAksiyonTipi> GetAll(int? Id, string AksiyonTipi, int? createdUserId, DateTime? createdDate, bool seeLog, int pageNumber, int pageSize)
			{
				var q = dal.Get(k => (k.Id == Id) || (Id == null) || (Id == 0)
			
				&& (k.aksiyonTipi == AksiyonTipi || AksiyonTipi == null || string.IsNullOrEmpty(AksiyonTipi))
				&& k.enabled == true)
				.OrderByDescending(k => k.createdDate);
				if (seeLog != true)
					q = q.Where(u => u.createdUserId == createdUserId).OrderByDescending(k => k.createdDate);
				return q.Skip(pageNumber* pageSize).Take(pageSize).ToList();
			}
			#endregion
			
		}
	}
	
}
