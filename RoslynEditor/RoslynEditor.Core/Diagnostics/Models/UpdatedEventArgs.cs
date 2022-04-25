using System;
using Microsoft.CodeAnalysis;

namespace RoslynEditor.Core.Diagnostics
{
    public class UpdatedEventArgs : EventArgs
    {
        #region Properties

        public object Id { get; }

        public Workspace Workspace { get; }

        public ProjectId? ProjectId { get; }

        public DocumentId? DocumentId { get; }
        #endregion

        #region Constructors

        internal UpdatedEventArgs(Microsoft.CodeAnalysis.Common.UpdatedEventArgs inner)
        {
            Id = inner.Id;
            Workspace = inner.Workspace;
            ProjectId = inner.ProjectId;
            DocumentId = inner.DocumentId;
        }
        #endregion
    }
}