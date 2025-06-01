using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Labb_03_version_02_Quiz_with_GUI.Enums;

namespace Labb_03_version_02_Quiz_with_GUI.Model
{
    //public enum Difficulty { Easy, Medium, Hard };
    public class QuestionPack
    {
        public QuestionPack(string name, Difficulty difficulty = Difficulty.Medium, int timeLimitInSeconds = 4)
        //public QuestionPack(string name, Difficulty difficulty = Difficulty.Medium, int timeLimitInSeconds = 30)
        {
            Name = name;
            Difficulty = difficulty;
            TimeLimitInSeconds = timeLimitInSeconds;
            Questions = new List<Question>();
        }

        // Denna konstruktor används för Json-konstruktorn i QuestionPackViewModel, som 
        // i sin tur används för LoadJsonCommand, för att kunna skapa en lista av
        // QuestionPackViewModel från en Json-fil.
        public QuestionPack()
        {
            Name = "";
            Difficulty = Difficulty.Medium;
            TimeLimitInSeconds = 30;
            Questions = new List<Question>();
        }

        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("difficulty")]
        public Difficulty Difficulty { get; set; }
        public int TimeLimitInSeconds { get; set; }
        public List<Question> Questions { get; set; }
    }
}
