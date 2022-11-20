using DG.Tweening;
using Plugins.ServiceLocator;
using SceneManager.ScriptableObjects;
using Services;
using UnityEngine;

namespace Scenes
{
    public class RunnerEndTrigger : MonoBehaviour
    {
        [SerializeField] private BusAnimation _busAnimation;
        private PlayerService _playerService;
        private RunnerService _runnerService;
        [SerializeField] private float _runEndDuration = 1;
        [SerializeField] private Ease _runEndEase;
        [SerializeField] private SceneLink _nextScene;
        private SceneLoadingService _sceneLoadingService;

        private void Awake()
        {
            _playerService = ServiceLocator.Get<PlayerService>();
            _runnerService = ServiceLocator.Get<RunnerService>();
            _sceneLoadingService = ServiceLocator.Get<SceneLoadingService>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other != _playerService.Player.Collider)
                return;
            
            var busSequence = _busAnimation.StartMove();

            var runSequence = DOTween.Sequence();
            runSequence.AppendInterval(0.5f);
            runSequence.Append(DOTween.To(() => _runnerService.CurrentSpeed, _ => _runnerService.SetCurrentSpeed(_), 0,
                _runEndDuration).SetEase(_runEndEase));
            runSequence.AppendInterval(2f);
            runSequence.onComplete += () => _sceneLoadingService.LoadScene(_nextScene);

            busSequence.Join(runSequence);
        }
    }
}