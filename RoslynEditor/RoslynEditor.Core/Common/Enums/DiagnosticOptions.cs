using System;
using Microsoft.CodeAnalysis;

namespace RoslynEditor.Core
{
    [Flags]
    public enum DiagnosticOptions
    {
        /// <summary>
        /// Include syntax errors
        /// </summary>
        Syntax = DiagnosticProvider.Options.Syntax,

        /// <summary>
        /// Include semantic errors
        /// </summary>
        Semantic = DiagnosticProvider.Options.Semantic,
    }
}