using Microsoft.Extensions.DependencyInjection;
using Periscope.Indexing.BlockIndexer.Host.Infrastructure.StepsHandlers;

namespace Periscope.Indexing.BlockIndexer.Host.Infrastructure.Extensions
{
    public static class BlockchainDependencyInjectionExtensions
    {
        public static void AddBlockchain(this IServiceCollection services)
        {
            services.AddSingleton<CinderBlockStorageStepHandler>();
            services.AddSingleton<CinderContractCreationStorageStepHandler>();
            services.AddSingleton<CinderFilterLogStorageStepHandler>();
            services.AddSingleton<CinderTransactionReceiptStorageStepHandler>();
        }
    }
}
