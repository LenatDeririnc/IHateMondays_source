using Plugins.ServiceLocator;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Services
{
    public class UIFactory : Service
    {
        [SerializeField] private GameObject defaultButtonPrefab;
        
        public Button CreateButton(string labelText, Vector3 position, UnityEngine.Events.UnityAction method, Transform parent = null)
        {
            Button button = Instantiate(defaultButtonPrefab, position, Quaternion.identity, parent).GetComponent<Button>();
            button.onClick.AddListener(method);
            TMP_Text text = button.GetComponentInChildren<TMP_Text>();
            text.text = labelText;
            return button;
        }
    }
}