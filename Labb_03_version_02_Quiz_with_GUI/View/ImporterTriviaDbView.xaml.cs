using Labb_03_version_02_Quiz_with_GUI.ViewModel;
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
using System.Windows.Shapes;

namespace Labb_03_version_02_Quiz_with_GUI.View
{
    /// <summary>
    /// Interaction logic for ImporterTriviaDbView.xaml
    /// </summary>
    public partial class ImporterTriviaDbView : Window
    {
        //public ImporterTriviaDbView(MainWindowViewModel mainWindowViewModel)
        public ImporterTriviaDbView(ImporterTriviaDbViewModel vm)
        {
            InitializeComponent();

            // Jag skulle inte skapa en ny ImporterTriviaDbViewModel här, utan gör det i 
            // OpenImportFromTriviaDb() i MainWindowViewModel.
            //var vm = new ImporterTriviaDbViewModel(mainWindowViewModel);
            DataContext = vm;

            Loaded += async (_, _) => await vm.InitializeAsync();
        }
    }
}
