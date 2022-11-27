using UnityEngine;

public class UiJrpgDamageNumbersLayout : MonoBehaviour
{
    public UiJrpgDamageNumbersText textPrefab;
    public RectTransform layout;

    public void Show(Transform target, string message, Color color)
    {
        var prefab = Instantiate(textPrefab, layout, false);
        prefab.gameObject.SetActive(true);
        prefab.text.color *= color;
        prefab.Show(Camera.main, target, message);
    }
}
