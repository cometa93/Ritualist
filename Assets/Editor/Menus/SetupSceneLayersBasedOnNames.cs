using System.Collections.Generic;
using System.Xml.Serialization;
using DevilMind;
using Fading.InteractiveObjects;
using UnityEditor;
using UnityEngine;

namespace Fading
{
    public class SetupSceneLayersBasedOnNames : ScriptableWizard
    {
        private const string GroundObjectName = "Ground";
        private const string FirstplaneObjectName = "First";
        private const string SecondPlaneObjectName = "Second";
        private const string BackgroundObjectName = "Background";
        private const string WorldObjectName = "World";
        private const string GroundMaskObjectName = "GroundMask";
        private const string GameplayObjects = "GameplayObjects";

        private const string GroundLayer = "Ground";
        private const string FirstLayer = "First";
        private const string SecondLayer = "Second";
        private const string BackgroundLayer = "Background";
        private const string Mask = "Mask";
        private const string CharacterLayer = "Character Graphics";

        private readonly Dictionary<string, string> ObjectNameToLayerName = new Dictionary<string, string>
        {
            {GroundObjectName, GroundLayer },
            {FirstplaneObjectName, FirstLayer },
            {SecondPlaneObjectName, SecondLayer },
            {BackgroundLayer, BackgroundLayer },
            {GroundMaskObjectName, Mask },
            {GameplayObjects,  CharacterLayer}
        }; 

        
        [MenuItem("Devilmind/Scenes/Setup Scene Objects")]
        static void CreateWizard()
        {
            ScriptableWizard.DisplayWizard(
                "Script will set order layers based on names of gameObjects \n" +
                "Ground, Second, First,Background", typeof(SetupSceneLayersBasedOnNames), "Setup Scene");
        }
        
        void OnWizardCreate()
        {
            var worldObjects = Selection.objects;
            for (int index = 0; index < worldObjects.Length; index++)
            {
                var world = worldObjects[index];
                Setup(world as GameObject);
            }
        }

        void Setup(GameObject world)
        {
            if (world == null)
            {
                Debug.LogWarning("Selected object is not world or its name is not world");
                return;
            }
            
            if (world.GetComponent<WorldBehaviour>() == null)
            {
                world.AddComponent<WorldBehaviour>();
                Log.Info(MessageGroup.Gameplay, "World behaviour added to world object");
            }

            string layerName = "";
            foreach (var keyValue in ObjectNameToLayerName)
            {
                //skipping backgrounds
                if (keyValue.Key == BackgroundObjectName)
                {
                    continue;
                }
                Transform child = world.transform.FindChild(keyValue.Key);
                
                var go = child != null ? child.gameObject : null;
                SetupLayer(go, keyValue.Value, keyValue.Key);
            }

            //TODO setting layers for skipped before backgrounds 
            foreach (Transform child in world.transform)
            {
                if (child.name == BackgroundObjectName && 
                    ObjectNameToLayerName.TryGetValue(child.name,out layerName))
                {
                    SetupLayer(child.gameObject, layerName, child.name);
                }
            }

        }

        private void SetupLayer(GameObject go, string layer, string objectName)
        {
            if (go == null)
            {
                Debug.LogWarning("Couldn't find object with name " +  objectName + " in world object");
                return;
            }

            if (go.name == GroundMaskObjectName)
            {
                var position = go.transform.position;
                position.z = 0.5f;
                go.transform.position = position;
            }

            SetOrderLayerInTransform(go.transform, layer);
        }

        private void SetOrderLayerInTransform(Transform t, string layer)
        {
            if (t.GetComponent<Renderer>() != null)
            {
                t.GetComponent<Renderer>().sortingLayerName = layer;
            }

            foreach (Transform child in t)
            {
                SetOrderLayerInTransform(child, layer);
            }
        }
    }
}