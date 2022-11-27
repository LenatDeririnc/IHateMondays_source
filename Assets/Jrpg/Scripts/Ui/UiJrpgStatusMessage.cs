using DG.Tweening;
using TMPro;
using UnityEngine;

public class UiJrpgStatusMessage : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public TMP_Text messageText;

    public float messageFadeDuration = 0.25f;
    public float messageShowDuration = 3f;
    public float messageTextAppearDuration = 0.5f;

    public void Awake()
    {
        canvasGroup.alpha = 0f;
    }

    public void ShowMessage(string message)
    {
        DOTween.Kill(this);
        
        canvasGroup.alpha = 0f;
        messageText.text = message;
        messageText.ForceMeshUpdate();
        
        messageText.maxVisibleCharacters = 0;
        
        DOTween.Sequence()
            .Append(canvasGroup.DOFade(1f, messageFadeDuration))
            .AppendInterval(messageShowDuration)
            .Append(canvasGroup.DOFade(0f, messageFadeDuration))
            .SetTarget(this)
            .Play();
        
        DOTween.To(
                getter: () => messageText.maxVisibleCharacters,
                setter: (float v) => messageText.maxVisibleCharacters = (int)v,
                endValue: messageText.textInfo.characterCount,
                duration: messageTextAppearDuration
            )
            .SetEase(Ease.Linear)
            .SetTarget(this);
    }
}
