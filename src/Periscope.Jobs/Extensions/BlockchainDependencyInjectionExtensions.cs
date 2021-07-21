using Microsoft.Extensions.DependencyInjection;
using Periscope.Core.StepsHandlers;

namespace Periscope.Jobs.Extensions
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
