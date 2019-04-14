using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace RestBuilder.Service
{
    public class ConsoleRbLoggingService : IRbLoggingService
    {
        public void Log(string message, IDictionary<string, string> properties = null)
        {
            var log = new StringBuilder();

            log.AppendLine();
            log.AppendLine($@"{message} ");

            foreach (var item in properties)
            {
                log.AppendLine($"{item.Key} - {item.Value}");
            }

            log.AppendLine();

            Console.WriteLine(log.ToString());
        }

        public void Log(Exception exception)
        {
            var message = CreateMessage(exception);

            Console.WriteLine(message);
        }

        private static string CreateMessage(Exception exception)
        {
            var baseException = exception.GetBaseException();
            var hasBaseException = (baseException != exception);
            var hasInnerException = (hasBaseException && baseException != exception.InnerException);

            var sb = new System.Text.StringBuilder();

            sb.AppendLine($@"{exception.GetType().FullName}: {exception.Message}");

            if (hasBaseException)
                sb.AppendLine($@"Base {baseException.GetType().FullName}: {baseException.Message}");

            sb.AppendLine($@"{exception.StackTrace}");

            foreach (var key in exception.Data.Keys)
                sb.AppendLine($@"Data {key} - {exception.Data[key] ?? "null"}");

            sb.AppendLine($@"- - - -");

            if (hasInnerException)
            {
                sb.AppendLine($@"Inner {exception.InnerException.GetType().FullName}: {exception.InnerException.Message}");
                sb.AppendLine($@"{exception.InnerException.StackTrace}");

                foreach (var key in exception.InnerException.Data.Keys)
                    sb.AppendLine($@"Data {key} - {exception.InnerException.Data[key].ToString()}");

                sb.AppendLine($@"- - - -");
            }

            if (hasBaseException)
            {
                sb.AppendLine($@"Base Exception: {baseException.Message} {Environment.NewLine}{baseException.StackTrace}{Environment.NewLine}");

                foreach (var key in baseException.Data.Keys)
                    sb.AppendLine($@"Data {key} - {baseException.Data[key].ToString()}");

                sb.AppendLine($@"- - - -");
            }

            sb.AppendLine();

            return sb.ToString();
        }
    }
}
