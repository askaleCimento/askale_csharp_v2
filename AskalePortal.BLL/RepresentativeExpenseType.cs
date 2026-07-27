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
        public class RepresentativeExpenseType : BaseBLL<AskalePortal.Data.Models.RepresentativeExpenseType>
        {
            public RepresentativeExpenseType(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
            {
            }
        }
    }
}