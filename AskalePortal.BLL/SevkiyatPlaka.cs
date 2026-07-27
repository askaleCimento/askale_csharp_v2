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
		public class SevkiyatPlaka : BaseBLL<AskalePortal.Data.Models.SevkiyatPlaka>
		{

            public SevkiyatPlaka(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
            {
            }
            public List<AskalePortal.Data.Models.SevkiyatPlaka> GetAll(int activePage=1,int pageSize=20)
			{
				var q = dal.Get(u => u.enabled == true);
				return q.Skip(activePage*pageSize).Take(pageSize).ToList();
				
			}
			public List<AskalePortal.Data.Models.SevkiyatPlaka> GetAll(string durum,int activePage = 1, int pageSize = 20)
			{
				if (durum == "aktif")
				{
					var q = dal.Get(u => u.listedeMi==false && u.enabled == true && u.createdDate.Year==DateTime.Today.Year && u.createdDate.Month==DateTime.Now.Month && u.createdDate.Day==DateTime.Now.Day);
					return q.OrderBy(u=>u.plakaNo).ToList();
				}
				else if (durum == "liste")
				{
					var q = dal.Get(u => u.listedeMi==true && u.enabled == true && u.createdDate.Year == DateTime.Today.Year && u.createdDate.Month == DateTime.Now.Month && u.createdDate.Day == DateTime.Now.Day);
					return q.OrderBy(u => u.plakaNo).ToList();
				}
				else if(durum=="tamamlanan")
				{
					var q = dal.Get(u => u.enabled == false && u.createdDate.Year == DateTime.Today.Year && u.createdDate.Month == DateTime.Now.Month && u.createdDate.Day == DateTime.Now.Day);
					return q.OrderBy(u => u.plakaNo).ToList();
				}

				return null;
			}
			public override AskalePortal.Data.Models.SevkiyatPlaka GetByID(int id)
			{
				var q = dal.Get(u => u.Id == id).FirstOrDefault();
				return q;
			}

		}
	}
}
