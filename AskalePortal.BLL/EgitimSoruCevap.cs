using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AskalePortal.Data.Models;
using AskalePortal.Data.SAP.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace AskalePortal.BLL
{
	
	public partial class BLLActions
	{
		public class EgitimSoruCevap : BaseBLL<AskalePortal.Data.Models.EgitimSoruCevap>
		{
            private readonly IConfiguration _configuration;
            private readonly IWebHostEnvironment _env;
            public EgitimSoruCevap(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
            {
                _configuration = configuration;
                _env = env;
            }
            public bool SoruVarMı(int id, int userId)
			{
				bool varMi = false;
				if (dal.Get(u => u.soruId == id && u.userId == userId).Count() != 0)
					varMi = true;
				return varMi;
			}
			public AskalePortal.Data.Models.EgitimSoruCevap GetBySoruIdAndUserId(int id, int userId) {
				return dal.Get(u => u.soruId == id && u.userId == userId).First();
			}

			public List<AskalePortal.Data.Models.EgitimSoruCevap> GetByVideoId(int id)
			{
                AskalePortal.Data.Models.EgitimSoruCevap egitimSoruCevap = new AskalePortal.Data.Models.EgitimSoruCevap();
                BLLActions.EgitimSorulariTable egitimSorulariTable = new BLLActions.EgitimSorulariTable(_configuration, _env);
                AskalePortal.Data.Models.EgitimSorulariTable? sorular = egitimSorulariTable.GetByID(id);
				List<AskalePortal.Data.Models.EgitimSoruCevap>? q = sorular?.EgitimSoruCevap.ToList();
				return q ??[];
			}
			public List<EgitimSoruCevapModel> GetByVideoId(int id,int UserID)
			{
				EgitimSoruCevap egitimSoruCevap = new EgitimSoruCevap(_configuration,_env);
				EgitimSorulariTable egitimSorulariTable = new EgitimSorulariTable(_configuration, _env);
				List<AskalePortal.Data.Models.EgitimSorulariTable> sorular = egitimSorulariTable.GetVideoById(id).OrderBy(u=>u.showVideoTime).ToList();
				List<EgitimSoruCevapModel> q = new List<EgitimSoruCevapModel>();
				int i = 1;
				foreach (var item in sorular)
				{

					int sayi = item.EgitimSoruCevap.Where(u => u.userId == UserID).Count();
					if (sayi > 0)
					{
						Data.Models.EgitimSoruCevap items = item.EgitimSoruCevap.Where(u => u.userId == UserID).First();
						EgitimSoruCevapModel modelss = new EgitimSoruCevapModel()
						{
						
							Sira = i,
							egitimSoruCevap=items
						};
						q.Add(modelss);
					}
					i++;
				}
				return q;
			}
			public List<AskalePortal.Data.Models.EgitimSoruCevap> GetByVideoIdAndUserId(int id, int userID)
			{


                BLLActions.EgitimSorulariTable egitimSorulariTable = new BLLActions.EgitimSorulariTable(_configuration,_env);
				List<int> sorular = egitimSorulariTable.GetVideoById(id).Select(u=>u.Id).ToList();
				return dal.Get(u => sorular.Contains(u.soruId) && u.userId == userID).ToList();
				
				
			}
			public List<AskalePortal.Data.Models.EgitimSoruCevap> GetBySoruIdAndUserId(int userId)
			{
				return dal.Get(u => u.userId == userId).ToList();
			}

            public List<AskalePortal.Data.Models.EgitimSoruCevap> GetAllBySon(int companyID)
            {
                return dal.Get(u => u.user.companyId == companyID && u.soru.sonSoruMu == true && u.enabled == true).ToList();
            }
            public List<AskalePortal.Data.Models.EgitimSoruCevap> GetAllBySon()
            {
                return dal.Get(u => u.soru.sonSoruMu == true && u.enabled == true).ToList();
            }
        }
	}
}
