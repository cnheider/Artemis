// Source: https://github.com/sector3studios/r3e-api/blob/505323ca095e2bca01369d34caee6dcd114e7eee/sample-csharp/src/Utilities.cs

using System;
using System.Diagnostics;

namespace Artemis.Plugins.Modules.Games.R3E.API
{
    public class Utilities
    {
        public static Single RpsToRpm(Single rps)
        {
            return rps * (60 / (2 * (Single)Math.PI));
        }

        public static Single MpsToKph(Single mps)
        {
            return mps * 3.6f;
        }

        public static bool IsRrreRunning()
        {
            return Process.GetProcessesByName("RRRE").Length > 0;
        }
    }
}
