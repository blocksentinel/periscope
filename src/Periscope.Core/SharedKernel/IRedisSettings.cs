namespace Periscope.Core.SharedKernel
{
    public interface IRedisSettings
    {
        string ConnectionString { get; set; }
    }
}
