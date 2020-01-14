namespace Cinder.Core.SharedKernel
{
    public interface IBusSettings
    {
        string ConnectionString { get; set; }
        string QueueName { get; set; }
    }
}
