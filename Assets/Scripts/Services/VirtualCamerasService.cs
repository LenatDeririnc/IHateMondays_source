using System.Collections.Generic;
using Plugins.ServiceLocator;
using Scenes;
using UnityEngine;

namespace Services
{
    public class VirtualCamerasService : Service
    {
        private Dictionary<ScriptableObjectWithID, VirtualCameraObject> _virtualCameras =
            new Dictionary<ScriptableObjectWithID, VirtualCameraObject>();

        [SerializeField] private ScriptableObjectWithID _defaultCamera;

        public VirtualCameraObject GetCamera(ScriptableObjectWithID cameraId)
        {
            if (cameraId == null || !_virtualCameras.ContainsKey(cameraId)) {
                return null;
            }
            return _virtualCameras[cameraId];
        }

        public VirtualCameraObject GetDefaultCamera()
        {
            return GetCamera(_defaultCamera);
        }

        public void AddCamera(VirtualCameraObject obj)
        {
            _virtualCameras.Add(obj.Info, obj);
        }
        
        public void DisableAllCameras()
        {
            foreach (VirtualCameraObject c in _virtualCameras.Values) {
                c.SetLowPriority();
            }
        }
    }
}