using System.Collections.Generic;
using Nethereum.BlockchainProcessing.BlockStorage.Entities;

namespace Periscope.Documents
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
        public IDictionary<string, decimal> BalanceHistory { get; set; } = new Dictionary<string, decimal>();

        public string Id
        {
            get => _id ?? $"{Hash}";
            set => _id = value;
        }
    }
}
