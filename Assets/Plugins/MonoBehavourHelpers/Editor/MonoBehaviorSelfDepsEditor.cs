using Plugins.MonoBehHelpers;
using Plugins.MonoBehSelfDeps;
using UnityEditor;
using UnityEngine;

namespace UnityOverriding.Editor
{
    [CustomEditor(typeof(SelfDepsBase), true)]
    public class MonoBehaviorSelfDepsEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Find Deps"))
            {
                var t = (ISelfDeps)target;
                t.SetupDeps();
                EditorUtility.SetDirty(target);
            }
            
            base.OnInspectorGUI();
        }
    }
}