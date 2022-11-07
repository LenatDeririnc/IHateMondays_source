using Player;
using Plugins.ServiceLocator;
using Plugins.ServiceLocator.Interfaces;
using Scenes;
using UnityEngine;

namespace Services
{
    public class PlayerService : Service, IAwakeService
    {
        [SerializeField] private bool _spawnPlayer = true;
        public PlayerBase Player { get; private set; }

        [SerializeField] private PlayerSpawnPointComponent defaultPlayerSpawn;
        private GameFactoryService _factoryService;

        public void AwakeService()
        {
            _factoryService = ServiceLocator.Get<GameFactoryService>();
            
            if (_spawnPlayer) {
                Player = _factoryService.SpawnPlayer(defaultPlayerSpawn.playerType, defaultPlayerSpawn.transform);
            }
        }
    }
}