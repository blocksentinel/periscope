using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Periscope.Core.SharedKernel;
using Periscope.Data.Repositories;
using Periscope.Indexing.HostBase.Repositories;

namespace Periscope.Indexing.HostBase.Extensions
{
    public static class DatabaseDependencyInjectionExtensions
    {
        public static void AddDatabase(this IServiceCollection services)
        {
            services.AddSingleton(sp =>
            {
                IOptions<Settings> options = sp.GetService<IOptions<Settings>>();

                return IndexerRepositoryFactory.Create(options.Value.Database);
            });
            services.AddSingleton<IAddressRepository>(sp =>
                sp.GetService<IIndexerRepositoryFactory>().CreateRepository<AddressRepository>());
            services.AddSingleton<IAddressTransactionRepository>(sp =>
                sp.GetService<IIndexerRepositoryFactory>().CreateRepository<AddressTransactionRepository>());
            services.AddSingleton<IBlockRepository>(sp =>
                sp.GetService<IIndexerRepositoryFactory>().CreateRepository<BlockRepository>());
            services.AddSingleton<IBlockProgressRepository>(sp =>
                sp.GetService<IIndexerRepositoryFactory>().CreateRepository<BlockProgressRepository>());
            services.AddSingleton<IContractRepository>(sp =>
                sp.GetService<IIndexerRepositoryFactory>().CreateRepository<ContractRepository>());
            services.AddSingleton<ITransactionLogRepository>(sp =>
                sp.GetService<IIndexerRepositoryFactory>().CreateRepository<TransactionLogRepository>());
            services.AddSingleton<ITransactionRepository>(sp =>
                sp.GetService<IIndexerRepositoryFactory>().CreateRepository<TransactionRepository>());
            services.AddSingleton<IAddressMetaRepository>(sp =>
                sp.GetService<IIndexerRepositoryFactory>().CreateRepository<AddressMetaRepository>());
        }
    }
}
