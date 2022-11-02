using Plugins.MonoBehHelpers;
using UnityEngine;

namespace Characters.Components
{
    public class PlayerLook : UpdateGetter, ISelfDeps
    {
        [SerializeField] private PlayerBase PlayerBase;
        
        private float _xRotation = 0f;

        private Vector3 _rotateDelta;
        
        public void SetRotateDelta(Vector3 delta)
        {
            _rotateDelta = delta;
        }
        
        private void RotateCamera(Vector3 rotateDelta)
        {
            PlayerBase.PlayerTransform.Rotate(Vector3.up, rotateDelta.x);

            _xRotation -= rotateDelta.y;
            _xRotation = Mathf.Clamp(_xRotation, -90, 90);
        
            PlayerBase.CameraTransform.localRotation = Quaternion.Euler(_xRotation, 0, 0);
        }

        protected override void SentUpdate()
        {
            RotateCamera(_rotateDelta);
        }

        public void SetupDeps()
        {
            PlayerBase = GetComponent<PlayerBase>();
        }
    }
}