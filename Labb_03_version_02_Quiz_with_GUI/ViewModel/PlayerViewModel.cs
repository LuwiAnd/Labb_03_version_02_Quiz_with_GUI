using Labb_03_version_02_Quiz_with_GUI.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Labb_03_version_02_Quiz_with_GUI.ViewModel
{
    internal class PlayerViewModel : ViewModelBase
    {
        private readonly MainWindowViewModel? mainWindowViewModel;

        // Lektion 114.
        // I WPF finns det en UI-tråd och det är bara den som får uppdatera WPF-kontrollerna. 
        // Man får inte uppdatera dem från en Task med Task.Run(), då kraschar programmet.
        // Om man vill uppdatera något från utanför UI-tråden, så finns det en Dispatcher, som 
        // är en sorts kö där man kan lägga upp saker som man vill utföra. Dispatcher används 
        // inte bara för UI-tråden, utan generellt när man vill utföra något mellan trådar.
        // Se Dispatcher.Invoke().
        private DispatcherTimer timer;

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


        public DelegateCommand UpdateButtonCommand { get; }

        public PlayerViewModel(MainWindowViewModel? mainWindowViewModel)
        {
            this.mainWindowViewModel = mainWindowViewModel;

            TestData = "Start valuee: ";

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            // timer.Start();

            //UpdateButtonCommand = new DelegateCommand(UpdateButton);
            UpdateButtonCommand = new DelegateCommand(UpdateButton, CanUpdateButton);
            /* Man kan alltså skapa flera DelegateCommand. Senare kommer jag behöva skapa 
            AddQuestionCommand = new DelegateCommand(AddQuestion, CanAddQuestion);
            med tillhörande två funktioner AddQuestion och CanAddQuestion
            */
        }

        private bool CanUpdateButton(object? arg)
        {
            return TestData.Length < 20;
        }

        private void UpdateButton(object obj)
        {
            TestData += 'x';
            UpdateButtonCommand.RaiseCanExecuteChanged();
        }



        private void Timer_Tick(object? sender, EventArgs e)
        {
            TestData += 'x';
        }
    }
}
