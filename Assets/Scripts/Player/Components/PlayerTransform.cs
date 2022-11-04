using Plugins.MonoBehHelpers;
using UnityEngine;

namespace Characters.Components
{
    public class PlayerTransform : MonoBehaviour, ISelfDeps
    {
        [SerializeField] private Transform value;

        public Transform Value => value;
        public void SetupDeps()
        {
            value = transform;
        }
    }
}