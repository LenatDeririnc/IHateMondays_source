using UnityEngine;
using UnityEngine.UI;

namespace MainMenu.Buttons
{
    public class ExitGameButton : MonoBehaviour
    {
        [SerializeField] private Button Button;

        private void Start()
        {
            Button.onClick.AddListener(Exit);
        }

        public void Exit()
        {
            Application.Quit();
        }
    }
}