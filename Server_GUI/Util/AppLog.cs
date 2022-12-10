using System.Windows.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Server.Events;
using System.Windows.Media;

namespace Server_GUI.Util {
    public static class AppLog {
        public static RichTextBox Logger;
        private static readonly BrushConverter BrushConverter = new();

        public static void Write(object text, string color = "", FontWeight weights = default, bool newLine = true) {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var document = Logger.Document;

                var textRange = new TextRange(document?.ContentEnd, document?.ContentEnd) {
                    Text = text.ToString() + (newLine ? '\n' : "")
                };

                textRange.ApplyPropertyValue(TextElement.ForegroundProperty, BrushConverter.ConvertFromString(color == "" ? Colors.White : color)!);
                textRange.ApplyPropertyValue(TextElement.FontWeightProperty, weights);

                Logger.ScrollToEnd();
            }, DispatcherPriority.Background);
        }

        public static void Information(string text) {
            Write("[", Colors.Gray, FontWeights.Normal, false);
            Write("INFO", Colors.DarkTurquoise, FontWeights.Bold, false);
            Write("] ", Colors.Gray, FontWeights.Normal, false);
            Write(text);
        }

        public static void Warning(string text) {
            Write("[", Colors.Gray, FontWeights.Normal, false);
            Write("WARNING", Colors.Yellow, FontWeights.Bold, false);
            Write("] ", Colors.Gray, FontWeights.Normal, false);
            Write(text);
        }

        public static void Error(string text) {
            Write("[", Colors.Gray, FontWeights.Normal, false);
            Write("ERROR", Colors.Red, FontWeights.Bold, false);
            Write("] ", Colors.Gray, FontWeights.Normal, false);
            Write(text);
        }

        public static void Command(string text) {
            Write("[", Colors.Gray, FontWeights.Normal, false);
            Write("COMMAND", Colors.Cyan, FontWeights.Bold, false);
            Write("] ", Colors.Gray, FontWeights.Normal, false);
            Write(text);
        }

        public static void Log(object sender, LoggerEventArgs e) {
            switch (e.EventType) {
                case LoggerEventType.Info:
                    Information(e.Message);
                    break;
                case LoggerEventType.Warn:
                    Warning(e.Message);
                    break;
                case LoggerEventType.Error:
                    Error(e.Message);
                    break;
                case LoggerEventType.Command:
                    Command(e.Message);
                    break;
            }
        }
    }
}
