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
		public class ToplantiKararlari : BaseBLL<AskalePortal.Data.Models.ToplantiKararlari>
		{
            public ToplantiKararlari(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
            {
            }
            public List<AskalePortal.Data.Models.ToplantiKararlari> GetByToplantiNo(int id)
			{
				var q = dal.Get(u => u.toplantiNo == id).ToList();
				return q;
			}

			public AskalePortal.Data.Models.ToplantiKararlari GetByLastToplantiNo(int adminUserId)
			{
                AskalePortal.Data.Models.ToplantiKararlari toplantiKararlari= dal.Get(u => u.createdUserId == adminUserId).OrderByDescending(u=>u.Id).First();
				return toplantiKararlari;
			}
			
		}
	}

}
