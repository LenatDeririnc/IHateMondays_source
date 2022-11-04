using Plugins.ServiceLocator;
using Services;
using UnityEngine;

namespace Scenes
{
    public class CameraTrigger : MonoBehaviour
    {
        [SerializeField] private ScriptableObjectWithID onEnterCamera;
        [SerializeField] private ScriptableObjectWithID onExitCamera;

        private VirtualCamerasService VirtualCamerasService;

        private void Awake()
        {
            VirtualCamerasService = ServiceLocator.Get<VirtualCamerasService>();
        }

        private void TrySetDefaultCamera()
        {
            var defaultCamera = VirtualCamerasService.GetDefaultCamera();
            
            if (defaultCamera != null)
                defaultCamera.Switch();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (onEnterCamera == null) {
                TrySetDefaultCamera();
                return;
            }

            VirtualCamerasService.GetCamera(onEnterCamera).Switch();
        }

        private void OnTriggerExit(Collider other)
        {
            if (onExitCamera == null) {
                TrySetDefaultCamera();
                return;
            }
            
            VirtualCamerasService.GetCamera(onExitCamera).Switch();
        }
    }
}