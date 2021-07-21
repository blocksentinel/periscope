using System;

namespace Periscope.Core.Documents
{
    public class Promotion : IDocument
    {
        public string Display { get; set; }
        public string Url { get; set; }
        public string Location { get; set; }
        public bool Active { get; set; }

        private string _id;

        public string Id
        {
            get => _id ?? $"{Guid.NewGuid()}";
            set => _id = value;
        }
    }
}
