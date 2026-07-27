using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AskalePortal.Data.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
namespace AskalePortal.BLL
{
    public partial class BLLActions
    {
        public class ISGUser : BaseBLL<AskalePortal.Data.Models.ISGUser>
        {

            public ISGUser(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
            {
            }
            public AskalePortal.Data.Models.ISGUser GetByCompanyId(int companyID)
            {
                return dal.Get(u => u.companyId == companyID && u.enabled == true).FirstOrDefault() ?? new AskalePortal.Data.Models.ISGUser();
            }

            public List<AskalePortal.Data.Models.ISGUser> GetAll(int? Id, int? companyId, int? isgID, int pageNumber, int pageSize)
            {
                var q = dal.Get(k => (k.Id == Id) || (Id == null) || (Id == 0)
                 && (k.companyId == companyId || companyId == null || companyId == 0)
                 && ( isgID == null || isgID == 0)
                 && k.enabled == true).OrderByDescending(u=>u.Id);
                return q.Skip(pageNumber * pageSize).Take(pageSize).ToList();
            }
        }
    }
}