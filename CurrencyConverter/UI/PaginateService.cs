using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyConverter.UI
{

    public interface IPaginationService
    {
        PaginationResult<T> Paginate<T>(IEnumerable<T> items, int page, int pageSize);
    }

    public record PaginationResult<T>
    {
        public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();
        public int Page { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }

        public bool HasNext => Page < TotalPages;
        public bool HasPrevious => Page > 1;

    }
    public class PaginateService: IPaginationService
    {     
        public PaginationResult<T> Paginate<T>(IEnumerable<T> items, int page, int pageSize)
        {
            var list = items.ToList();
            var totalItems = list.Count;
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            if (page < 1) page = 1;
            if (page > totalPages) page = totalPages;

            var pageItems = list
                .Skip((page - 1) * pageSize)
                .Take(pageSize);

            return new PaginationResult<T>
            {
                Items = pageItems,
                Page = page,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = totalPages
            };
        }
    }
}
