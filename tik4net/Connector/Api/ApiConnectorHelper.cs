﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using Tik4Net.Objects;

namespace Tik4Net.Connector.Api
{
    internal static class ApiConnectorHelper
    {
        internal static byte[] EncodeLength(int length)
        {
            if (length < 0x80)
            {
                byte[] tmp = BitConverter.GetBytes(length);
                return new byte[1] { tmp[0] };
            }
            if (length < 0x4000)
            {
                byte[] tmp = BitConverter.GetBytes(length | 0x8000);
                return new byte[2] { tmp[1], tmp[0] };
            }
            if (length < 0x200000)
            {
                byte[] tmp = BitConverter.GetBytes(length | 0xC00000);
                return new byte[3] { tmp[2], tmp[1], tmp[0] };
            }
            if (length < 0x10000000)
            {
                byte[] tmp = BitConverter.GetBytes((uint)length | 0xE0000000);
                return new byte[4] { tmp[3], tmp[2], tmp[1], tmp[0] };
            }
            else
            {
                byte[] tmp = BitConverter.GetBytes(length);
                return new byte[5] { 0xF0, tmp[3], tmp[2], tmp[1], tmp[0] };
            }
        }

        internal static string EncodePassword(string password, string hash)
        {
            byte[] hash_byte = new byte[hash.Length / 2];
            for (int i = 0; i <= hash.Length - 2; i += 2)
            {
                hash_byte[i / 2] = Byte.Parse(hash.Substring(i, 2), System.Globalization.NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            }
            byte[] heslo = new byte[1 + password.Length + hash_byte.Length];
            heslo[0] = 0;
            Encoding.ASCII.GetBytes(password.ToCharArray()).CopyTo(heslo, 1);
            hash_byte.CopyTo(heslo, 1 + password.Length);

            Byte[] hotovo;
            System.Security.Cryptography.MD5 md5;

            md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();

            hotovo = md5.ComputeHash(heslo);

            //Convert encoded bytes back to a 'readable' string
            string result = "";
            foreach (byte h in hotovo)
            {
                result += h.ToString("x2", CultureInfo.InvariantCulture);
            }
            return result;
        }

        internal static string LogLevelToCommandSufix(LogLevel level)
        {
            switch(level)
            {
                case LogLevel.Debug:
                    return "debug";
                case LogLevel.Info:
                    return "info";
                case LogLevel.Warning:
                    return "warning";
                case LogLevel.Error:
                    return "error";
                default:
                    throw new NotImplementedException(string.Format(CultureInfo.CurrentCulture, "LogLevel '{0}' not supported.", level));
            }
        }
    }
}
