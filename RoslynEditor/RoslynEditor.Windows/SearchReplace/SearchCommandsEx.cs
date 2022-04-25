using System.Windows.Input;

namespace RoslynEditor.Windows
{
    public static class SearchCommandsEx
    {
        /// <summary>Replaces the next occurrence in the document.</summary>
        public static readonly RoutedCommand ReplaceNext = new("ReplaceNext", typeof(SearchReplacePanel),
            new InputGestureCollection
            {
                new KeyGesture(Key.R, ModifierKeys.Alt)
            });

        /// <summary>Replaces all the occurrences in the document.</summary>
        public static readonly RoutedCommand ReplaceAll = new("ReplaceAll", typeof(SearchReplacePanel),
            new InputGestureCollection
            {
                new KeyGesture(Key.A, ModifierKeys.Alt)
            });
    }
}