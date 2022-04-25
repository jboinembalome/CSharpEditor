namespace RoslynEditor.Core.LanguageServices
{
    internal interface IRoslynDialog
    {
        object ViewModel { get; set; }

        bool? Show();
    }
}