using Labb_03_version_02_Quiz_with_GUI.Command;
using Labb_03_version_02_Quiz_with_GUI.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

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
                    if (mainWindowViewModel.ActivePack is not null)
                    {
                        //mainWindowViewModel.ActivePack.Name = this.PackName;
                        //mainWindowViewModel.ActivePack.Difficulty = this.Difficulty;
                        //mainWindowViewModel.ActivePack.TimeLimitInSeconds = this.TimeLimitInSeconds;

                        QuestionPack createdQuestionPack = new QuestionPack(
                            name: this.PackName,
                            difficulty: this.Difficulty,
                            timeLimitInSeconds: this.TimeLimitInSeconds
                        );

                        QuestionPackViewModel createdQuestionPackViewModel = new QuestionPackViewModel(createdQuestionPack);

                        mainWindowViewModel.Packs.Add(createdQuestionPackViewModel);
                        mainWindowViewModel.ActivePack = mainWindowViewModel.Packs.LastOrDefault();
                    }

                    if (window is Window w)
                    {
                        w.Close();
                    }
                }
            );

        }

        private bool CanCreateActivePack(object? arg)
        {
            return this.PackName != null && this.Difficulty != null && this.TimeLimitInSeconds != null;
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
