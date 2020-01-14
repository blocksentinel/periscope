using System;
using System.IO;

namespace Cinder.Core
{
    public static class VersionInfo
    {
        public const string Version = "1.6.200131";
        public static DateTime BuildDate => new FileInfo(typeof(VersionInfo).Assembly.Location).LastWriteTime;
    }
}
