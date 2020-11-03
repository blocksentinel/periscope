using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;

namespace Periscope.Events
{
    public class BlockFoundEvent
    {
        public ulong BlockNumber { get; set; }
        public ulong Difficulty { get; set; }
        public ulong Timestamp { get; set; }
        public ulong UncleCount { get; set; }
        public ulong TransactionCount { get; set; }

        public static BlockFoundEvent Create(BlockWithTransactions block)
        {
            return new BlockFoundEvent
            {
                BlockNumber = block.Number.ToUlong(),
                Difficulty = block.Difficulty.ToUlong(),
                Timestamp = block.Timestamp.ToUlong(),
                UncleCount = (ulong) block.Uncles.Length,
                TransactionCount = (ulong) block.Transactions.Length
            };
        }
    }
}
