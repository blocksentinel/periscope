using Nethereum.BlockchainProcessing.BlockStorage.Entities;

namespace Periscope.Documents
{
    public class CinderBlock : Block, IDocument
    {
        private string _id;

        public long UncleCount { get; set; }
        public string[] Uncles { get; set; }
        public string Sha3Uncles { get; set; }
        public decimal Value { get; set; }
        public decimal Fees { get; set; }

        public string Id
        {
            get => _id ?? $"{BlockNumber}";
            set => _id = value;
        }
    }
}
