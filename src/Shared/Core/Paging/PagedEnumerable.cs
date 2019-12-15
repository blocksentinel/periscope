using System.Collections.Generic;

namespace Cinder.Core.Paging
{
    public class PagedEnumerable<T> : IPage<T>
    {
        public PagedEnumerable() { }

        public PagedEnumerable(IEnumerable<T> items, int total, int page, int size)
        {
            Page = page;
            Size = size;
            Total = total;
            Items = items;
        }

        public int Page { get; }
        public int Size { get; }
        public int Total { get; }
        public IEnumerable<T> Items { get; } = new List<T>();
    }
}
