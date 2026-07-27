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
        public class CompanySection : BaseBLL<AskalePortal.Data.Models.CompanySection>
        {
            public CompanySection(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
            {
            }

            public List<Data.Models.CompanySection>? listGraph()
            {
                List<Data.Models.CompanySection>? q = dal.Get(k => k.enabled == true && k.isgGraphShow==true).ToList();
                return q.ToList();
            }
        }
    }
}
