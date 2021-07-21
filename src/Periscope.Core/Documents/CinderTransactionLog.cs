using Nethereum.BlockchainProcessing.BlockStorage.Entities;

namespace Periscope.Core.Documents
{
    public class CinderTransactionLog : TransactionLog, IDocument
    {
        private string _id;

        public string Id
        {
            get => _id ?? $"{TransactionHash}{LogIndex}";
            set => _id = value;
        }
    }
}
