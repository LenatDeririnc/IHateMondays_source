using System;
using DG.Tweening;
using UnityEngine;

namespace Scenes.Props
{
    public class LightLamp : MonoBehaviour
    {
        [SerializeField] private MeshRenderer _meshRenderer;
        [SerializeField] private Light[] _light;
        [SerializeField] private SwitchLightTrigger _switchLightTrigger;
        [SerializeField] private HorrorPathTrigger _horrorPathTriggerToTurnOff;
        [SerializeField] public GameObject OnEnterTrigger;
        [SerializeField] private AudioSource _source;
        [SerializeField] private AudioClip _enableSound;

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
        
        MaterialPropertyBlock _block;

        private float Alpha
        {
            get => _alpha;
            set
            {
                if (IsAlphaSame(value))
                    return;
                
                _alpha = value;
                UpdateLight();
            }
        }

        private bool IsAlphaSame(float value)
        {
            return Math.Abs(_alpha - value) < 0.001;
        }

        private void Awake()
        {
            if (OnEnterTrigger != null)
                OnEnterTrigger.SetActive(false);

            _block = new MaterialPropertyBlock();
            _defaultColor = _meshRenderer.sharedMaterial.GetColor(EmissionColor);

            _maxLightValue = _light[0].intensity;
            
            Reset();
            UpdateLight();
        }

        public void Reset()
        {
            HardSwitch(0);
            _switchLightTrigger.Reset();
            if (!_horrorPathTriggerToTurnOff.gameObject.activeSelf)
                _horrorPathTriggerToTurnOff.SetActive(true);
        }
        
        private void UpdateLight()
        {
            foreach (var l in _light) {
                l.intensity = _maxLightValue * _alpha;
            }

            _block.SetColor(EmissionColor, _defaultColor * _alpha);
            _meshRenderer.SetPropertyBlock(_block);
        }

        public Tween Switch(float endValue)
        {
            if (IsAlphaSame(endValue)) {
                return DOTween.Sequence();
            }

            return DOTween.To(() => Alpha, _ => Alpha = _, endValue, _switchDuration);
        }

        public Tween SwitchOn()
        {
            _source.PlayOneShot(_enableSound);
            return Switch(1);
        }

        public Tween SwitchOff()
        {
            return Switch(0);
        }

        public void HardSwitch(float value)
        {
            Alpha = value;
        }
    }
}