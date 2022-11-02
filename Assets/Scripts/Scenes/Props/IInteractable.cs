using UnityEngine;

namespace Props
{
    public interface IInteractable
    {
        public void Interact();

        public bool IsAvailableToInteract();
    }
}