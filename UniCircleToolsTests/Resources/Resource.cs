using System;
using System.IO;
using System.Reflection;

namespace UniCircleToolsTests.Resources
{
    public class Resource
    {
        public static string PathToResource(string resourceName)
        {
            string dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            return dir + "\\Resources\\" + resourceName;
        }
    }
}
