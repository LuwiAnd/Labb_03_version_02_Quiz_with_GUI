using Labb_03_version_02_Quiz_with_GUI.Command;
using Labb_03_version_02_Quiz_with_GUI.Dto;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Labb_03_version_02_Quiz_with_GUI.Enums;
using Labb_03_version_02_Quiz_with_GUI.Services;

namespace Labb_03_version_02_Quiz_with_GUI.ViewModel
{
    public class ImporterTriviaDbViewModel : ViewModelBase
    {
        private MainWindowViewModel mainWindowViewModel;
        //private List<string> _categories;
        //public List<string> Categories { 
        //    get => _categories;
        //    set
        //    {
        //        _categories = value;
        //        RaisePropertyChanged();
        //    }
        //}

        private readonly HttpClient _httpClient = new HttpClient();


        private string? _quizName = "Name your imported quiz";
        public string? QuizName
        {
            get => _quizName;
            set
            {
                _quizName = value;
                RaisePropertyChanged();
            }
        }

        private int? _timeLimitInSeconds = 30;
        public int? TimeLimitInSeconds
        {
            get => _timeLimitInSeconds;
            set
            {
                _timeLimitInSeconds = value;
                RaisePropertyChanged();
            }
        }




        private ObservableCollection<QuestionCategoryDto>? _categories;
        public ObservableCollection<QuestionCategoryDto>? Categories
        {
            get => _categories;
            set
            {
                _categories = value;
                RaisePropertyChanged();
            }
        }

        private QuestionCategoryDto? _selectedCategory;
        public QuestionCategoryDto? SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                _selectedCategory = value;
                RaisePropertyChanged();
            }
        }

        public DelegateCommandAsync LoadCategoriesCommand { get; }
        public DelegateCommandAsync ImportQuestionsCommand { get; }

        public List<Difficulty> Difficulties { get; } = Enum.GetValues(typeof(Difficulty)).Cast<Difficulty>().ToList();
        private Difficulty _selectedDifficulty = Difficulty.Medium;
        public Difficulty SelectedDifficulty
        {
            get => _selectedDifficulty;
            set
            {
                _selectedDifficulty = value;
                RaisePropertyChanged();

                foreach(var cat in Categories ?? new())
                {
                    //RaisePropertyChanged(nameof(cat.DisplayNameWithCount));
                    cat.CountFromViewModel = GetCountForCategory;
                }
            }
        }

        public ObservableCollection<TriviaCategoryDifficultyCount> CategoryCounts { get; } = new();

        public ImporterTriviaDbViewModel(MainWindowViewModel mainWindowViewModel)
        {
            this.mainWindowViewModel = mainWindowViewModel;
            Categories = new ObservableCollection<QuestionCategoryDto>();
            LoadCategoriesCommand = new DelegateCommandAsync(async _ => await LoadCategoriesAsync());
            //LoadCategoriesCommand.Execute(null); // Flyttar detta anrop till InitializeAsync

            ImportQuestionsCommand = new DelegateCommandAsync(
                async _ => await ImportQuestionsAsync(),
                _ => SelectedCategory != null
            );

            //await LoadCategoryDifficultyCountsAsync();
            //_ = InitializeAsync(); // Jag flyttar detta till ImporterTriviaDbView.xaml.cs.
        }

        public async Task LoadCategoriesAsync()
        {
            var url = "https://opentdb.com/api_category.php";

            //using var client = new HttpClient();
            //var json = await client.GetStringAsync(url);
            //var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            //var response = JsonSerializer.Deserialize<CategoryListResponse>(json, options);
            //if(response?.TriviaCategories != null)
            //{
            //    Categories = response.TriviaCategories.Select(c => c.Name ?? "").ToList();
            //}

            System.Net.Http.HttpResponseMessage response;
            try
            {
                response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var stream = await response.Content.ReadAsStreamAsync();
                    var data = await JsonSerializer.DeserializeAsync<CategoryListResponseDto>(stream);
                    Categories.Clear();
                    foreach (var cat in data?.TriviaCategories ?? new List<QuestionCategoryDto>())
                    {
                        cat.CountFromViewModel = GetCountForCategory;
                        Categories.Add(cat);
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Det gick inte att hämta kategorier. Felmeddelande: {ex.Message}");
            }

        }

        private int GetCountForCategory(int CategoryId)
        {
            var count = CategoryCounts.FirstOrDefault(c =>
                c.CategoryId == CategoryId &&
                c.Difficulty == SelectedDifficulty
                );

            return count?.Count ?? 0;
        }





        public async Task ImportQuestionsAsync()
        {
            if (SelectedCategory == null)
                return;

            var amount = 10; // Denna måste jag uppdatera senare, eftersom användaren ska kunna välja antal frågor själv!!
            //var difficulty = "medium";

            var difficulty = SelectedDifficulty.ToString().ToLower();

            var url = $"https://opentdb.com/api.php?amount={amount}&category={SelectedCategory.Id}&difficulty={difficulty}&type=multiple";

            try
            {
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode(); // Exception om inte 2xx-kod

                var stream = await response.Content.ReadAsStreamAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var data = await JsonSerializer.DeserializeAsync<TriviaQuestionListResponseDto>(stream, options);

                if (data?.ResponseCode == 0 && data.Results != null)
                {
                    Console.WriteLine($"Lyckades importera {data.Results.Count} frågor.");
                }
                else
                {
                    Console.WriteLine("API svarade men kunde inte returnera frågor (t.ex. för få frågor i databasen).");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fel vid import: {ex.Message}");
            }
        }


        private async Task LoadCategoryDifficultyCountsAsync()
        {
            if (Categories == null || !Categories.Any())
                return;

            var service = new CategoryCountService();
            var counts = await service.FetchQuestionCountsAsync(Categories);

            CategoryCounts.Clear();

            foreach (var count in counts)
                CategoryCounts.Add(count);

            //foreach(var cat in Categories)
            //{
            //    cat.CountFromViewModel = GetCountForCategory;
            //}

            foreach(var cat in Categories)
            {
                cat.RaisePropertyChanged(nameof(QuestionCategoryDto.DisplayNameWithCount));
            }
        }

        public async Task InitializeAsync()
        {
            await LoadCategoriesAsync();
            await LoadCategoryDifficultyCountsAsync();
            Console.WriteLine($"test {Categories}"); // Denna kodrad var bara för att kunna hovra över {Categories} vid debugging.
        }


    }
}
