using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib
{
    public static class Extenstions
    {
        public static void Fill(this byte[] bytes, byte[] data, int index)
        {
            for (int i = 0; i < data.Length; i++)
            {
                bytes[index + i] = data[i];
            }
        }

        public static byte[] ToBytes(this int data)
        {
            byte[] bytes = new byte[4];
            bytes[0] = (byte)(data >> 24);
            bytes[1] = (byte)(data >> 16);
            bytes[2] = (byte)(data >> 8);
            bytes[3] = (byte)(data);
            return bytes;
        }

        public static byte[] ToBytes(this UInt16 data)
        {
            byte[] bytes = new byte[2];
            bytes[0] = (byte)(data >> 8);
            bytes[1] = (byte)(data);
            return bytes;
        }

        public static byte[] ToBytes(this UInt32 data)
        {
            byte[] bytes = new byte[4];
            bytes[0] = (byte)(data >> 24);
            bytes[1] = (byte)(data >> 16);
            bytes[2] = (byte)(data >> 8);
            bytes[3] = (byte)(data);
            return bytes;
        }

        public static byte[] ToBytes(this UInt64 data)
        {
            byte[] bytes = new byte[8];
            bytes[0] = (byte)(data >> 56);
            bytes[1] = (byte)(data >> 48);
            bytes[2] = (byte)(data >> 40);
            bytes[3] = (byte)(data >> 32);
            bytes[4] = (byte)(data >> 24);
            bytes[5] = (byte)(data >> 16);
            bytes[6] = (byte)(data >> 8);
            bytes[7] = (byte)(data);
            return bytes;
        }


        public static UInt16 ToUInt16(this byte[] data, int startPos = 0)
        {
            return (UInt16)((data[startPos] << 8) + data[startPos + 1]);
        }

        public static UInt32 ToUInt32(this byte[] data, int startPos = 0)
        {
            return (UInt32)((data[startPos] << 24) + (data[startPos + 1] << 16) + (data[startPos + 2] << 8) + data[startPos + 3]);
        }

        public static UInt64 ToUInt64(this byte[] data, int startPos = 0)
        {
            return ((UInt64)data[startPos] << 56) + ((UInt64)data[startPos + 1] << 48) + ((UInt64)data[startPos + 2] << 40) + ((UInt64)data[startPos + 3] << 32) + ((UInt64)data[startPos + 4] << 24) + ((UInt64)data[startPos + 5] << 16) + ((UInt64)data[startPos + 6] << 8) + data[startPos + 7];
        }

        public static string ToTSTR(this byte[] data,out byte length, int startPos = 0)
        {
            length = data[startPos];
            byte[] buf = new byte[length];
            Buffer.BlockCopy(data, startPos + 1, buf, 0, length);

            if (buf[length - 1] != 0)
            {
                Logger.Error($"String is not null terminated! Error Data:{buf.ToHexString()}");
            }

            return Encoding.UTF8.GetString(buf).TrimEnd('\0');
        }

        public static string ToXSTR(this byte[] data, out byte length, int startPos = 0)
        {
            length = data[startPos];
            byte[] buf = new byte[length];
            Buffer.BlockCopy(data, startPos + 1, buf, 0, length);

            return Encoding.UTF8.GetString(buf);
        }

        public static byte[] ToTBytes(this string str)
        {
            if(string.IsNullOrEmpty(str))
            {
                return new byte[] { 0x01, 0x00};
            }
            var bytes = Encoding.UTF8.GetBytes(str);
            int len = bytes.Length + 1;
            if (len > byte.MaxValue)
            {
                Logger.Error($"string length lager than {byte.MaxValue}");
                return null;
            }
            var formated_bytes = new byte[len + 1];
            formated_bytes[0] = (byte)len;
            formated_bytes.Fill(bytes, 1);
            formated_bytes[len] = 0;
            return formated_bytes;
        }

        public static byte[] ToXBytes(this string str)
        {
            var bytes = Encoding.UTF8.GetBytes(str);
            //Logger.Debug($"Convert String, From {str} to {bytes.ToHexString()}");
            int len = bytes.Length;
            if (len > byte.MaxValue)
            {
                Logger.Error($"string length lager than {byte.MaxValue}");
                return null;
            }
            var formated_bytes = new byte[len + 1];
            formated_bytes[0] = (byte)len;
            formated_bytes.Fill(bytes, 1);
            return formated_bytes;
        }

        //public static string ToHexString(this byte[] bytes)
        //{
        //    return BitConverter.ToString(bytes).Replace('-', ' ');
        //}

        public static string ToHexString(this byte[] ba)
        {
            if(ba == null)
            {
                return string.Empty;
            }
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
            //return BitConverter.ToString(ba).Replace("-", string.Empty);
        }

        public static byte[] ToHexByteArray(this string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }


        public static uint ToUnixTimestamp(this DateTime date)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan diff = date.ToUniversalTime() - origin;
            return (uint)Math.Floor(diff.TotalSeconds);
        }
    }
}
