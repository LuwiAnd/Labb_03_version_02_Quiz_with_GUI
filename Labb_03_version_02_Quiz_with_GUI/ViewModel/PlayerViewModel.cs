using Labb_03_version_02_Quiz_with_GUI.Command;
using Labb_03_version_02_Quiz_with_GUI.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
//using System.Drawing;
using System.Windows.Media;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;
using System.Windows.Threading;

namespace Labb_03_version_02_Quiz_with_GUI.ViewModel
{
    enum QuizState { Asking, ShowingCorrectAnswer };
    internal class PlayerViewModel : ViewModelBase
    {

        public PlayerViewModel(MainWindowViewModel? mainWindowViewModel)
        {
            this.mainWindowViewModel = mainWindowViewModel;
            //this.quizCompletedViewModel = new QuizCompletedViewModel(this);

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


            AnswerCommand = new DelegateCommand(OnScoringQuestion);
            QuizState QuizState = QuizState.Asking;
            AnswerOptions = new ObservableCollection<AnswerOption>();
            LoadAnswerOptions();
        }

        private readonly MainWindowViewModel? mainWindowViewModel;
        private QuizCompletedViewModel quizCompletedViewModel;
        
        public QuizState QuizState { get; set; }

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
                mainWindowViewModel.QuizCompletedViewModel.RaisePropertyChanged();
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

        public bool ShowQuizCompletedView
        {
            get => mainWindowViewModel.ShowQuizCompletedView;
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
            if (!mainWindowViewModel.ShowPlayerView) { mainWindowViewModel.ShowPlayerView = true; }
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
            if(QuizState == QuizState.Asking)
            {

                if(SecondsRemainingToAnswer > 0)
                {
                    SecondsRemainingToAnswer--;
                }
                else if(CurrentQuestionIndex < (ActivePack.Questions.Count - 1))
                {
                    //CurrentQuestionIndex++;
                    //StartCurrentQuestion();
                    QuizState = QuizState.ShowingCorrectAnswer;
                    TimeToDisplayCorrectAnswerInSeconds = 3;
                }
                else
                {
                    mainWindowViewModel.ShowQuizCompletedView = true;
                }
            }
            else
            {
                if(TimeToDisplayCorrectAnswerInSeconds <= 0)
                {
                    QuizState = QuizState.Asking;

                    if(CurrentQuestionIndex < (ActivePack.Questions.Count - 1))
                    {
                        CurrentQuestionIndex++;
                        StartCurrentQuestion();
                    }
                    else
                    {
                        mainWindowViewModel.ShowQuizCompletedView = true;
                    }
                }
                TimeToDisplayCorrectAnswerInSeconds--;
            }
        }








        // Code for handling button clicks.
        public ObservableCollection<AnswerOption> AnswerOptions { get; set; }
        private readonly Random random = new Random();

        private int TimeToDisplayCorrectAnswerInSeconds = 3;
        private string _selectedAnswer;
        private int _correctAnswerIndex;
        
        public DelegateCommand AnswerCommand { get; }



        private void LoadAnswerOptions()
        {
            AnswerOptions.Clear();

            List<AnswerOption> options = new List<AnswerOption>
            {
                new AnswerOption(CurrentQuestion.CorrectAnswer, true)
            };

            options.AddRange(CurrentQuestion.IncorrectAnswers.Select(answer => new AnswerOption(answer, false)));

            
            List<AnswerOption> shuffledOptions = options.OrderBy(_ => random.Next()).ToList();

            for( int i = 0; i < shuffledOptions.Count; i++)
            {
                shuffledOptions[i].OptionIndex = i;
            }

            AnswerOptions = new ObservableCollection<AnswerOption>(shuffledOptions);
            
        }


        private void OnScoringQuestion(object? parameter)
        {
            var selectedOption = parameter as AnswerOption;

            DisplayCorrectAnswerButton();

            if(!selectedOption.IsCorrect)
            {
                AnswerOptions[selectedOption.OptionIndex].BackgroundColor = Brushes.Red;
            }

            TimeToDisplayCorrectAnswerInSeconds = 3;
            QuizState = QuizState.ShowingCorrectAnswer;
        }

        private void DisplayCorrectAnswerButton()
        {
            var correctAnswer = AnswerOptions.FirstOrDefault(option => option.IsCorrect);
            if(correctAnswer != null)
            {
                correctAnswer.BackgroundColor = Brushes.Green;
            }
        }



        private void UpdateAnswerButtonColors(bool isCorrect, string selectedAnswer)
        {
            if (isCorrect)
            {
                if(AnswerOption1 == selectedAnswer) { Button1Color = Brushes.Green; }
                if(AnswerOption2 == selectedAnswer) { Button2Color = Brushes.Green; }
                if(AnswerOption3 == selectedAnswer) { Button3Color = Brushes.Green; }
                if(AnswerOption4 == selectedAnswer) { Button4Color = Brushes.Green; }
            }
            else
            {
                if (AnswerOption1 == selectedAnswer) { Button1Color = Brushes.Red; }
                if (AnswerOption2 == selectedAnswer) { Button2Color = Brushes.Red; }
                if (AnswerOption3 == selectedAnswer) { Button3Color = Brushes.Red; }
                if (AnswerOption4 == selectedAnswer) { Button4Color = Brushes.Red; }

                if(AnswerOption1 == CurrentQuestion?.CorrectAnswer) { Button1Color = Brushes.Green; }
                if(AnswerOption2 == CurrentQuestion?.CorrectAnswer) { Button2Color = Brushes.Green; }
                if(AnswerOption3 == CurrentQuestion?.CorrectAnswer) { Button3Color = Brushes.Green; }
                if(AnswerOption4 == CurrentQuestion?.CorrectAnswer) { Button4Color = Brushes.Green; }
            }
        }

        private void ResetButtonColors()
        {
            Button1Color = Brushes.LightGray;
            Button2Color = Brushes.LightGray;
            Button3Color = Brushes.LightGray;
            Button4Color = Brushes.LightGray;
        }


        private Brush _button1Color = Brushes.LightGray;
        public Brush Button1Color
        {
            get => _button1Color;
            set { _button1Color = value; RaisePropertyChanged(); }
        }

        private Brush _button2Color = Brushes.LightGray;
        public Brush Button2Color
        {
            get => _button2Color;
            set { _button2Color = value; RaisePropertyChanged(); }
        }

        private Brush _button3Color = Brushes.LightGray;
        public Brush Button3Color
        {
            get => _button3Color;
            set { _button3Color = value; RaisePropertyChanged(); }
        }

        private Brush _button4Color = Brushes.LightGray;
        public Brush Button4Color
        {
            get => _button4Color;
            set { _button4Color = value; RaisePropertyChanged(); }
        }




        private string _answerOption1;
        public string AnswerOption1
        {
            get => _answerOption1;
            set { _answerOption1 = value; RaisePropertyChanged(); }
        }

        private string _answerOption2;
        public string AnswerOption2
        {
            get => _answerOption2;
            set { _answerOption2 = value; RaisePropertyChanged(); }
        }

        private string _answerOption3;
        public string AnswerOption3
        {
            get => _answerOption3;
            set { _answerOption3 = value; RaisePropertyChanged(); }
        }

        private string _answerOption4;
        public string AnswerOption4
        {
            get => _answerOption4;
            set { _answerOption4 = value; RaisePropertyChanged(); }
        }


    }
}
