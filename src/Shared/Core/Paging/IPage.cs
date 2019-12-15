using System.Collections.Generic;

namespace Cinder.Core.Paging
{
    public interface IPage<out T>
    {
        int Page { get; }
        int Size { get; }
        int Total { get; }
        IEnumerable<T> Items { get; }
    }
}
