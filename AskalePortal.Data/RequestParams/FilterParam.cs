using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.RequestParams
{

    public class FilterParam<T> where T : class
    {
        public T? liste { get; set; }
    }
}
