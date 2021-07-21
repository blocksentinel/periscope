namespace Periscope.Core.SharedKernel
{
    public interface IPerformanceSettings
    {
        int QueryCountLimiter { get; set; }
        int RichListMinimumBalance { get; set; }
    }
}
