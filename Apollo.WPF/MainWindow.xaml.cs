using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Apollo;
using Microsoft.Extensions.Configuration;

namespace Apollo.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IConfigurationRoot Configuration
            => new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

        private ApolloContext _context { get; }

        private TaggedFile SelectedFile
            => FileListBox.SelectedItem as TaggedFile;

        private Tag SelectedTag
            => TagListBox.SelectedItem as Tag;

        private ObservableCollection<Tag> Tags { get; }

        public IEnumerable<string> TagNames => Tags.Select(t => t.Name);

        public string NewTagText
        {
            get => (string)GetValue(NewTagTextProperty);
            set => SetValue(NewTagTextProperty, value);
        }

        public static readonly DependencyProperty NewTagTextProperty =
            DependencyProperty.Register("NewTagText", typeof(string), typeof(MainWindow));

        public MainWindow()
        {
            InitializeComponent();
            Dispatcher.ShutdownStarted += Dispatcher_ShutdownStarted;

            _context = new ApolloContext(Configuration);
            Tags = _context.Tags;

            _context.Save();

            DataContext = _context;
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Return)
                return;

            if (SelectedFile == null)
                return;

            var tagInput = TagTextBox.Text;

            if (SelectedFile.Tags.Any(t => t.Name == tagInput))
            {
                ClearTextBox();
                return;
            }

            var tag = Tags.FirstOrDefault(t => t.Name == tagInput)
                ?? new Tag(tagInput);

            SelectedFile.Tags.Add(tag);
            Tags.Add(tag);
            ClearTextBox();
        }

        private void ClearTextBox()
            => TagTextBox.Text = "";

        private void TagLabel_MouseDoubleClick(object sender, MouseButtonEventArgs e)
            => RemoveSelectedTagFromSelectedFile();

        private void TagListBox_KeyDown(object sender, KeyEventArgs e)
            => RemoveSelectedTagFromSelectedFile();

        private void RemoveSelectedTagFromSelectedFile()
        {
            if (!SelectedFile?.Tags.Any(t => t == SelectedTag) ?? true) // null file or no matching tags
                return;

            SelectedFile.Tags.Remove(SelectedTag);
        }

        private void Dispatcher_ShutdownStarted(object sender, EventArgs e)
        {
            _context.Save();
        }

        private void FileLabel_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (SelectedFile == null)
                return;

            var processStartInfo = new ProcessStartInfo()
            {
                FileName = "cmd.exe",
                Arguments = $"/c \"{SelectedFile.FilePath}\"",
                CreateNoWindow = true
            };

            Process.Start(processStartInfo);
        }
    }
}
