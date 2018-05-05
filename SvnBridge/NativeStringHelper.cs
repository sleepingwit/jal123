using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Jal123
{
    internal static class NativeStringHelper
    {
        public static IntPtr StringToNativeUtf8(string str)
        {
            int len = Encoding.UTF8.GetByteCount(str);
            byte[] buffer = new byte[len + 1];
            Encoding.UTF8.GetBytes(str, 0, str.Length, buffer, 0);
            IntPtr nativeUtf8 = Marshal.AllocHGlobal(buffer.Length);
            Marshal.Copy(buffer, 0, nativeUtf8, buffer.Length);
            return nativeUtf8;
        }
    }
}