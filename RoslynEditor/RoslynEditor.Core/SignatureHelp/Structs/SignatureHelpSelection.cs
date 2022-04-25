namespace RoslynEditor.Core.SignatureHelp
{
    internal sealed partial class AggregateSignatureHelpProvider
    {
        private struct SignatureHelpSelection
        {
            public int? SelectedParameter { get; }

            public SignatureHelpItem SelectedItem { get; }

            public bool UserSelected { get; }

            public SignatureHelpSelection(SignatureHelpItem selectedItem, bool userSelected, int? selectedParameter)
            {
                SelectedItem = selectedItem;
                UserSelected = userSelected;
                SelectedParameter = selectedParameter;
            }
        }
    }
}