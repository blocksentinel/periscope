using Periscope.Core.Data;

namespace Periscope.Core.Extensions
{
    public static class CollectionNameExtensions
    {
        public static string ToCollectionName(this CollectionName collectionName)
        {
            return collectionName.ToString().ToLowerInvariant();
        }
    }
}
