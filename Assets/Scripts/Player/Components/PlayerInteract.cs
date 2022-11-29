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

        private GameService _gameService;
        private InputBridgeService _inputBridgeService;


        private bool _isInteractable = false;
        private FungusService _fungusService;
        public bool FoundInteractable => _isInteractable;

        private void Awake()
        {
            _inputBridgeService = ServiceLocator.Get<InputBridgeService>();
            _fungusService = ServiceLocator.Get<FungusService>();
        }

        protected override void SentUpdate()
        {
            _isInteractable = false;
            
            if (!enabled)
                return;
            
            if (_fungusService.IsDialogue)
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

            _isInteractable = true;

            if (!_inputBridgeService.IsActionDown)
                return;

            interactable.Interact();
        }
    }
}