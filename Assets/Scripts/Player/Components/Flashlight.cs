using System;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using Random = UnityEngine.Random;

namespace Player.Components
{
    public class Flashlight : MonoBehaviour
    {
        [SerializeField] private GameObject lightGameObject;
        [SerializeField] private Light _flashlight;
        [SerializeField] private float _lerp = 10;

        public bool IsActive = false;

        private float _startIntensivity;

        private float _targetLight = 0;

        private void Awake()
        {
            lightGameObject.SetActive(IsActive);
            _startIntensivity = _flashlight.intensity;
            _targetLight = _startIntensivity;
        }

        public void SetActive(bool value)
        {
            IsActive = value;
            lightGameObject.SetActive(IsActive);
        }

        public void SwitchActive()
        {
            IsActive = !IsActive;
            lightGameObject.SetActive(IsActive);
        }

        private void FixedUpdate()
        {
            _targetLight = Random.Range(0, _startIntensivity);
            _flashlight.intensity = Mathf.Lerp(_flashlight.intensity, _targetLight, _lerp * Time.fixedDeltaTime);
        }
    }
}