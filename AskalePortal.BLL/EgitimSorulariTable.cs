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

		public class EgitimSorulariTable : BaseBLL<AskalePortal.Data.Models.EgitimSorulariTable>
		{
            private readonly IConfiguration _configuration;
            private readonly IWebHostEnvironment _env;
            public EgitimSorulariTable(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
            {
                _configuration = configuration;
                _env = env;
            }
            public List<AskalePortal.Data.Models.EgitimSorulariTable> GetVideoById(int id)
			{
				List<AskalePortal.Data.Models.EgitimSorulariTable> egitimSorulariTables = new List<AskalePortal.Data.Models.EgitimSorulariTable>();
				BLLActions.EgitimVideoTable bllEgitimVideoTable = new BLLActions.EgitimVideoTable(_configuration,_env);
				AskalePortal.Data.Models.EgitimVideoTable? video = bllEgitimVideoTable.GetByID(id);
                if (video != null)
                {
					egitimSorulariTables = video.EgitimSorulariTable.Where(u => u.enabled == true).ToList();
				}
				
				
				return egitimSorulariTables;
			}

			public AskalePortal.Data.Models.EgitimSorulariTable? GetFromVideoTime(int id, int time)
			{
				TimeSpan times = TimeSpan.FromSeconds(time);
				var q = dal.Get(u => u.videoId == id && u.showVideoTime == times && u.enabled==true).FirstOrDefault();
				return q;
			}

            public int? GetSonSoruId(int videoId)
            {
                return dal.Get(u => u.videoId == videoId && u.sonSoruMu == true && u.enabled == true).FirstOrDefault()?.Id;
            }
        }
	}
}
