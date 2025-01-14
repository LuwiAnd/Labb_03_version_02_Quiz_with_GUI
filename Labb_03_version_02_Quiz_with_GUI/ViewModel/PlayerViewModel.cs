using Labb_03_version_02_Quiz_with_GUI.Command;
using Labb_03_version_02_Quiz_with_GUI.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;
using System.Windows.Threading;

namespace Labb_03_version_02_Quiz_with_GUI.ViewModel
{
    internal class PlayerViewModel : ViewModelBase
    {

        public PlayerViewModel(MainWindowViewModel? mainWindowViewModel)
        {
            this.mainWindowViewModel = mainWindowViewModel;
            CurrentQuestionIndex = 0;
            SecondsRemainingToAnswer = mainWindowViewModel.ActivePack.TimeLimitInSeconds;
            StartQuizCommand = new DelegateCommand(execute: StartQuiz, canExecute: CanStartQuiz);

            TestData = "Start valuee: ";

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            //timer.Start();

            //UpdateButtonCommand = new DelegateCommand(UpdateButton);
            UpdateButtonCommand = new DelegateCommand(UpdateButton, CanUpdateButton);
            
        }


        private readonly MainWindowViewModel? mainWindowViewModel;

        public DelegateCommand StartQuizCommand { get; }
        public QuestionPackViewModel? ActivePack { get => mainWindowViewModel.ActivePack; }

        
        public int CurrentScore { get; set; }

        private int _secondsRemainingToDisplayCorrectAnswer;
        public int SecondsRemainingToDisplayCorrectAnswer {
            get => _secondsRemainingToDisplayCorrectAnswer;
            set
            {
                _secondsRemainingToDisplayCorrectAnswer = value;
                RaisePropertyChanged();
            }
        }

        private int _secondsRemainingToAnswer;
        public int SecondsRemainingToAnswer {
            get => _secondsRemainingToAnswer;
            set
            {
                _secondsRemainingToAnswer = value;
                RaisePropertyChanged();
            }
        }


        private int _currentQuestionIndex = 0;
        public int CurrentQuestionIndex
        {
            get => _currentQuestionIndex;
            set
            {
                _currentQuestionIndex = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(CurrentQuestion));
            }
        }

        public Question? CurrentQuestion {
            get => ActivePack.Questions[CurrentQuestionIndex];
        }

        //public string ProgressionString { get => $"Question  {CurrentQuestionIndex + 1}  of  {ActivePack?.Questions.Count}"; }
        //public string ProgressionString { get; set; }
        private string _progressionString = "Hejsan";
        public string ProgressionString
        {
            get => _progressionString;
            set
            {
                _progressionString = value;
                RaisePropertyChanged();
            }
        }



        // Lektion 114.
        // I WPF finns det en UI-tråd och det är bara den som får uppdatera WPF-kontrollerna. 
        // Man får inte uppdatera dem från en Task med Task.Run(), då kraschar programmet.
        // Om man vill uppdatera något från utanför UI-tråden, så finns det en Dispatcher, som 
        // är en sorts kö där man kan lägga upp saker som man vill utföra. Dispatcher används 
        // inte bara för UI-tråden, utan generellt när man vill utföra något mellan trådar.
        // Se Dispatcher.Invoke().
        public DispatcherTimer timer;

        //public string TestData { get => "This is test data."; private set; }
        //public string TestData { get; private set; } = "Start value.";
        
        private string _testData = "Start value.";
        public string TestData
        {
            get => _testData;
            private set
            {
                _testData = value;
                RaisePropertyChanged();
            }
        }

        public bool ShowPlayerView
        {
            get => mainWindowViewModel.ShowPlayerView;
        }


        public DelegateCommand UpdateButtonCommand { get; }

        

        private bool CanUpdateButton(object? arg)
        {
            return TestData.Length < 20;
        }

        private void UpdateButton(object obj)
        {
            TestData += 'x';
            UpdateButtonCommand.RaiseCanExecuteChanged();
        }


        private bool CanStartQuiz(object? arg)
        {
            return ActivePack?.Questions.Count > 0;
        }

        private void StartQuiz(object obj)
        {
            CurrentQuestionIndex = 0;
            CurrentScore = 0;

            ProgressionString = $"Question  {CurrentQuestionIndex + 1}  of  {mainWindowViewModel.ActivePack?.Questions.Count}\n Points: {CurrentScore} of {CurrentQuestionIndex}";

            StartCurrentQuestion();
        }

        private void StartCurrentQuestion()
        {
            //SecondsRemainingToDisplayCorrectAnswer = mainWindowViewModel.ActivePack.TimeLimitInSeconds;
            SecondsRemainingToAnswer = mainWindowViewModel.ActivePack.TimeLimitInSeconds;
        }



        private void Timer_Tick(object? sender, EventArgs e)
        {
            //TestData += 'x';
            if(SecondsRemainingToAnswer > 0)
            {
                SecondsRemainingToAnswer--;
            }
            else if(CurrentQuestionIndex < (ActivePack.Questions.Count - 1))
            {
                CurrentQuestionIndex++;
                StartCurrentQuestion();
            }
            else
            {

            }
        }
    }
}
