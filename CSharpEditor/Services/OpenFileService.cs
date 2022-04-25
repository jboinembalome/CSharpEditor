using Microsoft.Win32;
using CSharpEditor.Interfaces;

namespace CSharpEditor.Services
{
    public class OpenFileService : IOpenFileService
    {
        public string OpenFileDialog(bool checkFileExists = true)
        {
            OpenFileDialog openFileDialog = new()
            {
                CheckFileExists = checkFileExists
            };

            return openFileDialog.ShowDialog() ?? false ? openFileDialog.FileName : string.Empty;
        }

        public string OpenFileDialog(string defaultPath, bool checkFileExists = true)
        {
            OpenFileDialog openFileDialog = new()
            {
                InitialDirectory = defaultPath,
                CheckFileExists = checkFileExists
            };

            return openFileDialog.ShowDialog() ?? false ? openFileDialog.FileName : string.Empty;
        }
    }
}
