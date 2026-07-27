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
		public class EgitimBolumTable : BaseBLL<AskalePortal.Data.Models.EgitimBolumTable>
		{
            public EgitimBolumTable(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
            {
            }
        }
	}
}
