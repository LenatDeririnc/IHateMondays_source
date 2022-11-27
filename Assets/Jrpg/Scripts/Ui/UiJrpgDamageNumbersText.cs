using TMPro;
using UnityEngine;

public class UiJrpgDamageNumbersText : MonoBehaviour
{
    public TMP_Text text;
    
    private Transform _target;
    private Camera _camera;
    private RectTransform _rectTransform;
    
    public void Show(Camera camera, Transform target, string message)
    {
        _camera = camera;
        _target = target;
        text.text = message;
        text.alpha = 0f;
        _rectTransform = transform as RectTransform;

        Destroy(gameObject, 2f);
    }
    
    public void LateUpdate()
    {
        if(!_rectTransform)
            return;

        _rectTransform.position = _camera.WorldToScreenPoint(_target.position);;
    }
}
