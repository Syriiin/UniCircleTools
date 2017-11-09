using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace UniCircleTools.Beatmaps
{
    internal static class Extensions
    {
        public static string ReadLineTrim(this StreamReader reader)
        {
            string line = reader.ReadLine();
            return line?.Trim();
        }
    }
}
