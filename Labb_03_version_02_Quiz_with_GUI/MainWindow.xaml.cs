using Labb_03_version_02_Quiz_with_GUI.Model;
using Labb_03_version_02_Quiz_with_GUI.ViewModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Labb_03_version_02_Quiz_with_GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            // Lektion 112. När vi sätter denna DataContext, så gör det att vi kan bind:a mot Packs och ActivePack.
            DataContext = new MainWindowViewModel();
        }
    }
}