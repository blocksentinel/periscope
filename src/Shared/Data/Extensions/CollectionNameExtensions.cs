using Cinder.Data;

// ReSharper disable once CheckNamespace
namespace Cinder.Extensions
{
    public static class CollectionNameExtensions
    {
        public static string ToCollectionName(this CollectionName collectionName)
        {
            return collectionName.ToString().ToLowerInvariant();
        }
    }
}
