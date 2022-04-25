using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.Text;

namespace RoslynEditor.Core.SignatureHelp
{
    public class SignatureHelpItems
    {
        #region Properties

        public IList<SignatureHelpItem> Items { get; }

        public TextSpan ApplicableSpan { get; }

        public int ArgumentIndex { get; }

        public int ArgumentCount { get; }

        public string ArgumentName { get; }

        public int? SelectedItemIndex { get; internal set; }
        #endregion

        #region Constructors

        internal SignatureHelpItems(Microsoft.CodeAnalysis.SignatureHelp.SignatureHelpItems inner)
        {
            Items = inner.Items.Select(x => new SignatureHelpItem(x)).ToArray();
            ApplicableSpan = inner.ApplicableSpan;
            ArgumentIndex = inner.ArgumentIndex;
            ArgumentCount = inner.ArgumentCount;
            ArgumentName = inner.ArgumentName;
            SelectedItemIndex = inner.SelectedItemIndex;
        }
        #endregion
    }
}