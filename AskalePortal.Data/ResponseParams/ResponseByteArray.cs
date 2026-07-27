using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.ResponseModels
{
    public class ResponseByteArray
    {
        public string? fileName { get; set; }
        public byte[]? file { get; set; }
        public string? name { get; set; }
    }
}
