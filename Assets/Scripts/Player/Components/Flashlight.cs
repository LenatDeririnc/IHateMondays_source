using UnityEngine;

namespace Player.Custom
{
    public class Flashlight : MonoBehaviour
    {
        [SerializeField] private GameObject lightGameObject;

        public bool IsActive = false;

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
    }
}