namespace Periscope.Core.SharedKernel
{
    public interface IBusSettings
    {
        string ConnectionString { get; set; }
        string QueueName { get; set; }
    }
}
