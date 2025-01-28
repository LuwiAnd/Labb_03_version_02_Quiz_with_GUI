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
    /// Interaction logic for CreateQuestionPackView.xaml
    /// </summary>
    public partial class CreateQuestionPackView : Window
    {
        public CreateQuestionPackView(CreateQuestionPackViewModel viewmodel)
        {
            InitializeComponent();
            DataContext = viewmodel;
        }
    }
}
