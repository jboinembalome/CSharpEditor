using ICSharpCode.AvalonEdit.CodeCompletion;
using System.Windows;
using System.Windows.Controls;

namespace RoslynEditor.Windows
{
    partial class CodeTextEditor
    {
        #region Fields

        private SearchReplacePanel? _searchReplacePanel;
        #endregion

        #region Properties

        public SearchReplacePanel SearchReplacePanel => _searchReplacePanel!;
        #endregion

        #region Methods

        private void Initialize()
        {
            //ShowLineNumbers = true;

            MouseHover += OnMouseHover;
            MouseHoverStopped += OnMouseHoverStopped;

            ToolTipService.SetInitialShowDelay(this, 0);
            _searchReplacePanel = SearchReplacePanel.Install(this);
        }

        private void InitializeToolTip()
        {
            if (_toolTip != null)
            {
                _toolTip.Closed += (o, a) => _toolTip = null;
                ToolTipService.SetInitialShowDelay(_toolTip, 0);
                _toolTip.PlacementTarget = this; // required for property inheritance
            }
        }

        private void InitializeInsightWindow()
        {
            if (_insightWindow != null)
                _insightWindow.Style = TryFindResource(typeof(InsightWindow)) as Style;
        }

        private void InitializeCompletionWindow()
        {
            if (_completionWindow != null)
                _completionWindow.Background = CompletionBackground;
        }

        #endregion
    }
}
