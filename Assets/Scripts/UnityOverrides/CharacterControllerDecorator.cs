using System.Collections;
using UnityEngine;

namespace UnityOverrides
{
    public class CharacterControllerDecorator : MonoBehaviour
    {
        [SerializeField] private CharacterController _characterController;

        public CharacterController CharacterController => _characterController;

        [SerializeField] private Transform _characterTransform;
        private bool _isIgnoreFrame;
        private Coroutine _isIgnoreFrameCoroutine;

        public void SetPosition(Vector3 position)
        {
            _characterTransform.position = position;
            WaitOneFrame();
        }

        public void SetRotation(Quaternion rotation)
        {
            _characterTransform.rotation = rotation;
            WaitOneFrame();
        }

        public void Move(Vector3 move)
        {
            if (_isIgnoreFrame)
                return;
            
            _characterController.Move(move);
        }

        private void WaitOneFrame()
        {
            _isIgnoreFrameCoroutine ??= StartCoroutine(IgnoreOneFrame());
        }

        protected IEnumerator IgnoreOneFrame()
        {
            _isIgnoreFrame = true;
            yield return null;
            _isIgnoreFrame = false;
            _isIgnoreFrameCoroutine = null;
        }
    }
}