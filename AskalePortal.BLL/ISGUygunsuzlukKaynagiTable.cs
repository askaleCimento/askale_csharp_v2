using System;
using System.Collections;
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
        public class ISGUygunsuzlukKaynagiTable : BaseBLL<AskalePortal.Data.Models.ISGUygunsuzlukKaynagiTable>
        {
            public ISGUygunsuzlukKaynagiTable(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
            {
            }
            public List<AskalePortal.Data.Models.ISGUygunsuzlukKaynagiTable> GetAllByMaviYaka()
            {
                return dal.Get(u => u.maviYakaMi == true && u.enabled == true).ToList();
            }
        }
    }
}
