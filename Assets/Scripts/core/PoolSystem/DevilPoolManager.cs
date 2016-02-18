using System.Collections.Generic;

namespace DevilMind
{
    public class DevilPoolManager
    {
        private static readonly Dictionary<string,DevilPool> _devilPools = new Dictionary<string, DevilPool>();

        public static void RegisterPool(string name, DevilPool pool)
        {
            if (_devilPools.ContainsKey(name))
            {
                Log.Error(MessageGroup.Common, "Current pool is already registered");
                return;
            }
            if (pool == null)
            {
                Log.Error(MessageGroup.Common, "Cannot register pool cause it is null.");
                return;
            }
            _devilPools.Add(name,pool);
        }

        public static DevilPool GetPool(string name)
        {
            if (!_devilPools.ContainsKey(name))
            {
                Log.Error(MessageGroup.Common, name+" pool is not registered");
                return null;
            }
            return _devilPools[name];
        }

        public static void UnregisterPool(string name)
        {
            if (!_devilPools.ContainsKey(name))
            {
                Log.Error(MessageGroup.Common, name + " pool is not registered");
                return;
            }
            _devilPools.Remove(name);
        }

    }
}