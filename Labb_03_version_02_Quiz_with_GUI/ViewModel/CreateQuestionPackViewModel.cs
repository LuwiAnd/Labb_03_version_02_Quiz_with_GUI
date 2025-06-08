using Labb_03_version_02_Quiz_with_GUI.Command;
using Labb_03_version_02_Quiz_with_GUI.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Labb_03_version_02_Quiz_with_GUI.Enums;


namespace Labb_03_version_02_Quiz_with_GUI.ViewModel
{
    public class CreateQuestionPackViewModel : ViewModelBase
    {
        
        public CreateQuestionPackViewModel(MainWindowViewModel mainWindowViewModel)
        {
            this.mainWindowViewModel = mainWindowViewModel;
            CreateActivePackCommand = new DelegateCommand(CreateActivePack, CanCreateActivePack);
            
                this.PackName = "Name your quiz";
                this.Difficulty = Difficulty.Medium;
                this.TimeLimitInSeconds = 30;
            
            this.Difficulties = new List<Difficulty>();
            this.Difficulties.AddRange([Difficulty.Easy, Difficulty.Medium, Difficulty.Hard]);

            CloseActivePackConfiguration = new DelegateCommand(
                execute: (window) =>
                {
                    if (window is Window w)
                    {
                        w.Close();
                    }
                });

            SaveActivePackChangesCommand = new DelegateCommand(
                execute: (window) =>
                {
                    if (string.IsNullOrEmpty(this.PackName) || this.PackName.Length > 32 )
                    {
                        MessageBox.Show($"Quiz name cannot be more than 32 characters. \nYour name is {this.PackName.Length} characters.");
                        return;
                    }

                    //if (mainWindowViewModel.ActivePack is not null)
                    //{
                        
                        QuestionPack createdQuestionPack = new QuestionPack(
                            name: this.PackName,
                            difficulty: this.Difficulty,
                            timeLimitInSeconds: this.TimeLimitInSeconds
                        );

                        QuestionPackViewModel createdQuestionPackViewModel = new QuestionPackViewModel(createdQuestionPack);

                        mainWindowViewModel.Packs.Add(createdQuestionPackViewModel);
                        mainWindowViewModel.ActivePack = mainWindowViewModel.Packs.LastOrDefault();
                    //}

                    if (window is Window w)
                    {
                        w.Close();
                    }

                    mainWindowViewModel.SaveJsonCommand.Execute(null);
                }
            );

        }

        private bool CanCreateActivePack(object? arg)
        {
            return 
                !string.IsNullOrWhiteSpace(this.PackName)
                && this.PackName.Length <= 32
                && this.Difficulty != null 
                && this.TimeLimitInSeconds != null;
        }

        private void CreateActivePack(object obj)
        {
            QuestionPack questionPack = new QuestionPack(
                name: this.PackName,
                Difficulty = this.Difficulty,
                timeLimitInSeconds: this.TimeLimitInSeconds
                );
        }


        public bool QuizHasChanged = false;

        private MainWindowViewModel mainWindowViewModel;

        private string _packName;
        public string PackName
        {
            get => _packName;
            set
            {
                _packName = value;
                QuizHasChanged = true;

                RaisePropertyChanged();
                CreateActivePackCommand.RaiseCanExecuteChanged();
            }
        }

        private Difficulty _difficulty;

        public Difficulty Difficulty
        {
            get { return _difficulty; }
            set
            {
                _difficulty = value;
                QuizHasChanged = true;

                RaisePropertyChanged();
                CreateActivePackCommand.RaiseCanExecuteChanged();
            }
        }

        private int _timeLimitInSeconds;

        public int TimeLimitInSeconds
        {
            get { return _timeLimitInSeconds; }
            set
            {
                _timeLimitInSeconds = value;
                QuizHasChanged = true;

                RaisePropertyChanged();
                CreateActivePackCommand.RaiseCanExecuteChanged();
            }
        }

        public DelegateCommand CreateActivePackCommand;


        public List<Difficulty> Difficulties { get; }

        public DelegateCommand CloseActivePackConfiguration { get; }
        public DelegateCommand SaveActivePackChangesCommand { get; }
    }
}
