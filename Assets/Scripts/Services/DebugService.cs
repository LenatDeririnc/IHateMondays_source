using Plugins.ServiceLocator;
using Plugins.ServiceLocator.Interfaces;
using UnityEngine;

namespace Services
{
    public class DebugService : Service, IUpdateService
    {
        [SerializeField] private int clampedFps;
        private int _previousClampedFps;

        public void UpdateService()
        {
            if (_previousClampedFps != clampedFps)
            {
                Application.targetFrameRate = clampedFps;
                _previousClampedFps = clampedFps;
            }
        }
    }
}