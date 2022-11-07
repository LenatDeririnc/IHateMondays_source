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
    }
}