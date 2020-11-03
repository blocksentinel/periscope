using System.Collections.Generic;

namespace Periscope.Documents
{
    public class CinderAddressMeta : IDocument
    {
        private string _id;

        public string Hash { get; set; }
        public string Name { get; set; }
        public string Website { get; set; }
        public ICollection<string> Tags { get; set; } = new List<string>();

        public string Id
        {
            get => _id ?? $"{Hash}";
            set => _id = value;
        }
    }
}
