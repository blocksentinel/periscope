using Nethereum.BlockchainProcessing.BlockStorage.Entities;

namespace Periscope.Documents
{
    public class CinderContract : Contract, IDocument
    {
        private string _id;

        public string Id
        {
            get => _id ?? $"{Address}";
            set => _id = value;
        }
    }
}
