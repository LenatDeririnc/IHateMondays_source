using System;
using Player;
using Plugins.ServiceLocator;
using Scenes.Props;
using UnityEngine;
using Random = UnityEngine.Random;

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
        [SerializeField] private GameObject[] RoadVariants;
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

        private RunnerDamageTrigger SpawnDamageTrigger(Vector3 position)
        {
            var GO = Instantiate(DamageTriggerVariants.Random(), position, Quaternion.identity);
            RunnerDamageTrigger obj = GO.GetComponent<RunnerDamageTrigger>();
            return obj;
        }

        public Road SpawnRoad(Vector3 position, bool spawnWithDamageTriggers = true)
        {
            var GO = Instantiate(RoadVariants.Random(), position, Quaternion.identity);
            Road road = GO.GetComponent<Road>();

            if (spawnWithDamageTriggers) {
                int spawnCount = 0;

                for (var index = 0; index < road.SpawnDamageTriggerPositions.Length; index++) {
                    Transform pos = road.SpawnDamageTriggerPositions[index];

                    if (!(Random.value <= _spawnDamageTriggerProbability))
                        continue;

                    var damageTrigger = SpawnDamageTrigger(pos.position);
                    spawnCount += 1;

                    if (spawnCount >= road.SpawnDamageTriggerPositions.Length - 1)
                        break;
                }
            }

            return road;
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