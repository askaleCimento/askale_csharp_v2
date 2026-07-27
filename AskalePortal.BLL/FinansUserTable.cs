using AskalePortal.Data.ResponseModels;
using AskalePortal.Data.ResponseParams;
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
        public class FinansUserTable : BaseBLL<AskalePortal.Data.Models.FinansUserTable>
        {
            public FinansUserTable(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
            {
            }
            public List<AskalePortal.Data.Models.FinansUserTable> GetByCompanyId(int companyID)
            {
               return dal.Get(u => u.companyId == companyID && u.enabled == true).ToList();
            }

            public int GetByuserIdAndCompanyId(int userId, int companyId)
            {
                return dal.Get(u => u.userId==userId && u.companyId == companyId && u.enabled == true).Count();
            }

            public List<FinansUserDto> listFinansUser()
            {
                List<FinansUserDto> liste = dal.Get(u => u.enabled).Select(u => new FinansUserDto()
                { 
                    companyName=u.company.vtext,
                    id=u.Id,
                    username=u.user.name

                }).OrderByDescending(u=>u.id).ToList();
                return liste;
            }

         
        }
    }

}
