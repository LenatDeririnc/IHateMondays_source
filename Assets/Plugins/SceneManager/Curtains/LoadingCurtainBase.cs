using System;
using UnityEngine;

namespace SceneManager
{
    public abstract class LoadingCurtainBase : MonoBehaviour
    {
        [SerializeField] protected float _defaultSpeed;
        
        /// <summary>
        /// Можно ли активировать сцену
        /// Пока не завершен Transition на show, загруженная сцена будет висеть в памяти без активации
        /// Только тогда когда завершен Transition, сцена может загрузиться
        /// </summary>
        public abstract bool CanActivateScene { get; }
        
        /// <summary>
        /// Можно ли начать загрузку сцены
        /// </summary>
        public virtual bool CanLoadScene { get; } = true;
        
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

        public virtual void Reset() 
        {}
    }
}