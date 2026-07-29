

using System.Linq.Expressions;
using AskalePortal.Data.ResponseParams;

namespace AskalePortal.Data.ResponseParams
{
    public class PageReturn<T>
    {
        public List<T>? content { get; set; }
        public Pageable? pageable { get; set; }
        public bool? last { get; set; }
        public int? totalElements { get; set; }
        public int? totalPages { get; set; }
        public int? number { get; set; }
        public Sort? sort { get; set; }
        public bool? first { get; set; }
        public int? numberOfElements { get; set; }
        public int? size { get; set; }
        public bool? empty { get; set; }

        public PageReturn<T> GetPage(IQueryable<T> querry, int pageNumber, int pageSize)
        {
            PageReturn<T> pageReturn = new PageReturn<T>();
            pageReturn.content = querry.Skip((pageNumber) * pageSize).Take(pageSize).ToList();
            pageReturn.totalPages = (int)Math.Ceiling((querry.Count() / (double)pageSize));
            pageReturn.totalElements = querry.Count();
            pageReturn.size = pageSize;
            pageReturn.number = pageNumber;
            pageReturn.last = pageNumber == pageReturn.totalPages;
            pageReturn.first = pageNumber == 0;
            return pageReturn;
        }
    }
    public class Pageable
    {
        public Sort? sort { get; set; }
        public int? pageNumber { get; set; }
        public int? pageSize { get; set; }
        public int? offset { get; set; }
        public bool? unpaged { get; set; }
        public bool? paged { get; set; }
    }
    public class Sort
    {
        public bool? sorted { get; set; }
        public bool? unsorted { get; set; }
        public bool? empty { get; set; }
    }



}
