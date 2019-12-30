using Cinder.Indexers.BlockIndexer.Host.Infrastructure.StepsHandlers;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable once CheckNamespace
namespace Cinder.Extensions.DependencyInjection
{
    public static class Blockchain
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
