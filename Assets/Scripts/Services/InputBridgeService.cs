using System.Globalization;
using MainMenu.SettingsMenu;
using Plugins.ServiceLocator;
using Plugins.ServiceLocator.Interfaces;
using UnityEngine;

namespace Services
{
    public class InputBridgeService : Service, IUpdateService, IAwakeService
    {
        private MainMenuSceneService _mainMenuSceneService;
        private static string MouseSensitivityKey => "MouseSensitivity";
        
        public float DefaultSensitivity = 1;
        
        public float LookSensitivity;

        public Vector2 Movement { get; private set; }
        public Vector2 DPad { get; private set; }
        public Vector2 Look { get; private set; }
        public bool IsPauseButtonDown { get; private set; }
        public bool IsJumpUp { get; private set; }
        public bool IsJumpDown { get; private set; }
        public bool IsActionDown { get; private set; }
        
        public bool IsFlashlightDown { get; private set; }
        public bool LeftMoveIsDown { get; set; }
        public bool RightMoveIsDown { get; set; }

        public void InitSensitivityOption(SetSensitivity setSensitivityOption)
        {
            setSensitivityOption.OnValueChanged += OnSensitivityMenuOnValueChanged;
            SetupPlayerPrefsSensitivity();
            setSensitivityOption.InputField.text = LookSensitivity.ToString(CultureInfo.InvariantCulture);
        }

        private void SetupPlayerPrefsSensitivity()
        {
            if (PlayerPrefs.HasKey(MouseSensitivityKey)) {
                LookSensitivity = PlayerPrefs.GetFloat(MouseSensitivityKey);
                return;
            }

            PlayerPrefs.SetFloat(MouseSensitivityKey, DefaultSensitivity);
            LookSensitivity = DefaultSensitivity;
        }

        private void OnSensitivityMenuOnValueChanged(string _)
        {
            LookSensitivity = float.Parse(_);
            PlayerPrefs.SetFloat(MouseSensitivityKey, LookSensitivity);
        }

        public void SetCursorLocked(bool value)
        {
            Cursor.lockState = value ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !value;
        }

        public void AwakeService()
        {
            SetupPlayerPrefsSensitivity();
        }

        public void UpdateService()
        {
            Movement = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            DPad = new Vector2(Input.GetAxis("DPad X"), Input.GetAxis("DPad Y"));
            Look = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * LookSensitivity;
            IsPauseButtonDown = Input.GetKeyDown(KeyCode.Escape);
            IsJumpDown = Input.GetButtonDown("Jump");
            IsJumpUp = Input.GetButtonUp("Jump");
            IsActionDown = Input.GetButtonDown("Fire1");
            IsFlashlightDown = Input.GetButtonDown("Fire2");
            LeftMoveIsDown = Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow);
            RightMoveIsDown = Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow);
        }
    }
}