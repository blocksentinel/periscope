using Cinder.Data.Repositories;
using Cinder.Indexers.HostBase;
using Cinder.Indexers.HostBase.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

// ReSharper disable once CheckNamespace
namespace Cinder.Extensions.DependencyInjection
{
    public static class Database
    {
        public static void AddDatabase(this IServiceCollection services)
        {
            services.AddSingleton(sp =>
            {
                IOptions<SettingsBase> options = sp.GetService<IOptions<SettingsBase>>();

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
            services.AddSingleton<IBlockProgressRepository>(sp =>
                sp.GetService<IIndexerRepositoryFactory>().CreateRepository<BlockProgressRepository>());
            services.AddSingleton<IAddressMetaRepository>(sp =>
                sp.GetService<IIndexerRepositoryFactory>().CreateRepository<AddressMetaRepository>());
        }
    }
}
