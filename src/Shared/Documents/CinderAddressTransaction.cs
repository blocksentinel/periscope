using Nethereum.BlockchainProcessing.BlockStorage.Entities;

namespace Cinder.Documents
{
    public class CinderAddressTransaction : AddressTransaction, IDocument
    {
        private string _id;

        public string Id
        {
            get => _id ?? $"{BlockNumber}{Hash}{Address}";
            set => _id = value;
        }
    }
}
