using Nethereum.BlockchainProcessing.BlockStorage.Entities;

namespace Cinder.Documents
{
    public class CinderAddress : TableRow, IDocument
    {
        private string _id;

        public string Hash { get; set; }
        public decimal Balance { get; set; }
        public ulong? BlocksMined { get; set; }
        public ulong? TransactionCount { get; set; }
        public ulong? Timestamp { get; set; }
        public bool ForceRefresh { get; set; }
        public string Temp { get; set; }

        public string Id
        {
            get => _id ?? $"{Hash}";
            set => _id = value;
        }
    }
}
