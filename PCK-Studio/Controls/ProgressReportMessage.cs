using System.Drawing;

namespace PckStudio.Controls
{
    internal class ProgressReportMessage
    {
        private ProgressReportMessage(string message, MessageType type, Color messageColor)
        {
            Message = message;
            Type = type;
            MessageColor = messageColor;
        }

        public enum MessageType
        {
            Info,
            Warning,
            Error,
            Debug
        }
        public string Message { get; }
        public MessageType Type { get; }
        public Color MessageColor { get; }

        public static ProgressReportMessage Info(string message) => new ProgressReportMessage(message, MessageType.Info, Color.AliceBlue);
        public static ProgressReportMessage Warning(string message) => new ProgressReportMessage(message, MessageType.Warning, Color.Yellow);
        public static ProgressReportMessage Error(string message) => new ProgressReportMessage(message, MessageType.Error, Color.Red);
        public static ProgressReportMessage Debug(string message) => new ProgressReportMessage(message, MessageType.Debug, Color.DarkBlue);
    }
}
