using MainMenu.SettingsMenu;
using UnityEditor;
using UnityEngine;

namespace Plugins.MainMenu.SettingsMenu.Editor
{
    [CustomEditor(typeof(SetMonitor))]
    public class SetMonitorEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            EditorGUILayout.Space();
            var targetObject = (SetMonitor) target;
            if (GUILayout.Button("Clear Player Prefs"))
            {
                PlayerPrefs.DeleteKey(SetMonitor.PlayerPrefsSelectedDisplayKey);
            };
        }
    }
}