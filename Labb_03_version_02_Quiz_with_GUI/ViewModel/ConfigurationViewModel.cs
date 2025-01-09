using Labb_03_version_02_Quiz_with_GUI.Command;
using Labb_03_version_02_Quiz_with_GUI.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labb_03_version_02_Quiz_with_GUI.ViewModel
{
    internal class ConfigurationViewModel : ViewModelBase
    {
        private readonly MainWindowViewModel? mainWindowViewModel;

        public DelegateCommand RemoveQuestionCommand { get; }
        public DelegateCommand AddQuestionCommand { get; }

        public ConfigurationViewModel(MainWindowViewModel? mainWindowViewModel)
        {
            this.mainWindowViewModel = mainWindowViewModel;
            //this.HasSelectedQuestion = mainWindowViewModel.HasSelectedQuestion;

            RemoveQuestionCommand = new DelegateCommand(RemoveQuestion, CanRemoveQuestion);
            AddQuestionCommand = new DelegateCommand(AddQuestion, CanAddQuestion);
        }

        public QuestionPackViewModel? ActivePack { get => mainWindowViewModel.ActivePack; }

        
        public Question? SelectedQuestion { 
            get => mainWindowViewModel.SelectedQuestion;
            set => mainWindowViewModel.SelectedQuestion = value;
        }

        public bool HasSelectedQuestion
        {
            get => mainWindowViewModel.HasSelectedQuestion;
            set => mainWindowViewModel.HasSelectedQuestion = value;
        }


        // Funktioner till DelegateCommand ---------------------------

        // DelegateCommand för att ta bort en fråga.
        private bool CanRemoveQuestion(object? arg)
        {
            return mainWindowViewModel.SelectedQuestion != null;
        }

        private void RemoveQuestion(object arg)
        {
            if (ActivePack != null && SelectedQuestion != null)
            {
                ActivePack.Questions.Remove(SelectedQuestion);
                //SelectedQuestion = null;
            }
        }

        // DelegateCommand för att lägga till en fråga.

        // Jag vet att jag troligtvis inte behöver lägga till en CanExecute-function
        // för att lägga till en fråga, men jag gör det ändå, ifall jag kommer på en
        // anledning senare.
        private bool CanAddQuestion(object? arg)
        {
            return true;
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
                        
                //SelectedQuestion = null;
            }
        }

    }
}
