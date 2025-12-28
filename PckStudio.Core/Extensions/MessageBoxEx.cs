using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PckStudio.Core.Extensions
{
    public static class MessageBoxEx
    {
        public static MessageBoxResult Show(string messageBoxText, string title, MessageBoxButton button, MessageBoxImage icon) => MessageBox.Show(messageBoxText, title, button, icon);

        public static void ShowError(string messageBoxText, string title) => Show(messageBoxText, title, MessageBoxButton.OK, MessageBoxImage.Error);
        public static MessageBoxResult ShowWarning(string messageBoxText, string title, MessageBoxButton button) => Show(messageBoxText, title, button, MessageBoxImage.Warning);
        public static MessageBoxResult AskQuestion(string question, string title, MessageBoxButton button) => Show(question, title, button, MessageBoxImage.Question);
        public static MessageBoxResult ShowInfo(string messageBoxText, string title) => Show(messageBoxText, title, MessageBoxButton.OK, MessageBoxImage.Information);
    }
}
