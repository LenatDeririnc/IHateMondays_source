using Services;
using UnityEngine;

namespace Scenes.Props
{
    public class Road : MonoBehaviour, IOnDestroyCommand
    {
        [SerializeField] private Transform[] _spawnDamageTriggerPositions;

        public Transform[] SpawnDamageTriggerPositions => _spawnDamageTriggerPositions;

        private RunnerService _service;

        public void Construct(RunnerService service)
        {
            _service = service;
        }

        public void OnDestroyCommand()
        {
            _service.RemoveFirstRoad();
            _service.TryCreateNewRoad();
        }
    }
}