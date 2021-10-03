using System;

namespace Assets.Scripts
{
    [Serializable]
    public class GameData
    {
        private int MaxScore;

        public GameData()
        {
            MaxScore = 0;
        }

        public int GetMaxScore()
        {
            return MaxScore;
        }

        public void SetMaxScore(int value)
        {
            MaxScore = value;
        }
    }

}

