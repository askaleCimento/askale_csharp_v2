
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
		public class MusteriSikayetTipi:BaseBLL<AskalePortal.Data.Models.MusteriSikayetTipi>
		{

            public MusteriSikayetTipi(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
            {
            }
            #region GetAll
            public List<AskalePortal.Data.Models.MusteriSikayetTipi> GetAllTipi()
			{

				var q = dal.Get(k => k.enabled == true).OrderBy(k => k.createdDate);
				return q.ToList();

			}
			public List<AskalePortal.Data.Models.MusteriSikayetTipi> GetAll(int? Id, string SikayetTipi,int? categoryId, int? createdUserId, DateTime? createdDate, bool seeLog, int pageNumber, int pageSize)
			{
				var q = dal.Get(k => (k.Id == Id) || (Id == null) || (Id == 0)
				&& (k.categoryId == categoryId) || (categoryId == null) || (categoryId == 0)
				&& (k.sikayetTipi == SikayetTipi || SikayetTipi == null || string.IsNullOrEmpty(SikayetTipi))
				&& k.enabled == true)
				.OrderByDescending(k => k.createdDate);
				if (seeLog != true)
					q = q.Where(u => u.createdUserId == createdUserId).OrderByDescending(k => k.createdDate);
				return q.Skip(pageNumber * pageSize).Take(pageSize).ToList();
			}

            public List<MusteriSikayetTipiSaveDto> getByCategoryId(int categoryId)
            {
				List<MusteriSikayetTipiSaveDto> liste = dal.Get(u => u.enabled == true && u.categoryId == categoryId).Select(u => new MusteriSikayetTipiSaveDto
				{
					 categoryId=u.categoryId,
					  enabled=u.enabled,
					   createdDate=u.createdDate.ToString(),
					    createdUserId=u.createdUserId,
						 id=u.Id,
						  sikayetTipi=u.sikayetTipi,
						   updateDate=u.updatedDate.ToString(),
						    updatedUserId=u.updatedUserId,
				}
					
					).ToList();
				return liste;
            }

            public List<MusteriSikayetTipiSaveDto>? GetAllDto()
            {
				List<MusteriSikayetTipiSaveDto> liste = dal.Get(u => u.enabled == true).Select(u => new MusteriSikayetTipiSaveDto
				{
					enabled = u.enabled,
					categoryId = u.categoryId,

					 createdDate=u.createdDate.ToString(),
					createdUserId=u.createdUserId,
					  id=u.Id,
					   sikayetTipi=u.sikayetTipi,
					    updateDate=u.updatedDate.ToString(),
						 updatedUserId=u.updatedUserId

				}).ToList();
				return liste;

            }
            #endregion
        }
	}
}
