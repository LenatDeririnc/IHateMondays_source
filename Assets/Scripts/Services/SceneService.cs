using Plugins.ServiceLocator;
using Scenes;
using UnityEngine;

namespace Services
{
    public class SceneService : Service
    {
        [SerializeField] private PlayerSpawnPointComponent _defaultPlayerSpawn;
        [SerializeField] private PlayerSpawnPointComponent[] _spawns;

        public PlayerSpawnPointComponent GetPlayerSpawnComponent(string id = "")
        {
            if (id.Length <= 0)
                return _defaultPlayerSpawn;

            foreach (PlayerSpawnPointComponent s in _spawns) {
                if (s.SpawnInfo.ID == id)
                    return s;
            }

            return _defaultPlayerSpawn;
        }
    }
}