using System;
using System.IO;
using System.Reflection;

namespace UniCircleToolsTests.Resources
{
    public class Resource
    {
        /// <summary>
        /// Converts relative /Resource/ path to full path
        /// </summary>
        /// <param name="resourcePath">Relative path to resource</param>
        /// <returns>Full path to specified resource</returns>
        public static string PathToResource(string resourcePath)
        {
            string dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            return dir + "\\Resources\\" + resourcePath;
        }
    }
}
