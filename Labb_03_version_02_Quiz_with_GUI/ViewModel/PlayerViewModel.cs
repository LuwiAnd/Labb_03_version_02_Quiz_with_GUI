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
using System.Security.AccessControl;

namespace Labb_03_version_02_Quiz_with_GUI.ViewModel
{
    public enum QuizState { Asking, ShowingCorrectAnswer };
    public class PlayerViewModel : ViewModelBase
    {

        public PlayerViewModel(MainWindowViewModel? mainWindowViewModel)
        {
            this.mainWindowViewModel = mainWindowViewModel;
            //this.quizCompletedViewModel = new QuizCompletedViewModel(this);

            //CurrentQuestionIndex = 0;
            SecondsRemainingToAnswer = mainWindowViewModel.ActivePack.TimeLimitInSeconds;
            StartQuizCommand = new DelegateCommand(execute: StartQuiz, canExecute: CanStartQuiz);

            //TestData = "Start valuee: ";

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            //timer.Start();

            //UpdateButtonCommand = new DelegateCommand(UpdateButton);
            UpdateButtonCommand = new DelegateCommand(UpdateButton, CanUpdateButton);


            AnswerCommand = new DelegateCommand(OnScoringQuestion);
            QuizState QuizState = QuizState.Asking;
            AnswerOptions = new ObservableCollection<AnswerOption> { 
                new AnswerOption("", false), 
                new AnswerOption("", false), 
                new AnswerOption("", false), 
                new AnswerOption("", false)
            };

            AnswerOption1 = new AnswerOption("", false);
            AnswerOption2 = new AnswerOption("", false);
            AnswerOption3 = new AnswerOption("", false);
            AnswerOption4 = new AnswerOption("", false);

            if(mainWindowViewModel?.ActivePack?.Questions.Any() == true)
            {
                LoadAnswerOptions();
            }
        }

        private AnswerOption _answerOption1;
        private AnswerOption _answerOption2;
        private AnswerOption _answerOption3;
        private AnswerOption _answerOption4;
        public AnswerOption AnswerOption1
        {
            get => _answerOption1;
            set { _answerOption1 = value; RaisePropertyChanged(); }
        }

        public AnswerOption AnswerOption2
        {
            get => _answerOption2;
            set { _answerOption2 = value; RaisePropertyChanged(); }
        }

        public AnswerOption AnswerOption3
        {
            get => _answerOption3;
            set { _answerOption3 = value; RaisePropertyChanged(); }
        }

        public AnswerOption AnswerOption4
        {
            get => _answerOption4;
            set { _answerOption4 = value; RaisePropertyChanged(); }
        }




        private readonly MainWindowViewModel? mainWindowViewModel;
        private QuizCompletedViewModel quizCompletedViewModel;
        
        public QuizState QuizState { get; set; }

        public DelegateCommand StartQuizCommand { get; }
        public QuestionPackViewModel? ActivePack { get => mainWindowViewModel.ActivePack; }


        private int _currentScore;
        public int CurrentScore {
            get => _currentScore;
            set
            {
                _currentScore = value;
                //RaisePropertyChanged();
                UpdateProgressionString();
            }
        }

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

