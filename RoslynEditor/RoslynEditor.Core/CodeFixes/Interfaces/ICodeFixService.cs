﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Text;

namespace RoslynEditor.Core.CodeFixes
{
    public interface ICodeFixService
    {
        Task<IEnumerable<CodeFixCollection>> GetFixesAsync(Document document, TextSpan textSpan, bool includeSuppressionFixes, 
            CancellationToken cancellationToken);
        CodeFixProvider? GetSuppressionFixer(string language, IEnumerable<string> diagnosticIds);
    }
}