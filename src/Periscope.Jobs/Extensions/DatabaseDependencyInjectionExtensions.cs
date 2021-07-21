using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Periscope.Core.Data.Repositories;
using Periscope.Core.SharedKernel;
using Periscope.Jobs.Repositories;

namespace Periscope.Jobs.Extensions
{
    public static class DatabaseDependencyInjectionExtensions
    {
        public static void AddDatabase(this IServiceCollection services)
        {
            services.AddSingleton(sp =>
            {
                IOptions<Settings> options = sp.GetRequiredService<IOptions<Settings>>();

                return IndexerRepositoryFactory.Create(options.Value.Database);
            });
            services.AddSingleton<IAddressRepository>(sp =>
                sp.GetRequiredService<IIndexerRepositoryFactory>().CreateRepository<AddressRepository>());
            services.AddSingleton<IAddressTransactionRepository>(sp =>
                sp.GetRequiredService<IIndexerRepositoryFactory>().CreateRepository<AddressTransactionRepository>());
            services.AddSingleton<IBlockRepository>(sp =>
                sp.GetRequiredService<IIndexerRepositoryFactory>().CreateRepository<BlockRepository>());
            services.AddSingleton<IBlockProgressRepository>(sp =>
                sp.GetRequiredService<IIndexerRepositoryFactory>().CreateRepository<BlockProgressRepository>());
            services.AddSingleton<IContractRepository>(sp =>
                sp.GetRequiredService<IIndexerRepositoryFactory>().CreateRepository<ContractRepository>());
            services.AddSingleton<ITransactionLogRepository>(sp =>
                sp.GetRequiredService<IIndexerRepositoryFactory>().CreateRepository<TransactionLogRepository>());
            services.AddSingleton<ITransactionRepository>(sp =>
                sp.GetRequiredService<IIndexerRepositoryFactory>().CreateRepository<TransactionRepository>());
            services.AddSingleton<IAddressMetaRepository>(sp =>
                sp.GetRequiredService<IIndexerRepositoryFactory>().CreateRepository<AddressMetaRepository>());
            services.AddSingleton<IPromotionRepository>(sp =>
                sp.GetRequiredService<IIndexerRepositoryFactory>().CreateRepository<PromotionRepository>());
        }
    }
}
