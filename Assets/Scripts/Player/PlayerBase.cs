using UnityEngine;

namespace Player
{
    public abstract class PlayerBase : MonoBehaviour
    {
        [SerializeField] private Collider _collider;
        public Collider Collider => _collider;

        public virtual void ReceiveDamage()
        {
        }

        public virtual void SetPositionAndRotation(Transform positionToRespawn) {}
        public virtual void SetPosition(Vector3 position) {}
        public virtual void SetRotation(Quaternion rotation) {}

        public virtual Vector3 GetPosition() => transform.position;
    }
}