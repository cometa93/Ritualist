using System;
using Fading;
using Fading.Settings;
using UnityEngine;

namespace DevilMind
{
    public class GameMaster
    {        
        public uint BuildNumber { private set; get; }
        public BundleVersion Bundle {private set; get;}
      
        #region Important static  Properties
        public static EventSystem.EventSystem Events { get { return Instance._events; } }
        public static Hero Hero { get { return Instance._hero; } }
        public static GameSave GameSave { get { return Instance._saves; } }
        #endregion

        #region Important properties
        private EventSystem.EventSystem _events;
        private Hero _hero;
        private GameSave _saves;
        #endregion

        private static GameMaster _instance;

        public static GameMaster Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GameMaster();
                    _instance.ReadGameVersion();
                    _instance._events = new EventSystem.EventSystem();
                    _instance._hero = new Hero();
                    _instance._saves = new GameSave();
                }
                return _instance;
            }
        }


        public void MainLoop(float deltaTime)
        {
        }

        public void ReadGameVersion()
        {            
            try
            {
                TextAsset textFile = Resources.Load("version", typeof(TextAsset)) as TextAsset;
                if (textFile == null)
                {
                    throw new Exception("text File is null");
                }

                VersionParser parser = new VersionParser(textFile.text);
                Bundle = parser.Bundle;
            }
            catch (Exception e)
            {
                Log.Exception(MessageGroup.Common, "Couldn't get game version" + e.StackTrace);
            }
        }
    }
}

