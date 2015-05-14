using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Reflection;
using System.Collections;

namespace ADOL.APP.CurrentAccountService.Helpers
{
    public static class LogHelper
    {
        private const string ErrorFileName = @"{0}\log\{1}\Error{2}.log";
        private const string ActivityFileName = @"{0}\log\{1}\Activity{2}.log";
        private const string CustomFileName = @"{0}\log\{1}\{2}.log";
        
        public static void LogError(string errorMessage, Exception ex = null, object instance = null,
                                    [CallerMemberName] string memberName = "",
                                    [CallerFilePath] string sourceFilePath = "",
                                    [CallerLineNumber] int sourceLineNumber = 0)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Caller Information:");
            sb.AppendLine(string.Format("Code file: {0}.", sourceFilePath));
            sb.AppendLine(string.Format("Member name: {0}.", memberName));
            sb.AppendLine(string.Format("Line number: {0}.", sourceLineNumber.ToString()));
            sb.AppendLine(string.Format("Message: {0}.", errorMessage));
            if (ex != null)
            {
                sb.AppendLine("Stack trace:");
                sb.AppendLine(ex.StackTrace.ToString());
                sb.AppendLine("exception message collection:");
                GetExceptionMessageCollection(sb, ex);
                sb.AppendLine("end exception message collection:");
            }

            if (instance != null)
            {
                FlattenObject(sb, instance);
            }

            WriteToFile(EventType.Error, sb.ToString());
        }

        public static void LogActivity(string activity, object instance = null,
                                    [CallerMemberName] string memberName = "",
                                    [CallerFilePath] string sourceFilePath = "",
                                    [CallerLineNumber] int sourceLineNumber = 0)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Caller Information:");
            sb.AppendLine(string.Format("Code file: {0}.", sourceFilePath));
            sb.AppendLine(string.Format("Member name: {0}.", memberName));
            sb.AppendLine(string.Format("Line number: {0}.", sourceLineNumber.ToString()));
            sb.AppendLine(string.Format("Message: {0}.", activity));

            if (instance != null)
            {
                FlattenObject(sb, instance);
            }

            WriteToFile(EventType.Activity, sb.ToString());
        }

        public static void CustomLog(string logBody, string path, bool isFullPath = false, object instance = null)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(logBody);
            if (instance != null)
            {
                FlattenObject(sb, instance);
            }
            WriteToFile(EventType.Activity, sb.ToString(), path, isFullPath);
        }

        private static void WriteToFile(EventType logType, string messageToLog, string fileName = "", bool isfullpath = false)
        {
            DateTime logTime = DateTime.Now;
            string logFileName = BuildFileName(logType, logTime, fileName, isfullpath);
            using (StreamWriter logWriter = new StreamWriter(fileName, true))
            {
                logWriter.WriteLine("============================== Start logging ==========================================");
                logWriter.WriteLine(string.Format("Log time: {0}",logTime));
                logWriter.WriteLine(messageToLog);
                logWriter.WriteLine("============================== Logging ends ==========================================");
                logWriter.Flush();
            }
        }

        private static void GetExceptionMessageCollection(StringBuilder sb, Exception ex)
        {
            sb.AppendLine(ex.Message);
            if (ex.InnerException != null)
            {
                GetExceptionMessageCollection(sb, ex.InnerException);
            }
        }

        private static string BuildFileName(EventType logType, DateTime logTime, string fileName, bool isfullpath)
        {
            
            if (!string.IsNullOrEmpty(fileName) && isfullpath)
            {
                return fileName;
            }

            if (!string.IsNullOrEmpty(fileName))
            {
                return string.Format(
                    CustomFileName,
                    System.Web.HttpContext.Current.Server.MapPath("~/"),
                    string.Format("{0}{1}{2}", logTime.Year, logTime.Month, logTime.Day),
                    string.Format("{0}_{1}_{2}", logTime.Hour, logTime.Minute, logTime.Second));
            }
            
            return string.Format(
                logType == EventType.Activity ? ActivityFileName : ErrorFileName,
                System.Web.HttpContext.Current.Server.MapPath("~/"),
                string.Format("{0}{1}{2}", logTime.Year, logTime.Month, logTime.Day),
                logType == EventType.Activity ? logTime.Hour.ToString() : string.Format("{0}_{1}_{2}", logTime.Hour, logTime.Minute, logTime.Second));
        }

        private static void FlattenObject(StringBuilder sb, object instance)
        {
            sb.AppendLine("--------------- start object details ---------------");
            sb.AppendLine(string.Format("object type: {0}", instance.GetType().FullName));
            foreach (PropertyInfo prop in instance.GetType().GetProperties())
            {
                if (!prop.GetType().GetInterfaces().Contains(typeof(IEnumerable)) && !prop.GetType().GetInterfaces().Contains(typeof(ICollection)))
                {
                    sb.AppendLine(string.Format("     {0}: {1}",
                    prop.Name,
                    prop.GetValue(instance).ToString()
                    ));
                }
                else
                {
                    sb.AppendLine(string.Format("     {0}: {1}",
                    prop.Name,
                    "Is collection"
                    ));
                }
            }
            sb.AppendLine("--------------- end object details ---------------");
        }
    }

    internal enum EventType
    {
        Activity,
        Error
    }
}
