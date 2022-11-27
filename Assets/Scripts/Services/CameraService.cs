using Cinemachine;
using Plugins.ServiceLocator;
using UnityEngine;

namespace Services
{
    public class CameraService : Service
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private Camera _uiCamera;
        [SerializeField] private CinemachineBrain _brain;

        public Camera Camera => _camera;
        public Camera UICamera => _uiCamera;
        public CinemachineBrain Brain => _brain;
    }
}