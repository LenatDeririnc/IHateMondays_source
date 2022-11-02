using UnityEditor;
using UnityEngine;

namespace SceneManager.ScriptableObjects
{
    public class SceneLink : ScriptableObject
    {
        
#if UNITY_EDITOR
        public SceneAsset sceneAsset;
#endif
        public string scenePath;
        public string sceneName;
    }
}