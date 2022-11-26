using Props;
using UnityEngine;

namespace Fungus
{
    public class InteractableFungus : MonoBehaviour, IInteractable
    {
        [SerializeField] private Flowchart _flowchart;
        [SerializeField] private string _blockName;
        
        public void Interact()
        {
            _flowchart.ExecuteBlock(_blockName);
        }

        public bool IsAvailableToInteract()
        {
            return true;
        }
    }
}