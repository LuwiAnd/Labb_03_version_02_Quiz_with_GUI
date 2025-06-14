﻿using Labb_03_version_02_Quiz_with_GUI.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Labb_03_version_02_Quiz_with_GUI.Enums;


namespace Labb_03_version_02_Quiz_with_GUI.ViewModel
{
    public class QuestionPackViewModel : ViewModelBase
    {
        private readonly QuestionPack model;

        public QuestionPackViewModel(QuestionPack model)
        {
            this.model = model;
            this.Questions = new ObservableCollection<Question>(model.Questions);
            this.SelectedQuestion = this.Questions.FirstOrDefault();
        }

        [JsonConstructor]
        public QuestionPackViewModel(
            string name, 
            Difficulty difficulty,
            int timeLimitInSeconds,
            ObservableCollection<Question> questions
        )
        {
            model = new QuestionPack();
            model.Name = name;
            model.Difficulty = difficulty;
            model.TimeLimitInSeconds = timeLimitInSeconds;
            Questions = questions;
        }

        public string Name {
            get => model.Name;
            set
            {
                model.Name = value;
                RaisePropertyChanged();
            }
        }
        public Difficulty Difficulty
        {
            get => model.Difficulty;
            set
            {
                model.Difficulty = value;
                RaisePropertyChanged();
            }
        }
        public int TimeLimitInSeconds
        {
            get => model.TimeLimitInSeconds;
            set
            {
                model.TimeLimitInSeconds = value;
                //RaisePropertyChanged("TimeLimitInSeconds");
                RaisePropertyChanged(); // Tack vare [CallerMemberName] i metoddefinitionen, så blir denna kodrad samma som ovanstående.
            }
        }
        public ObservableCollection<Question> Questions { get; set; }

        public Question? SelectedQuestion;
    }
}
