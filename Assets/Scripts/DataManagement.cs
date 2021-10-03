using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Assets.Scripts.Data
{
    public static class DataManagement
    {
        public static void Save(GameData data)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream fs = new FileStream(GetPath(), FileMode.Create);
            formatter.Serialize(fs, data);
            fs.Close();
// NB (lac): webgl instances seems to need this to "force" syncronization of temp/cache. If this is not done, closing the browser doesn't update the data
#if UNITY_WEBGL
            PlayerPrefs.SetString("dummy", string.Empty);
            PlayerPrefs.Save();
#endif
        }

        public static GameData Load()
        {
            if (!File.Exists(GetPath()))
            {
                GameData emptyData = new GameData();
                Save(emptyData);
                return emptyData;
            }
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream fs = new FileStream(GetPath(), FileMode.Open);
            GameData data = (GameData)formatter.Deserialize(fs);
            fs.Close();

            return data;
        }

        private static string GetPath()
        {
            return Path.Combine(Application.persistentDataPath, "gameData");
        }
    }
}
