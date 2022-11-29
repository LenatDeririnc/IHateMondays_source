using Characters.Components;
using Plugins.ServiceLocator;
using Services;
using UnityEngine;

namespace Player.Components
{
    public class InteractLamp : MonoBehaviour
    {
        [SerializeField] private PlayerInteract _interact;
        [SerializeField] private GameObject _lampInteract;
        
        private bool _currentCrosshairState;
        private FungusService _fungusService;

        private bool CrosshairState
        {
            get => _currentCrosshairState;
            set
            {
                if (_currentCrosshairState != value)
                    SetCrosshairState(value);
                _currentCrosshairState = value;
            }
        }

        private void SetCrosshairState(bool value)
        {
            _lampInteract.SetActive(value);
        }
        
        private void Awake()
        {
            _fungusService = ServiceLocator.Get<FungusService>();
            SetCrosshairState(false);
        }

        private void Update()
        {
            if (!_interact.FoundInteractable) {
                CrosshairState = false;
                return;
            }
            
            if (_fungusService.IsDialogue) {
                CrosshairState = false;
                return;
            }

            CrosshairState = true;
        }
    }
}