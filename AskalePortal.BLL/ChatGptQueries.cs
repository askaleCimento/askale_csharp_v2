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
        public class ChatGptQueries : BaseBLL<AskalePortal.Data.Models.ChatGptQueries>
        {
            public ChatGptQueries(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
            {

            }
        }
    }
}
