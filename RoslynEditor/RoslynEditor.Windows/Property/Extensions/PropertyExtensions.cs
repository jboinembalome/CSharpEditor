namespace RoslynEditor.Windows
{
    public static class PropertyExtensions
    {
        public static bool Has(this PropertyOptions options, PropertyOptions value) =>
            (options & value) == value;
    }
}