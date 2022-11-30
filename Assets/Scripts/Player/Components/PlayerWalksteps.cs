using Characters.Components;
using DG.Tweening;
using Plugins.ServiceLocator;
using Services;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Player.Components
{
    public class PlayerWalksteps : MonoBehaviour
    {
        [SerializeField] AudioClip[] _defaultWalkSteps;
        [SerializeField] AudioClip[] _parsedWalkSteps;
        
        [SerializeField] [Range(0, 1)] private float _maxVolume = 1;

        [SerializeField] private PlayerMovementBase  PlayerMovement;
        [SerializeField] private GroundCheckComponent groundCheckComponent;
        [SerializeField] private AudioSource _walkstepsAudioSouece;
        
        [SerializeField] private float walkStepsTime = 1;
        [SerializeField] [Range(0, 1)] private float walkInputSoundOffsetMovement = 1f;
        
        [SerializeField] private float _volumeDuration = 0.5f;

        private Coroutine walkCoroutine;
        
        private FungusService _fungusService;
        private Sequence _fadeSequence;
        private float _fadeTargetAmount = -1;
        private Sequence _walkstepsIntervalSequence;

        private AudioClip[] CurrentWalkSteps => _parsedWalkSteps is not { Length: > 0 } ? _defaultWalkSteps : _parsedWalkSteps;

        private int currentRandomIndex = 0;
        private int[] randomIndexes;

        private void Awake()
        {
            _fungusService = ServiceLocator.Get<FungusService>();
            _walkstepsAudioSouece.volume = _maxVolume;
        }

        private void WalkStepsStart(float time)
        {
            if (_walkstepsIntervalSequence != null)
                return;

            _walkstepsIntervalSequence = DOTween.Sequence();

            _walkstepsIntervalSequence.AppendInterval(time / 2);
            _walkstepsIntervalSequence.AppendCallback(PlayStep);
            _walkstepsIntervalSequence.AppendInterval(time / 2);
            _walkstepsIntervalSequence.SetLoops(-1);
        }

        private void RegenerateWalkstepsIndexes()
        {
            if (randomIndexes == null) {
                randomIndexes = new int[CurrentWalkSteps.Length];
                for (int i = 0; i < CurrentWalkSteps.Length; i++) {
                    randomIndexes[i] = i;
                }
            }

            for (int i = 0; i < randomIndexes.Length; i++) {
                var randomIndex = (int)Random.value * (randomIndexes.Length - 1);
                (randomIndexes[i], randomIndexes[randomIndex]) = (randomIndexes[randomIndex], randomIndexes[i]);
            }
            
            currentRandomIndex = 0;
        }

        private AudioClip PeekRandomSound()
        {
            if (randomIndexes == null || currentRandomIndex > randomIndexes.Length - 1) {
                RegenerateWalkstepsIndexes();
            }

            return CurrentWalkSteps[randomIndexes[currentRandomIndex++]];
        }

        private void PlayStep()
        {
            var shot = PeekRandomSound();

            if (shot == null)
                return;
        
            _walkstepsAudioSouece.clip = shot;
            _walkstepsAudioSouece.Play();
        }

        protected void Update()
        {
            if (_fungusService.IsDialogue)
            {
                EndWalkStepsCoroutine();
                return;
            }
            
            var velocity = PlayerMovement.MoveDirection;
            velocity.y = 0;

            if (velocity.magnitude > walkInputSoundOffsetMovement && groundCheckComponent.IsGrounded)
            {
                WalkStepsStart(walkStepsTime);
                SetVolume(_maxVolume);
            }
            else
            {
                SetVolume(0, EndWalkStepsCoroutine);
            }
        }

        void SetVolume(float volume, TweenCallback onComplete = null)
        {
            if (!Mathf.Approximately(_fadeTargetAmount, volume)) {
                _fadeSequence.Kill();
                _fadeSequence = DOTween.Sequence()
                    .Join(_walkstepsAudioSouece.DOFade(volume, _volumeDuration));
                _fadeSequence.onComplete += onComplete;
            }
            
            _fadeTargetAmount = volume;
        }

        private void EndWalkStepsCoroutine()
        {
            if (_walkstepsIntervalSequence != null)
                _walkstepsIntervalSequence.Kill();

            _walkstepsIntervalSequence = null;
        }

        public void SetFloorSteps(AudioClip[] clips)
        {
            _parsedWalkSteps = clips;
        }
    }
}