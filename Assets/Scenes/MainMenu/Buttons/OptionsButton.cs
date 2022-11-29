using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace MainMenu.Buttons
{
    public class OptionsButton : MonoBehaviour
    {
        public UnityEvent OpenOptionsAction;
        public UnityEvent CloseOptionsAction;
        public Button OpenOptionsButton;
        public Button CloseOptionsButton;

        private void Start()
        {
            OpenOptionsButton.onClick.AddListener(OnPressOpenButton);
            CloseOptionsButton.onClick.AddListener(OnPressCloseButton);
        }

        public void OnPressOpenButton()
        {
            OpenOptionsAction.Invoke();
        }

        public void OnPressCloseButton()
        {
            CloseOptionsAction.Invoke();
        }
    }
}
