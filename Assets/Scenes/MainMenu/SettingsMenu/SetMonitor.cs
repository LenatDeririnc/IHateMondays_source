using MainMenu.Helpers;
using TMPro;
using UnityEngine;

namespace MainMenu.SettingsMenu
{
    public class SetMonitor : MonoBehaviour
    {
        public static string PlayerPrefsSelectedDisplayKey = "UnitySelectMonitor";

        [SerializeField] private int DefaultValue = 0;
        
        [SerializeField] private TMP_Dropdown DropdownMenu;
        [SerializeField] private TMP_Text DropdownMenuText;

        [SerializeField] private DisplayChanger DisplayChanger;
        
        private void Start()
        {
            for (int i = 1; i <= Display.displays.Length; ++i)
            {
                var option = new TMP_Dropdown.OptionData(i.ToString());
                DropdownMenu.options.Add(option);
            }
            
            DropdownMenu.onValueChanged.AddListener(Set);

            if (!PlayerPrefs.HasKey(PlayerPrefsSelectedDisplayKey))
            {
                Set(DefaultValue);
                DropdownMenu.SetValueWithoutNotify(DefaultValue + 1);
                return;
            }

            var display = PlayerPrefs.GetInt(PlayerPrefsSelectedDisplayKey);
            Set(display);
            DropdownMenu.SetValueWithoutNotify(display+1);
        }

        public void Set(int id)
        {
            if (!Screen.fullScreen)
                return;
            
            if (id >= Display.displays.Length)
                return;
            
            PlayerPrefs.SetInt(PlayerPrefsSelectedDisplayKey, id);
            DisplayChanger.SetDisplay(id);
        }
    }
}