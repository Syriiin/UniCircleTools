using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace UniCircleTools
{
    public static class Extensions
    {
        public static string ReadLineTrim(this StreamReader reader)
        {
            string line = reader.ReadLine();
            return line?.Trim();
        }

        public static string ReadNullString(this BinaryReader reader)
        {
            if (reader.ReadByte() == 0x00)
            {
                return null;
            }

            return reader.ReadString();
        }

        public static byte[] ReadByteArray(this BinaryReader reader)
        {
            int len = reader.ReadInt32();
            if (len <= 0)
            {
                return null;
            }

            return reader.ReadBytes(len);
        }
    }
}
