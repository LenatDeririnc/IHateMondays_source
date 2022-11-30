using System;
using UnityEngine;

namespace SceneManager
{
    public class RoughAlphaCurtain : LoadingCurtainBase
    {
        public override bool CanActivateScene => true;
        [SerializeField] private AlphaCurtain _alphaCurtain;
        public override void Hide(float speed, Action entireEndLoadAction = null)
        {
            _alphaCurtain.Hide(entireEndLoadAction);
        }

        public override void Show(float speed, Action entireEndLoadAction = null)
        {
            entireEndLoadAction?.Invoke();
        }
    }
}