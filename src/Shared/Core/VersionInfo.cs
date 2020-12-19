using System;
using System.IO;

namespace Periscope.Core
{
    public static class VersionInfo
    {
        public const string Version = "1.9.0-beta.3";
        public static DateTime BuildDate => new FileInfo(typeof(VersionInfo).Assembly.Location).LastWriteTime;
    }
}
