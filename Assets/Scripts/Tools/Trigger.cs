using System;
using System.Collections.Generic;
using UnityEngine;

namespace Tools
{
    public class Trigger : MonoBehaviour
    {
        public List<Collider> _colliders = new ();
        public List<Collider> Colliders => _colliders;

        public Action<Collider> OnEnter;
        public Action<Collider> OnExit;

        private void OnTriggerEnter(Collider other)
        {
            OnEnter?.Invoke(other);
            _colliders.Add(other);
        }

        private void OnTriggerExit(Collider other)
        {
            OnExit?.Invoke(other);
            if (_colliders.Contains(other))
                _colliders.Remove(other);
        }
    }
}