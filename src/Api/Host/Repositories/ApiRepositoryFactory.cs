using Periscope.Core.SharedKernel;
using Periscope.Data;

namespace Periscope.Api.Host.Repositories
{
    public class ApiRepositoryFactory : RepositoryFactoryBase, IApiRepositoryFactory
    {
        public ApiRepositoryFactory(string connectionString, string dbTag) : base(connectionString, dbTag) { }

        public static IApiRepositoryFactory Create(IDatabaseSettings settings)
        {
            string connectionString = settings.ConnectionString;
            string tag = settings.Tag;

            IApiRepositoryFactory factoryBase = new ApiRepositoryFactory(connectionString, tag);

            return factoryBase;
        }
    }
}
