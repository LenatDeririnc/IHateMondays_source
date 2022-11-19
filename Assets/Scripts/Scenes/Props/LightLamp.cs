﻿using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

namespace Scenes.Props
{
    public class LightLamp : MonoBehaviour
    {
        [SerializeField] private MeshRenderer _meshRenderer;
        [SerializeField] private Light _light;
        [SerializeField] private SwitchLightTrigger _switchLightTrigger;
        [SerializeField] private HorrorPathTrigger _horrorPathTriggerToTurnOff;
        [SerializeField] public GameObject OnEnterTrigger;

        [Space] 
        [SerializeField] private float _maxLightValue;
        [SerializeField][Range(0, 1)] private float _alpha = 1f;
        [SerializeField] private float _switchDuration;

        [Space]
        [SerializeField] private LightLamp _lampToTurnOn;
        [SerializeField] private LightLamp _lampToTurnOff;

        public LightLamp LampToTurnOn => _lampToTurnOn;

        public HorrorPathTrigger HorrorPathTriggerToTurnOff => _horrorPathTriggerToTurnOff;
        public LightLamp LampToTurnOff => _lampToTurnOff;

        private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");
        private Color _defaultColor;

        private void Awake()
        {
            if (OnEnterTrigger != null)
                OnEnterTrigger.SetActive(false);
            
            _light.intensity = _maxLightValue * _alpha;
            _defaultColor = _meshRenderer.material.GetColor(EmissionColor);
            _meshRenderer.material.SetColor(EmissionColor, _defaultColor * _alpha);
        }

        public void Reset()
        {
            HardSwitch(0);
            _switchLightTrigger.Reset();
            if (!_horrorPathTriggerToTurnOff.gameObject.activeSelf)
                _horrorPathTriggerToTurnOff.SetActive(true);
        }

        private void Update()
        {
            _light.intensity = _maxLightValue * _alpha;
            _meshRenderer.material.SetColor(EmissionColor, _defaultColor * _alpha);
        }

        public TweenerCore<float, float, FloatOptions> Switch(float endValue)
        {
            if (Math.Abs(_alpha - endValue) < 0.01) {
                return null;
            }
            return DOTween.To(() => _alpha, _ => _alpha = _, endValue, _switchDuration);
        }

        public void HardSwitch(float value)
        {
            _alpha = value;
        }
    }
}