﻿using Labb_03_version_02_Quiz_with_GUI.Command;
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
using System.CodeDom.Compiler;
using System.Windows;
using Labb_03_version_02_Quiz_with_GUI.Model;

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

        private int _timeLimitInSeconds = 30;
        public int TimeLimitInSeconds
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
                ImportQuestionsCommand.RaiseCanExecuteChanged();
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

        public DelegateCommand CloseImporter { get; }

        private int _questionAmount = 10;
        public int QuestionAmount
        {
            get => _questionAmount;
            set
            {
                if (value >= 1 && value <= 999)
                {
                    _questionAmount = value;
                    RaisePropertyChanged();
                    ImportQuestionsCommand.RaiseCanExecuteChanged();
                }
            }
        }






        public ImporterTriviaDbViewModel(MainWindowViewModel mainWindowViewModel)
        {
            this.mainWindowViewModel = mainWindowViewModel;
            Categories = new ObservableCollection<QuestionCategoryDto>();
            LoadCategoriesCommand = new DelegateCommandAsync(async _ => await LoadCategoriesAsync());
            //LoadCategoriesCommand.Execute(null); // Flyttar detta anrop till InitializeAsync

            ImportQuestionsCommand = new DelegateCommandAsync(
                //async _ => await ImportQuestionsAsync(),
                async (window) =>
                {
                    bool importSuccess = await ImportQuestionsAsync();

                    if (window is Window w && importSuccess)
                    {
                        w.Close();
                    }
                },
                _ => SelectedCategory != null && QuestionAmount > 0
            );

            //await LoadCategoryDifficultyCountsAsync();
            //_ = InitializeAsync(); // Jag flyttar detta till ImporterTriviaDbView.xaml.cs.

            CloseImporter = new DelegateCommand(
                execute: (window) =>
                {
                    if (window is Window w)
                    {
                        w.Close();
                    }
                });

        }

        public async Task LoadCategoriesAsync()
        {

            Categories?.Clear();


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
                    //Categories.Clear();
                    foreach (var cat in data?.TriviaCategories ?? new List<QuestionCategoryDto>())
                    {
                        cat.CountFromViewModel = GetCountForCategory;
                        Categories?.Add(cat);
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show($"Unable to fetch categories. Error message: {ex.Message}.");
                Console.WriteLine($"Det gick inte att hämta kategorier. Felmeddelande: {ex.Message}");
            }

            //if (Categories != null && Categories.Any())
            //{
            //    SelectedCategory = Categories.First();
            //}

        }

        private int GetCountForCategory(int CategoryId)
        {
            var count = CategoryCounts.FirstOrDefault(c =>
                c.CategoryId == CategoryId &&
                c.Difficulty == SelectedDifficulty
                );

            return count?.Count ?? 0;
        }





        //public async Task ImportQuestionsAsync()
        public async Task<bool> ImportQuestionsAsync()
        {
            
            if (string.IsNullOrWhiteSpace(QuizName) || QuizName?.Length > 32)
            //if (QuizName?.Length > 32)
            {
                MessageBox.Show($"Quiz name must be between 1 and 32 characters.");
                //MessageBox.Show($"Quiz name must be at most 32 characters.");
                return false;
            }

            string categoryString = "";
            if (SelectedCategory != null && SelectedCategory.Id != 0) // Id == 0 för min egen kategori "Mixed categories".
            {
                categoryString = $"&category={SelectedCategory.Id}";
            }

            // Jag funderar på att ha blandade svårighetsgrader i framtiden, så därför har
            // jag redan nu en if-sats som kollar om den är null.
            var difficulty = SelectedDifficulty.ToString().ToLower();
            string difficultyString = "";
            if (SelectedDifficulty != null)
            {
                difficultyString = $"&difficulty={difficulty}";
            }
            
            var amount = QuestionAmount;
            var maxQuestions = CategoryCounts.FirstOrDefault(c =>
                c.CategoryId == SelectedCategory?.Id &&
                c.Difficulty == SelectedDifficulty
            )?.Count ?? 0;
            if (amount < 1 || amount > maxQuestions)
            {
                MessageBox.Show($"Number of questions must be between 1 and {maxQuestions}.");
                return false;
            }


            //var url = $"https://opentdb.com/api.php?amount={amount}&category={SelectedCategory.Id}&difficulty={difficulty}&type=multiple";
            var url = $"https://opentdb.com/api.php?amount={amount}{categoryString}{difficultyString}&type=multiple";

            try
            {
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode(); // Exception om inte 2xx-kod

                var stream = await response.Content.ReadAsStreamAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var data = await JsonSerializer.DeserializeAsync<TriviaQuestionListResponseDto>(stream, options);

                // Jag använder inte Token, så jag behöver inte oroa mig för de fallen
                // egentligen. Jag har dock med de fallen ifall jag uppdaterar mitt program
                // i framtiden eller om TriviaDB ändras så att man måste ha en Token.
                // 
                // Man bör inte heller kunna försöka importera för många frågor, eftersom 
                // jag redan har en egen spärr mot det.
                switch (data?.ResponseCode)
                {
                    case 0:
                        break;
                    case 1:
                        MessageBox.Show("No results: The API doesn't have enough questions for your query.");
                        return false;
                    case 2:
                        MessageBox.Show("Invalid parameter: Check amount, category, or difficulty.");
                        return false;
                    case 3:
                        MessageBox.Show("Token not found: Session token is missing or invalid.");
                        return false;
                    case 4:
                        MessageBox.Show("Token empty: All questions for this token have been exhausted.");
                        return false;
                    case 5:
                        MessageBox.Show("Rate limit: Too many requests. Wait a few seconds and try again.");
                        return false;
                    default:
                        MessageBox.Show($"Unknown response code: {data?.ResponseCode}");
                        return false;
                }

                if (data.Results == null || !data.Results.Any())
                {
                    MessageBox.Show("No questions were returned from the API.");
                    return false;
                }

                var questionPack = new QuestionPack
                {
                    Name = QuizName ?? "Unnamed Quiz",
                    Difficulty = SelectedDifficulty,
                    TimeLimitInSeconds = this.TimeLimitInSeconds
                };

                foreach (var q in data.Results)
                {
                    if (q.IncorrectAnswers.Count != 3)
                        continue;


                    string decodedQuestion = System.Net.WebUtility.HtmlDecode(q.QuestionText ?? "");
                    string decodedCorrect = System.Net.WebUtility.HtmlDecode(q.CorrectAnswer ?? "");
                    string decodedIncorrect1 = System.Net.WebUtility.HtmlDecode(q.IncorrectAnswers[0] ?? "");
                    string decodedIncorrect2 = System.Net.WebUtility.HtmlDecode(q.IncorrectAnswers[1] ?? "");
                    string decodedIncorrect3 = System.Net.WebUtility.HtmlDecode(q.IncorrectAnswers[2] ?? "");

                    if (
                        decodedQuestion.Length > 150 ||
                        decodedCorrect.Length > 50 ||
                        decodedIncorrect1.Length > 50 ||
                        decodedIncorrect2.Length > 50 ||
                        decodedIncorrect3.Length > 50
                        )
                    {
                        // Här skulle jag kunna göra en loop som försöker importera en 
                        // ny fråga att ersätta den fråga som inte uppfyller kriterierna 
                        // i denna if-sats. Loopen skulle kunna köras ett begränsat antal
                        // gånger (ifall det inte finns någon mer fråga som uppfyller
                        // kraven eller för att det inte ska ta för lång tid). Om man
                        // får en response med en godkänd fråga avbryts loopen och den
                        // frågan kommer med i importen.
                        //
                        // Bättre kanske vore att kolla hur många frågor som inte godkändes 
                        // när man kört denna funktion (ImportQuestionsAsync) och försöka 
                        // importera ersättare för alla saknade frågor på en gång.
                        // 
                        // Jag tror dock inte att det är så många frågor, om ens några, som
                        // inte uppfyller kraven.
                        MessageBox.Show($"The question {decodedQuestion} was removed from the imported questions. Either the question itself was too long or an answer option was too long.");
                        continue;
                    }


                    //var question = new Question(
                    //    query: System.Net.WebUtility.HtmlDecode(q.QuestionText ?? ""),
                    //    correctAnswer: System.Net.WebUtility.HtmlDecode(q.CorrectAnswer ?? ""),
                    //    incorrectAnswer1: System.Net.WebUtility.HtmlDecode(q.IncorrectAnswers[0] ?? ""),
                    //    incorrectAnswer2: System.Net.WebUtility.HtmlDecode(q.IncorrectAnswers[1] ?? ""),
                    //    incorrectAnswer3: System.Net.WebUtility.HtmlDecode(q.IncorrectAnswers[2] ?? "")
                    //);


                    var question = new Question(
                        query: decodedQuestion,
                        correctAnswer: decodedCorrect,
                        incorrectAnswer1: decodedIncorrect1,
                        incorrectAnswer2: decodedIncorrect2,
                        incorrectAnswer3: decodedIncorrect3
                    );

                    questionPack.Questions.Add(question);
                }


                if (questionPack.Questions.Count == 0)
                {
                    MessageBox.Show("No valid questions could be imported.");
                    return false;
                }


                var packViewModel = new QuestionPackViewModel(questionPack);
                mainWindowViewModel.Packs.Add(packViewModel);
                mainWindowViewModel.ActivePack = packViewModel;
                mainWindowViewModel.ActivePack.SelectedQuestion = questionPack.Questions.FirstOrDefault();
                mainWindowViewModel.ConfigurationViewModel.HasSelectedQuestion = false;

                //    //mainWindowViewModel.ConfigurationViewModel.HasSelectedQuestion.RaisePropertyChanged(); // Detta fungerade inte.
                //    //mainWindowViewModel.ConfigurationViewModel.RaisePropertyChanged(nameof(ConfigurationViewModel.HasSelectedQuestion)); // Detta bör fungera men används inte pga RaisePropertyChanged används i settern för HasSelectedQuestion.


                mainWindowViewModel.SaveJsonCommand.Execute(null);

                Console.WriteLine($"Lyckades importera {data.Results.Count} frågor.");
                MessageBox.Show($"Quiz '{QuizName}' with {questionPack.Questions.Count} questions imported.");
                return true;

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fel vid import: {ex.Message}");
                MessageBox.Show($"Error while importing: {ex.Message}");
                return false;
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

            // Kod för att skapa mitt förvalda "Mixed Categories", alltså alla frågekategorier.
            var mixedCategory = new QuestionCategoryDto
            {
                Id = 0, 
                Name = "Mixed categories",
                CountFromViewModel = _ => CategoryCounts
                    .FirstOrDefault(c => c.CategoryId == 0 && c.Difficulty == SelectedDifficulty)
                    ?.Count ?? 0
            };


            Categories.Insert(0, mixedCategory);
            SelectedCategory ??= mixedCategory;
        }

        public async Task InitializeAsync()
        {
            await LoadCategoriesAsync();
            await LoadCategoryDifficultyCountsAsync();
            Console.WriteLine($"test {Categories}"); // Denna kodrad var bara för att kunna hovra över {Categories} vid debugging.
        }


        //private async Task<int?> FetchTotalQuestionCountAsync()
        //{
        //    try
        //    {
        //        var response = await _httpClient.GetAsync("https://opentdb.com/api_count_global.php");

        //        if (response.IsSuccessStatusCode)
        //        {
        //            var stream = await response.Content.ReadAsStreamAsync();
        //            var json = await JsonSerializer.DeserializeAsync<JsonElement>(stream);

        //            if (json.TryGetProperty("overall", out var overall) &&
        //                overall.TryGetProperty("total_num_of_verified_questions", out var total))
        //            {
        //                return total.GetInt32();
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Kunde inte hämta global frågecount: {ex.Message}");
        //    }

        //    return null;
        //}


    }
}
