using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AskalePortal.BLL
{
    public partial class BLLActions
    {
        public class Configs : BaseBLL<AskalePortal.Data.Models.Config>
        {
            public Configs(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
            {
            }

            public List<AskalePortal.Data.Models.Config> GetAllWithPage(string searchQuery, int activePage, out double totalPages, out int totalRecords, int recordsPerPage = 10)
            {
                var q = dal.Get(k => k.enabled == true)
                                     .GetPage(activePage, recordsPerPage, out totalPages, out totalRecords);
                return q.ToList();
            }

            public AskalePortal.Data.Models.Config? GetFirst()
            {
                var q = dal.Get(k => k.enabled == true).FirstOrDefault();
                return q;
            }

            public async Task<Data.Models.Config?> save(Data.Models.Config configs,int userId)
            {
                if (configs.Id == 0)
                {
                    configs.createdDate = DateTime.Now;
                    configs.createdUserId=userId;
                    return await Add(configs);
                }
                else
                {
                    configs.updatedDate = DateTime.Now;
                    configs.updatedUserId = userId;
                    return await Update(configs);
                }
                
            }
        }
    }

    
}
