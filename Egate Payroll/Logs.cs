using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Egate_Payroll
{
    public static class Logs
    {
        public static void Debug(string txt)
        {
            WriteLog(txt);
        }

        public static void Exception(Exception ex)
        {
            WriteLog(ex.ToString());
        }

        private static void WriteLog(string msg)
        {
            string file = Path.Combine(GetLogDirectory(), "log.txt");
            File.AppendAllText(file, string.Format("[{0}] {1}", DateTime.Now.ToString("MM-dd-yyyy hh:mm:ss tt"), msg));
        }

        private static string GetLogDirectory()
        {
            string dir = "./logs";
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            return dir;
        }
    }
}
