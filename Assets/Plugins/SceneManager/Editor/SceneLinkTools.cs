using SceneManager.ScriptableObjects;
using UnityEditor;
using UnityEditor.Callbacks;
// using UnityEditor.SceneManagement;
using UnityEngine;

namespace SceneManager.Editor
{
    public static class SceneLinkTools
    {
        private const string AssetPathName = "Assets/Create/Scene Links";

        public static SceneLink[] CreateInstances(Object[] selections)
        {
            SceneLink[] newObjects = new SceneLink[selections.Length];
            
            for (int i = 0; i < selections.Length; i++)
            {
                Object selection = selections[i];
                SceneLink asset = ScriptableObjectAssetCreator.Create<SceneLink>(selection.name);
                asset.sceneAsset = (SceneAsset) selection;
                asset.scenePath = $"{ScriptableObjectAssetCreator.AssetPath(selection)}\\{asset.sceneAsset.name}";
                asset.sceneName = selection.name;

                EditorUtility.SetDirty(asset);
                
                newObjects[i] = asset;
            }

            return newObjects;
        }
        
        public static SceneLink[] CreateInstances(SceneAsset[] sceneAssets)
        {
            SceneLink[] newObjects = new SceneLink[sceneAssets.Length];
            
            for (int i = 0; i < sceneAssets.Length; i++)
            {
                Object selection = sceneAssets[i];
                SceneLink asset = ScriptableObjectAssetCreator.Create<SceneLink>(selection.name);
                asset.sceneAsset = (SceneAsset) selection;
                asset.scenePath = $"{ScriptableObjectAssetCreator.AssetPath(selection)}\\{asset.sceneAsset.name}";
                asset.sceneName = selection.name;

                EditorUtility.SetDirty(asset);
                
                newObjects[i] = asset;
            }

            return newObjects;
        }
        
        [MenuItem(AssetPathName)]
        private static void CreateInstances()
        {
            Object[] newObjects = CreateInstances(Selection.objects);
            Selection.objects = newObjects;
            AssetDatabase.SaveAssets();
        }

        [MenuItem(AssetPathName, true)]
        private static bool IsSelectionValidate() => 
            ScriptableObjectAssetCreator.Validate<SceneAsset>();

        private static void UpdateLink(SceneLink asset)
        {
            asset.sceneName = asset.sceneAsset.name;
            asset.scenePath = AssetDatabase.GetAssetPath(asset.sceneAsset);
            EditorUtility.SetDirty(asset);
            AssetDatabase.SaveAssets();
            Debug.Log($"SceneLink \"{asset.name}\" was updated!");
        }
        
        [OnOpenAsset(1)]
        private static bool UpdateLink(int instanceID, int line)
        {
            if (!ScriptableObjectAssetCreator.Validate<SceneLink>())
                return false;
            var sceneLink = (SceneLink) Selection.activeObject;
            UpdateLink(sceneLink);
            // EditorSceneManager.OpenScene(sceneLink.scenePath);
            return true;
        }
    }
}