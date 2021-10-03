using System;

namespace Assets.Scripts
{
    [Serializable]
    public class GameData
    {
        private int MaxScore;
        private int MaxTrainingTime;

        public GameData()
        {
            MaxScore = 0;
            MaxTrainingTime = 0;
        }

        public int GetMaxScore()
        {
            return MaxScore;
        }

        public void SetMaxScore(int value)
        {
            MaxScore = value;
        }

        public int GetMaxTrainingTime()
        {
            return MaxTrainingTime;
        }

        public void SetMaxTrainingTime(int value)
        {
            MaxTrainingTime = value;
        }
    }

}

