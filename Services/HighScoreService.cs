using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C_Snake.Services
{
    public static class HighScoreService
    {
        private const string HighScoreFilePath = "highscore.txt";

        public static int LoadHighScore()
        {
            if (File.Exists(HighScoreFilePath) && int.TryParse(File.ReadAllText(HighScoreFilePath), out int score))
                return score;
            return 0;
        }

        public static void SaveHighScore(int score)
        {
            File.WriteAllText(HighScoreFilePath, score.ToString());
        }
    }
}
