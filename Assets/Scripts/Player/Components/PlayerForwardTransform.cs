using Plugins.MonoBehHelpers;
using UnityEngine;

namespace Characters.Components
{
    public class PlayerForwardTransform : MonoBehaviour, ISelfDeps
    {
        [SerializeField] private Transform value;

        public Transform Value => value;
        public void SetupDeps()
        {
            value = transform;
        }
    }
}