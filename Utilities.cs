using System.IO;
using System.Reflection;

namespace AtlyssEmotes
{
    public static class Utilities
    {
        public static string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
    }
}
