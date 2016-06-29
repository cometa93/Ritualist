using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;

namespace DevilMind.QuestsSystem
{
    public class QuestToolEditor : EditorWindow
    {
        private bool _editRequairedIds;

        private Vector2 _scrollPosition;

        private const string _questsJsonPath = "/Resources/QuestManager/PreparedQuests.txt";

        private QuestManager.QuestsSave _save;

        private Dictionary<int, Quest> Quests
        {
            get
            {
                return _save != null ? _save.Quests : new Dictionary<int, Quest>();
            }
        }

        private readonly Dictionary<int, AnimBool> _displayedQuests = new Dictionary<int, AnimBool>();
        [MenuItem("Devilmind/Quests Editor")]
        static void Init()
        {

              var window = GetWindow<QuestToolEditor>(true, "Devilmind - Quests Editor", true);
              window.minSize = new Vector2(100, 100);
              window.maxSize = new Vector2(300, 600);
              window.Repaint();
        }

        private void SaveQuests()
        {
            if (_save == null)
            {
                _save = new QuestManager.QuestsSave();
            }

            var textFile = JsonConvert.SerializeObject(_save, Formatting.Indented, Settings.JsonSerializerSettings());

            try
            {
                System.IO.File.WriteAllText(Application.dataPath + _questsJsonPath, textFile);
                EditorUtility.DisplayDialog("Quests Saved !", "Quest were save succesfully", "ok");
            }
            catch (Exception)
            {
                Debug.LogWarning("something went wrong with saving quests into file");
            }

        }

        private void LoadQuests()
        {
            var textAsset = ResourceLoader.Load<TextAsset>("QuestManager/PreparedQuests");
            if (textAsset == null || string.IsNullOrEmpty(textAsset.text))
            {
                _save = new QuestManager.QuestsSave();
                Debug.Log("Quests just first time created");
                SaveQuests();
                return;
            }

            _save = JsonConvert.DeserializeObject<QuestManager.QuestsSave>(textAsset.text);
        }

        private void OnGUI()
        {
            if (_save == null)
            {
                if (GUILayout.Button("Load Quests"))
                {
                    LoadQuests();
                }
                return;
            }

            EditorGUILayout.LabelField("Quest List");
            EditorGUILayout.Space();
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
            
            foreach (var quest in Quests)
            {
                EditorGUILayout.BeginVertical();
                DisplayQuestEditor(quest.Value);
                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.EndScrollView();

            if (GUILayout.Button("New Quest", GUILayout.Width(100), GUILayout.Height(30)))
            {
                Quests.Add(_save.LastId,new Quest(_save.LastId));
                _save.LastId++;
            }


            UnityEditor.EditorGUI.indentLevel--;
            EditorGUILayout.Separator();
            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Save Quests", GUILayout.Width(100), GUILayout.Height(30)))
            {
                SaveQuests();
            }

            if (GUILayout.Button("Revert", GUILayout.Width(100), GUILayout.Height(30)))
            {
                _save = null;
                LoadQuests();
            }

            EditorGUILayout.EndHorizontal();
        }

        private void DisplayQuestEditor(Quest quest)
        {
            AnimBool isVisible;
            if (_displayedQuests.TryGetValue(quest.ID, out isVisible) == false)
            {
                isVisible = new AnimBool(false);
                _displayedQuests.Add(quest.ID, isVisible);
            }

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            isVisible.value = EditorGUILayout.Foldout(isVisible.value, (quest.ID + "# " + quest.Title).ToUpper());
            if (isVisible.value)
            {
                UnityEditor.EditorGUI.indentLevel++;

                EditorGUILayout.PrefixLabel("Title");
                quest.Title = EditorGUILayout.TextArea(quest.Title, GUILayout.Height(25));

                EditorGUILayout.PrefixLabel("Summary");
                quest.Summary = EditorGUILayout.TextArea(quest.Summary, GUILayout.Height(50));

                EditorGUILayout.PrefixLabel("Long Description");
                quest.LongDescription = EditorGUILayout.TextArea(quest.LongDescription, GUILayout.Height(150));

                DisplayRequairedIdsEditor(quest);
            
                if (GUILayout.Button("Delete", GUILayout.Width(100), GUILayout.Height(25)))
                {
                    if (EditorUtility.DisplayDialog("Confirm", "Are you sure?", "Yes", "No"))
                    {
                        Quests.Remove(quest.ID);
                    }
                }

                UnityEditor.EditorGUI.indentLevel--;
            }

            EditorGUILayout.EndHorizontal();
        }

        private void DisplayRequairedIdsEditor(Quest quest)
        {
            _editRequairedIds = EditorGUILayout.Toggle("Edit Requaired Quests", _editRequairedIds);
            if (_editRequairedIds == false)
            {
                return;
            }

            string requairedQuestIds = "";
            foreach (var id in quest.RequairedQuestIds)
            {
                if (requairedQuestIds == "")
                {
                    requairedQuestIds += id;
                    continue;
                }
                requairedQuestIds += "," + id;
            }

            requairedQuestIds = EditorGUILayout.TextField("Quest ID's", requairedQuestIds);
            string[] splittedIds = requairedQuestIds.Split(',');
            foreach (var id in splittedIds)
            {
                int convertedId;
                if (int.TryParse(id, out convertedId) == false ||
                    quest.RequairedQuestIds.Contains(convertedId))
                {
                    continue;
                }

                if (Quests.ContainsKey(convertedId))
                {
                    quest.RequairedQuestIds.Add(convertedId);
                }
            }

        }
    }
}
