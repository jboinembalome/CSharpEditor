using System.Windows.Input;
using System.Windows.Threading;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Search;

namespace RoslynEditor.Windows
{
    public partial class SearchReplacePanel
    {
        private class SearchReplaceInputHandler : TextAreaInputHandler
        {
            #region Fields

            private readonly SearchReplacePanel _panel;
            #endregion

            #region Constructors

            internal SearchReplaceInputHandler(TextArea textArea, SearchReplacePanel panel) : base(textArea)
            {
                RegisterCommands();
                _panel = panel;
            }
            #endregion

            #region Methods

            internal void RegisterGlobalCommands(CommandBindingCollection commandBindings)
            {
                commandBindings.Add(new CommandBinding(ApplicationCommands.Find, ExecuteFind));
                commandBindings.Add(new CommandBinding(SearchCommands.FindNext, ExecuteFindNext, CanExecuteWithOpenSearchPanel));
                commandBindings.Add(new CommandBinding(SearchCommands.FindPrevious, ExecuteFindPrevious, CanExecuteWithOpenSearchPanel));
            }

            private void RegisterCommands()
            {
                CommandBindings.Add(new CommandBinding(ApplicationCommands.Find, ExecuteFind));
                CommandBindings.Add(new CommandBinding(ApplicationCommands.Replace, ExecuteReplace));
                CommandBindings.Add(new CommandBinding(SearchCommands.FindNext, ExecuteFindNext, CanExecuteWithOpenSearchPanel));
                CommandBindings.Add(new CommandBinding(SearchCommands.FindPrevious, ExecuteFindPrevious, CanExecuteWithOpenSearchPanel));
                CommandBindings.Add(new CommandBinding(SearchCommandsEx.ReplaceNext, ExecuteReplaceNext, CanExecuteWithOpenSearchPanel));
                CommandBindings.Add(new CommandBinding(SearchCommandsEx.ReplaceAll, ExecuteReplaceAll, CanExecuteWithOpenSearchPanel));
                CommandBindings.Add(new CommandBinding(SearchCommands.CloseSearchPanel, ExecuteCloseSearchPanel, CanExecuteWithOpenSearchPanel));
            }

            private void ExecuteFind(object sender, ExecutedRoutedEventArgs e) => FindOrReplace(isReplaceMode: false);

            private void ExecuteReplace(object sender, ExecutedRoutedEventArgs e) => FindOrReplace(isReplaceMode: true);

            private void FindOrReplace(bool isReplaceMode)
            {
                _panel.IsReplaceMode = isReplaceMode;
                _panel.Open();

                if (!TextArea.Selection.IsEmpty && !TextArea.Selection.IsMultiline)
                    _panel.SearchPattern = TextArea.Selection.GetText();

                TextArea.Dispatcher.InvokeAsync(() => _panel.Reactivate(), DispatcherPriority.Input);
            }

            private void CanExecuteWithOpenSearchPanel(object sender, CanExecuteRoutedEventArgs e)
            {
                if (_panel.IsClosed)
                {
                    e.CanExecute = false;
                    e.ContinueRouting = true;
                }
                else
                {
                    e.CanExecute = true;
                    e.Handled = true;
                }
            }

            private void ExecuteFindNext(object sender, ExecutedRoutedEventArgs e) => ClosePanel(e);

            private void ExecuteFindPrevious(object sender, ExecutedRoutedEventArgs e) => ClosePanel(e);

            private void ExecuteReplaceNext(object sender, ExecutedRoutedEventArgs e) => ClosePanel(e);

            private void ExecuteReplaceAll(object sender, ExecutedRoutedEventArgs e) => ClosePanel(e);

            private void ExecuteCloseSearchPanel(object sender, ExecutedRoutedEventArgs e) => ClosePanel(e);

            private void ClosePanel(ExecutedRoutedEventArgs e)
            {
                if (_panel.IsClosed)
                    return;

                _panel.Close();
                e.Handled = true;
            }
            #endregion
        }
    }
}