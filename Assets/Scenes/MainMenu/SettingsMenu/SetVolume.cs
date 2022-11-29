using UnityEngine;
using UnityEngine.UI;

namespace MainMenu.SettingsMenu
{
    public class SetVolume : MonoBehaviour
    {
        private static string PlayerPrefsVolumeKey = "GameVolume";

        [SerializeField] private Slider Slider;

        private void Start()
        {
            Slider.onValueChanged.AddListener(Set);
            
            if (!PlayerPrefs.HasKey(PlayerPrefsVolumeKey))
            {
                Set(1);
                return;
            }

            var currentSliderValue = PlayerPrefs.GetFloat(PlayerPrefsVolumeKey);
            Slider.SetValueWithoutNotify(currentSliderValue);
        }

        public void Set(float volume)
        {
            Slider.SetValueWithoutNotify(volume);
            AudioListener.volume = volume;
            PlayerPrefs.SetFloat(PlayerPrefsVolumeKey, volume);
        }
    }
}