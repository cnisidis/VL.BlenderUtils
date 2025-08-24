using Stride.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VL.Core;

namespace VL.BlenderUtils.Parser.Logger
{
    public enum LogType
    {
        info,
        warning,
        error
    }

    public enum VerboseLevel
    {
        all,
        warnings,
        errors,
        info
    }
    public class Logger:IDisposable
    {
        private VerboseLevel VerboseLevel;
        private StringBuilder sb;
        private string filePath;
        public Logger(string filePath=null, VerboseLevel verboseLevel = VerboseLevel.all)
        {
            VerboseLevel = verboseLevel;
            sb = new StringBuilder();
            if(filePath.IsNullOrEmpty())
            {
                this.filePath = Path.GetDirectoryName(AppHost.Current.AppPath) + "/log.txt";
            }
            else
            {
                this.filePath = filePath;
            }
                
        }
        /// <summary>
        /// Add mew Log
        /// </summary>
        /// <param name="message">the message to add</param>
        /// <param name="level">Verbose level</param>
        public void Add(string message, LogType level = LogType.info)
        {
            string logMsg = DateTime.Now.ToString("dd|MM|yyyy|HH:mm:ss.ff");
            logMsg += "\t" + message;
            sb.AppendLine(logMsg);
        }
        /// <summary>
        /// Output logs to file
        /// </summary>
        /// <returns></returns>
        public void ToFile()
        {
            File.WriteAllText(this.filePath, String.Empty);
            File.AppendAllText(this.filePath, sb.ToString());
           
        }
        /// <summary>
        /// outputs logs to Console
        /// </summary>
        public void ToConsole()
        {
            Console.WriteLine(sb.ToString());
        }

        public void Clear()
        {
            sb.Clear();
        }

        public void Dispose()
        {
            this.Clear();
        }
    }
}
