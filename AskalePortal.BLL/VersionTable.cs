using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AskalePortal.Data.ResponseModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

    namespace AskalePortal.BLL
    {
        public partial class BLLActions
        {
            public class VersionTable : BaseBLL<Data.Models.VersionTable>
            {
                public VersionTable(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
                {
                }

                public ForceUpdateModel? getVersion(string version, int platform)
                {
                    return dal.Get(u => u.platform == platform && u.enabled).Select(u => new ForceUpdateModel
                    {
                        isForceUpdate = u.currentVersion != version,
                        currentVersion = u.currentVersion
                    }).FirstOrDefault();
                }
            }

        }
    }
