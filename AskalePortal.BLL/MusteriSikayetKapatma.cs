
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
		public class MusteriSikayetKapatma: BaseBLL<AskalePortal.Data.Models.MusteriSikayetKapatma>
		{

            public MusteriSikayetKapatma(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
            {
            }

            public List<AskalePortal.Data.Models.MusteriSikayetKapatma> GetBydirectorId(int directorId)
			{
				var q = dal.Get(k => k.directorId == directorId).OrderBy(k => k.Id);
				return q.ToList();
			}

			public List<AskalePortal.Data.Models.MusteriSikayetKapatma> GetByfabrikaId(int fabrikaId)
			{
				var q = dal.Get(k => k.fabrikaId == fabrikaId).OrderBy(k => k.Id);
				return q.ToList();
			}

			public List<AskalePortal.Data.Models.MusteriSikayetKapatma> GetAll(int? Id, string KapatmaAdi, int? createdUserId, DateTime? createdDate, bool seeLog, int pageNumber, int pageSize)
			{
				var q = dal.Get(k => (k.Id == Id) || (Id == null) || (Id == 0)
				&& (k.kapatmaAdi == KapatmaAdi || KapatmaAdi == null || string.IsNullOrEmpty(KapatmaAdi))
				&& k.enabled == true)
				.OrderBy(k => k.director.name);
				if (seeLog != true)
					q = q.Where(u => u.createdUserId == createdUserId).OrderByDescending(k => k.createdDate);

				return q.Skip(pageNumber * pageSize).Take(pageSize).ToList();
			}
		}
	
			
		

	}
}
