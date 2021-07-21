namespace Periscope.Core.Data
{
    public interface IRepositoryFactory
    {
        TRepository CreateRepository<TRepository>() where TRepository : IRepository;
    }
}
