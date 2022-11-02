using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace MainMenu.Buttons
{
    public class StartGameButton : MonoBehaviour
    {
        public UnityEvent SceneLoaderAction;
        public Button Button;

        private void Start()
        {
            Button.onClick.AddListener(OnPressButton);
        }

        public void OnPressButton()
        {
            SceneLoaderAction.Invoke();
        }
    }
}