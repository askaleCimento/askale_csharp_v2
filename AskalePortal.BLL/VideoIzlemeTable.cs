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
		public class VideoIzlemeTable : BaseBLL<AskalePortal.Data.Models.EgitimVideoIzlemeTable>
		{
            public VideoIzlemeTable(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
            {
            }
            public EgitimVideoIzlemeTable? GetByVideoIdAndUserId(int videoId, int userId)
			{
				EgitimVideoIzlemeTable? video = dal.Get(u => u.userId == userId && u.videoId == videoId).FirstOrDefault();
				return video;
			}

			public bool GetCount(int videoId, int userId)
			{
				return dal.Get(u => u.userId == userId && u.videoId == videoId).Any();
			}

			public bool GetFinished(int videoId, int userId)
			{
				return dal.Get(u => u.userId == userId && u.videoId == videoId && u.bittiMi==true).Any();
			}

			public List<EgitimVideoIzlemeTable> GetAllByUser(int userId)
			{
				return dal.Get(u => u.userId==userId).ToList();
			}

			public List<EgitimVideoIzlemeTable> GetBySirketKullaniciEgitim(List<int> listsirket, List<int> listkullanicilar, List<int> listegitimler,string date1,string date2)
			{
				if (string.IsNullOrEmpty(date1))
				{
					return dal.Get(u => (listsirket.Count() == 0  ? true :listsirket.Contains(u.user.companyId)) && (listkullanicilar.Count() == 0 ? true : listkullanicilar.Contains(u.userId)) && (listegitimler.Count() == 0? true : listegitimler.Contains(u.video.course.Id)) && u.enabled == true && u.bittiMi == true).ToList();
				}
				else
				{
					if (string.IsNullOrWhiteSpace(date2))
					{
						DateTime dateTime1 = Convert.ToDateTime(date1);
						return dal.Get(u => (listsirket.Count() == 0 ? true : listsirket.Contains(u.user.companyId)) && (listkullanicilar.Count() == 0 ? true : listkullanicilar.Contains(u.userId)) && (listegitimler.Count() == 0 ? true : listegitimler.Contains(u.video.course.Id)) && u.enabled == true && u.bittiMi == true && u.izlemeTarihi==dateTime1).ToList();
					}
					else
					{
						DateTime dateTime1 = Convert.ToDateTime(date1);
						DateTime dateTime2 = Convert.ToDateTime(date2);
						return dal.Get(u => (listsirket.Count() == 0 ? true : listsirket.Contains(u.user.companyId)) && (listkullanicilar.Count() == 0 ? true : listkullanicilar.Contains(u.userId)) && (listegitimler.Count() == 0 ? true : listegitimler.Contains(u.video.course.Id)) && u.enabled == true && u.bittiMi == true && u.izlemeTarihi >= dateTime1 && u.izlemeTarihi<=dateTime2).ToList();
					}
				}
				
			}
		}
	}
}
