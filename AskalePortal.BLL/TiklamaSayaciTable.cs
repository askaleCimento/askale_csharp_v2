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
		public class TiklamaSayaciTable : BaseBLL<AskalePortal.Data.Models.TiklamaSayaciTable>
		{
            public TiklamaSayaciTable(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
            {
            }
            public List<AskalePortal.Data.Models.TiklamaSayaciTable> getAllBy(string neresi)
			{
				var q = dal.Get(u => u.neresi == neresi);

				return q.ToList();

			}
			public AskalePortal.Data.Models.TiklamaSayaciTable getKisi(int Id, string neresi)
			{
				var q = dal.Get(u => u.userId == Id && u.neresi == neresi);

				return q.FirstOrDefault()??new AskalePortal.Data.Models.TiklamaSayaciTable();
			}

		}
	}
}
