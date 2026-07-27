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
        public class HRExpenseTypeTable : BaseBLL<AskalePortal.Data.Models.HRExpenseTypeTable>
        {
            public HRExpenseTypeTable(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
            {
            }
            public int GetByName(string expenseTypeName)
            {
                return dal.Get(u => u.expenseTypeName == expenseTypeName.Trim() && u.enabled == true).Count();
            }

          

            public int GetByNameClass(AskalePortal.Data.Models.HRExpenseTypeTable entity)
            {
                return dal.Get(u => u.expenseTypeName==entity.expenseTypeName.Trim() && u.Id != entity.Id && u.enabled == true).Count();
            }
        }
    }
}
