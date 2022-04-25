﻿using System.Collections.Generic;

namespace RoslynEditor.Windows
{
    public sealed class CompletionResult
    {
        public CompletionResult(IList<ICompletionDataEx>? completionData, IOverloadProviderEx? overloadProvider, bool useHardSelection)
        {
            CompletionData = completionData;
            OverloadProvider = overloadProvider;
            UseHardSelection = useHardSelection;
        }

        public bool UseHardSelection { get; }

        public IList<ICompletionDataEx>? CompletionData { get; }

        public IOverloadProviderEx? OverloadProvider { get; }
    }
}