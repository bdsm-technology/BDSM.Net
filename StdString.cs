using System;
using System.Runtime.InteropServices;
using System.Text;

namespace BDSM.Net {
  [StructLayout(LayoutKind.Sequential, Size = 24)]
  public struct StdString {
    private IntPtr innerString, length, c;

    [DllImport("support")]
    private static extern void initString(IntPtr str, ref StdString self);
    [DllImport("support")]
    private static extern void setString(IntPtr str, ref StdString self);
    [DllImport("support")]
    private static extern void deleteString(in StdString self);

    public void init(string str) {
      var handle = Marshal.StringToHGlobalAnsi(str);
      initString(handle, ref this);
      Marshal.FreeHGlobal(handle);
    }

    public void set(string str) {
      var handle = Marshal.StringToHGlobalAnsi(str);
      setString(handle, ref this);
      Marshal.FreeHGlobal(handle);
    }

    public void delete() {
      deleteString(this);
    }

    public string toString() {
      return Marshal.PtrToStringAnsi(innerString);
    }
  }
}