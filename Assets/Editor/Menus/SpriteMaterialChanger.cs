using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Fading
{

    public class SpriteMaterialChanger : ScriptableWizard
    {
        static Material materialToset = null;
        public Material MaterialToSet = null;

        [MenuItem("Devilmind/Sprite Renderers/Replace Material")]
        static void CreateWizard()
        {
            ScriptableWizard.DisplayWizard(
                "Replace Material", typeof(SpriteMaterialChanger), "Replace");
        }

        public SpriteMaterialChanger()
        {
            MaterialToSet = materialToset;
        }

        void OnWizardUpdate()
        {
            materialToset = MaterialToSet;
        }

        void OnWizardCreate()
        {
            if (materialToset == null)
            {
                Debug.Log("First please set proper material");
                return;
            }

            Transform[] transforms = Selection.GetTransforms(SelectionMode.DeepAssets | SelectionMode.Unfiltered | SelectionMode.Deep);

            foreach (Transform t in transforms)
            {
                SpriteRenderer renderer = t.GetComponent<SpriteRenderer>();
                if (renderer == null)
                {
                    continue;
                }

                renderer.material = materialToset;
                Debug.Log("Material changed in transform with name : " + t.name);
            }
        }
    }
}