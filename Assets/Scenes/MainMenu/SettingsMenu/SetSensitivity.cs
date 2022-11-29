using System;
using TMPro;
using UnityEngine;

namespace MainMenu.SettingsMenu
{
    public class SetSensitivity : MonoBehaviour
    {
        [SerializeField] private TMP_InputField _inputField;

        public Action<string> OnValueChanged;

        public TMP_InputField InputField => _inputField;

        private void Start()
        {
            _inputField.onValueChanged.AddListener(OnChangeValue);
        }

        private void OnChangeValue(string arg0)
        {
            OnValueChanged?.Invoke(arg0);
        }
    }
}