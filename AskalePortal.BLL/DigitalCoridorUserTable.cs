
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
        public class DigitalCoridorUserTable : BaseBLL<AskalePortal.Data.Models.DigitalCoridorUserTable>
        {
            public DigitalCoridorUserTable(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
            {
            }
            public List<AskalePortal.Data.Models.DigitalCoridorUserTable> getUsers(int[] sectionId,string location)
            {
                return dal.Get(u => u.location == location && sectionId.Contains(u.sectionId!.Value) && u.enabled == true).ToList();
            }

            public List<AskalePortal.Data.Models.DigitalCoridorUserTable> GetAll(string username, string location)
            {
              

                var q = dal.Get(k => (k.userName.Contains(username) || string.IsNullOrEmpty(username)) &&
                                   (k.location.Contains(location) || string.IsNullOrEmpty(location)) &&
                                 
                                   k.enabled == true)
                                   .OrderBy(k => k.userName);
                return q.ToList();
            }

            public List<AskalePortal.Data.Models.DigitalCoridorUserTable> GetAllWithPage(string username, string location, int activePage, int recordsPerPage)
            {
                var q = dal.Get(k => (k.userName.Contains(username) || string.IsNullOrEmpty(username)) &&
                                  (k.location.Contains(location) || string.IsNullOrEmpty(location)) &&

                                  k.enabled == true)
                                  .OrderBy(k => k.userName).Skip(activePage * recordsPerPage).Take(recordsPerPage).ToList();
                return q;
            }
        }
    }
}
