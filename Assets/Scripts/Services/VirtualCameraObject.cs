using Cinemachine;
using Plugins.ServiceLocator;
using Scenes;
using UnityEngine;

namespace Services
{
    public class VirtualCameraObject : MonoBehaviour
    {
        [SerializeField] private ScriptableObjectWithID _info;
        [SerializeField] private CinemachineVirtualCamera _virtualCamera;
        private VirtualCamerasService _virtualCamerasService;
        
        public ScriptableObjectWithID Info => _info;

        private void Awake()
        {
            _virtualCamerasService = ServiceLocator.Get<VirtualCamerasService>();
            _virtualCamerasService.AddCamera(this);
        }

        public void Switch()
        {
            _virtualCamerasService.DisableAllCameras();
            _virtualCamera.Priority = 100;
        }

        public void SetLowPriority()
        {
            _virtualCamera.Priority = -1;
        }
    }
}