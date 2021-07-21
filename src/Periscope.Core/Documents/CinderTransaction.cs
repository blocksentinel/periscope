﻿using Nethereum.BlockchainProcessing.BlockStorage.Entities;

namespace Periscope.Core.Documents
{
    public class CinderTransaction : Transaction, IDocument
    {
        private string _id;

        public string Id
        {
            get => _id ?? $"{BlockNumber}{Hash}";
            set => _id = value;
        }
    }
}