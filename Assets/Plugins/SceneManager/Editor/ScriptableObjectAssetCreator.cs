using System.IO;
using UnityEditor;
using UnityEngine;

namespace SceneManager.Editor
{
    internal static class ScriptableObjectAssetCreator
    {
        public static string AssetPath(Object selection)
        {
            return Path.GetDirectoryName(AssetDatabase.GetAssetPath(selection));
        }
        
        public static T Create<T>(string name, string path = "") where T : ScriptableObject
        {
            T asset = ScriptableObject.CreateInstance<T>();
            
            if (path == "")
                path = AssetPath(Selection.activeObject);
            
            AssetDatabase.CreateAsset(asset, $"{path}/{name}.asset");
            AssetDatabase.SaveAssets();

            return asset;
        }

        public static Object ActiveObject() => 
            Selection.activeObject;

        public static bool Validate<T>() where T : UnityEngine.Object => 
            ActiveObject().Validate<T>();

        public static bool Validate<T>(this UnityEngine.Object selection) where T : UnityEngine.Object
        {
            T script = selection as T;
            return script != null;
        }
    }
}