namespace Periscope.Data.Extensions
{
    public static class CollectionNameExtensions
    {
        public static string ToCollectionName(this CollectionName collectionName)
        {
            return collectionName.ToString().ToLowerInvariant();
        }
    }
}
