using System;
using Player;
using Plugins.ServiceLocator;
using Scenes.Props;
using UnityEngine;

namespace Services
{
    public class GameFactoryService : Service
    {
        public enum EntityType
        {
            Default,
            FPSController,
            ZeldaController,
            RunnerController,
        }
        
        [SerializeField] private GameObject PlayerPrefab;
        [SerializeField] private GameObject FPSControllerPrefab;
        [SerializeField] private GameObject ZeldaControllerPrefab;
        [SerializeField] private GameObject RunnerControllerPrefab;
        [SerializeField] private GameObject[] DamageTriggerVariants;
        [SerializeField] [Range(0, 1)] private float _spawnDamageTriggerProbability = .5f;
        
        public PlayerBase SpawnPlayer(Transform spawn)
        {
            GameObject GO = Instantiate(PlayerPrefab, spawn.position, spawn.rotation);
            PlayerBase info = GO.GetComponent<PlayerBase>();
            return info;
        }

        public PlayerBase SpawnPlayer(EntityType type, Transform spawn)
        {
            GameObject GO;
            switch (type) {
                case EntityType.Default:
                    GO = Instantiate(PlayerPrefab, spawn.position, spawn.rotation);
                    break;
                case EntityType.FPSController:
                    GO = Instantiate(FPSControllerPrefab, spawn.position, spawn.rotation);
                    break;
                case EntityType.ZeldaController:
                    GO = Instantiate(ZeldaControllerPrefab, spawn.position, spawn.rotation);
                    break;
                case EntityType.RunnerController:
                    GO = Instantiate(RunnerControllerPrefab, spawn.position, spawn.rotation);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
            PlayerBase info = GO.GetComponent<PlayerBase>();
            return info;
        }
    }
    
    public static class ListObjectsExtensions
    {
        public static T Random<T>(this T[] list)
        {
            return list[UnityEngine.Random.Range(0, list.Length - 1)];
        }
    }
}