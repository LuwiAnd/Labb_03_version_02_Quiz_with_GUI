using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

using System.ComponentModel;

namespace Labb_03_version_02_Quiz_with_GUI.Dto
{
    public class QuestionCategoryDto : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public void RaisePropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }


        

        private Func<int, int>? _countFromViewModel;
        [JsonIgnore]
        public Func<int, int>? CountFromViewModel {
            get => _countFromViewModel;
            set
            {
                _countFromViewModel = value;
                RaisePropertyChanged(nameof(DisplayNameWithCount));
            }
        }

        [JsonPropertyName("id")]
        public int Id { get; set; }

        private string? _name;
        [JsonPropertyName("name")]
        public string? Name {
            get => _name;
            set
            {
                _name = value;
                RaisePropertyChanged(nameof(Name));
                RaisePropertyChanged(nameof(DisplayNameWithCount));
            }
        }

        public string DisplayNameWithCount
        {
            get
            {
                var count = CountFromViewModel?.Invoke(this.Id) ?? 0;
                return $"{Name} ({count} frågor)";
            }
        }
    }
}
