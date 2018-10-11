using System;
using System.Runtime.InteropServices;
using System.Security;

namespace SpectoLogic.Azure.CosmosDB.Metrics.Extensions
{
    /// <summary>
    /// based on code from: https://www.codeproject.com/tips/549109/working-with-securestring
    /// </summary>
    public static class SecureStringExtensions
    {
        public static SecureString ConvertToSecureString(string strPassword)
        {
            var secureStr = new SecureString();
            if (strPassword.Length > 0)
            {
                foreach (var c in strPassword.ToCharArray()) secureStr.AppendChar(c);
            }
            return secureStr;
        }

        public static string ToUnSecureString(this SecureString secstrPassword)
        {
            IntPtr unmanagedString = IntPtr.Zero;
            try
            {
                unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(secstrPassword);
                return Marshal.PtrToStringUni(unmanagedString);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);
            }
        }
    }
}
