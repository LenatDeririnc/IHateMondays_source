using UnityEditor;
using UnityEngine;

namespace Scenes.Editor
{
    [CustomEditor(typeof(ScriptableObjectWithID), true), CanEditMultipleObjects]
    public class ObjectIDInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Regenerate ID"))
            {
                var t = (ScriptableObjectWithID)target;
                t.RegenerateID();
                EditorUtility.SetDirty(target);
            }
            
            base.OnInspectorGUI();
        }
    }
}