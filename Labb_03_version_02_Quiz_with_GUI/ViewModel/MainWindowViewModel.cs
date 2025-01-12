using Labb_03_version_02_Quiz_with_GUI.Command;
using Labb_03_version_02_Quiz_with_GUI.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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


        /* Lösningen med CurrentView och ContentControl gör så att bindings slutar att fungera, så 
         * jag testar en enklare lösning tillsvidare.
        private object _currentView;
        public object CurrentView
        {
            get { return _currentView; }
            set {
                _currentView = value;
                RaisePropertyChanged();
            }
        }
        */

        private bool _showConfigurationView;
        public bool ShowConfigurationView
        {
            get { return _showConfigurationView; }
            set {
                Debug.Write($"Nu har _showConfigurationView värdet {_showConfigurationView} som uppdateras till ");
                _showConfigurationView = value;
                Debug.WriteLine(_showConfigurationView);
                if (value)
                {
                    ShowPlayerView = false;
                }
                RaisePropertyChanged(); // Ingenting bindar till den här, men jag har kvar denna kodrad för att slippa felsökning om den behövs senare.
                ConfigurationViewModel.RaisePropertyChanged();
                SwitchToConfigurationViewCommand.RaiseCanExecuteChanged();
                SwitchToPlayerViewCommand.RaiseCanExecuteChanged();
            }
        }

        private bool _showPlayerView;
        public bool ShowPlayerView
        {
            get { return _showPlayerView; }
            set { 
                _showPlayerView = value;
                if (value)
                {
                    ShowConfigurationView = false;
                }
                //RaisePropertyChanged();
                PlayerViewModel.RaisePropertyChanged();
                SwitchToConfigurationViewCommand.RaiseCanExecuteChanged();
                SwitchToPlayerViewCommand.RaiseCanExecuteChanged();
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

            

            //SwitchToConfigurationViewCommand = new DelegateCommand(_ => CurrentView = ConfigurationViewModel, _ => CurrentView != ConfigurationViewModel);
            //SwitchToPlayerViewCommand = new DelegateCommand(_ => CurrentView = PlayerViewModel, _ => CurrentView != PlayerViewModel);
            SwitchToConfigurationViewCommand = new DelegateCommand(
                _ => { ShowConfigurationView = true; ShowPlayerView = false; },
                _ => !ShowConfigurationView
            );
            SwitchToPlayerViewCommand = new DelegateCommand(
                _ => { ShowConfigurationView = false; ShowPlayerView = true; },
                _ => !ShowPlayerView
            );

            //CurrentView = ConfigurationViewModel;
            ShowConfigurationView =  true;
            ShowPlayerView =  false;
        }
    }
}
