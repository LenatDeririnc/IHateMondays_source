using Plugins.MonoBehHelpers;
using Props;
using UnityEngine;

namespace Characters.Components
{
    public class PlayerInteract : UpdateGetter
    {
        [SerializeField] private PlayerBase _playerBase;
        [SerializeField] private Transform _raycastForward;
        [SerializeField] private float _maxDistance = 1f;
        [SerializeField] private LayerMask _layerMask;

        protected override void SentUpdate()
        {
            if (!enabled)
                return;
            
            if (_playerBase.GameService.IsPaused)
                return;
            
            if (!_playerBase.InputBridgeService.IsActionDown)
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