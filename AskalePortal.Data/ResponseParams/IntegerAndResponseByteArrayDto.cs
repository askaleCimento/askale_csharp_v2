using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.ResponseModels
{
    public class IntegerAndResponseByteArrayDto
    {
        public int userId { get; set; }
        public ResponseByteArray? responseByteArray { get; set; }
    }
}
