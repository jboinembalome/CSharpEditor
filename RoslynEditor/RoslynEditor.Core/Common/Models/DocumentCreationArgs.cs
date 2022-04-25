using System;
using Microsoft.CodeAnalysis.Text;
using RoslynEditor.Core.Diagnostics;

namespace RoslynEditor.Core
{
    public class DocumentCreationArgs
    {
        #region Properties

        public SourceTextContainer SourceTextContainer { get; }
        public string WorkingDirectory { get; }
        public Action<DiagnosticsUpdatedArgs>? OnDiagnosticsUpdated { get; }
        public Action<SourceText>? OnTextUpdated { get; }
        public string? Name { get; }
        #endregion

        #region Constructors

        public DocumentCreationArgs(SourceTextContainer sourceTextContainer, string workingDirectory, Action<DiagnosticsUpdatedArgs>? onDiagnosticsUpdated = null, Action<SourceText>? onTextUpdated = null, string? name = null)
        {
            SourceTextContainer = sourceTextContainer;
            WorkingDirectory = workingDirectory;
            OnDiagnosticsUpdated = onDiagnosticsUpdated;
            OnTextUpdated = onTextUpdated;
            Name = name;
        }
        #endregion
    }
}