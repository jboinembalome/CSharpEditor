using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Editing;

namespace RoslynEditor.Windows
{
    class SearchReplacePanelAdorner : Adorner
    {
        private readonly SearchReplacePanel _panel;

        public SearchReplacePanelAdorner(TextArea textArea, SearchReplacePanel panel) : base(textArea)
        {
            _panel = panel;
            AddVisualChild(panel);
        }

        protected override int VisualChildrenCount => 1;

        protected override Visual GetVisualChild(int index)
        {
            if (index != 0)
                throw new ArgumentOutOfRangeException(nameof(index));

            return _panel;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            _panel.Arrange(new Rect(new Point(0, 0), finalSize));

            return new Size(_panel.ActualWidth, _panel.ActualHeight);
        }
    }
}