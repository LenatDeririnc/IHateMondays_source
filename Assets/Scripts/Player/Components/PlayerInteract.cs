using System;
using Plugins.MonoBehHelpers;
using Plugins.ServiceLocator;
using Props;
using Services;
using UnityEngine;

namespace Characters.Components
{
    public class PlayerInteract : UpdateGetter
    {
        [SerializeField] private Transform _raycastForward;
        [SerializeField] private float _maxDistance = 1f;
        [SerializeField] private LayerMask _layerMask;

        private GameService GameService;
        private InputBridgeService InputBridgeService;

        private void Awake()
        {
            ServiceLocator.Get(ref GameService);
            ServiceLocator.Get(ref InputBridgeService);
        }

        protected override void SentUpdate()
        {
            if (!enabled)
                return;
            
            if (GameService.IsPaused)
                return;
            
            if (!InputBridgeService.IsActionDown)
                return;

            var isRaycast = Physics.Raycast(
                _raycastForward.position, 
                _raycastForward.forward, 
                out var hit, 
                _maxDistance, 
                _layerMask);
            
            if (!isRaycast)
                return;

            var interactable = hit.collider.GetComponent<IInteractable>();
            
            if (interactable == null)
                return;

            if (!interactable.IsAvailableToInteract())
                return;
            
            interactable.Interact();
        }
    }
}