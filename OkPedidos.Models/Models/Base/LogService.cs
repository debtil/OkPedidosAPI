using Microsoft.Extensions.Logging;

namespace OkPedidos.Models.Models.Base
{
    public static class LogService
    {
        private static ILogger _logger;
        private const string MessageTemplateError = "An unhandled exception occurred: {ErrorMessage}";
        private const string MessageTemplateInformation = "Information: {InfoMessage}";


        public static void ConfigureLogger(ILoggerFactory loggerFactory)
        {
            const string logName = "LogService";
            _logger = loggerFactory.CreateLogger(logName);
        }

        public static void Information(string message)
        {
            _logger?.LogInformation(MessageTemplateInformation, message);
        }
        public static void Error(string message)
        {
            _logger?.LogError(MessageTemplateError, message);
        }
        public static void Error(Exception ex)
        {
            _logger?.LogError(ex, MessageTemplateError, ex.Message);
        }
        public static void Critical(string message)
        {
            _logger?.LogCritical(MessageTemplateError, message);
        }
        public static void Critical(Exception ex)
        {
            _logger?.LogCritical(ex, MessageTemplateError, ex.Message);
        }

        public static void ToConsoleTitleStart(string message, ConsoleColor foregroundColor = ConsoleColor.Gray)
        {
            const int Width = 40;
            const string Space = " ";
            const string Hyphen = "-";
            const string Bar = "|";

            message = message.PadCenter(Width);
            message = message.Replace(Space, Hyphen);
            message = $"|{message}{Bar}";
            Console.ForegroundColor = foregroundColor;
            Console.WriteLine(message);
            Console.ForegroundColor = ConsoleColor.Gray;

        }
        public static void ToConsoleTitleEnd(ConsoleColor foregroundColor = ConsoleColor.Gray)
        {
            const string message = "|----------------------------------------|";
            Console.ForegroundColor = foregroundColor;
            Console.WriteLine(message);
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        public static void ToConsoleEmpty(ConsoleColor foregroundColor = ConsoleColor.Gray)
        {
            Console.ForegroundColor = foregroundColor;
            Console.WriteLine(string.Empty);
            Console.ForegroundColor = ConsoleColor.Gray;
        }
        public static void ToConsole(string message, ConsoleColor foregroundColor = ConsoleColor.Gray)
        {
            Console.ForegroundColor = foregroundColor;
            const string caracterStarEnd = "|";
            message = message.PadCenter(40);
            message = $"{caracterStarEnd}{message}{caracterStarEnd}";
            Console.WriteLine(message);
            Console.ForegroundColor = ConsoleColor.Gray;

        }
    }
    public static class StringExtensions
    {
        public static string PadCenter(this string input, int totalWidth, char paddingChar = ' ')
        {
            if (string.IsNullOrEmpty(input) || totalWidth <= input.Length)
                return input;

            int padding = totalWidth - input.Length;
            int padLeft = padding / 2 + input.Length;
            return input.PadLeft(padLeft, paddingChar).PadRight(totalWidth, paddingChar);
        }
    }
}
