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
        public class DigitalCoridorTable : BaseBLL<AskalePortal.Data.Models.DigitalCoridorTable>
        {
            public DigitalCoridorTable(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
            {
            }
        }
    }
}
