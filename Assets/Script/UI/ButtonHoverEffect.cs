using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class ButtonHoverEffect : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler
{
    [Header("Sprite")]
    public Sprite normalSprite;
    public Sprite hoverSprite; // sprite màu vàng

    [Header("Scale")]
    public float hoverScale = 1.1f;
    public float tweenDuration = 0.2f;

    Image img;
    Vector3 originalScale;
    Tween scaleTween;

    void Awake()
    {
        img = GetComponent<Image>();
        originalScale = transform.localScale;

        if (img != null && normalSprite != null)
            img.sprite = normalSprite;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (img != null && hoverSprite != null)
            img.sprite = hoverSprite;

        scaleTween?.Kill();
        scaleTween = transform
            .DOScale(originalScale * hoverScale, tweenDuration)
            .SetEase(Ease.OutBack);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (img != null && normalSprite != null)
            img.sprite = normalSprite;

        scaleTween?.Kill();
        scaleTween = transform
            .DOScale(originalScale, tweenDuration)
            .SetEase(Ease.OutBack);
    }
}
