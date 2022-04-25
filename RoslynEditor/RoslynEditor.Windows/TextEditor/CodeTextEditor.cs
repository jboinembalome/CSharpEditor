using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Highlighting;

namespace RoslynEditor.Windows
{
    public partial class CodeTextEditor : TextEditor
    {
        #region Fields

        private CustomCompletionWindow? _completionWindow;
        private OverloadInsightWindow? _insightWindow;
        private ToolTip? _toolTip;

        public static readonly StyledProperty<Brush> CompletionBackgroundProperty = CommonProperty
            .Register<CodeTextEditor, Brush>(nameof(CompletionBackground), CreateDefaultCompletionBackground());

        #endregion

        #region Properties

        public bool IsCompletionWindowOpen => _completionWindow?.IsVisible == true;

        public bool IsInsightWindowOpen => _insightWindow?.IsVisible == true;

        public Brush CompletionBackground
        {
            get => this.GetValue(CompletionBackgroundProperty);
            set => this.SetValue(CompletionBackgroundProperty, value);
        }

        public ICodeEditorCompletionProvider? CompletionProvider { get; set; }

        public Func<ToolTipRequestEventArgs, Task>? AsyncToolTipRequest { get; set; }
        #endregion

        #region Constructors

        public CodeTextEditor()
        {
            Options = new TextEditorOptions
            {
                ConvertTabsToSpaces = true,
                AllowScrollBelowDocument = true,
                IndentationSize = 4,
                EnableEmailHyperlinks = false,
            };

            TextArea.TextView.VisualLinesChanged += OnVisualLinesChanged;
            TextArea.TextEntering += OnTextEntering;
            TextArea.TextEntered += OnTextEntered;

            var commandBindings = TextArea.CommandBindings;
            var deleteLineCommand = commandBindings.OfType<CommandBinding>()
                .FirstOrDefault(x => x.Command == AvalonEditCommands.DeleteLine);

            if (deleteLineCommand != null)
                commandBindings.Remove(deleteLineCommand);

            Initialize();
        }
        #endregion

        #region Events

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (e.Key == Key.Space && e.HasModifiers(ModifierKeys.Control))
            {
                e.Handled = true;
                var mode = e.HasModifiers(ModifierKeys.Shift) ? TriggerMode.SignatureHelp : TriggerMode.Completion;

                _ = ShowCompletion(mode);
            }
        }

        public static readonly RoutedEvent ToolTipRequestEvent = CommonEvent.Register<CodeTextEditor, ToolTipRequestEventArgs>(
            nameof(ToolTipRequest), RoutingStrategy.Bubble);

        public event EventHandler<ToolTipRequestEventArgs> ToolTipRequest
        {
            add => AddHandler(ToolTipRequestEvent, value);
            remove => RemoveHandler(ToolTipRequestEvent, value);
        }

        private void OnVisualLinesChanged(object? sender, EventArgs e) => _toolTip?.Close(this);

        private void OnMouseHoverStopped(object? sender, MouseEventArgs e)
        {
            if (_toolTip != null)
            {
                _toolTip.Close(this);
                e.Handled = true;
            }
        }

        private async void OnMouseHover(object sender, MouseEventArgs e)
        {
            TextViewPosition? position;

            try
            {
                position = TextArea.TextView.GetPositionFloor(e.GetPosition(TextArea.TextView) + TextArea.TextView.ScrollOffset);
            }
            catch (ArgumentOutOfRangeException)
            {
                // TODO: check why this happens
                e.Handled = true;
                return;
            }

            var args = new ToolTipRequestEventArgs { InDocument = position.HasValue };

            if (!position.HasValue || position.Value.Location.IsEmpty)
                return;

            args.LogicalPosition = position.Value.Location;
            args.Position = Document.GetOffset(position.Value.Line, position.Value.Column);

            RaiseEvent(args);

            if (args.ContentToShow == null)
            {
                var asyncRequest = AsyncToolTipRequest?.Invoke(args);
                if (asyncRequest != null)
                    await asyncRequest.ConfigureAwait(true);
            }

            if (args.ContentToShow == null)
                return;

            if (_toolTip == null)
            {
                _toolTip = new ToolTip { MaxWidth = 400 };
                InitializeToolTip();
            }

            if (args.ContentToShow is string stringContent)
            {
                _toolTip.SetContent(this, new TextBlock
                {
                    Text = stringContent,
                    TextWrapping = TextWrapping.Wrap
                });
            }
            else
            {
                _toolTip.SetContent(this, new ContentPresenter
                {
                    Content = args.ContentToShow,
                    MaxWidth = 400
                });
            }

            e.Handled = true;
            _toolTip.Open(this);

            //AfterToolTipOpen();
        }

