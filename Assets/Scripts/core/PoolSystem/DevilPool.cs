using System;
using System.Collections.Generic;
using UnityEngine;

namespace DevilMind
{
    [Serializable]
    public class DevilObjectData
    {
        private string ID;
        public string PrefabName;
        public GameObject Prefab;
        public int PreloadedInstances;
        public int MaximumInstances;

//        public void Inspector()
//        {
//            PrefabName = EditorGUILayout.TextField("Prefab Name :", PrefabName);
//            Prefab = EditorGUILayout.ObjectField("Prefab: ", Prefab,typeof(GameObject),false) as GameObject;
//            PreloadedInstances = EditorGUILayout.IntField("Preloaded Instances :", PreloadedInstances);
//            MaximumInstances = EditorGUILayout.IntField("Maximum Instances : ",MaximumInstances);
//        }
    }

    public class DevilPool : DevilBehaviour
    {
        [SerializeField] private List<DevilObjectData> _listOfPrefabs;
        [SerializeField] private string _poolName ;
        [SerializeField] private int _countOfObjects;

        private Transform _poolTransform;
        private readonly Dictionary<GameObject,string> _spawnedObjects = new Dictionary<GameObject,string>();
        private readonly Dictionary<string,int> _instanceCount = new Dictionary<string, int>(); 
        private readonly Dictionary<string,List<GameObject>> _preloadedObjects = new Dictionary<string, List<GameObject>>(); 

        protected override void OnEnable()
        {
            RegisterMyself();
            InitializePool();
        }

        private void RegisterMyself()
        {
            if (string.IsNullOrEmpty(_poolName))
            {
                Log.Exception(MessageGroup.Common, "Pool name is nulled or empty !!!");
                return;
            }
            if (_listOfPrefabs == null)
            {
                _listOfPrefabs = new List<DevilObjectData>();
            }
            DevilPoolManager.RegisterPool(_poolName,this);
        }

        private void InitializePool()
        {
            var pool = new GameObject(_poolName);
            _poolTransform = pool.transform;
            _poolTransform.parent = transform;
            foreach (var data in _listOfPrefabs)
            {
                PreloadInstances(data);
            }
        }

        protected override void Reset()
        {
            if (string.IsNullOrEmpty(_poolName))
            {
                Log.Exception(MessageGroup.Common, "Pool name is nulled or empty !!!");
                return;
            }
            DevilPoolManager.UnregisterPool(_poolName);
        }

        public GameObject Spawn(string name)
        {
            DevilObjectData data = GetDataByName(name);
            GameObject go = null;
            if (!_preloadedObjects.ContainsKey(name))
            {
                Log.Error(MessageGroup.Common, "This pool doesn't preload " + name + " objects.");
                return null;
            }
            if (_preloadedObjects[name].Count > 0)
            {
                go = _preloadedObjects[name][0];
                _preloadedObjects[name].RemoveAt(0);
            }
            if ( _instanceCount[name] >= data.MaximumInstances-1 || go == null)
            {
                Log.Warning(MessageGroup.Common, "This game object ( " + name + " )reached maximum number of instances in that pool ( " + _poolName + ")");
                go = InstantiatePrefab(data.Prefab);
            }
            if (go == null)
            {
                Log.Error(MessageGroup.Common, "Something went wrong in spawn function and game object is null");
                return null;
            }
            ++_instanceCount[name];
            go.SetActive(true);
            _spawnedObjects.Add(go,name);
            return go;
        }

