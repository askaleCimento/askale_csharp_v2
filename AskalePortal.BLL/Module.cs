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
        public class Module : BaseBLL<AskalePortal.Data.Models.Module>
        {
            public Module(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
            {
            }
            #region GetAll

            public override List<AskalePortal.Data.Models.Module> GetAll()
            {
                var q = dal.Get(k => k.enabled == true).OrderBy(k => k.title);
                return q.ToList();
            }

            #endregion GetAll
        }
    }
}
