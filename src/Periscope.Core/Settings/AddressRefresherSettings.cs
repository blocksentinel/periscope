namespace Periscope.Core.Settings
{
    public class AddressRefresherSettings
    {
        public int Age { get; set; }
        public int Limit { get; set; }
        public BalanceHistorySettings BalanceHistory { get; set; }

        public class BalanceHistorySettings
        {
            public bool Enabled { get; set; }
            public int[] Days { get; set; } = { };
        }
    }
}
