using Labb_03_version_02_Quiz_with_GUI.Command;
using Labb_03_version_02_Quiz_with_GUI.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labb_03_version_02_Quiz_with_GUI.ViewModel
{
    internal class MainWindowViewModel : ViewModelBase
    {
        public ObservableCollection<QuestionPackViewModel> Packs { get; set; }

        public ConfigurationViewModel ConfigurationViewModel{ get; }

        public PlayerViewModel PlayerViewModel{ get; }

        private object _currentView;

        public object CurrentView
        {
            get { return _currentView; }
            set {
                _currentView = value;
                RaisePropertyChanged();
            }
        }













        // Lektion 112. Frågetecknet talar om för kompilatorn att vi är medvetna om att variabeln kan vara null.
        private QuestionPackViewModel? _activePack;

        public QuestionPackViewModel? ActivePack
        {
            get => _activePack;
            set
            {
                _activePack = value;

                if(_activePack == null)
                {
                    this.HasActivePack = false;
                }
                else
                {
                    this.HasActivePack = true;
                }

                // Lektion 112. Alla som gjort en binding kommer få meddelande när denna ändras.
                RaisePropertyChanged();

                // Lektion 114. 46 minuter in. Vi skriver nedanstående rad för att ConfigurationView.cs inte har en egen set-funktion för detta.
                // Vi behöver troligtvis göra liknande detta på flera ställen när vi gör Labb3!
                //ConfigurationViewModel.RaisePropertyChanged("Active Pack"); // Luwi 2025-01-09. Så här stod det innan, men jag tror att mellanslaget var ett typo.
                ConfigurationViewModel.RaisePropertyChanged("ActivePack");
            }
        }

        private bool _hasActivePack;

        public bool HasActivePack
        {
            get { return _hasActivePack; }
            set { 
                _hasActivePack = value;
                RaisePropertyChanged();
            }
        }

        private Question? _selectedQuestion;
        public Question? SelectedQuestion
        {
            get => _selectedQuestion;
            set
            {
                if(_selectedQuestion != value)
                {
                    _selectedQuestion = value;
                    RaisePropertyChanged();

                    
                }

                HasSelectedQuestion = _selectedQuestion != null;
                ConfigurationViewModel.RaisePropertyChanged("SelectedQuestion"); // Tillagt 2025-01-09.
                ConfigurationViewModel.RemoveQuestionCommand.RaiseCanExecuteChanged();
            }
        }


        private bool _hasSelectedQuestion;

        public bool HasSelectedQuestion
        {
            get { return _hasSelectedQuestion; }
            set { 
                _hasSelectedQuestion = value;
                RaisePropertyChanged();
                ConfigurationViewModel.RaisePropertyChanged("HasSelectedQuestion");
            }
        }




        public DelegateCommand SwitchToConfigurationViewCommand { get; }
        public DelegateCommand SwitchToPlayerViewCommand { get; }




        public MainWindowViewModel()
        {
            ConfigurationViewModel = new ConfigurationViewModel(this);
            PlayerViewModel = new PlayerViewModel(this);
            // Lektion 112. 26 minuter.
            ActivePack = new QuestionPackViewModel(new QuestionPack("My Question Pack"));

            // Testfrågor tillagda 2025-01-04.
            ActivePack.Questions.Add(new Question(
                query: "Vad vill man svara på frågor?",
                correctAnswer: "korrekt svar",
                incorrectAnswer1: "inkorr1",
                incorrectAnswer2: "inkorr2",
                incorrectAnswer3: "inkorr3kt"
            ));

            SelectedQuestion = ActivePack.Questions.FirstOrDefault();

            CurrentView = ConfigurationViewModel;

            SwitchToConfigurationViewCommand = new DelegateCommand(_ => CurrentView = ConfigurationViewModel, _ => CurrentView != ConfigurationViewModel);
            SwitchToPlayerViewCommand = new DelegateCommand(_ => CurrentView = PlayerViewModel, _ => CurrentView != PlayerViewModel);
        }
    }
}
