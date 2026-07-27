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
        public class TransferPaymentSAPTable : BaseBLL<AskalePortal.Data.Models.TransferPaymentSAPTable>
        {
            public TransferPaymentSAPTable(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
            {
            }
            public AskalePortal.Data.Models.TransferPaymentSAPTable GetByHENUM(string hENUM)
            {
                return dal.Get(u => u.enabled == true && u.henum == hENUM ).FirstOrDefault() ?? new AskalePortal.Data.Models.TransferPaymentSAPTable();
            }

           
        }
    }

}
