using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Periscope.Api.Host.Repositories;
using Periscope.Core.SharedKernel;
using Periscope.Data.Repositories;

namespace Periscope.Api.Host.Extensions
{
    public static class DatabaseDependencyInjectionExtensions
    {
        public static void AddDatabase(this IServiceCollection services)
        {
            services.AddSingleton(sp =>
            {
                IOptions<Settings> options = sp.GetService<IOptions<Settings>>();

                return ApiRepositoryFactory.Create(options.Value.Database);
            });
            services.AddSingleton<IAddressRepository>(sp =>
                sp.GetService<IApiRepositoryFactory>().CreateRepository<AddressRepository>());
            services.AddSingleton<IAddressTransactionRepository>(sp =>
                sp.GetService<IApiRepositoryFactory>().CreateRepository<AddressTransactionRepository>());
            services.AddSingleton<IBlockRepository>(sp =>
                sp.GetService<IApiRepositoryFactory>().CreateRepository<BlockRepository>());
            services.AddSingleton<IContractRepository>(sp =>
                sp.GetService<IApiRepositoryFactory>().CreateRepository<ContractRepository>());
            services.AddSingleton<ITransactionLogRepository>(sp =>
                sp.GetService<IApiRepositoryFactory>().CreateRepository<TransactionLogRepository>());
            services.AddSingleton<ITransactionRepository>(sp =>
                sp.GetService<IApiRepositoryFactory>().CreateRepository<TransactionRepository>());
            services.AddSingleton<IAddressMetaRepository>(sp =>
                sp.GetService<IApiRepositoryFactory>().CreateRepository<AddressMetaRepository>());
        }
    }
}
