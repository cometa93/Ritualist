using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace DevilMind.EditorGUI
{
    [CustomEditor(typeof(GameMasterBehaviour))]
    public class GameMasterBehaviourEditor : Editor
    {
        
        public BundleVersion Bundle { private set; get; }
        private bool _buildingVersion = false;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (_buildingVersion)
            {

                GUILayout.Label("Wait untill building done.");
                return;
            }
            GUILayout.Label("Current Bundle Version: " + Bundle );

            
            if (GUILayout.Button("Load Current Version"))
            {
                ReadVersion();
            }

            if (GUILayout.Button("Next Develop Version"))
            {

                _buildingVersion = true;
                ReadVersion();
                Bundle.Develop++;
                Bundle.BuildNumber++;
                GenerateVersionFile();
            }
            if (GUILayout.Button("Next Release Version"))
            {

                _buildingVersion = true;
                ReadVersion();
                Bundle.Release++;
                Bundle.BuildNumber++;
                Bundle.Develop = 0;
                GenerateVersionFile();
            }
            if (GUILayout.Button("Next Public Version"))
            {

                _buildingVersion = true;
                ReadVersion();
                Bundle.Public++;
                Bundle.BuildNumber++;
                Bundle.Develop = 0;
                Bundle.Release = 0;
                GenerateVersionFile();
            }
        }
        

        private void ReadVersion()
        {
                TextAsset textFile = Resources.Load("version", typeof(TextAsset)) as TextAsset;
                Debug.Log(textFile.text);
                if (textFile == null)
                {
                    Log.Exception(MessageGroup.Common, "Couldn't get game version");
                    //TODO: RESET HARD APPLICATION AND DOWNLOAD NEWEST VERSION WITH ASSET BUNDLES 
                }
                VersionParser parser = new VersionParser(textFile.text);
                Bundle = parser.Bundle;
                Resources.UnloadAsset(textFile);
        }

        private void GenerateVersionFile()
        {
            string parsedGameVarsion = JsonConvert.SerializeObject(Bundle);

            File.WriteAllText("Assets/Resources/version.txt",parsedGameVarsion);
            StreamWriter streamWriter = new StreamWriter("Assets/Resources/version.txt");
            Debug.Log(parsedGameVarsion);
            streamWriter.Write(parsedGameVarsion);
            streamWriter.Close();
            _buildingVersion = false;
        }
    }
}
