using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;

namespace RoslynEditor.Core.Scripting
{
    public sealed partial class ScriptRunner
    {
        private class DiagnosticBag
        {
            #region Fields
            private ConcurrentQueue<Diagnostic>? _lazyBag;

            #endregion

            #region Properties
            private ConcurrentQueue<Diagnostic> Bag
            {
                get
                {
                    var bag = _lazyBag;
                    if (bag != null)
                    {
                        return bag;
                    }

                    var newBag = new ConcurrentQueue<Diagnostic>();
                    return Interlocked.CompareExchange(ref _lazyBag, newBag, null) ?? newBag;
                }
            }

            public bool IsEmptyWithoutResolution
            {
                get
                {
                    var bag = _lazyBag;
                    return bag == null || bag.IsEmpty;
                }
            }
            #endregion

            #region Methods
            public void AddRange<T>(ImmutableArray<T> diagnostics) where T : Diagnostic
            {
                if (!diagnostics.IsDefaultOrEmpty)
                {
                    var bag = Bag;
                    foreach (var t in diagnostics)
                        bag.Enqueue(t);
                }
            }

            public IEnumerable<Diagnostic> AsEnumerable()
            {
                var bag = Bag;
                var foundVoid = bag.Any(diagnostic => diagnostic.Severity == DiagnosticSeverityVoid);

                return foundVoid ? AsEnumerableFiltered() : bag;
            }

            internal void Clear()
            {
                var bag = _lazyBag;
                if (bag != null)
                    _lazyBag = null;
            }

            private static DiagnosticSeverity DiagnosticSeverityVoid => ~DiagnosticSeverity.Info;

            private IEnumerable<Diagnostic> AsEnumerableFiltered() => 
                Bag.Where(diagnostic => diagnostic.Severity != DiagnosticSeverityVoid);
            #endregion
        }
    }
}
