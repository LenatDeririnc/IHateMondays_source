using UnityEngine;

namespace Scenes.Props
{
    public abstract class MovingObjectBase : MonoBehaviour
    {
        protected Transform _transform;
        [SerializeField] protected bool _isMoving;

        protected virtual void Awake()
        {
            _transform = transform;
        }

        protected abstract void MoveProcess();
        
        private void Update()
        {
            if (!_isMoving)
                return;

            MoveProcess();
        }

        public void StartMove()
        {
            _isMoving = true;
        }
    }
}