using CSharpEditor.Interfaces;
using HighlightingLib.Interfaces;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.CSharp.Scripting.Hosting;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis.Scripting.Hosting;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TextEditLib.ViewModels;

namespace CSharpEditor.ViewModels
{
    public class MainViewModel : ObservableRecipient
    {
        #region Fields
        private readonly IThemedHighlightingManager _hlManager;
        private readonly IOpenFileService _openFileService;
        private readonly IFileService _fileService;
        private readonly IDialogService _dialogService;
        private string result;
        #endregion

        #region Constructors

        public MainViewModel(IOpenFileService openFileService, IFileService fileService, IDialogService dialogService)
        {
            _openFileService = openFileService;
            _fileService = fileService;
            _dialogService = dialogService;

            InitializeCommands();
        }

        public MainViewModel(IOpenFileService openFileService, IFileService fileService, IDialogService dialogService, 
            IThemedHighlightingManager hlManager) : this(openFileService, fileService, dialogService)
        {
            _hlManager = hlManager;
            DocumentViewModel = new DocumentViewModel(_hlManager);
        }
        #endregion

        #region Properties

        /// <summary>
		/// Gets the DocumentViewModel and all its properties and commands.
		/// </summary>
		public DocumentViewModel DocumentViewModel { get; }

        /// <summary>
        /// Gets the script.
        /// </summary>
        public Script<object> Script { get; private set; }

        /// <summary>
        /// Gets the value that checks if the script has an error.
        /// </summary>
        public bool HasError { get; private set; }

        /// <summary>
        /// Gets/Sets the result of the script.
        /// </summary>
        public string Result
        {
            get => result;
            private set => SetProperty(ref result, value);
        }

        /// <summary>
        /// Gets the value that check if the script has a submission result.
        /// </summary>
        private static MethodInfo HasSubmissionResult { get; } =
            typeof(Compilation).GetMethod(nameof(HasSubmissionResult),
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        /// <summary>
        /// Gets the print options.
        /// </summary>
        private static PrintOptions PrintOptions { get; } =
            new PrintOptions { MemberDisplayFormat = MemberDisplayFormat.SeparateLines };
        #endregion

        #region Commands
        /// <summary>
        /// Gets a command that opens a file dialog.
        /// </summary>
        public ICommand OpenFileCommand { get; private set; }

        /// <summary>
        /// Gets a command that displays the script text in a dialog box.
        /// </summary>
        public ICommand GetTextCommand { get; private set; }

        /// <summary>
        /// Gets a command that runs the script.
        /// </summary>
        public ICommand RunScriptCommand { get; private set; }

        /// <summary>
        /// Gets a command that stops (abort) the script.
        /// </summary>
        public ICommand StopScriptCommand { get; private set; }
        #endregion

        #region Methods
        /// <summary>
        /// Initializes all commands.
        /// </summary>
        public void InitializeCommands()
        {
            OpenFileCommand = new RelayCommand(
                                  () =>
                                  {
                                      var filePath = _openFileService.OpenFileDialog();

                                      if (File.Exists(filePath))
                                      {
                                          if (DocumentViewModel.Document == null)
                                              DocumentViewModel.Document = new TextDocument();
                                          else
                                              DocumentViewModel.Document.Text = string.Empty;

                                          var fileEncoding = _fileService.GetEncoding(filePath);
                                          var fileInformation = _fileService.ReadToEnd(filePath, fileEncoding);
                                          var fileExtension = Path.GetExtension(filePath);

                                          DocumentViewModel.FilePath = filePath;

                                          DocumentViewModel.Document = new TextDocument(fileInformation.Text);
                                          DocumentViewModel.FileEncoding = fileInformation.CurrentEncoding;
                                          DocumentViewModel.IsContentLoaded = true;

                                          // Setting this to null and then to some useful value ensures that the Foldings work
                                          // Installing Folding Manager is invoked via HighlightingChange
                                          // (so this works even when changing from test.XML to test1.XML)
                                          DocumentViewModel.HighlightingDefinition = null;
                                          DocumentViewModel.HighlightingDefinition = _hlManager.GetDefinitionByExtension(fileExtension);
                                      }
                                  });

            GetTextCommand = new RelayCommand(
                                          () =>
                                          {
                                              _dialogService.ShowMessage(DocumentViewModel.Document.Text);
                                          });

            RunScriptCommand = new AsyncRelayCommand(TrySubmit);

            StopScriptCommand = new RelayCommand(
                                  () =>
                                  {

                                  });
        }

        /// <summary>
        /// Try to submit the script.
        /// </summary>
        /// <returns></returns>
        private async Task<bool> TrySubmit()
        {
            Result = null;

            using (var loader = new InteractiveAssemblyLoader())
            {
                var code = DocumentViewModel.Document.Text;
                var opts = ScriptOptions.Default
                    .AddReferences(new[]
                    {
                        MetadataReference.CreateFromFile(typeof(MessageBox).Assembly.Location),
                        MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                        MetadataReference.CreateFromFile(typeof(System.Text.RegularExpressions.Regex).Assembly.Location),
                        MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location),
                    })
                    .AddImports(new[]
                    {
                        "System",
                        "System.Threading",
                        "System.Threading.Tasks",
                        "System.Collections",
                        "System.Collections.Generic",
                        "System.Text",
                        "System.Text.RegularExpressions",
                        "System.Linq",
                        "System.IO",
                        "System.Reflection",
                        "System.Windows"
                    });
                Script = CSharpScript.Create(code, opts, assemblyLoader: loader);
            }

            var compilation = Script.GetCompilation();
            var hasResult = (bool)HasSubmissionResult.Invoke(compilation, null);
            var diagnostics = Script.Compile();

            if (diagnostics.Any(t => t.Severity == DiagnosticSeverity.Error))
            {
                Result = string.Join(Environment.NewLine, diagnostics.Select(FormatObject));
                return false;
            }

            DocumentViewModel.IsReadOnly = true;

            await RunScript(hasResult);
            return true;
        }

        /// <summary>
        /// Runs the script.
        /// </summary>
        /// <param name="hasResult"></param>
        /// <returns></returns>
        private async Task RunScript(bool hasResult)
        {
            try
            {
                var result = await Script.RunAsync();

                if (result.Exception != null)
                {
                    HasError = true;
                    Result = FormatException(result.Exception);
                }
                else
                    Result = hasResult ? FormatObject(result.ReturnValue) : null;
            }
            catch (Exception ex)
            {
                HasError = true;
                Result = FormatException(ex);
            }
            finally
            {
                Script = null;
                GC.Collect();
            }
        }

        /// <summary>
        /// Gets the script exception in a good format.
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        private static string FormatException(Exception ex) => CSharpObjectFormatter.Instance.FormatException(ex);

        /// <summary>
        /// Gets the script result in a good format.
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        private static string FormatObject(object o) => CSharpObjectFormatter.Instance.FormatObject(o, PrintOptions);
        #endregion
    }
}