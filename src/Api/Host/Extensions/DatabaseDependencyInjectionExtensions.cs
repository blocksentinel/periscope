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
                IOptions<Settings> options = sp.GetRequiredService<IOptions<Settings>>();

                return ApiRepositoryFactory.Create(options.Value.Database);
            });
            services.AddSingleton<IAddressRepository>(sp =>
                sp.GetRequiredService<IApiRepositoryFactory>().CreateRepository<AddressRepository>());
            services.AddSingleton<IAddressTransactionRepository>(sp =>
                sp.GetRequiredService<IApiRepositoryFactory>().CreateRepository<AddressTransactionRepository>());
            services.AddSingleton<IBlockRepository>(sp =>
                sp.GetRequiredService<IApiRepositoryFactory>().CreateRepository<BlockRepository>());
            services.AddSingleton<IContractRepository>(sp =>
                sp.GetRequiredService<IApiRepositoryFactory>().CreateRepository<ContractRepository>());
            services.AddSingleton<ITransactionLogRepository>(sp =>
                sp.GetRequiredService<IApiRepositoryFactory>().CreateRepository<TransactionLogRepository>());
            services.AddSingleton<ITransactionRepository>(sp =>
                sp.GetRequiredService<IApiRepositoryFactory>().CreateRepository<TransactionRepository>());
            services.AddSingleton<IAddressMetaRepository>(sp =>
                sp.GetRequiredService<IApiRepositoryFactory>().CreateRepository<AddressMetaRepository>());
            services.AddSingleton<IPromotionRepository>(sp =>
                sp.GetRequiredService<IApiRepositoryFactory>().CreateRepository<PromotionRepository>());
        }
    }
}
