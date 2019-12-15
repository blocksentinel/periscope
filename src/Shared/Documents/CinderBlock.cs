using Nethereum.BlockchainProcessing.BlockStorage.Entities;

namespace Cinder.Documents
{
    public class CinderBlock : Block, IDocument
    {
        private string _id;

        public long UncleCount { get; set; }
        public string[] Uncles { get; set; }
        public string Sha3Uncles { get; set; }

        public string Id
        {
            get => _id ?? $"{BlockNumber}";
            set => _id = value;
        }
    }
}
