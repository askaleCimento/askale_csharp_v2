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
        public class FazlaMesaiUniteler : BaseBLL<AskalePortal.Data.Models.FazlaMesaiUniteler>
        {
            public FazlaMesaiUniteler(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
            {
            }
            public List<AskalePortal.Data.Models.FazlaMesaiUniteler> GetByCompanyId(int companyId)
            {
                    return dal.Get(u => u.companyId == companyId && u.enabled==true).OrderBy(u=>u.siraId).ToList();
            }

            public List<AskalePortal.Data.Models.FazlaMesaiUniteler> GetByPages(int? companyId,string UniteAdi, int activePage, int pageSize)
            {
                return dal.Get(u => (companyId.HasValue ? u.companyId == companyId : true) && (UniteAdi==null?true:u.uniteAdi== UniteAdi) && u.enabled == true).OrderBy(u=>u.companyId).Skip(activePage * pageSize).Take(pageSize).ToList();
            }
        }
    }
}
