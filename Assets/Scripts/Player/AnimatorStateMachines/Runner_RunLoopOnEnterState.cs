using Plugins.ServiceLocator;
using Services;
using UnityEngine;

namespace Player.AnimatorStateMachines
{
    public class Runner_RunLoopOnEnterState : StateMachineBehaviour
    {
        [SerializeField] private AudioClip _clip;
        private RunnerService _runnerService;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _runnerService = ServiceLocator.Get<RunnerService>();
            _runnerService.RunnerController.PlayMoveSound(_clip);
        }
        
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _runnerService.RunnerController.StopMoveSound();
        }
    }
}