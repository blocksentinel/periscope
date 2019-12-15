using System.Collections.Generic;

namespace Cinder.Api.Infrastructure.Services
{
    public class MinerLookupService : IMinerLookupService
    {
        private readonly Dictionary<string, string> _miners = new Dictionary<string, string>
        {
            // NOTE: If you would like your pool listed here, open an issue on GitHub:
            //       https://github.com/majordutch/Cinder/issues
            {"0x65767ec6d4d3d18a200842352485cdc37cbf3a21", "Ellaism Dev Pool"},
            {"0xd2871774292e5c7f91fcd5d03b802905e8b414a5", "Pool Sexy"},
            {"0x3c4a268283b8c5508011d48dfe06d0c1568a7963", "Comining.io"},
            {"0x3af4d662b8a6a66340d09873c3e6bfc657a88bc4", "Myminers.org Solo"},
            {"0x168bedca940ff4a2c6bc80a4eec543724e08a176", "BaikalMine Solo"},
            {"0xf35074bbd0a9aee46f4ea137971feec024ab704e", "SoloPool.org"}
        };

        public string GetByAddressOrDefault(string hash)
        {
            return _miners.TryGetValue(hash, out string name) ? name : "Unknown";
        }
    }
}
