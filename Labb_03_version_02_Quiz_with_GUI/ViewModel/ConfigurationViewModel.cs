﻿using Labb_03_version_02_Quiz_with_GUI.Command;
using Labb_03_version_02_Quiz_with_GUI.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labb_03_version_02_Quiz_with_GUI.ViewModel
{
    public class ConfigurationViewModel : ViewModelBase
    {

        // Ändrade från private till public för att komma åt från code-behind i ConfigurationView.xaml.cs.
        //public readonly MainWindowViewModel? mainWindowViewModel;
        public readonly MainWindowViewModel? mainWindowViewModel;
        public QuestionPackViewModel? ActivePack { get => mainWindowViewModel?.ActivePack; }

        public Question? SelectedQuestion
        {
            get => mainWindowViewModel?.SelectedQuestion;
            
            set
            {
                mainWindowViewModel.SelectedQuestion = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(HasSelectedQuestion));
            }
        }

        public DelegateCommand OpenActivePackConfigurationCommand { get => mainWindowViewModel!.OpenActivePackConfigurationCommand; }
        public DelegateCommandAsync SaveJsonCommand { get => mainWindowViewModel!.SaveJsonCommand; }

        private ObservableCollection<Question>? _selectedQuestions = new ObservableCollection<Question>();
        public ObservableCollection<Question>? SelectedQuestions
        {
            get => _selectedQuestions;
            set
            {
                _selectedQuestions = value;
                RaisePropertyChanged();
                //RaisePropertyChanged(nameof(HasSelectedQuestion));
                RaisePropertyChanged("HasSelectedQuestion");
                RemoveQuestionCommand.RaiseCanExecuteChanged();
            }
        }

        public bool? HasActivePack
        {
            get => mainWindowViewModel?.HasActivePack;
        }


        public bool HasSelectedQuestion
        {
            get => mainWindowViewModel.HasSelectedQuestion;
            set => mainWindowViewModel.HasSelectedQuestion = value;
        }
        
        //public bool HasSelectedQuestions => SelectedQuestions != null && SelectedQuestions.Any();

        public bool ShowConfigurationView
        {
            get => mainWindowViewModel.ShowConfigurationView;
        }



        public DelegateCommand RemoveQuestionCommand { get; }
        public DelegateCommand AddQuestionCommand { get; }

        public ConfigurationViewModel(MainWindowViewModel? mainWindowViewModel)
        {
            this.mainWindowViewModel = mainWindowViewModel;
            //this.HasSelectedQuestion = mainWindowViewModel.HasSelectedQuestion;

            RemoveQuestionCommand = new DelegateCommand(RemoveQuestion, CanRemoveQuestion);
            //RemoveQuestionCommand = new DelegateCommand(RemoveQuestions, CanRemoveQuestions);
            AddQuestionCommand = new DelegateCommand(AddQuestion, CanAddQuestion);
        }


        
        


        // Funktioner till DelegateCommand ---------------------------

        // DelegateCommand för att ta bort en fråga.
        private bool CanRemoveQuestion(object? arg)
        {
            //return mainWindowViewModel.SelectedQuestion != null;
            return mainWindowViewModel.SelectedQuestion != null && mainWindowViewModel.ShowConfigurationView;
        }
        /*
        private bool CanRemoveQuestions(object? arg)
        {
            return HasSelectedQuestions;
        }
        */

        private void RemoveQuestion(object arg)
        {
            if (ActivePack != null && SelectedQuestion != null)
            {
                ActivePack.Questions.Remove(SelectedQuestion);

                mainWindowViewModel.PlayerViewModel.StartQuizCommand.RaiseCanExecuteChanged();
                mainWindowViewModel.SwitchToPlayerViewCommand.RaiseCanExecuteChanged();

                mainWindowViewModel.SaveJsonCommand.Execute(null);
            }
        }
        /*
        private void RemoveQuestions(object arg)
        {
            if (ActivePack != null && SelectedQuestions != null)
            {
                foreach(var question in SelectedQuestions.ToList())
                {
                    ActivePack.Questions.Remove(question);
                }

                SelectedQuestions.Clear();

                mainWindowViewModel?.PlayerViewModel.StartQuizCommand.RaiseCanExecuteChanged();
                mainWindowViewModel?.SwitchToPlayerViewCommand.RaiseCanExecuteChanged();
            }
        }
        */

        // DelegateCommand för att lägga till en fråga.

        // Jag vet att jag troligtvis inte behöver lägga till en CanExecute-function
        // för att lägga till en fråga, men jag gör det ändå, ifall jag kommer på en
        // anledning senare.
        private bool CanAddQuestion(object? arg)
        {
            //return true;
            return mainWindowViewModel.ShowConfigurationView && mainWindowViewModel.ActivePack != null;
        }

        private void AddQuestion(object arg)
        {
            if (ActivePack != null)
            {
                ActivePack.Questions.Add(
                    new Question(
                        query: "New question",
                        correctAnswer: "Type the correct answer here",
                        incorrectAnswer1: "Type an incorrect answer here",
                        incorrectAnswer2: "Type a second incorrect answer here",
                        incorrectAnswer3: "Type a third incorrect answer here"
                    )
                );

                mainWindowViewModel?.PlayerViewModel.StartQuizCommand.RaiseCanExecuteChanged();
                mainWindowViewModel?.SwitchToPlayerViewCommand.RaiseCanExecuteChanged();

                mainWindowViewModel?.SaveJsonCommand.Execute(null);
            }
        }

    }
}
