using System;
using UnityEngine;
using UnityOverrides;

namespace Player.Components
{
    public class CharacterControllerAccelerator : MonoBehaviour
    {
        [SerializeField] protected CharacterControllerDecorator characterController;
        [SerializeField] protected float Sensitivity = 1;
        [SerializeField] protected float Gravity = 0.5f;
        private Vector3 _input;
        private Vector3 _currentInput;

        public CharacterController CharacterController => characterController.CharacterController;

        public void Move(Vector3 input)
        {
            _input = input;
        }

        private void Update()
        {
            var modifier = _input.magnitude > _currentInput.magnitude ? Sensitivity : Gravity;
            modifier *= Time.deltaTime;
            
            _currentInput = new Vector3(
                Mathf.MoveTowards(_currentInput.x, _input.x, modifier),
                Mathf.MoveTowards(_currentInput.y, _input.y, modifier),
                Mathf.MoveTowards(_currentInput.z, _input.z, modifier)
            );
            
            characterController.Move(_currentInput);
            _input = Vector3.zero;
        }
    }
}