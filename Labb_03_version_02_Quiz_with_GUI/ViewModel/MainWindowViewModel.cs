using Labb_03_version_02_Quiz_with_GUI.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labb_03_version_02_Quiz_with_GUI.ViewModel
{
    internal class MainWindowViewModel : ViewModelBase
    {
        public ObservableCollection<QuestionPackViewModel> Packs { get; set; }

        public ConfigurationViewModel ConfigurationViewModel{ get; }

        public PlayerViewModel PlayerViewModel{ get; }

        // Lektion 112. Frågetecknet talar om för kompilatorn att vi är medvetna om att variabeln kan vara null.
        private QuestionPackViewModel? _activePack;

        public QuestionPackViewModel? ActivePack
        {
            get => _activePack;
            set
            {
                _activePack = value;
                // Lektion 112. Alla som gjort en binding kommer få meddelande när denna ändras.
                RaisePropertyChanged();

                // Lektion 114. 46 minuter in. Vi skriver nedanstående rad för att ConfigurationView.cs inte har en egen set-funktion för detta.
                // Vi behöver troligtvis göra liknande detta på flera ställen när vi gör Labb3!
                ConfigurationViewModel.RaisePropertyChanged("Active Pack");
            }
        }

        public MainWindowViewModel()
        {
            ConfigurationViewModel = new ConfigurationViewModel(this);
            PlayerViewModel = new PlayerViewModel(this);
            // Lektion 112. 26 minuter.
            ActivePack = new QuestionPackViewModel(new QuestionPack("My Question Pack"));
        }
    }
}
