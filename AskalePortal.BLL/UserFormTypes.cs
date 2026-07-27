
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
        public class UserFormTypes : BaseBLL<AskalePortal.Data.Models.UserFormType>
        {
            public UserFormTypes(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
            {
            }

        }
    }
}
