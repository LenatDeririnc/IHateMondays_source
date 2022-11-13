using System;
using Player;
using Plugins.ServiceLocator;
using Services;
using Tools;
using UnityEngine;

namespace Scenes.Props
{
    public class Road : MonoBehaviour
    {
        public enum MoveDirection
        {
            Forward,
            Left,
            Right,
        }
        
        [SerializeField] private Transform _transform;
        [SerializeField] private MoveDirection _moveAction;
        [SerializeField] private Trigger _trigger;

        private RunnerService _runerService;
        private PlayerService _playerService;
        private RunnerController _player;

        private void Awake()
        {
            _playerService = ServiceLocator.Get<PlayerService>();
            _runerService = ServiceLocator.Get<RunnerService>();
            _player = _runerService.RunnerController;
            _trigger.OnEnter += OnEnter;
        }

        private void OnEnter(Collider obj)
        {
            if (obj != _playerService.Player.Collider)
                return;

            switch (_moveAction) {
                case MoveDirection.Forward:
                    break;
                case MoveDirection.Left:
                    _player.Rotate(_transform, -90);
                    break;
                case MoveDirection.Right:
                    _player.Rotate(_transform, 90);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            float radius = .5f;
            float lineLength = 3f;
            Transform trans = transform;

            switch (_moveAction) {
                case MoveDirection.Forward:
                    Gizmos.DrawLine(trans.position - trans.forward * lineLength, trans.position + trans.forward * lineLength);
                    Gizmos.DrawWireSphere(trans.position + trans.forward * lineLength, radius);
                    break;
                case MoveDirection.Left:
                    Gizmos.DrawLine(trans.position - trans.forward * lineLength, trans.position - trans.right * lineLength);
                    Gizmos.DrawWireSphere(trans.position - trans.right * lineLength, radius);
                    break;
                case MoveDirection.Right:
                    Gizmos.DrawLine(trans.position - trans.forward * lineLength, trans.position + trans.right * lineLength);
                    Gizmos.DrawWireSphere(trans.position + trans.right * lineLength, radius);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}