using System;
using Microsoft.CodeAnalysis.Text;

namespace RoslynEditor.Core.QuickInfo
{
    public sealed class QuickInfoItem
    {
        private readonly Func<object> _contentFactory;

        public TextSpan TextSpan { get; }

        public object Create() => _contentFactory();

        internal QuickInfoItem(TextSpan textSpan, Func<object> contentFactory)
        {
            TextSpan = textSpan;
            _contentFactory = contentFactory;
        }
    }
}