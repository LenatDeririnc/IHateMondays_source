using System.Linq;
using Plugins.MonoBehHelpers;
using UnityEditor;
using UnityEngine;

namespace ECM.Helpers.Editor
{
    public static class SelfDepsOptions
    {
        [MenuItem("Tools/SelfDeps/Setup Self Deps", false, 0)]
        public static void Setup()
        {
            var selfDeps = Resources.FindObjectsOfTypeAll<MonoBehaviour>().OfType<ISelfDeps>();

            foreach (var sd in selfDeps)
            {
                sd.SetupDeps();
                EditorUtility.SetDirty((MonoBehaviour)sd);
            }
        }
    }
}