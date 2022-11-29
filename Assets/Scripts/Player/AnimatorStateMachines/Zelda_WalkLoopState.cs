using Plugins.ServiceLocator;
using Services;
using UnityEngine;

namespace Player.AnimatorStateMachines
{
    public class Zelda_WalkLoopState : StateMachineBehaviour
    {
        [SerializeField] private AudioClip _clip;
        private ZeldaService _runnerService;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _runnerService = ServiceLocator.Get<ZeldaService>();
            _runnerService.Controller.PlayMoveSound(_clip);
        }
        
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _runnerService.Controller.StopMoveSound();
        }
    }
}