using System;
using System.Runtime.InteropServices;
using System.Text;

namespace BDSM.Net {
  class VersionHook {
    delegate void getServerVersion(out StdString s);
    [Hook("_ZN6Common22getServerVersionStringB5cxx11Ev", nameof(getServerVersionHook))]
    static getServerVersion getServerVersionOrig = null;
    public static void getServerVersionHook(out StdString s) {
      getServerVersionOrig(out s);
      s.set(s.toString() + " Modded");
    }
  }
}