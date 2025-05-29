using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Text.Json.Serialization;

namespace Labb_03_version_02_Quiz_with_GUI.Dto
{
    public class CategoryDifficultyCountResponseDto
    {
        [JsonPropertyName("total_question_count")]
        public int Total { get; set; }

        [JsonPropertyName("total_easy_question_count")]
        public int Easy { get; set; }

        [JsonPropertyName("total_medium_question_count")]
        public int Medium { get; set; }

        [JsonPropertyName("total_hard_question_count")]
        public int Hard { get; set; }
    }
}
