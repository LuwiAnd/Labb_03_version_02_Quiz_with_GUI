using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labb_03_version_02_Quiz_with_GUI.ViewModel
{
    public class AnswerOption : ViewModelBase
    {
        private string _answerText;
        public string AnswerText { 
            get => _answerText;
            set
            {
                _answerText = value;
                RaisePropertyChanged();
            }
        }

        private bool _isCorrect;
        public bool IsCorrect
        {
            get => _isCorrect;
            set
            {
                _isCorrect = value;
                RaisePropertyChanged();
            }
        }

        private Brush _backgroundColor;
        public Brush BackgroundColor {
            get => _backgroundColor;
            set
            {
                _backgroundColor = value;
                RaisePropertyChanged();
            }
        }

        private int _optionIndex;
        public int OptionIndex {
            get => _optionIndex;
            set
            {
                _optionIndex = value;
                RaisePropertyChanged();
            }
        }

        public AnswerOption(string answerText, bool isCorrect)
        {
            this.AnswerText = answerText;
            this.IsCorrect = isCorrect;
            this.BackgroundColor = Brushes.LightGray;
        }
    }
}
