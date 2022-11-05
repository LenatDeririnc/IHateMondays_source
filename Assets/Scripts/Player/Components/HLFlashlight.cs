using Plugins.MonoBehHelpers;
using UnityEngine;

namespace Player.Custom
{
    public class HLFlashlight : UpdateGetter
    {
        [SerializeField] private Transform cameraTransform;
        [SerializeField] private GameObject lightGameObject;
        [SerializeField] private Transform lightTransform;
        [SerializeField] private float normalDistance = 0.5f;

        public bool IsActive = false;

        public void SetActive(bool value)
        {
            IsActive = value;
        }

        public void SwitchActive()
        {
            IsActive = !IsActive;
        }
        
        protected override void SentUpdate()
        {
            if (!IsActive)
            {
                lightGameObject.SetActive(false);
                return;
            }

            if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out var hit, 1000f))
            {
                lightGameObject.SetActive(true);
                lightTransform.position = hit.point + hit.normal * normalDistance;
            }
            else
            {
                lightGameObject.SetActive(false);
            }
        }
    }
}