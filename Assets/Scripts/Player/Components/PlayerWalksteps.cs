using System.Collections;
using Plugins.MonoBehHelpers;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Characters.Components
{
    public class PlayerWalksteps : UpdateGetter, ISelfDeps
    {
        [SerializeField] private PlayerBase PlayerBase;
        [SerializeField] private float walkStepsTime = 1;
        // [SerializeField] [Range(0, 1)] private float volume = 1;
        [SerializeField] [Range(0, 1)] private float walkInputSoundOffsetMovement = 1f;
        [SerializeField] private bool showDebugPlaySteps = false;
        [SerializeField] private bool showDebugVelocityMagnitude = false;

        private Coroutine walkCoroutine;
        
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
            
            float floatrand = Random.value * (PlayerBase.PlayerSound.walkSteps.Length - 1);
            int rand = (int)floatrand;

            var shot = PlayerBase.PlayerSound.walkSteps[rand];
            
            if (shot == null)
                return;
        
            PlayerBase.AudioSourcesService.Sounds.PlayOneShot(shot, PlayerBase.PlayerSound.volume * PlayerBase.PlayerMovement.MoveVector.magnitude);
        }

        public void PlayJump()
        {
            if (!enabled)
                return;
            if (PlayerBase.PlayerSound.Jump != null) PlayerBase.AudioSourcesService.Sounds.PlayOneShot(PlayerBase.PlayerSound.Jump, PlayerBase.PlayerSound.volume);
        }

        public void PlayGrounded()
        {
            if (!enabled)
                return;
            if (PlayerBase.PlayerSound.Grounding != null) PlayerBase.AudioSourcesService.Sounds.PlayOneShot(PlayerBase.PlayerSound.Grounding,  PlayerBase.PlayerSound.volume);
        }

        protected override void SentUpdate()
        {
            if (PlayerBase.GameService.IsPaused)
            {
                EndWalkStepsCoroutine();
                return;
            }
            
            var velocity = PlayerBase.PlayerMovement.MoveVector;
            velocity.y = 0;

            if (showDebugVelocityMagnitude)
            {
                Debug.Log($"Player Velocity: {velocity.magnitude}, grounded: {PlayerBase.PlayerGravityBase.IsGrounded}");
            }
            
            if (velocity.magnitude > walkInputSoundOffsetMovement && PlayerBase.PlayerGravityBase.IsGrounded)
            {
                StartWalkStepsCoroutine();
            }
            else
            {
                EndWalkStepsCoroutine();
            }
        }

        public void SetupDeps()
        {
            PlayerBase = GetComponent<PlayerBase>();
        }
    }
}