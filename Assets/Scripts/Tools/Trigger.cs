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
        private bool _enabled;

        private void Awake()
        {
            var meshRenderer = GetComponent<MeshRenderer>();
            
            if (meshRenderer != null)
                meshRenderer.material.color = Color.clear;
        }

        private void OnEnable()
        {
            _enabled = true;
        }

        private void OnDisable()
        {
            _enabled = false;
            _colliders.Clear();
        }

        private void OnDestroy()
        {
            OnEnter = null;
            OnExit = null;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!_enabled)
                return;
            
            OnEnter?.Invoke(other);
            _colliders.Add(other);
        }

        private void OnTriggerExit(Collider other)
        {
            if (!_enabled)
                return;
            
            OnExit?.Invoke(other);
            if (_colliders.Contains(other))
                _colliders.Remove(other);
        }
    }
}