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

		public class EgitimVideoTable : BaseBLL<AskalePortal.Data.Models.EgitimVideoTable>
		{
            public EgitimVideoTable(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
            {
            }
            public List<AskalePortal.Data.Models.EgitimVideoTable> GetByListEgitim(List<AskalePortal.Data.Models.EgitimTable> egitimTables)
			{
				List<AskalePortal.Data.Models.EgitimVideoTable> listVideo = new List<AskalePortal.Data.Models.EgitimVideoTable>();

				foreach (var item in egitimTables)
				{
					var videos = dal.Get(u => u.courseId == item.Id && u.enabled==true);
					foreach (var item2 in videos)
					{
						listVideo.Add(item2);
					}
					
				}
				return listVideo;
			}

            public int GetByCourseId(int deger)
            {
                var q = dal.Get(u => u.courseId == deger && u.enabled == true).ToList();
                foreach (var item in q)
                {
                    if(item.EgitimSorulariTable.Any(s => s.videoId == item.Id && s.sonSoruMu == true && s.enabled == true))
                    {
                        return item.Id;
                    }
                }
                return 0;
            }

            public int getNextVideoId(AskalePortal.Data.Models.EgitimVideoTable egitimVideoTable)
            {
				if(dal.Get(u => u.courseId == egitimVideoTable.courseId && u.enabled == true && u.videoOrder > egitimVideoTable.videoOrder).Count() > 0)
                {
					return dal.Get(u => u.courseId == egitimVideoTable.courseId && u.enabled == true && u.videoOrder > egitimVideoTable.videoOrder).OrderBy(u => u.videoOrder).First().Id;
				}
				return 0;
            }
        }
	}
}
