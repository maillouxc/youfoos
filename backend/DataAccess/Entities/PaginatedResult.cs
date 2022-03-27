using System.Collections.Generic;

namespace YouFoos.Api.Dtos
{
    /// <summary>
    /// This class is used to wrap responses that return paginated values - contains some useful metadata.
    /// </summary>
    public class PaginatedResult<T>
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public PaginatedResult(int pageNumber, int totalResults, int pageSize, List<T> results)
        {
            PageNumber = pageNumber;
            TotalResults = totalResults;
            PageSize = pageSize;
            Results = results;
        }

        /// <summary>
        /// The current page number being returned.
        /// </summary>
        public int PageNumber { get; set; }

        /// <summary>
        /// The total number of results available - can be used to calculate the number of pages available.
        /// </summary>
        public int TotalResults { get; set; }

        /// <summary>
        /// The requested size of this page of results (this is not always the actual size).
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// The page of results itself.
        /// </summary>
        public List<T> Results { get; set; }
    }
}
