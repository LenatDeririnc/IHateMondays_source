using MainMenu.SettingsMenu;
using UnityEditor;
using UnityEngine;

namespace Plugins.MainMenu.SettingsMenu.Editor
{
    [CustomEditor(typeof(SetResolution))]
    public class SetResolutionEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            EditorGUILayout.Space();
            var targetObject = (SetResolution) target;
            if (GUILayout.Button("Clear Player Prefs"))
            {
                PlayerPrefs.DeleteKey(SetResolution.PlayerPrefsSelectedScreenKey);
            };
        }
    }
}