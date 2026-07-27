using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.ResponseModels
{
    public class ForceUpdateModel
    {
        public bool isForceUpdate {  get; set; }
        public string? type { get; set; }
        public required string currentVersion { get; set; }
    }
}
