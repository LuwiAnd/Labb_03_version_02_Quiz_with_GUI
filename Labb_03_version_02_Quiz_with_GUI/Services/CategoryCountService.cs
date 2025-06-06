using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net.Http;
using System.Text.Json;

using Labb_03_version_02_Quiz_with_GUI.Dto;
using Labb_03_version_02_Quiz_with_GUI.Model;
using Labb_03_version_02_Quiz_with_GUI.Enums;
using System.Text.Json.Serialization;
using System.Windows;

namespace Labb_03_version_02_Quiz_with_GUI.Services
{
    class CategoryCountService
    {
        private readonly HttpClient _httpClient = new();

        public async Task<List<TriviaCategoryDifficultyCount>> FetchQuestionCountsAsync(IEnumerable<QuestionCategoryDto> categories)
        {
            var result = new List<TriviaCategoryDifficultyCount>();

            foreach (var category in categories)
            {
                try
                {
                    var url = $"https://opentdb.com/api_count.php?category={category.Id}";
                    var stream = await _httpClient.GetStreamAsync(url);
                    var outerResponse = await JsonSerializer.DeserializeAsync<CategoryCountApiResponse>(stream);

                    if (outerResponse?.CategoryQuestionCount != null)
                    {
                        var count = outerResponse.CategoryQuestionCount;
                        result.Add(new TriviaCategoryDifficultyCount(category.Id, null, count.Total));
                        result.Add(new TriviaCategoryDifficultyCount(category.Id, Difficulty.Easy, count.Easy));
                        result.Add(new TriviaCategoryDifficultyCount(category.Id, Difficulty.Medium, count.Medium));
                        result.Add(new TriviaCategoryDifficultyCount(category.Id, Difficulty.Hard, count.Hard));
                    }

                    
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error fetching count for category {category.Id}: {ex.Message}");
                    Console.WriteLine($"Fel vid hämtning av antal för kategori {category.Id}: {ex.Message}");
                }
            }


            result.Add(
                        new TriviaCategoryDifficultyCount(
                            0,
                            null,
                            result
                                .Where(_ => true) // För att det ska vara lättare att jämföra med nedanstående kod.
                                .Sum(r => r.Count)
                        )
                    );

            result.Add(
                new TriviaCategoryDifficultyCount(
                    0,
                    Difficulty.Easy,
                    result
                        .Where(r => r.Difficulty == Difficulty.Easy)
                        .Sum(r => r.Count)
                )
            );

            result.Add(
                new TriviaCategoryDifficultyCount(
                    0,
                    Difficulty.Medium,
                    result
                        .Where(r => r.Difficulty == Difficulty.Medium)
                        .Sum(r => r.Count)
                )
            );

            result.Add(
                new TriviaCategoryDifficultyCount(
                    0,
                    Difficulty.Hard,
                    result
                        .Where(r => r.Difficulty == Difficulty.Hard)
                        .Sum(r => r.Count)
                )
            );

            return result;
        }
    }

    public class CategoryCountApiResponse
    {
        [JsonPropertyName("category_question_count")]
        public CategoryDifficultyCountResponseDto? CategoryQuestionCount { get; set; }
    }

    public class TriviaCategoryDifficultyCount
    {
        public int CategoryId { get; }
        public Difficulty? Difficulty { get; }
        public int Count { get; }

        public TriviaCategoryDifficultyCount(int categoryId, Difficulty? difficulty, int count)
        {
            CategoryId = categoryId;
            Difficulty = difficulty;
            Count = count;
        }
    }
}
