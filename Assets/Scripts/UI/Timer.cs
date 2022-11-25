using System;
using TMPro;
using UnityEngine;

namespace UI
{
    public class Timer : MonoBehaviour
    {
        [SerializeField] private TMP_Text _counterText;
        [SerializeField] private float _milisecondsStart;
        [SerializeField] private float _secondsStart;
        [SerializeField] private float _minutesStart;
        private float _timeRemaining;
        public Action OnEndTimer;
        private bool isPlaying = false;

        public void StartTimer()
        {
            isPlaying = true;
        }

        private void Awake()
        {
            _timeRemaining = _milisecondsStart / 1000 + _secondsStart + _minutesStart * 60;
            UpdateTimerUI();
        }

        private void UpdateTimerUI()
        {
            int milliseconds = (int)(_timeRemaining * 1000f) % 1000;
            int seconds = (int)(_timeRemaining % 60);
            int minutes = (int)((_timeRemaining / 60) % 60);

            _counterText.text = $"{minutes:D2}:{seconds:D2}:{milliseconds:D3}";
        }

        private void Update()
        {
            if (!isPlaying)
                return;
            
            _timeRemaining -= Time.deltaTime;
            if (_timeRemaining <= 0) {
                OnEndTimer?.Invoke();
                _timeRemaining = 0;
                isPlaying = false;
            }
            UpdateTimerUI();
        }
    }
}