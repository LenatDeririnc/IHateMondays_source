using UnityEngine;
using UnityEngine.UI;

namespace MainMenu.Buttons
{
    public class SwitchScreenButton : MonoBehaviour
    {
        [SerializeField] private CanvasScreen _nextScreen;
        [SerializeField] private Button _button;

        CanvasScreen _thisScreen;

        private void Start() 
        {
            if (_thisScreen == null) _thisScreen = GetComponentInParent<CanvasScreen>();
            _button.onClick.AddListener(SwitchPanel);
        }

        private void SwitchPanel()
        {
            _thisScreen.SetActive(false);
            _nextScreen.SetActive(true);
        }
    }
}