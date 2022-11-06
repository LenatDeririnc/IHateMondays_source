using Plugins.ServiceLocator;
using Scenes.Props;
using UnityEngine;

namespace Services
{
    public class RoadsService : Service
    {
        [SerializeField] private Roads _roads;

        public Roads Roads => _roads;
    }
}