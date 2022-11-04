using Plugins.MonoBehSelfDeps;
using UnityEngine;
using UnityOverrides;

namespace Characters
{
    public class Character : SelfDepsBase
    {
        [SerializeField] protected CharacterControllerDecorator _characterController;
        [SerializeField] protected float _hp = 100;

        public CharacterControllerDecorator CharacterController => _characterController;


        public override void SetupDeps()
        {
            _characterController = GetComponent<CharacterControllerDecorator>();
        }
    }
}