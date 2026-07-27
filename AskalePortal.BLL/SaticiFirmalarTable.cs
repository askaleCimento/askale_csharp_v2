using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AskalePortal.Data.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace AskalePortal.BLL
{
	public partial class BLLActions
	{

		public class SaticiFirmalarTable:BaseBLL<AskalePortal.Data.Models.SaticiFirmalarTable>
		{
            public SaticiFirmalarTable(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
            {
            }
            public void Ekle(List<AskalePortal.Data.Models.SaticiFirmalarTable> saticiFirmalars)
			{
				
				foreach (var item in saticiFirmalars)
				{
					dal.AddAsync(item);
				}
				
			}

			public void DeleteAllBySirketId(int SirketId)
			{
				
				var q = dal.Get(u => u.companyId == SirketId).ToList();
				foreach (var item in q)
				{
					dal.DeletePermanently(item);
				}

			}
			public List<AskalePortal.Data.Models.SaticiFirmalarTable> GetFirmaId(int Id)
			{
				return dal.Get(u => u.companyId == Id).ToList();

			}

            public List<string> GetByFirmaAdiLike(string firmaadi)
            {
				return dal.Get(u => u.firmaAdi.Contains(firmaadi)).Select(u=>u.firmaKodu).Distinct().ToList();
            }

            public AskalePortal.Data.Models.SaticiFirmalarTable GetByKod(string firmaKodu,int sirketId)
			{
				
				return dal.Get(u => u.firmaKodu == firmaKodu && u.companyId==sirketId).FirstOrDefault() ?? new AskalePortal.Data.Models.SaticiFirmalarTable();
			}

			public List<AskalePortal.Data.Models.SaticiFirmalarTable> GetCompanyId(int sirketID)
			{
				return dal.Get(u => u.companyId == sirketID).ToList();
			}


			public List<AskalePortal.Data.Models.SaticiFirmalarTable> GetFirmaKodu(string firmaKodu)
			{
				return dal.Get(u => u.firmaKodu == firmaKodu).ToList();
			}

            public List<AskalePortal.Data.Models.SaticiFirmalarTable>? findByCompanyId(int companyId)
            {
				return dal.Get(u => u.enabled && u.companyId == companyId).ToList();
            }

            public List<Data.Models.SaticiFirmalarTable> findByFirmaAdiCompany(string firmaAdi, int companyId)
            {
                return dal.Get(u=>u.enabled &&u.companyId == companyId && u.firmaAdi.Contains(firmaAdi)).ToList();
            }

            public Data.Models.SaticiFirmalarTable? findByFirmaKodu(string firmaKodu, int companyId)
            {
				return dal.Get(u => u.firmaKodu.Contains(firmaKodu) && u.companyId == companyId).First();
            }
        }
	}
}