        public void Despawn(GameObject obj)
        {
            if (obj == null)
            {
                Log.Error(MessageGroup.Common, "Given object is null");
                return;
            }
            obj.SetActive(false);
            if (_spawnedObjects.ContainsKey(obj) == false)
            {
                Log.Info(MessageGroup.Common, "Given game object ( ID : "+obj.GetInstanceID()+" ) is not in _spawnedObjects list in pool ( "+_poolName+")");
                Destroy(obj);
                return;
            }
            string spawnedObjectName = _spawnedObjects[obj];
            var data = GetDataByName(spawnedObjectName);
            if (data == null)
            {
                Log.Error(MessageGroup.Common, "Can't find data for given game object ( ID : "+obj.GetInstanceID()+" ) in pool( "+_poolName+")");
                return;
            }
            if (_preloadedObjects[spawnedObjectName].Contains(obj))
            {
                Log.Error(MessageGroup.Common, "Given object is already in preloaded instances list");
                return;
            }
            if (_instanceCount[spawnedObjectName] > data.MaximumInstances && data.MaximumInstances > 0)
            {
                _spawnedObjects.Remove(obj);
                Destroy(obj);
                return;
            }
            --_instanceCount[spawnedObjectName];
            obj.name = spawnedObjectName;
            obj.transform.parent = _poolTransform;
            _spawnedObjects.Remove(obj);
            _preloadedObjects[spawnedObjectName].Add(obj);


        }

        private void PreloadInstances(DevilObjectData data)
        {
            if (data == null)
            {
                Log.Error(MessageGroup.Common, "Given DevilObjectData with is null, in pool named: " + _poolName);
                return;
            }
            GameObject prefab = data.Prefab;
            if (prefab == null)
            {
                Log.Error(MessageGroup.Common, "Couldn't get prefab from DevilObjectData with that name ( " + data.PrefabName + " ) in pool named: " + _poolName);
                return;
            }
            int preloadedInstances = data.PreloadedInstances;
            if (!_preloadedObjects.ContainsKey(data.PrefabName))
            {
                _preloadedObjects.Add(data.PrefabName, new List<GameObject>());
            }
            if (_instanceCount.ContainsKey(data.PrefabName) == false)
            {
                _instanceCount.Add(data.PrefabName, 0);
            }
            for (int i = 0, c = preloadedInstances; i < c; ++i)
            {
              prefab =  InstantiatePrefab(prefab);
              _preloadedObjects[data.PrefabName].Add(prefab);
            }
        }

        private GameObject InstantiatePrefab(GameObject prefabToSpawn)
        {
            var prefab = Instantiate(prefabToSpawn);
            Transform instanceTransform = prefab.transform;
            instanceTransform.parent = _poolTransform;
            instanceTransform.localPosition = Vector3.zero;
            instanceTransform.localScale = Vector3.one;
            instanceTransform.localRotation = Quaternion.identity;
            prefab.SetActive(false);
            return prefab;
        }

        private GameObject GetPrefabByName(string name)
        {
            DevilObjectData data = GetDataByName(name);
            if (data == null)
            {
                Log.Error(MessageGroup.Common, "Can't return prefab because data is null");
                return null;
            }
            if (data.Prefab == null)
            {

                Log.Error(MessageGroup.Common, "Can't return prefab because prefab in data is null");
                return null;
            }
            return data.Prefab;

        }

        public DevilObjectData GetDataByName(string name)
        {
            for (int index = 0; index < _listOfPrefabs.Count; index++)
            {
                var data = _listOfPrefabs[index];
                if (data.PrefabName == name)
                {
                    return data;
                }
            }
            Log.Error(MessageGroup.Common, "This pool doesn't contain data with that name.(" + name +")");
            return null;
        }

        //        #region Editor GUI funtionality
        //        public void Inspector()
        //        {
        //            for(int i = 0,c = _listOfPrefabs.Count;i<c;++i)
        //            {
        //                _listOfPrefabs[i].Inspector();
        //                if(GUILayout.Button("Remove"))
        //                {
        //                    RemovePrefab(_listOfPrefabs[i]);
        //                    return;
        //                }
        //                EditorGUILayout.Separator();
        //            }
        //            if (GUILayout.Button("Add prefab"))
        //            {
        //                AddPrefab();
        //            }
        //        }
        //
        //        public void RemovePrefab(DevilObjectData data)
        //        {
        //            if (_listOfPrefabs.Contains(data))
        //            {
        //                _listOfPrefabs.Remove(data);
        //            }
        //        }
        //
        //        public void AddPrefab()
        //        {
        //            _listOfPrefabs.Add(new DevilObjectData());
        //        }
        //        #endregion

    }
}