        #endregion

        #region Methods
        public void CloseCompletionWindow()
        {
            if (_completionWindow != null)
            {
                _completionWindow.Close();
                _completionWindow = null;
            }
        }

        public void CloseInsightWindow()
        {
            if (_insightWindow != null)
            {
                _insightWindow.Close();
                _insightWindow = null;
            }
        }

        private static Brush CreateDefaultCompletionBackground() => new SolidColorBrush(Color.FromRgb(240, 240, 240)).AsFrozen();

        //partial void AfterToolTipOpen();

        #region Open & Save File

        public void OpenFile(string fileName)
        {
            if (!File.Exists(fileName))
                throw new FileNotFoundException(fileName);

            _completionWindow?.Close();
            _insightWindow?.Close();

            Load(fileName);

            Document.FileName = fileName;

            SyntaxHighlighting = HighlightingManager.Instance.GetDefinitionByExtension(Path.GetExtension(fileName));
        }

        public bool SaveFile()
        {
            if (string.IsNullOrEmpty(Document.FileName))
                return false;

            Save(Document.FileName);

            return true;
        }

        #endregion

        #region Code Completion

        private void OnTextEntered(object sender, TextCompositionEventArgs e) => _ = ShowCompletion(TriggerMode.Text);

        private async Task ShowCompletion(TriggerMode triggerMode)
        {
            if (CompletionProvider == null)
                return;

            GetCompletionDocument(out var offset);

            var completionChar = triggerMode == TriggerMode.Text ? Document.GetCharAt(offset - 1) : (char?)null;
            var results = await CompletionProvider.GetCompletionData(offset, completionChar,
                        triggerMode == TriggerMode.SignatureHelp).ConfigureAwait(true);

            if (results.OverloadProvider != null)
            {
                results.OverloadProvider.Refresh();

                if (_insightWindow != null && _insightWindow.IsOpen())
                    _insightWindow.Provider = results.OverloadProvider;
                else
                {
                    _insightWindow = new OverloadInsightWindow(TextArea)
                    {
                        Provider = results.OverloadProvider,
                        //Background = CompletionBackground,
                    };

                    InitializeInsightWindow();

                    _insightWindow.Closed += (o, args) => _insightWindow = null;
                    _insightWindow.Show();
                }

                return;
            }

            if (_completionWindow?.IsOpen() != true && results.CompletionData != null && results.CompletionData.Any())
            {
                _insightWindow?.Close();

                // Open code completion after the user has pressed dot:
                _completionWindow = new CustomCompletionWindow(TextArea)
                {
                    MinWidth = 300,
                    CloseWhenCaretAtBeginning = triggerMode == TriggerMode.Completion || triggerMode == TriggerMode.Text,
                    UseHardSelection = results.UseHardSelection,
                };

                InitializeCompletionWindow();

                if (completionChar != null && char.IsLetterOrDigit(completionChar.Value))
                    _completionWindow.StartOffset -= 1;

                var data = _completionWindow.CompletionList.CompletionData;
                ICompletionDataEx? selected = null;

                foreach (var completion in results.CompletionData)
                {
                    if (completion.IsSelected)
                        selected = completion;

                    data.Add(completion);
                }

                try
                {
                    _completionWindow.CompletionList.SelectedItem = selected;
                }
                catch (Exception)
                {
                    // TODO-AV: Fix this in AvaloniaEdit
                }

                _completionWindow.Closed += (o, args) => { _completionWindow = null; };
                _completionWindow.Show();
            }
        }

        private void OnTextEntering(object sender, TextCompositionEventArgs args)
        {
            if (args.Text.Length > 0 && _completionWindow != null)
                if (!IsCharIdentifier(args.Text[0]))
                    // Whenever no identifier letter is typed while the completion window is open,
                    // insert the currently selected element.
                    _completionWindow.CompletionList.RequestInsertion(args);
            // Do not set e.Handled=true.
            // We still want to insert the character that was typed.
        }
        /// <summary>
        /// Checks if a provided char is a well-known identifier
        /// </summary>
        /// <param name="c">The charcater to check</param>
        /// <returns><c>true</c> if <paramref name="c"/> is a well-known identifier.</returns>
        private static bool IsCharIdentifier(char c) => char.IsLetterOrDigit(c) || c == '_';

        /// <summary>
        /// Gets the document used for code completion, can be overridden to provide a custom document
        /// </summary>
        /// <param name="offset"></param>
        /// <returns>The document of this text editor.</returns>
        protected virtual IDocument GetCompletionDocument(out int offset)
        {
            offset = CaretOffset;

            return Document;
        }

        #endregion

        #endregion
    }
}
