using System.Collections.Generic;

namespace Api.Dtos.Common
{
    /// <summary>
    /// Standard Wrapper object used to return results from a paginated API route.
    /// </summary>
    public class PaginatedResult<T>
    {
        /// <summary>
        /// The 0-indexed page number of results that was requested.
        /// </summary>
        public int PageNumber { get; set; }

        /// <summary>
        /// The number of records requested per page.
        /// 
        /// Note that this may be greater than the actual number of records returned in this response.
        /// This happens when there aren't enough records left to fill the page to the requested size.
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// The total number of records that are available to be returned.
        /// </summary>
        public int TotalRecordsAvailable { get; set; }
        
        /// <summary>
        /// The collection of results that comprise this page of the response.
        /// </summary>
        public IEnumerable<T> Records { get; set; } = new List<T>();

        /// <summary>
        /// Constructor.
        /// </summary>
        public PaginatedResult(int pageNumber, int pageSize, int totalAvailable, IEnumerable<T> records)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalRecordsAvailable = totalAvailable;
            Records = records;
        }
    }
}
