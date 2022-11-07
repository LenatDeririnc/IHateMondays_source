using System.Collections.Generic;
using Plugins.ServiceLocator;
using Plugins.ServiceLocator.Interfaces;
using Scenes.Props;
using UnityEngine;

namespace Services
{
    public class RunnerService : Service, IAwakeService
    {
        [SerializeField] private Paths _paths;
        [SerializeField] private List<Road> _roads;
        [SerializeField] private float _defaultSpeed;
        [SerializeField] private float _slowDownSpeed;
        [SerializeField] private float _destroyDistance = -1f;
        [SerializeField] private float _roadSize = 20f;
        [SerializeField] private float _roadSizeWithoutTriggers = 5f;
        [SerializeField] private int _roadCount = 10;
        [SerializeField] private float _damageSeconds = 5f;
        
        private float _currentSpeed;
        private GameFactoryService _gameFactoryService;

        public float DestroyDistance => _destroyDistance;
        public float CurrentSpeed => _currentSpeed;
        public Paths Paths => _paths;
        public float DamageSeconds => _damageSeconds;

        public void AwakeService()
        {
            _gameFactoryService = ServiceLocator.Get<GameFactoryService>();
            SetDefaultSpeed();
            for (int i = 0; i < _roadCount; i++)
            {
                TryCreateNewRoad();
            }
        }

        public void TryCreateNewRoad()
        {
            var count = _roads.Count;
            var spawnDistance = count * _roadSize;
            bool spawnWithTrigger = _roadSizeWithoutTriggers < count;
            var component = _gameFactoryService.SpawnRoad(Vector3.forward * spawnDistance, spawnWithTrigger);
            component.Construct(this);
            _roads.Add(component);
        }

        public void RemoveFirstRoad()
        {
            _roads.RemoveAt(0);
        }

        public void SetDefaultSpeed()
        {
            _currentSpeed = _defaultSpeed;
        }

        public void SetSlownDownSpeed()
        {
            _currentSpeed = _slowDownSpeed;
        }
    }
}