using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace Labb_03_version_02_Quiz_with_GUI.Dto
{
    public class CategoryListResponseDto
    {
        [JsonPropertyName("trivia_categories")]
        public List<QuestionCategoryDto>? TriviaCategories { get; set; }
    }
}
