namespace Cinder.Core.SharedKernel
{
    public interface ISettings
    {
        IBusSettings Bus { get; set; }
        IDatabaseSettings Database { get; set; }
        INodeSettings Node { get; set; }
        IRedisSettings Redis { get; set; }
    }
}
