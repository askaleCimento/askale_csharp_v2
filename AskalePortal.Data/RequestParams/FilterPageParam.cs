using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskalePortal.Data.RequestParams
{
    public class FilterPageParam<T>
    {
        public int? page { get; set; }
        public int? size { get; set; }
        public string? sort { get; set; }
        public List<SortingModel>? sorting { get; set; }
        public T? liste { get; set; }
        public int? userId { get; set; }

        public bool? refresh;
    }

    public class SortingModel
    {
        public string? key { get; set; }
        public string? value { get; set; }
        public string? sorting { get; set; }
    }

}
