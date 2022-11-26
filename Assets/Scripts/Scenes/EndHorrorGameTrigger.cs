using DG.Tweening;
using Plugins.ServiceLocator;
using SceneManager.ScriptableObjects;
using Scenes.Props;
using Services;
using UnityEngine;

namespace Scenes
{
    public class EndHorrorGameTrigger : MonoBehaviour
    {
        [SerializeField] private LightLamp _lightLamp;
        [SerializeField] private float waitInterval = 5f;
        [SerializeField] private float _loadSceneInterval = 5f;
        [SerializeField] private SceneLink _nextScene;
        private PlayerService _playerService;
        private bool canUseReposition = true;
        private SceneLoadingService _sceneService;

        private void Awake()
        {
            _playerService = ServiceLocator.Get<PlayerService>();
            _sceneService = ServiceLocator.Get<SceneLoadingService>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_playerService.Player.Collider != other)
                return;
            
            var sequence = DOTween.Sequence();
            sequence.AppendInterval(waitInterval);
            sequence.onComplete += TurnOffLight;
        }

        private void TurnOffLight()
        {
            canUseReposition = false;
            var sequence = DOTween.Sequence();
            sequence.Join(_lightLamp.Switch(0));
            sequence.onComplete += NextScene;
        }

        private void NextScene()
        {
            var sequence = DOTween.Sequence();
            sequence.AppendInterval(_loadSceneInterval);
            sequence.onComplete += () => _sceneService.LoadScene(_nextScene);
        }

        private void OnTriggerExit(Collider other)
        {
            if (_playerService.Player.Collider != other)
                return;
            
            var sequence = DOTween.Sequence();
            sequence.Join(_lightLamp.Switch(0));
            sequence.onComplete += Reposition;
        }

        private void Reposition()
        {
            if (!canUseReposition)
                return;
            
            var currentposition = _playerService.Player.GetPosition();
            var direction = (_lightLamp.transform.position - currentposition) / 2;
            _playerService.Player.SetPosition(_lightLamp.transform.position + direction);
            _playerService.Player.SetRotation(Quaternion.LookRotation(direction, Vector3.up));
            _lightLamp.Switch(1);
        }
    }
}