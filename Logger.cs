

using System.Text;
using TNG.Shared.Lib.Intefaces;

namespace TNG.Shared.Lib
{
    public class Logger : ILogger
    {

        public Logger()
        {

        }

        public void LogAnalytics(string controller, string method, string ip = "")
        {

        }

        public void LogError(string controller, string method, string error, Exception ex, string ip = "", string appName = "")
        {


            var csv = new StringBuilder();
            try
            {
                string value = controller + "/" + method;
                string dateOfLog = DateTime.UtcNow.ToString();
                var first = value;
                var second = error;
                var third = ip;
                var newLine = string.Format("{0},{1},{2},{3}", dateOfLog, first, second, third);
                csv.AppendLine(newLine);
                var log_path = System.IO.Directory.GetCurrentDirectory();
                File.AppendAllText("" + log_path + "_log.csv", csv.ToString());
            }
            catch
            {

            }

        }
    }
}