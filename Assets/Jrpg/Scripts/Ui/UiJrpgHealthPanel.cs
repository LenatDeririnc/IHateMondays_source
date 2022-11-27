using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiJrpgHealthPanel : MonoBehaviour
{
    public TMP_Text hpText;
    public Image hpImage;
    public CanvasGroup group;

    public void SetHp(int totalHp, int hp, bool animate)
    {
        DOTween.Kill(this);

        var targetFill = (float) hp / totalHp;
        var currentFill = hpImage.fillAmount;
        var currentHp = Mathf.RoundToInt(hpImage.fillAmount * totalHp);

        DOTween.To(() => 0f, v =>
            {
                var animatedHp = Mathf.RoundToInt(Mathf.Lerp(currentHp, hp, v));
                hpImage.fillAmount = Mathf.Lerp(currentFill, targetFill, v);
                hpText.text = $"{animatedHp} / {totalHp}";

            }, 1f, animate ? 1f : 0f)
            .SetTarget(this);
    }
    
    public void Show()
    {
        group.DOFade(1f, 0.25f);
    }

    public void Hide()
    {
        group.DOFade(0, 0.25f);
    }
}
