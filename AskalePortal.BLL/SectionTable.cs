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
		public class SectionTable : BaseBLL<AskalePortal.Data.Models.SectionTable>
		{
            public SectionTable(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
            {
            }
        }
	}

}
