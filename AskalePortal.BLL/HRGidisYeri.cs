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
        public class HRGidisYeri : BaseBLL<AskalePortal.Data.Models.HRGidisYeri>
        {
            public HRGidisYeri(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
            {
            }

        }
    }
}
