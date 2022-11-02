using UnityEditor;
using UnityEngine;

namespace UVariableSystem.Editor
{
    [CustomEditor(typeof(UVariable))]
    public class UVariableEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            if (!Application.isPlaying)
                return;

            var uvariable = (UVariable)target;
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Runtime Value: ", uvariable.Value().ToString());
        }
    }
}