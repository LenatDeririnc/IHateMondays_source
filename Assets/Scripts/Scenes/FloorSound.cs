using System;
using Plugins.ServiceLocator;
using Services;
using UnityEngine;

namespace Scenes
{
    public class FloorSound : MonoBehaviour
    {
        [SerializeField] private AudioClip[] _clips;
        private PlayerService _playerService;

        private void Awake()
        {
            _playerService = ServiceLocator.Get<PlayerService>();
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.collider != _playerService.Player.Collider)
                return;

            _playerService.Player.TrySetFloorSteps(_clips);
        }
    }
}