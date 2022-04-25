using Microsoft.CodeAnalysis;
using RoslynEditor.Core.Diagnostics;
using System;
using System.Windows;

namespace RoslynEditor.Windows
{
    public class CreatingDocumentEventArgs : RoutedEventArgs
    {
        public AvalonEditTextContainer TextContainer { get; }

        public Action<DiagnosticsUpdatedArgs> ProcessDiagnostics { get; }

        public DocumentId? DocumentId { get; set; }

        public CreatingDocumentEventArgs(AvalonEditTextContainer textContainer, Action<DiagnosticsUpdatedArgs> processDiagnostics)
        {
            TextContainer = textContainer;
            ProcessDiagnostics = processDiagnostics;
            RoutedEvent = RoslynCodeEditor.CreatingDocumentEvent;
        }
    }
}
