using Plugins.ServiceLocator;
using Services;
using UnityEngine;

namespace Scenes.Props
{
    public class MovingRunnerObject : MonoBehaviour
    {
        [SerializeField] private Transform _transform;
        private RunnerService _runnerService;
        private IOnDestroyCommand[] _onDestroyCommands;
        
        private void Awake()
        {
            _runnerService = ServiceLocator.Get<RunnerService>();
            _onDestroyCommands = GetComponents<IOnDestroyCommand>();
        }

        private void Update()
        {
            _transform.position += Vector3.back * (_runnerService.CurrentSpeed * Time.deltaTime);

            if (!(_transform.position.z < _runnerService.DestroyDistance))
                return;

            foreach (var onDestroy in _onDestroyCommands) {
                onDestroy.OnDestroyCommand();
            }
            
            Destroy(gameObject);
        }
    }
}