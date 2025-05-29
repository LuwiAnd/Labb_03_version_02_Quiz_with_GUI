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
    public class ConfigureActiveQuestionPackViewModel : ViewModelBase
    {
        public ConfigureActiveQuestionPackViewModel(MainWindowViewModel mainWindowViewModel)
        {
            this.mainWindowViewModel = mainWindowViewModel;
            UpdateActivePackCommand = new DelegateCommand(UpdateActivePack, CanUpdateActivePack);
            if (mainWindowViewModel.ActivePack is not null)
            {
                this.PackName = mainWindowViewModel.ActivePack.Name ?? "Name your quiz";
                this.Difficulty = mainWindowViewModel.ActivePack.Difficulty;
                this.TimeLimitInSeconds = mainWindowViewModel.ActivePack.TimeLimitInSeconds;
            }
            else
            {
                this.PackName = "Name your quiz";
                this.Difficulty = Difficulty.Medium;
                this.TimeLimitInSeconds = 30;
            }
            this.Difficulties = new List<Difficulty>();
            this.Difficulties.AddRange([Difficulty.Easy, Difficulty.Medium, Difficulty.Hard]);

            CloseActivePackConfiguration = new DelegateCommand(
                execute: (window) =>
                {
                    if(window is Window w)
                    {
                        w.Close();
                    }
                });

            SaveActivePackChangesCommand = new DelegateCommand(
                execute: (window) =>
                {
                    if (mainWindowViewModel.ActivePack is not null)
                    {
                        mainWindowViewModel.ActivePack.Name = this.PackName;
                        mainWindowViewModel.ActivePack.Difficulty = this.Difficulty;
                        mainWindowViewModel.ActivePack.TimeLimitInSeconds = this.TimeLimitInSeconds;
                    }

                    if (window is Window w)
                    {
                        w.Close();
                    }

                    mainWindowViewModel.SaveJsonCommand.Execute(null);
                }
            );

        }

        private bool CanUpdateActivePack(object? arg)
        {
            return QuizHasChanged;
        }

        private void UpdateActivePack(object obj)
        {
            mainWindowViewModel.ActivePack.Name = this.PackName;
            mainWindowViewModel.ActivePack.Difficulty = this.Difficulty;
            mainWindowViewModel.ActivePack.TimeLimitInSeconds = this.TimeLimitInSeconds;

            mainWindowViewModel.SaveJsonCommand.Execute(null);
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
                UpdateActivePackCommand.RaiseCanExecuteChanged();
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
                UpdateActivePackCommand.RaiseCanExecuteChanged();
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
                UpdateActivePackCommand.RaiseCanExecuteChanged();
            }
        }

        public DelegateCommand UpdateActivePackCommand;


        public List<Difficulty> Difficulties { get; }

        public DelegateCommand CloseActivePackConfiguration { get; }
        public DelegateCommand SaveActivePackChangesCommand { get; }



    }
}
