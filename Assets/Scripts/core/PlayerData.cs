
using UnityEngine;

namespace DevilMind
{
    public class PlayerData
    {
        
        #region Const Tags for loading datas

        private const string TAG_TOKEN = "token";

        #endregion

        private static PlayerData _instance;
        public static PlayerData Instance
        {
            get { return _instance ?? (_instance = new PlayerData()); }
        }
        
        private void SaveData(string key, string data)
        {
            if (PlayerPrefs.HasKey(key))
            {
                Log.Info(MessageGroup.Common, "Data have been already saved under " + key + " key. Given data : \n\n"+data);
            }
            PlayerPrefs.SetString(key, data);
            PlayerPrefs.Save();
        }

        private string LoadData(string key)
        {
            string uncodedData = "";
            if (PlayerPrefs.HasKey(key))
            {
                uncodedData = PlayerPrefs.GetString(key);
            }
            else
            {
                Log.Info(MessageGroup.Common, "Data under "+ key + " key, doesn't exist");
            }
            return uncodedData;
        }

        #region ready functions to save and load datas

        public string GetToken()
        {
            return LoadData(TAG_TOKEN);
        }

        public void SaveToken(string token)
        {
            Log.Info(MessageGroup.Common, "Device is logged in now.");
            SaveData(TAG_TOKEN, token);
        }

        #endregion

    }
}