using System;
using Characters.Components;
using DG.Tweening;
using Plugins.ServiceLocator;
using Services;
using UnityEngine;
using UnityEngine.UI;

namespace Player.Components
{
    public class Crosshair : MonoBehaviour
    {
        private enum CurrentCrosshairState
        {
            Disabled,
            Enabled,
            EnabledLighted,
        }

        [SerializeField] private PlayerInteract _interact;
        [SerializeField] private Image _crosshairImage;
        [SerializeField] private CanvasGroup _crosshair;
        [SerializeField] private CanvasGroup _lighting;
        [SerializeField] private float _updateDuration = 1f;
        [SerializeField] private Color _interactableColor = Color.yellow;
        private GameService _gameService;

        private float _corsshairAlphaTarget = 1;
        private float _lightingAlphaTarget = 0;

        private CurrentCrosshairState _currentCrosshairState;
        private Color _currentColor;
        private FungusService _fungusService;

        private CurrentCrosshairState CrosshairState
        {
            get => _currentCrosshairState;
            set
            {
                if (_currentCrosshairState != value)
                    SetCrosshairState(value);
                _currentCrosshairState = value;
            }
        }

        private void Awake()
        {
            _gameService = ServiceLocator.Get<GameService>();
            _fungusService = ServiceLocator.Get<FungusService>();

            _crosshairImage.color = Color.white;
            _crosshair.alpha = 0;
            _lighting.alpha = 0;
        }

        private void Update()
        {
            if (_gameService.IsPaused || _fungusService.IsDialogue) {
                CrosshairState = CurrentCrosshairState.Disabled;
                return;
            }

            if (_interact.FoundInteractable) {
                CrosshairState = CurrentCrosshairState.EnabledLighted;
                return;
            }

            CrosshairState = CurrentCrosshairState.Enabled;
        }

        private void SetCrosshairState(CurrentCrosshairState state)
        {
            switch (state) {
                case CurrentCrosshairState.Disabled:
                    _currentColor = Color.white;
                    _corsshairAlphaTarget = 0;
                    _lightingAlphaTarget = 0;
                    break;
                case CurrentCrosshairState.Enabled:
                    _currentColor = Color.white;
                    _corsshairAlphaTarget = 0.5f;
                    _lightingAlphaTarget = 0;
                    break;
                case CurrentCrosshairState.EnabledLighted:
                    _currentColor = _interactableColor;
                    _corsshairAlphaTarget = 1;
                    _lightingAlphaTarget = 1;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
            UpdateUI();
        }

        public void UpdateUI()
        {
            DOTween.To(() => _crosshairImage.color, _ => _crosshairImage.color = _, _currentColor, _updateDuration);
            DOTween.To(() => _crosshair.alpha, _ => _crosshair.alpha = _, _corsshairAlphaTarget, _updateDuration);
            DOTween.To(() => _lighting.alpha, _ => _lighting.alpha = _, _lightingAlphaTarget, _updateDuration);
        }
    }
}