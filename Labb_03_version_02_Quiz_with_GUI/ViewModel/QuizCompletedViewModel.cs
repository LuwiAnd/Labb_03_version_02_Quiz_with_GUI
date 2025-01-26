using Labb_03_version_02_Quiz_with_GUI.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labb_03_version_02_Quiz_with_GUI.ViewModel
{
    public class QuizCompletedViewModel : ViewModelBase
    {
        //private readonly PlayerViewModel playerViewModel;
        private readonly MainWindowViewModel? mainWindowViewModel;

        public bool ShowQuizCompletedView
        {
            get => mainWindowViewModel.ShowQuizCompletedView;
        }

        public string ProgressionString { get => mainWindowViewModel.PlayerViewModel.ProgressionString; }

        public DelegateCommand StartQuizCommand { get => mainWindowViewModel.PlayerViewModel.StartQuizCommand; }
        public QuizCompletedViewModel(MainWindowViewModel mainWindowViewModel)
        {
            //this.playerViewModel = playerViewModel;
            this.mainWindowViewModel = mainWindowViewModel;
        }
    }
}
