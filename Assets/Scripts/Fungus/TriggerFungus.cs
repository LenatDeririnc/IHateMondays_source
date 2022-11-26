using Fungus;
using Plugins.ServiceLocator;
using Services;
using UnityEngine;

namespace Props
{
    public class TriggerFungus : MonoBehaviour
    {
        [SerializeField] private Flowchart _flowchart;
        [SerializeField] private string _blockName;
        [SerializeField] private bool _playOnce = false;
        
        private PlayerService _playerService;
        private bool _isPlayed;

        private void Awake()
        {
            _playerService = ServiceLocator.Get<PlayerService>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_playerService.Player.Collider != other)
                return;
            
            if (_playOnce && _isPlayed)
                return;
            
            _flowchart.ExecuteBlock(_blockName);
            _isPlayed = true;
        }
    }
}