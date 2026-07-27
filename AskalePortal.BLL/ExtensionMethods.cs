using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AskalePortal.BLL
{
    public static class ExtensionMethods
    {
        public static IEnumerable<T> GetPage<T>(this IEnumerable<T> source, int page, int recordsPerPage, out double totalPages)
        {
            if (recordsPerPage <= 0)
            {
                throw new ArgumentOutOfRangeException("recordsPerPage",
                    recordsPerPage,
                    string.Format("recordsPerPage must have a value greater than zero.  The value you provided was {0}",
                    recordsPerPage));
            }
            // get the first record ordinal position
            int skip = (page - 1) * recordsPerPage;

            // get the records per page
            var totalRecords = source.Count();

            // get the total number of pages
            var tp = totalRecords / (double)recordsPerPage;
            totalPages = Math.Ceiling(tp);

            return source.Skip<T>(skip).Take<T>(recordsPerPage);
        }

        public static IEnumerable<T> GetPage<T>(this IEnumerable<T> source, int page, int recordsPerPage, out double totalPages, out int totalRecords)
        {
            if (recordsPerPage <= 0)
            {
                throw new ArgumentOutOfRangeException("recordsPerPage",
                    recordsPerPage,
                    string.Format("recordsPerPage must have a value greater than zero.  The value you provided was {0}",
                    recordsPerPage));
            }
            // get the first record ordinal position
            int skip = (page - 1) * recordsPerPage;

            // get the records per page
            totalRecords = source.Count();

            // get the total number of pages
            var tp = totalRecords / (double)recordsPerPage;
            totalPages = Math.Ceiling(tp);

            return source.Skip<T>(skip).Take<T>(recordsPerPage);
        }

    }
}
