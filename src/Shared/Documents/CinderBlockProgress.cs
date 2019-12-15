using Nethereum.BlockchainProcessing.BlockStorage.Entities;

namespace Cinder.Documents
{
    public class CinderBlockProgress : BlockProgress, IDocument
    {
        private string _id;

        public new string LastBlockProcessed { get; set; } = "-1";

        public string Id
        {
            get => _id ?? "1";
            set => _id = value;
        }
    }
}
