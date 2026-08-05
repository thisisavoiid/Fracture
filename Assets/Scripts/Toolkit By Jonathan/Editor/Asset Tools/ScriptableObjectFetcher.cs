using System.Collections.Generic;

namespace ToolkitByJonathan
{
    public static class ScriptableObjectFetcher
    {
        public static List<T> FetchObjectsOfType<T>() where T : class
        {
#if UNITY_EDITOR

            UnityEditor.GUID[] guids = UnityEditor.AssetDatabase.FindAssetGUIDs($"t:{typeof(T)}");

            if (guids.Length == 0)
                return null;

            List<T> objects = new();

            foreach (UnityEditor.GUID guid in guids)
            {
                UnityEngine.Object obj = UnityEditor.AssetDatabase.LoadAssetByGUID(guid, typeof(T));

                if (obj == null)
                    continue;

                objects.Add(obj as T);
            }

            return objects;

#endif
        }

        public static T FindFirstObjectOfType<T>() where T : class
        {
#if UNITY_EDITOR

            UnityEditor.GUID[] guids = UnityEditor.AssetDatabase.FindAssetGUIDs($"t:{typeof(T)}");

            if (guids.Length == 0)
                return null;

            UnityEngine.Object obj = UnityEditor.AssetDatabase.LoadAssetByGUID(guids[0], typeof(T));

            if (obj == null)
                return null;

            return obj as T;

#endif
        }

    }
}