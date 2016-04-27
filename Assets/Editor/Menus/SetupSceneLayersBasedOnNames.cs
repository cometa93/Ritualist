using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEditor;
using UnityEngine;

namespace Ritualist
{
    public class SetupSceneLayersBasedOnNames : ScriptableWizard
    {
        private const string GroundObjectName = "Ground";
        private const string FirstplaneObjectName = "First";
        private const string SecondPlaneObjectName = "Second";
        private const string BackgroundObjectName = "Background";
        private const string WorldObjectName = "World";
        private const string GroundMaskObjectName = "GroundMask";

        private const string GroundLayer = "Ground";
        private const string FirstLayer = "First";
        private const string SecondLayer = "Second";
        private const string BackgroundLayer = "Background";
        private const string Mask = "Mask";

        private readonly Dictionary<string, string> ObjectNameToLayerName = new Dictionary<string, string>
        {
            {GroundObjectName, GroundLayer },
            {FirstplaneObjectName, FirstLayer },
            {SecondPlaneObjectName, SecondLayer },
            {BackgroundLayer, BackgroundLayer },
            {GroundMaskObjectName, Mask }
        }; 

        private GameObject _world;

        [MenuItem("Devilmind/Scenes/Setup Scene Objects")]
        static void CreateWizard()
        {
            ScriptableWizard.DisplayWizard(
                "Script will set order layers based on names of gameObjects \n" +
                "Ground, Second, First,Background", typeof(SetupSceneLayersBasedOnNames), "Setup Scene");
        }

        void OnWizardCreate()
        {
            _world = GameObject.Find(WorldObjectName);
            if (_world == null)
            {
                Debug.LogWarning("Couldn't find gameobject with name World on scen Fix It");
                return;
            }

            foreach (Transform child in _world.transform)
            {
                string layerName = "";
                if (ObjectNameToLayerName.TryGetValue(child.name, out layerName))
                {
                    if (child.name == GroundMaskObjectName)
                    {
                        var position = child.position;
                        position.z = 0.5f;
                        child.position = position;
                        return;
                    }

                    SetupLayer(child.gameObject, layerName);
                }
            }
        }

        private void SetupLayer(GameObject go, string layer)
        {
            if (go == null)
            {
                Debug.LogWarning("Couldn't find object with name " + name + "in world object");
                return;
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