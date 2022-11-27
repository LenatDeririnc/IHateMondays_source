using System;
using System.Collections;
using Plugins.MonoBehHelpers;
using Plugins.ServiceLocator;
using Services;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Characters.Components
{
    public class PlayerWalksteps : UpdateGetter, ISelfDeps
    {
        [SerializeField] private PlayerSound PlayerSound;
        [SerializeField] private PlayerMovementBase  PlayerMovement;
        [SerializeField] private GroundCheckComponent groundCheckComponent;
        
        [SerializeField] private float walkStepsTime = 1;
        [SerializeField] [Range(0, 1)] private float walkInputSoundOffsetMovement = 1f;
        [SerializeField] private bool showDebugPlaySteps = false;
        [SerializeField] private bool showDebugVelocityMagnitude = false;

        private Coroutine walkCoroutine;
        
        private SoundService _soundService;
        private GameService GameService;

        private void Awake()
        {
            GameService = ServiceLocator.Get<GameService>();
            _soundService = ServiceLocator.Get<SoundService>();
        }

        public void SetupDeps()
        {
            PlayerSound = GetComponent<PlayerSound>();
            PlayerMovement = GetComponent<PlayerMovementBase>();
            groundCheckComponent = GetComponent<GroundCheckComponent>();
        }

        private void StartWalkStepsCoroutine()
        {
            if (walkCoroutine != null)
                return;
            
            walkCoroutine = StartCoroutine(WalkStepsCoroutine(walkStepsTime));
        }

        private void EndWalkStepsCoroutine()
        {
            if (walkCoroutine == null)
                return;
            
            StopCoroutine(walkCoroutine);
            walkCoroutine = null;
        }

        private IEnumerator WalkStepsCoroutine(float time)
        {
            yield return new WaitForSeconds(time / 2);
            PlayStep();
            yield return new WaitForSeconds(time / 2);
            EndWalkStepsCoroutine();
        }

        private void PlayStep()
        {
            if (showDebugPlaySteps)
                Debug.Log("Step");
            
            float floatrand = Random.value * (PlayerSound.walkSteps.Length - 1);
            int rand = (int)floatrand;

            var shot = PlayerSound.walkSteps[rand];
            
            if (shot == null)
                return;
        
            _soundService.Sounds.PlayOneShot(shot, PlayerSound.volume * PlayerMovement.MoveDirection.magnitude);
        }

        public void PlayJump()
        {
            if (!enabled)
                return;
            if (PlayerSound.Jump != null) _soundService.Sounds.PlayOneShot(PlayerSound.Jump, PlayerSound.volume);
        }

        public void PlayGrounded()
        {
            if (!enabled)
                return;
            if (PlayerSound.Grounding != null) _soundService.Sounds.PlayOneShot(PlayerSound.Grounding,  PlayerSound.volume);
        }

        protected override void SentUpdate()
        {
            if (GameService.IsPlayingDialogue)
            {
                EndWalkStepsCoroutine();
                return;
            }
            
            var velocity = PlayerMovement.MoveDirection;
            velocity.y = 0;

            if (showDebugVelocityMagnitude)
            {
                Debug.Log($"Player Velocity: {velocity.magnitude}, grounded: {groundCheckComponent.IsGrounded}");
            }
            
            if (velocity.magnitude > walkInputSoundOffsetMovement && groundCheckComponent.IsGrounded)
            {
                StartWalkStepsCoroutine();
            }
            else
            {
                EndWalkStepsCoroutine();
            }
        }
    }
}