namespace Cinder.Core.SharedKernel
{
    public interface IDatabaseSettings
    {
        string ConnectionString { get; set; }
        string Tag { get; set; }
        string Locale { get; set; }
    }
}
