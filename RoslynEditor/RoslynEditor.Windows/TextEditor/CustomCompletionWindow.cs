using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Editing;

namespace RoslynEditor.Windows
{
    public partial class CodeTextEditor
    {
        private partial class CustomCompletionWindow : CompletionWindow
        {
            #region Fields

            private bool _isSoftSelectionActive;
            private KeyEventArgs? _keyDownArgs;
            #endregion

            #region Properties

            public bool UseHardSelection { get; set; }
            #endregion

            #region Constructors

            public CustomCompletionWindow(TextArea textArea) : base(textArea)
            {
                _isSoftSelectionActive = true;
                CompletionList.SelectionChanged += CompletionListOnSelectionChanged;

                Initialize();
            }
            #endregion

            #region Events

            private void CompletionListOnSelectionChanged(object sender, SelectionChangedEventArgs args)
            {
                if (!UseHardSelection && _isSoftSelectionActive && _keyDownArgs?.Handled != true && args.AddedItems?.Count > 0)
                    CompletionList.SelectedItem = null;
            }

            protected override void OnKeyDown(KeyEventArgs e)
            {
                if (e.Key == Key.Home || e.Key == Key.End)
                    return;

                _keyDownArgs = e;

                base.OnKeyDown(e);

                SetSoftSelection(e);
            }
            #endregion

            #region Methods

            private void Initialize()
            {
                CompletionList.ListBox.BorderThickness = new Thickness(0);
                CompletionList.ListBox.PreviewMouseDown += (o, e) => _isSoftSelectionActive = false;
            }

            private void SetSoftSelection(RoutedEventArgs e)
            {
                if (e.Handled)
                    _isSoftSelectionActive = false;
            }

            #endregion
        }
    }
}
