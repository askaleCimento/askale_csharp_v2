using AskalePortal.Data.ResponseModels;
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
        public class HRDepartmanTable : BaseBLL<AskalePortal.Data.Models.HRDepartmanTable>
        {
            public HRDepartmanTable(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
            {
            }
            public int GetByName(string departmanAdi)
            {
                return dal.Get(u => u.departmanAdi == departmanAdi.Trim() && u.enabled == true).Count();
            }
            public int GetByNameClass(AskalePortal.Data.Models.HRDepartmanTable entity)
            {
                return dal.Get(u => u.departmanAdi == entity.departmanAdi.Trim() && u.Id!=entity.Id && u.enabled == true).Count();
            }
            public List<IdandText> GetDepartmanIdAndName()
            {
                return dal.Get(u => u.enabled == true).Select(u=>new IdandText() { text=u.departmanAdi,id=u.Id}).ToList();
            }
        }
    }
}
