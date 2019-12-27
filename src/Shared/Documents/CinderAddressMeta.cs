namespace Cinder.Documents
{
    public class CinderAddressMeta : IDocument
    {
        private string _id;

        public string Hash { get; set; }
        public string Name { get; set; }
        public string Website { get; set; }

        public string Id
        {
            get => _id ?? $"{Hash}";
            set => _id = value;
        }
    }
}
