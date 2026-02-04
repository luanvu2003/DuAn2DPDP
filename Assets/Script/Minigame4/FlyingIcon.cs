using UnityEngine;
using DG.Tweening;

public class FlyingIcon : MonoBehaviour
{
    [Header("Move")]
    public float speed = 200f;
    public int damage = 1;

    [Header("Scale Tween")]
    public float minScaleMultiplier = 1.15f;
    public float maxScaleMultiplier = 1.35f;
    public float minScaleDuration = 0.3f;
    public float maxScaleDuration = 0.7f;

    RectTransform rt;
    RectTransform canvasRect;
    IconSpawner spawner;

    Tween scaleTween;

    void Start()
    {
        rt = GetComponent<RectTransform>();
        canvasRect = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
        spawner = GetComponentInParent<IconSpawner>();

        float randomScale = Random.Range(minScaleMultiplier, maxScaleMultiplier);
        float randomDuration = Random.Range(minScaleDuration, maxScaleDuration);

        scaleTween = rt
            .DOScale(randomScale, randomDuration)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }

    void Update()
    {
        rt.anchoredPosition += Vector2.up * speed * Time.deltaTime;

        if (rt.anchoredPosition.y > canvasRect.rect.height / 2f + 100f)
        {
            DestroySelf(true);
        }
    }

    // Click để phá huỷ
    public void OnClick()
    {
        DestroySelf(false);
    }

    void DestroySelf(bool dealDamage)
    {
        if (scaleTween != null && scaleTween.IsActive())
            scaleTween.Kill();

        if (dealDamage && HPManager.Instance != null)
            HPManager.Instance.TakeDamage(damage);

        // 🔥 BÁO VỀ ICONSPAWNER
        if (spawner != null)
            spawner.OnIconDestroyed();

        Destroy(gameObject);
    }
}
