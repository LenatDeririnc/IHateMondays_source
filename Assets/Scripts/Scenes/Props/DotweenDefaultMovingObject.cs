using DG.Tweening;
using UnityEngine;

namespace Scenes.Props
{
    public class DotweenDefaultMovingObject : MovingObjectBase
    {
        [SerializeField] private Transform _from;
        [SerializeField] private Transform _to;
        [SerializeField] private float _duration = 1f;
        [SerializeField] private Ease _ease;

        private bool isStarted = false;

        protected override void Awake()
        {
            base.Awake();
            _transform.position = _from.position;
            _transform.rotation = _from.rotation;
        }

        protected override void MoveProcess()
        {
            if (isStarted)
                return;
            
            _transform.DOMove(_to.position, _duration).SetEase(_ease);
            _transform.DORotate(_to.rotation.eulerAngles, _duration).SetEase(_ease);
            isStarted = true;
        }
    }
}