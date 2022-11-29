using Characters.Components;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Plugins.MonoBehHelpers;
using Plugins.ServiceLocator;
using Services;
using UnityEngine;

namespace Player
{
    public class ZeldaController : PlayerBase, ISelfDeps
    {
        public PlayerMovementZelda PlayerMovement;
        public PlayerInput_Zelda PlayerInput;
        public PlayerInteract PlayerInteract;
        [SerializeField] private AudioSource _animatorAudioSource;

        private SceneLoadingService _sceneLoadingService;
        private Sequence _moveSoundStop;

        private void Awake()
        {
            _sceneLoadingService = ServiceLocator.Get<SceneLoadingService>();
            _sceneLoadingService.SceneLoader.OnStartLoad += OnLoading;
        }

        private void OnDestroy()
        {
            _sceneLoadingService.SceneLoader.OnStartLoad -= OnLoading;
        }

        public void OnLoading()
        {
            PlayerMovement.enabled = false;
        }

        public void Update()
        {
            PlayerInput.UpdateInvoke();
            PlayerInteract.UpdateInvoke();
        }

        public void SetupDeps()
        {
            PlayerMovement = GetComponent<PlayerMovementZelda>();
            PlayerInput = GetComponent<PlayerInput_Zelda>();
        }

        public void PlayMoveSound(AudioClip clip)
        {
            _moveSoundStop.Kill();
            _animatorAudioSource.Stop();
            _animatorAudioSource.clip = clip;
            _animatorAudioSource.loop = true;
            _animatorAudioSource.volume = 1.0f;
            _animatorAudioSource.Play();
        }

        public void StopMoveSound()
        {
            _moveSoundStop = DOTween.Sequence();

            TweenerCore<float, float, FloatOptions> turnOffSound = DOTween.To(() => _animatorAudioSource.volume,
                _ => _animatorAudioSource.volume = _,
                0,
                0.1f);

            turnOffSound.onComplete += () => _animatorAudioSource.Stop();

            _moveSoundStop.Append(turnOffSound);
        }
    }
}