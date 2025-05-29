using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Labb_03_version_02_Quiz_with_GUI.Dto
{
    public class TriviaQuestionListResponseDto
    {
        [JsonPropertyName("response_code")]
        public int ResponseCode { get; set; }

        [JsonPropertyName("results")]
        public List<TriviaQuestionDto>? Results { get; set; }
    }
}
