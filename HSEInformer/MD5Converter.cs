using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Security;

namespace HSEInformer
{
    public static class MD5Converter
    {
        public static string Convert(string password)
        {
            MessageDigest digest = MessageDigest.GetInstance("MD5");
            byte[] bytes = Encoding.ASCII.GetBytes(password);
            digest.Update(bytes);
            byte[] messageDigest = digest.Digest();

            StringBuilder hexString = new StringBuilder();

            foreach (var aMessageDigest in messageDigest)
            {
                string hexValue = aMessageDigest.ToString("X");

                while (hexValue.Length < 2)
                    hexValue = "0" + hexValue;

                hexString.Append(hexValue);
            }
            return hexString.ToString();
        }
    }
}