using Plugins.MonoBehHelpers;
using UnityEngine;

namespace Characters.Components
{
    internal class CameraTransform : MonoBehaviour, ISelfDeps
    {
        [SerializeField] private Transform value;
        public Transform Value => value;

        public void SetupDeps()
        {
            value = GetComponentInChildren<Camera>().transform;
        }
    }
}