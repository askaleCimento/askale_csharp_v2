using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.Models
{
    public class VersionTable
    {
        public int Id { get; set; }
        public  int type { get; set; }
        public required string currentVersion { get; set; }

        public int platform { get; set; }
        public bool enabled { get; set; }
    }
}
