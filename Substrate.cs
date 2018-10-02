using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Reflection;

namespace BDSM.Net {
  public static class Substrate {
    [DllImport("libdl")]
    private static extern IntPtr dlopen(IntPtr name, int flag);
    [DllImport("libdl")]
    private static extern IntPtr dlsym(IntPtr handle, string name);
    [DllImport("libdl")]
    private static extern string dlerror();
    [DllImport("libsubstrate")]
    private static extern void MSHookFunction(IntPtr src, IntPtr hooked, ref IntPtr original);

    private static IntPtr handle = dlopen(IntPtr.Zero, 1);

    public static bool Hook<T>(string name, T hook, ref T original) where T : Delegate {
      IntPtr ret = dlsym(handle, name);
      if (ret == IntPtr.Zero) {
        Console.WriteLine(dlerror());
        original = null;
        return false;
      }
      IntPtr rev = IntPtr.Zero;
      MSHookFunction(ret, Marshal.GetFunctionPointerForDelegate(hook), ref rev);
      if (rev == IntPtr.Zero) {
        original = null;
        return false;
      }
      original = Marshal.GetDelegateForFunctionPointer<T>(rev);
      return original != null;
    }

    private static MethodInfo HookMethod = typeof(Substrate).GetMethod(nameof(Hook));

    public static void RegisterClass(Type type) {
      foreach (var (field, attr) in from field in type.GetRuntimeFields()
                                    let atts = field.GetCustomAttributes(typeof(HookAttribute), false)
                                    where atts.Count() > 0
                                    select (field, attr: atts[0] as HookAttribute)) {
        var T = field.FieldType;
        var symbol = attr.symbol;
        var hooked = Delegate.CreateDelegate(T, null, type.GetMethod(attr.hooked));
        var hookM = HookMethod.MakeGenericMethod(T);
        object[] args = new object[] { symbol, hooked, null };
        hookM.Invoke(null, args);
        field.SetValue(null, args[2]);
      }
    }
  }

  [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
  public class HookAttribute : Attribute {
    public string symbol { get; set; }
    public string hooked { get; set; }
    public HookAttribute(string symbol, string hooked) {
      this.symbol = symbol;
      this.hooked = hooked;
      // AttributeTargets.
    }
  }
}