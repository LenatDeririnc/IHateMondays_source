using Plugins.MonoBehHelpers;
using UnityEngine;

namespace Characters.Components
{
    public class PlayerLook : UpdateGetter, ISelfDeps
    {
        [SerializeField] private PlayerTransform PlayerTransform;
        [SerializeField] private CameraTransform CameraTransform;
        
        private float _xRotation = 0f;

        private Vector3 _rotateDelta;

        public void SetupDeps()
        {
            PlayerTransform = GetComponent<PlayerTransform>();
            CameraTransform = GetComponent<CameraTransform>();
        }

        public void SetRotateDelta(Vector3 delta)
        {
            _rotateDelta = delta;
        }

        private void RotateCamera(Vector3 rotateDelta)
        {
            PlayerTransform.Value.Rotate(Vector3.up, rotateDelta.x);

            _xRotation -= rotateDelta.y;
            _xRotation = Mathf.Clamp(_xRotation, -90, 90);
        
            CameraTransform.Value.localRotation = Quaternion.Euler(_xRotation, 0, 0);
        }

        protected override void SentUpdate()
        {
            RotateCamera(_rotateDelta);
        }
    }
}