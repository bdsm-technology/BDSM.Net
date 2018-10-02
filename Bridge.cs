using System;
using System.Runtime.InteropServices;
using System.Text;

namespace BDSM.Net {
  public class Bridge {
    public static void Init() {
      Console.WriteLine("Hello from netcore 233");
      Substrate.RegisterClass(typeof(VersionHook));
    }
  }
}