using System;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using Microsoft.CodeAnalysis.Text;
using TextChangeEventArgs = Microsoft.CodeAnalysis.Text.TextChangeEventArgs;
using RoslynEditor.Core;

namespace RoslynEditor.Windows
{
    public sealed partial class AvalonEditTextContainer : SourceTextContainer, IEditorCaretProvider, IDisposable
    {
        #region Fields

        private SourceText _currentText;
        private bool _updatding;

        public override event EventHandler<TextChangeEventArgs>? TextChanged;
        #endregion

        #region Properties

        public TextDocument Document { get; }

        /// <summary>
        /// If set, <see cref="TextEditor.CaretOffset"/> will be updated.
        /// </summary>
        public TextEditor? Editor { get; set; }

        public override SourceText CurrentText => _currentText;

        int IEditorCaretProvider.CaretPosition => Editor?.CaretOffset ?? 0;
        #endregion

        #region Constructors

        public AvalonEditTextContainer(TextDocument document)
        {
            Document = document ?? throw new ArgumentNullException(nameof(document));
            _currentText = new AvalonEditSourceText(this, Document.Text);

            Document.Changed += DocumentOnChanged;
        }
        #endregion

        #region Events

        private void DocumentOnChanged(object? sender, DocumentChangeEventArgs e)
        {
            if (_updatding) 
                return;

            var oldText = _currentText;
            var textSpan = new TextSpan(e.Offset, e.RemovalLength);
            var textChangeRange = new TextChangeRange(textSpan, e.InsertionLength);

            _currentText = _currentText.WithChanges(new TextChange(textSpan, e.InsertedText?.Text ?? string.Empty));

            TextChanged?.Invoke(this, new TextChangeEventArgs(oldText, _currentText, textChangeRange));
        }
        #endregion

        #region Methods

        public void Dispose() => Document.Changed -= DocumentOnChanged;

        public void UpdateText(SourceText newText)
        {
            _updatding = true;

            Document.BeginUpdate();

            var editor = Editor;
            var caretOffset = editor?.CaretOffset ?? 0;
            var documentOffset = 0;

            try
            {
                var changes = newText.GetTextChanges(_currentText);

                foreach (var change in changes)
                {
                    var newTextChange = change.NewText ?? string.Empty;
                    Document.Replace(change.Span.Start + documentOffset, change.Span.Length, new StringTextSource(newTextChange));

                    var changeOffset = newTextChange.Length - change.Span.Length;
                    if (caretOffset >= change.Span.Start + documentOffset + change.Span.Length)
                        // If caret is after text, adjust it by text size difference
                        caretOffset += changeOffset;
                    else if (caretOffset >= change.Span.Start + documentOffset)
                        // If caret is inside changed text, but go out of bounds of the replacing text after the change,
                        // go back inside
                        if (caretOffset >= change.Span.Start + documentOffset + newTextChange.Length)
                            caretOffset = change.Span.Start + documentOffset;

                    documentOffset += changeOffset;
                }

                _currentText = newText;
            }
            finally
            {
                _updatding = false;

                Document.EndUpdate();

                if (caretOffset < 0)
                    caretOffset = 0;

                if (caretOffset > newText.Length)
                    caretOffset = newText.Length;

                if (editor != null)
                    editor.CaretOffset = caretOffset;
            }
        }

        bool IEditorCaretProvider.TryMoveCaret(int position)
        {
            if (Editor != null)
                Editor.CaretOffset = position;

            return true;
        }

        #endregion
    }
}
