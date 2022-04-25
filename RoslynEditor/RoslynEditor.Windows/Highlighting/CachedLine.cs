// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;

namespace RoslynEditor.Windows
{
    internal sealed partial class RoslynSemanticHighlighter
    {
        // If a line gets edited and we need to display it while no parse information is ready for the
        // changed file, the line would flicker (semantic highlightings disappear temporarily).
        // We avoid this issue by storing the semantic highlightings and updating them on document changes
        // (using anchor movement)
        private class CachedLine
        {
            public readonly HighlightedLine HighlightedLine;
            public readonly ITextSourceVersion OldVersion;
            public readonly int Offset;

            /// <summary>
            /// Gets whether the cache line is valid (no document changes since it was created).
            /// This field gets set to false when Update() is called.
            /// </summary>
            public readonly bool IsValid;

            public IDocumentLine DocumentLine => HighlightedLine.DocumentLine;

            public CachedLine(HighlightedLine highlightedLine, ITextSourceVersion fileVersion)
            {
                HighlightedLine = highlightedLine ?? throw new ArgumentNullException(nameof(highlightedLine));
                OldVersion = fileVersion ?? throw new ArgumentNullException(nameof(fileVersion));
                IsValid = true;
                Offset = HighlightedLine.DocumentLine.Offset;
            }
        }
    }
}