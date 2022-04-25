using Microsoft.CodeAnalysis;
using RoslynEditor.Core;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace RoslynEditor.Windows.Sample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ObservableCollection<DocumentViewModel> _documents;
        private RoslynHost _host;

        public MainWindow()
        {
            InitializeComponent();

            _documents = new ObservableCollection<DocumentViewModel>();
            Items.ItemsSource = _documents;
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnLoaded;

            _host = new RoslynHost(additionalAssemblies: new[]
            {
                Assembly.Load("RoslynEditor.Core.Windows"),
                Assembly.Load("RoslynEditor.Windows")
            }, RoslynHostReferences.NamespaceDefault.With(new[]
            { 
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(System.Text.RegularExpressions.Regex).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(System.Linq.Enumerable).Assembly.Location),
            }));

            AddNewDocument();
        }

        private void AddNewDocument(DocumentViewModel previous = null)
        {
            _documents.Add(new DocumentViewModel(_host, previous));
        }

        private void OnItemLoaded(object sender, EventArgs e)
        {
            var editor = (RoslynCodeEditor)sender;
            editor.Loaded -= OnItemLoaded;
            editor.Focus();

            editor.TextArea.SelectionCornerRadius = 0;
            editor.TextArea.SelectionBorder = new Pen(new SolidColorBrush(Colors.Black), 0);
            editor.TextArea.SelectionBrush = new SolidColorBrush(Color.FromArgb(100, 51, 153, 255));
            editor.TextArea.SelectionForeground = new SolidColorBrush(Color.FromRgb(220, 220, 220));

            editor.SearchReplacePanel.MarkerBrush = new SolidColorBrush(Color.FromRgb(119, 56, 0));

            var viewModel = (DocumentViewModel)editor.DataContext;
            var workingDirectory = Directory.GetCurrentDirectory();

            var previous = viewModel.LastGoodPrevious;
            if (previous != null)
            {
                editor.CreatingDocument += (o, args) =>
                {
                    args.DocumentId = _host.AddRelatedDocument(previous.Id, new DocumentCreationArgs(
                        args.TextContainer, workingDirectory, args.ProcessDiagnostics,
                        args.TextContainer.UpdateText));
                };
            }

            var documentId = editor.Initialize(_host, new DarkModeHighlightColors(),
                workingDirectory, string.Empty);

            viewModel.Initialize(documentId);
        }

        private async void OnEditorKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var editor = (RoslynCodeEditor)sender;
                if (editor.IsCompletionWindowOpen)
                {
                    return;
                }

                e.Handled = true;

                var viewModel = (DocumentViewModel)editor.DataContext;
                if (viewModel.IsReadOnly) return;

                viewModel.Text = editor.Text;
                if (await viewModel.TrySubmit())
                {
                    AddNewDocument(viewModel);
                }
            }
        }
    }
}
