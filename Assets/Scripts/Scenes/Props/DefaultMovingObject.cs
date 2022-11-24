using UnityEngine;

namespace Scenes.Props
{
    public class DefaultMovingObject : MovingObjectBase
    {
        [SerializeField] protected float _speed;
        [SerializeField] private Transform _direction;

        protected override void MoveProcess()
        {
            _transform.position += _direction.forward * (Time.deltaTime * _speed);
        }
    }
}