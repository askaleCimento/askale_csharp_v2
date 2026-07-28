namespace AskalePortal.Data.ResponseParams
{
    /// <summary>
    /// Non-generic pagination response contract used by the API pagination filter.
    /// </summary>
    public interface IPaginationResult
    {
        void NormalizePagination(int pageNumber, int pageSize);
    }

    public class PageReturn<T> : IPaginationResult
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

        /// <summary>
        /// Completes Spring-style page metadata consistently for every paginated endpoint.
        /// The requested page is zero based, matching the Flutter FilterPageParam contract.
        /// </summary>
        public void NormalizePagination(int pageNumber, int pageSize)
        {
            pageNumber = Math.Max(pageNumber, 0);

            var effectivePageSize = pageSize > 0
                ? pageSize
                : size is > 0
                    ? size.Value
                    : 20;

            var effectiveTotalElements = Math.Max(totalElements ?? content?.Count ?? 0, 0);
            var effectiveTotalPages = effectiveTotalElements == 0
                ? 0
                : (int)Math.Ceiling(effectiveTotalElements / (double)effectivePageSize);
            var currentElementCount = content?.Count ?? 0;

            totalElements = effectiveTotalElements;
            totalPages = effectiveTotalPages;
            size = effectivePageSize;
            number = pageNumber;
            numberOfElements = currentElementCount;
            first = pageNumber == 0;
            last = effectiveTotalPages == 0 || pageNumber >= effectiveTotalPages - 1;
            empty = currentElementCount == 0;

            sort ??= new Sort
            {
                sorted = false,
                unsorted = true,
                empty = true
            };

            pageable = new Pageable
            {
                sort = sort,
                pageNumber = pageNumber,
                pageSize = effectivePageSize,
                offset = pageNumber * effectivePageSize,
                paged = true,
                unpaged = false
            };
        }

        public PageReturn<T> GetPage(IQueryable<T> query, int pageNumber, int pageSize)
        {
            pageNumber = Math.Max(pageNumber, 0);
            pageSize = Math.Max(pageSize, 1);

            var total = query.Count();
            content = query
                .Skip(pageNumber * pageSize)
                .Take(pageSize)
                .ToList();
            totalElements = total;

            NormalizePagination(pageNumber, pageSize);
            return this;
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
