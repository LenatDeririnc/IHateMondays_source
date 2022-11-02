using Player;
using Plugins.ServiceLocator;
using Plugins.ServiceLocator.Interfaces;
using UnityEngine;

namespace Services
{
    public class PlayerService : Service, IAwakeService
    {
        public PlayerInfo Player { get; private set; }

        [SerializeField] private Transform defaultPlayerSpawn;
        private GameFactoryService _factoryService;

        public void AwakeService()
        {
            _factoryService = ServiceLocator.Get<GameFactoryService>();
            Player = _factoryService.SpawnPlayer(defaultPlayerSpawn);
        }
    }
}