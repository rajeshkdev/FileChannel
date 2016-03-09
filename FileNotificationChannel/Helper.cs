using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FileNotificationChannel
{
    class Helper
    {
        internal static string GetResourceFileContent(string filename)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            string name = assembly.GetName().Name;

            using (Stream stream = assembly.GetManifestResourceStream(name + "." + filename))
            {
                if (stream == null) throw new Exception(string.Format("Cannot read {0} make sure the file exists and it's an embedded resource", filename));
                using (StreamReader sr = new StreamReader(stream))
                {
                    return sr.ReadToEnd();
                }
            }
        }
    }
}
