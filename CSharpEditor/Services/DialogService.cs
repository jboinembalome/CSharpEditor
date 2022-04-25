using System.Windows;
using CSharpEditor.Interfaces;

namespace CSharpEditor.Services
{
    public class DialogService : IDialogService
    {
        public void ShowMessage(string message) => MessageBox.Show(message);
    }
}
