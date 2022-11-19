using System;
using Plugins.MonoBehHelpers;
using Plugins.ServiceLocator;
using Services;
using UnityEngine;

namespace Characters.Components
{
    public class PlayerLook : UpdateGetter, ISelfDeps
    {
        [SerializeField] private PlayerForwardTransform playerForwardTransform;
        [SerializeField] private CameraTransform CameraTransform;
        
        private float _xRotation = 0f;

        private Vector3 _rotateDelta;

        private InputBridgeService InputBridgeService;
        
        private void Awake()
        {
            ServiceLocator.Get(ref InputBridgeService);
        }

        public void SetupDeps()
        {
            playerForwardTransform = GetComponent<PlayerForwardTransform>();
            CameraTransform = GetComponent<CameraTransform>();
        }

        public void SetRotateDelta(Vector3 delta)
        {
            _rotateDelta = delta;
        }

        private void RotateCamera(Vector3 rotateDelta)
        {
            playerForwardTransform.Value.Rotate(Vector3.up, rotateDelta.x);

            _xRotation -= rotateDelta.y;
            _xRotation = Mathf.Clamp(_xRotation, -90, 90);
        
            CameraTransform.Value.localRotation = Quaternion.Euler(_xRotation, 0, 0);
        }

        public void SetRotation(float x, float y)
        {
            playerForwardTransform.Value.rotation = Quaternion.AngleAxis(y, Vector3.up);
            _xRotation = x;
            CameraTransform.Value.localRotation = Quaternion.Euler(_xRotation, 0, 0);
        }

        protected override void SentUpdate()
        {
            RotateCamera(_rotateDelta);
        }
    }
}