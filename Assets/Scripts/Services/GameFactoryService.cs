using Player;
using Plugins.ServiceLocator;
using UnityEngine;

namespace Services
{
    public class GameFactoryService : Service
    {
        [SerializeField] private GameObject PlayerPrefab;
        
        public PlayerInfo SpawnPlayer(Transform spawn)
        {
            var GO = Instantiate(PlayerPrefab, spawn.position, spawn.rotation);
            var info = GO.GetComponent<PlayerInfo>();
            return info;
        }
    }
}