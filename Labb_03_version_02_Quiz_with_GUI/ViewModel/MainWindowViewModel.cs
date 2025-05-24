using Labb_03_version_02_Quiz_with_GUI.Command;
using Labb_03_version_02_Quiz_with_GUI.Model;
using Labb_03_version_02_Quiz_with_GUI.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.IO;
using System.Windows;

namespace Labb_03_version_02_Quiz_with_GUI.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        public ObservableCollection<QuestionPackViewModel> Packs { get; set; }

        public ConfigurationViewModel ConfigurationViewModel{ get; }

        public PlayerViewModel PlayerViewModel{ get; }

        public QuizCompletedViewModel QuizCompletedViewModel { get; }


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

                // Oavsett om jag växlar till eller från configurationsvyn, så vill jag stanna tiden.
                PlayerViewModel.timer.Stop();
                if (!_showConfigurationView && value)
                {
                    // Oavsett om jag växlar till eller från configuration, så vill jag stanna tiden.
                    //PlayerViewModel.timer.Stop(); 

                    // Detta gör jag för att spelaren inte ska få mer eller mindre tid om han
                    // eller hon startar ett nytt Quiz, för annars kommer timer ihåg hur lång
                    // tid som hade gått av intervallet och startar därifrån.
                    PlayerViewModel.timer.Interval = TimeSpan.FromSeconds(1); 
                }
                if (value)
                {
                    ShowPlayerView = false;
                    ShowQuizCompletedView = false;
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
                if(!_showPlayerView && value)
                {
                    PlayerViewModel.timer.Start();
                }
                _showPlayerView = value;
                if (value)
                {
                    ShowConfigurationView = false;
                    ShowQuizCompletedView = false;
                    PlayerViewModel.timer.Start();
                }
                //RaisePropertyChanged();
                PlayerViewModel.RaisePropertyChanged();
                SwitchToConfigurationViewCommand.RaiseCanExecuteChanged();
                SwitchToPlayerViewCommand.RaiseCanExecuteChanged();
            }
        }

        private bool _showQuizCompletedView;
        public bool ShowQuizCompletedView
        {
            get { return _showQuizCompletedView; }
            set
            {
                _showQuizCompletedView = value;
                if (value)
                {
                    ShowConfigurationView = false;
                    ShowPlayerView = false;
                }
                //RaisePropertyChanged();
                QuizCompletedViewModel.RaisePropertyChanged();
                //SwitchToConfigurationViewCommand.RaiseCanExecuteChanged();
                //SwitchToPlayerViewCommand.RaiseCanExecuteChanged();
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
        //public DelegateCommand UpdateActivePackCommand { get; }

        //public DelegateCommand StartQuizCommand { get; }

        public DelegateCommand OpenActivePackConfigurationCommand { get; }
        public DelegateCommand OpenCreateNewQuestionPackCommand { get; }

        //public DelegateCommand<QuestionPackViewModel> SelectPackCommand { get; }

        //public DelegateCommand SaveJsonCommand { get; }
        public DelegateCommandAsync SaveJsonCommand { get; }

        public DelegateCommand RemoveActivePackCommand { get; }

        public MainWindowViewModel()
        {
            ConfigurationViewModel = new ConfigurationViewModel(this);

            Packs = new ObservableCollection<QuestionPackViewModel>();

            //LoadJsonCommand = new DelegateCommand(
            LoadJsonCommand = new DelegateCommandAsync(
                execute: LoadQuizesFromJsonAsync,
                canExecute: _ => true
            );

            LoadJsonCommand.Execute(null);

            if(ActivePack == null)
            {
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

                Packs.Add(ActivePack);
            }
            

            

            SelectedQuestion = ActivePack.Questions.FirstOrDefault();

            
            
            
            PlayerViewModel = new PlayerViewModel(this);

            QuizCompletedViewModel = new QuizCompletedViewModel(this);


            //SwitchToConfigurationViewCommand = new DelegateCommand(_ => CurrentView = ConfigurationViewModel, _ => CurrentView != ConfigurationViewModel);
            //SwitchToPlayerViewCommand = new DelegateCommand(_ => CurrentView = PlayerViewModel, _ => CurrentView != PlayerViewModel);
            SwitchToConfigurationViewCommand = new DelegateCommand(
                //_ => { ShowConfigurationView = true; ShowPlayerView = false; },
                _ => { ShowConfigurationView = true; },
                _ => !ShowConfigurationView
            );
            SwitchToPlayerViewCommand = new DelegateCommand(
                //_ => { ShowConfigurationView = false; ShowPlayerView = true; PlayerViewModel.StartQuizCommand.Execute(null); },
                _ => { ShowPlayerView = true; PlayerViewModel.StartQuizCommand.Execute(null); },
                _ => !ShowPlayerView && ActivePack.Questions.Count > 0
            );

            OpenActivePackConfigurationCommand = new DelegateCommand(
                execute: OpenActivePackConfiguration,
                canExecute: _ => HasActivePack
            );
            //UpdateActivePackCommand = new DelegateCommand(...)

            OpenCreateNewQuestionPackCommand = new DelegateCommand(
                execute: OpenCreateNewQuestionPack,
                canExecute: _ => true
            );

            SelectPackCommand = new DelegateCommand(
                execute: param =>
                {
                    if (param is QuestionPackViewModel pack)
                    {
                        ActivePack = pack;
                    }
                },
                canExecute: param => param is QuestionPackViewModel
            );

            //SaveJsonCommand = new DelegateCommand(
            SaveJsonCommand = new DelegateCommandAsync(
                execute: SaveAllQuestionPacksToJson, 
                canExecute: CanSaveAllQuestionPacksToJson
            );

            RemoveActivePackCommand = new DelegateCommand(
                execute: RemoveActivePack,
                canExecute: _ => ActivePack != null
            );

            ToggleFullScreenCommand = new DelegateCommand(
                execute: ToggleFullScreen,
                canExecute: _ => true
            );

            ExitCommand = new DelegateCommand(
                execute: Exit,
                canExecute: _ => true
            );

            //CurrentView = ConfigurationViewModel;
            ShowConfigurationView =  true;
            ShowPlayerView =  false;
            ShowQuizCompletedView =  false;
        }

        public DelegateCommand SelectPackCommand { get; }

        public void OpenActivePackConfiguration(object? arg)
        {
            var activePackConfiguration = new ConfigureActiveQuestionPackViewModel(this);
            var configureWindow = new ConfigureActiveQuestionPack(activePackConfiguration);
            //{
            //    DataContext = activePackConfiguration
            //};
            //activePackConfiguration.showDialog();
            configureWindow.ShowDialog();
        }

        public void OpenCreateNewQuestionPack(object? arg)
        {
            var newPackConfiguration = new CreateQuestionPackViewModel(this);
            var createQuestionPackWindow = new CreateQuestionPackView(newPackConfiguration);

            createQuestionPackWindow.ShowDialog();
        }


        private readonly JsonSerializerOptions _options = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        //public void SaveAllQuestionPacksToJson(object? arg)
        public async Task SaveAllQuestionPacksToJson(object? arg)
        {
            string json = JsonSerializer.Serialize(this.Packs, _options);

            //string projectFolder = AppDomain.CurrentDomain.BaseDirectory;
            //string jsonFolder = Path.Combine(projectFolder, "JsonSaves");
            string appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string jsonFolder = Path.Combine(appDataFolder, "Labb03_QuizApp");



            Directory.CreateDirectory(jsonFolder);

            string fileName = "UserQuizes.json";
            string filePath = Path.Combine(jsonFolder, fileName);

            try
            {
                //File.WriteAllText(filePath, json);
                await File.WriteAllTextAsync(filePath, json);

                MessageBox.Show(
                    $"Quizes saved successfully to:\n{filePath}",
                    "Save Successful",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );
            }
            catch(Exception ex)
            {
                MessageBox.Show(
                    $"Error saving quizes:\n{ex.Message}",
                    "Save Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }

        public bool CanSaveAllQuestionPacksToJson(object? arg)
        {
            return true;
        }


        //public DelegateCommand LoadJsonCommand { get; }
        public DelegateCommandAsync LoadJsonCommand { get; }


        //public async void LoadQuizesFromJsonAsync(object? arg)
        public async Task LoadQuizesFromJsonAsync(object? arg)
        {
            //string projectFolder = AppDomain.CurrentDomain.BaseDirectory;
            //string jsonFolder = Path.Combine(projectFolder, "JsonSaves");
            string appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string jsonFolder = Path.Combine(appDataFolder, "Labb03_QuizApp");

            string fileName = "UserQuizes.json";
            string filePath = Path.Combine(jsonFolder, fileName);

            try
            {
                if (!File.Exists(filePath))
                {
                    MessageBox.Show(
                        $"No saved quizes found at:\n{filePath}",
                        "Loading error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning
                    );
                    return;
                }

                //string json =     File.ReadAllText     (filePath);
                string json = await File.ReadAllTextAsync(filePath);
                var loadedPacks = JsonSerializer.Deserialize<ObservableCollection<QuestionPackViewModel>>(json, _options);

                if(loadedPacks != null && loadedPacks.Any())
                {
                    Packs.Clear();

                    foreach(var pack in loadedPacks)
                    {
                        Packs.Add(pack);
                    }

                    ActivePack = Packs.LastOrDefault();

                    MessageBox.Show(
                        $"Quizes loaded successfully from:\n{filePath}",
                        "Load Successful",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information
                    );
                }
                else
                {
                    MessageBox.Show(
                        $"No Quizes found in:\n{filePath}",
                        "Load empty",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning
                    );
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(
                    $"Error loading quizes:\n{ex.Message}",
                    "Load Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }

        public void RemoveActivePack(object? args)
        {
            if(ActivePack != null)
            {
                Packs.Remove(ActivePack);
                ActivePack = Packs.LastOrDefault();

                SwitchToPlayerViewCommand.RaiseCanExecuteChanged();
                OpenActivePackConfigurationCommand.RaiseCanExecuteChanged();
            }
        }

        public DelegateCommand ToggleFullScreenCommand { get; }

        public void ToggleFullScreen(object? arg)
        {
            var window = System.Windows.Application.Current.MainWindow;
            if(window != null)
            {
                if(window.WindowState != WindowState.Maximized)
                {
                    window.WindowState = WindowState.Maximized;
                    window.WindowStyle = WindowStyle.None;
                }
                else
                {
                    window.WindowState = WindowState.Normal;
                    window.WindowStyle = WindowStyle.SingleBorderWindow;
                }
            }
        }

        public DelegateCommand ExitCommand { get; }

        public void Exit(object? arg)
        {
            SaveJsonCommand.Execute(null);
            Application.Current.Shutdown();
        }
    }
}
