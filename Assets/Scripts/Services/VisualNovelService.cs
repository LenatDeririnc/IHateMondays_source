using Fungus;
using Plugins.ServiceLocator;
using Plugins.ServiceLocator.Interfaces;
using UnityEngine;

namespace Services
{
    public class VisualNovelService : Service, IAwakeService
    {
        [SerializeField] private Stage _stage;
        [SerializeField] private Flowchart _flowchart;
        [SerializeField] private string _blockNameStart = "Start";
        public Stage Stage => _stage;
        public void AwakeService()
        {
            _flowchart.ExecuteBlock(_blockNameStart);
        }
    }
}