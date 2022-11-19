using System;
using DG.Tweening;
using Plugins.ServiceLocator;
using Scenes.Props;
using Services;
using UnityEngine;

namespace Scenes
{
    public class EndHorrorGameTrigger : MonoBehaviour
    {
        [SerializeField] private LightLamp _lightLamp;
        private PlayerService _playerService;

        private void Awake()
        {
            _playerService = ServiceLocator.Get<PlayerService>();
        }

        private void OnTriggerExit(Collider other)
        {
            var sequence = DOTween.Sequence();
            sequence.Join(_lightLamp.Switch(0));
            sequence.onComplete += Reposition;
        }

        private void Reposition()
        {
            var currentposition = _playerService.Player.GetPosition();
            var direction = (_lightLamp.transform.position - currentposition) / 2;
            _playerService.Player.SetPosition(_lightLamp.transform.position + direction);
            _playerService.Player.SetRotation(Quaternion.LookRotation(direction, Vector3.up));
            _lightLamp.Switch(1);
        }
    }
}