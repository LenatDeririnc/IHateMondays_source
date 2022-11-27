using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UiJrpgButtonPanel : MonoBehaviour
{
    public CanvasGroup group;
    public Button[] buttons;
    public TMP_Text[] buttonTexts;
    public TMP_Text titleText;

    public AudioSource sfxSource;
    public AudioClip clipMove;
    public AudioClip clipError;

    public Action onCancelListener;

    private RectTransform _rectTransform;
    private Vector2 _openedSizeDelta;
    private Vector2 _closedSizeDelta;
    
    protected void Awake()
    {
        _rectTransform = (RectTransform) transform;
        _openedSizeDelta = _rectTransform.sizeDelta;
        _closedSizeDelta = new Vector2(_openedSizeDelta.x * 0.8f, 0f);
        
        group.interactable = false;
        group.alpha = 0f;
        _rectTransform.sizeDelta = _closedSizeDelta;

        foreach (var button in buttons)
        {
            var focus = button.gameObject.AddComponent<ButtonFocusListener>();
            focus.target = button;
            focus.panel = this;
        }
    }

    private void Update()
    {
        if (group.interactable && (Input.GetButtonDown("Cancel") || Input.GetButtonDown("Fire2")))
            OnCancel();
    }

    public void SetInteractable(bool isActive)
    {
        group.interactable = isActive;
        
        DOTween.Kill(group);
        group.DOFade(isActive ? 1f : 0.25f, 0.25f);
    }

    public void Show(string title, params Entry[] entries)
    {
        DOTween.Kill(group);
        titleText.text = title;
        
        for (var i = 0; i < buttons.Length; i++)
        {
            var b = buttons[i];
            var t = buttonTexts[i];
            
            if (i < entries.Length)
            {
                var entry = entries[i];
                
                b.gameObject.SetActive(true);
                b.onClick.RemoveAllListeners();
                b.onClick.AddListener(() =>
                {
                    if (entry.onClick == null)
                    {
                        PlaySFX(clipError);
                    }
                    else
                    {
                        entry.onClick();
                    }
                });
 
                t.alpha = entry.onClick != null ? 1f : 0.25f;
                t.text = entry.name;
            }
            else
            {
                b.gameObject.SetActive(false);
            }
        }

        group.DOFade(1f, 0.05f);
        _rectTransform.DOSizeDelta(_openedSizeDelta, 0.15f);
        
        SelectButton(0);
    }

    public void SelectButton(int buttonId)
    {
        EventSystem.current.SetSelectedGameObject(buttons[buttonId].gameObject);
    }
    
    public void Hide()
    {
        group.interactable = false;

        DOTween.Kill(group);
        group.DOFade(0f, 0.05f).SetDelay(0.1f);
        _rectTransform.DOSizeDelta(_closedSizeDelta, 0.15f);
    }
    
    private void OnCancel()
    {
        if (onCancelListener != null)
        {
            Hide();
            onCancelListener?.Invoke();
        }
    }

    private void PlaySFX(AudioClip clip)
    {
        sfxSource.Stop();
        sfxSource.clip = clip;
        sfxSource.Play();
    }

    private void OnButtonSelected()
    {
        PlaySFX(clipMove);
    }
    
    public class ButtonFocusListener : MonoBehaviour, IPointerEnterHandler, ISelectHandler
    {
        public Button target;
        public UiJrpgButtonPanel panel;

        public void OnPointerEnter(PointerEventData eventData)
        {
            if(!target.IsInteractable())
                return;

            target.Select();
        }

        public void OnSelect(BaseEventData eventData)
        {
            panel.OnButtonSelected();
        }
    }

    public struct Entry
    {
        public string name;
        public Action onClick;
    }
}
