using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.RequestModel
{
    public class UserFilterDtoRequest
    {
        public string? filterName { get; set; }
        public int? filterRol { get; set; }
        public string? filterKullaniciAdi { get; set; }
        public string? filterEmail { get; set; }
        public string? filterSapUserName { get; set; }
        public int? filterCompany { get; set; }
    }
}
