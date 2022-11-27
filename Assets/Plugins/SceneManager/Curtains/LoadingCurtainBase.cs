using System;
using UnityEngine;

namespace SceneManager
{
    public abstract class LoadingCurtainBase : MonoBehaviour
    {
        public abstract CurtainType Type { get; }
        [SerializeField] protected float _defaultSpeed;
        
        public abstract void Hide(float speed, Action entireEndLoadAction = null);
        public abstract void Show(float speed, Action entireEndLoadAction = null);

        public void Hide(Action entireEndLoadAction = null)
        {
            Hide(_defaultSpeed, entireEndLoadAction);
        }
        
        public void Show(Action entireEndLoadAction = null)
        {
            Show(_defaultSpeed, entireEndLoadAction);
        }
    }
}