using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using Microsoft.CodeAnalysis.Text;

namespace RoslynEditor.Windows
{
    public sealed partial class AvalonEditTextContainer
    {
        private class AvalonEditSourceText : SourceText
        {
            #region Fields

            private readonly AvalonEditTextContainer _container;
            private readonly SourceText _sourceText;
            #endregion

            #region Constructors

            public AvalonEditSourceText(AvalonEditTextContainer container, string text) : this(container, From(text))
            {
            }

            private AvalonEditSourceText(AvalonEditTextContainer container, SourceText sourceText)
            {
                _container = container;
                _sourceText = sourceText;
            }
            #endregion

            #region Methods

            public override void CopyTo(int sourceIndex, char[] destination, int destinationIndex, int count) =>
                _sourceText.CopyTo(sourceIndex, destination, destinationIndex, count);

            public override Encoding? Encoding => _sourceText.Encoding;

            public override int Length => _sourceText.Length;

            public override char this[int position] => _sourceText[position];

            public override SourceText GetSubText(TextSpan span) =>
                new AvalonEditSourceText(_container, _sourceText.GetSubText(span));

            public override void Write(TextWriter writer, TextSpan span,
                CancellationToken cancellationToken = new CancellationToken()) =>
                _sourceText.Write(writer, span, cancellationToken);

            public override string ToString() => _sourceText.ToString();

            public override string ToString(TextSpan span) => _sourceText.ToString(span);

            public override IReadOnlyList<TextChangeRange> GetChangeRanges(SourceText oldText) =>
                _sourceText.GetChangeRanges(GetInnerSourceText(oldText));

            public override IReadOnlyList<TextChange> GetTextChanges(SourceText oldText) =>
                _sourceText.GetTextChanges(GetInnerSourceText(oldText));

            protected override TextLineCollection GetLinesCore() => _sourceText.Lines;

            protected override bool ContentEqualsImpl(SourceText other) => _sourceText.ContentEquals(GetInnerSourceText(other));

            public override SourceTextContainer Container => _container ?? _sourceText.Container;

            public override bool Equals(object? obj) => _sourceText.Equals(obj);

            public override int GetHashCode() => _sourceText.GetHashCode();

            public override SourceText WithChanges(IEnumerable<TextChange> changes) =>
                new AvalonEditSourceText(_container, _sourceText.WithChanges(changes));

            private static SourceText GetInnerSourceText(SourceText oldText) =>
                (oldText as AvalonEditSourceText)?._sourceText ?? oldText;
            #endregion
        }
    }
}
