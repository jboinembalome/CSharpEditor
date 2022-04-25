// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Host;
using Microsoft.CodeAnalysis.Text;
using System;

namespace RoslynEditor.Core
{
    public class RoslynWorkspace : Workspace
    {
        public event Action<DocumentId, SourceText>? ApplyingTextChange;

        #region Properties

        public DocumentId? OpenDocumentId { get; private set; }
        public RoslynHost? RoslynHost { get; }
        public override bool CanOpenDocuments => true;
        #endregion

        #region Constructors

        public RoslynWorkspace(HostServices hostServices, string workspaceKind = WorkspaceKind.Host, 
            RoslynHost? roslynHost = null): base(hostServices, workspaceKind)
        {
            DiagnosticProvider.Enable(this, DiagnosticProvider.Options.Semantic);

            RoslynHost = roslynHost;
        }

        #endregion

        #region Methods

        public new void SetCurrentSolution(Solution solution)
        {
            var oldSolution = CurrentSolution;
            var newSolution = base.SetCurrentSolution(solution);
            RaiseWorkspaceChangedEventAsync(WorkspaceChangeKind.SolutionChanged, oldSolution, newSolution);
        }

        public override bool CanApplyChange(ApplyChangesKind feature)
        {
            return feature switch
            {
                ApplyChangesKind.ChangeDocument or ApplyChangesKind.ChangeDocumentInfo or ApplyChangesKind.AddMetadataReference 
                or ApplyChangesKind.RemoveMetadataReference or ApplyChangesKind.AddAnalyzerReference 
                or ApplyChangesKind.RemoveAnalyzerReference => true,
                _ => false,
            };
        }

        public void OpenDocument(DocumentId documentId, SourceTextContainer textContainer)
        {
            OpenDocumentId = documentId;
            OnDocumentOpened(documentId, textContainer);
            OnDocumentContextUpdated(documentId);
        }

        protected override void Dispose(bool finalize)
        {
            base.Dispose(finalize);

            ApplyingTextChange = null;

            DiagnosticProvider.Disable(this);
        }

        protected override void ApplyDocumentTextChanged(DocumentId document, SourceText newText)
        {
            if (OpenDocumentId != document)
                return;

            ApplyingTextChange?.Invoke(document, newText);

            OnDocumentTextChanged(document, newText, PreservationMode.PreserveIdentity);
        }

        #endregion
    }
}