                LoadAnswerOptions();
                UpdateProgressionString();
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(CurrentQuestion));
            }
        }

        public Question? CurrentQuestion {
            //get => ActivePack.Questions[CurrentQuestionIndex];
            get => ActivePack.Questions != null && ActivePack.Questions.Count > CurrentQuestionIndex ? ActivePack.Questions[CurrentQuestionIndex] : null;
        }

        //public string ProgressionString { get => $"Question  {CurrentQuestionIndex + 1}  of  {ActivePack?.Questions.Count}"; }
        //public string ProgressionString { get; set; }
        private string _progressionString = "Hejsan";
        //private string _progressionString = UpdateProgressionString();
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

        private void UpdateProgressionString()
        {
            string questionNumberString;
            string questionScoreString;

            int questionNumber = CurrentQuestionIndex + 1;
            int? totalQuestionNumber = mainWindowViewModel?.ActivePack?.Questions.Count;

            int? questionsAnswered;

            if (mainWindowViewModel.ShowQuizCompletedView)
            {
                questionsAnswered = mainWindowViewModel?.ActivePack?.Questions.Count;
            }
            else if (QuizState == QuizState.Asking)
            {
                questionsAnswered = CurrentQuestionIndex;
            }
            else
            {
                questionsAnswered = CurrentQuestionIndex + 1;
            }

            questionNumberString = $"Question  {questionNumber}  of  {totalQuestionNumber}\n ";
            questionScoreString = $"Points: {CurrentScore} of {questionsAnswered}";

            
            ProgressionString = questionNumberString + questionScoreString;
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

            ResetButtonColors();

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
            
            //if (mainWindowViewModel?.ShowQuizCompletedView ?? false) { timer.Stop(); }
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
                    OnScoringQuestion(null);
                    UpdateProgressionString();
                    TimeLeftToDisplayCorrectAnswerInSeconds = TotalTimeToDisplayCorrectAnswerInSeconds;
                }
                else
                {
                    //mainWindowViewModel.ShowQuizCompletedView = true;
                    //UpdateProgressionString();
                    QuizState = QuizState.ShowingCorrectAnswer;
                    OnScoringQuestion(null);
                }
            }
            else
            {
                if(TimeLeftToDisplayCorrectAnswerInSeconds <= 0)
                {
                    QuizState = QuizState.Asking;

                    if(CurrentQuestionIndex < (ActivePack.Questions.Count - 1))
                    {
                        CurrentQuestionIndex++;
                        UpdateProgressionString();
                        StartCurrentQuestion();
                    }
                    else
                    {

                        mainWindowViewModel.ShowQuizCompletedView = true;
                        UpdateProgressionString();
                    }
                }
                TimeLeftToDisplayCorrectAnswerInSeconds--;
            }
        }








        // Code for handling button clicks.
        public ObservableCollection<AnswerOption> AnswerOptions { get; }
        private readonly Random random = new Random();

        private const int TotalTimeToDisplayCorrectAnswerInSeconds = 1;
        private int TimeLeftToDisplayCorrectAnswerInSeconds = TotalTimeToDisplayCorrectAnswerInSeconds;
        private string _selectedAnswer;
        private int _correctAnswerIndex;
        
        public DelegateCommand AnswerCommand { get; }



        private void LoadAnswerOptions()
        {
            //AnswerOptions.Clear();

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

            //AnswerOptions = new ObservableCollection<AnswerOption>(shuffledOptions);
            for (int i = 0; i < AnswerOptions.Count; i++)
            {
                AnswerOptions[i].AnswerText = shuffledOptions[i].AnswerText;
                AnswerOptions[i].IsCorrect = shuffledOptions[i].IsCorrect;
                AnswerOptions[i].OptionIndex = shuffledOptions[i].OptionIndex;
                AnswerOptions[i].BackgroundColor = shuffledOptions[i].BackgroundColor;
            }

            //AnswerOption1.AnswerText      = shuffledOptions[0].AnswerText;
            //AnswerOption1.IsCorrect       = shuffledOptions[0].IsCorrect;
            //AnswerOption1.OptionIndex     = shuffledOptions[0].OptionIndex;
            //AnswerOption1.BackgroundColor = shuffledOptions[0].BackgroundColor;

            //AnswerOption2.AnswerText      = shuffledOptions[1].AnswerText;
            //AnswerOption2.IsCorrect       = shuffledOptions[1].IsCorrect;
            //AnswerOption2.OptionIndex     = shuffledOptions[1].OptionIndex;
            //AnswerOption2.BackgroundColor = shuffledOptions[1].BackgroundColor;

            //AnswerOption3.AnswerText      = shuffledOptions[2].AnswerText;
            //AnswerOption3.IsCorrect       = shuffledOptions[2].IsCorrect;
            //AnswerOption3.OptionIndex     = shuffledOptions[2].OptionIndex;
            //AnswerOption3.BackgroundColor = shuffledOptions[2].BackgroundColor;

            //AnswerOption4.AnswerText      = shuffledOptions[3].AnswerText;
            //AnswerOption4.IsCorrect       = shuffledOptions[3].IsCorrect;
            //AnswerOption4.OptionIndex     = shuffledOptions[3].OptionIndex;
            //AnswerOption4.BackgroundColor = shuffledOptions[3].BackgroundColor;



            RaisePropertyChanged("AnswerOptions");
        }


        private void OnScoringQuestion(object? parameter)
        {
            var selectedOption = parameter as AnswerOption;

            if (selectedOption?.IsCorrect ?? false) { 
                CurrentScore++;
            }

            DisplayCorrectAnswerButton();

            if(!selectedOption?.IsCorrect ?? false && selectedOption != null)
            {
                AnswerOptions[selectedOption.OptionIndex].BackgroundColor = Brushes.Red;
            }

            TimeLeftToDisplayCorrectAnswerInSeconds = TotalTimeToDisplayCorrectAnswerInSeconds;
            QuizState = QuizState.ShowingCorrectAnswer;
            UpdateProgressionString();
        }

        private void DisplayCorrectAnswerButton()
        {
            var correctAnswer = AnswerOptions.FirstOrDefault(option => option.IsCorrect);
            if(correctAnswer != null)
            {
                correctAnswer.BackgroundColor = Brushes.Green;
            }
            RaisePropertyChanged("AnswerOptions");
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




        


    }
}
