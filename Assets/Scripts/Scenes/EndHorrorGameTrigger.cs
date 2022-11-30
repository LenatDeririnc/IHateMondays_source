using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Fungus;
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
        [SerializeField] private AudioSource _source;
        [SerializeField] private AudioClip _spookySaspensSound;
        [SerializeField] private float _startSoundSeconds = 10f;
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

            var entireSequence = DOTween.Sequence();
            
            var turnOffLightSequence = DOTween.Sequence();
            var soundSequence = DOTween.Sequence();

            turnOffLightSequence.AppendInterval(waitInterval);
            turnOffLightSequence.Append(TurnOffLight());
            turnOffLightSequence.onComplete += TurnOffPlayer;

            soundSequence.AppendInterval(_startSoundSeconds);
            soundSequence.AppendCallback(() => _source.PlayOneShot(_spookySaspensSound));
            soundSequence.AppendInterval(_spookySaspensSound.length);

            entireSequence.Join(turnOffLightSequence);
            entireSequence.Join(soundSequence);
            entireSequence.onComplete += NextScene;
        }

        private void TurnOffPlayer()
        {
            _playerService.Player.gameObject.SetActive(false);
        }

        private TweenerCore<float, float, FloatOptions> TurnOffLight()
        {
            canUseReposition = false;
            return _lightLamp.SwitchOff();
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
            sequence.Join(_lightLamp.SwitchOff());
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
            _lightLamp.SwitchOn();
        }
    }
}