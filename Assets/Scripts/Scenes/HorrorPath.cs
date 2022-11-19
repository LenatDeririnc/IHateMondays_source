using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Plugins.ServiceLocator;
using Scenes.Props;
using Services;
using Tools.ReadOnlyAttribute;
using UnityEngine;

namespace Scenes
{
    public class HorrorPath : MonoBehaviour
    {
        [ReadOnly][SerializeField] private List<HorrorPathTrigger> _currentPathEntered = new List<HorrorPathTrigger>();
        [SerializeField] private EnterLightTriggerZone _enterTrigger;
        [SerializeField] private LightLamp[] _lamps;
        private HorrorService _horrorService;

        public List<HorrorPathTrigger> CurrentPathEntered
        {
            get => _currentPathEntered;
            set { _currentPathEntered = value; }
        }

        public LightLamp[] Lamps => _lamps;

        private void Awake()
        {
            _horrorService = ServiceLocator.Get<HorrorService>();
        }

        public void CheckReset()
        {
            if (_currentPathEntered.Count > 0)
                return;

            TurnLightsOffAndResetAll();
        }
        
        void TurnLightsOffAndResetAll()
        {
            Sequence sequence = DOTween.Sequence();
            foreach (var l in _lamps) {
                var s = l.Switch(0);
                if (s != null)
                    sequence.Join(s);
            }
            sequence.onComplete += _horrorService.ResetAll;
        }

        public void Reset()
        {
            _currentPathEntered.Clear();
            _enterTrigger.Reset();
            foreach (var l in _lamps) {
                l.Reset();
            }
        }
    }
}