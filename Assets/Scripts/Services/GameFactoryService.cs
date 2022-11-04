using Player;
using Plugins.ServiceLocator;
using UnityEngine;

namespace Services
{
    public class GameFactoryService : Service
    {
        [SerializeField] private GameObject PlayerPrefab;
        
        public PlayerBase SpawnPlayer(Transform spawn)
        {
            var GO = Instantiate(PlayerPrefab, spawn.position, spawn.rotation);
            var info = GO.GetComponent<PlayerBase>();
            return info;
        }
    }
}