using System;
using System.Collections.Generic;
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
        static IConfigurationRoot Configuration
            => new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

        public MainWindow()
        {
            InitializeComponent();
            var context = new ApolloContext(Configuration);
            context.Save();

            DataContext = context;
        }
    }
}
