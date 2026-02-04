using UnityEngine;
using TMPro;
using DG.Tweening;

public class IconSpawner : MonoBehaviour
{
    [Header("References")]
    public RectTransform canvasRect;
    public GameObject[] iconPrefabs;

    [Header("Spawn Base")]
    public float spawnInterval = 0.4f;
    public int maxSpawn = 200;

    [Header("Difficulty")]
    public int spawnPerWave = 1;
    public int maxSpawnPerWave = 5;
    public int increaseEvery = 25;

    [Header("Spawn Padding")]
    public float horizontalPadding = 80f;

    [Header("Luot Xem (TextMeshPro)")]
    public TextMeshProUGUI luotXemText;
    public int startViewCount = 1300;

    [Header("End Effect")]
    public CanvasGroup whiteOverlay; // Image trắng che gameplay
    public float overlaySlideDuration = 1.2f;

    int totalSpawned;
    int aliveIcons;
    int destroyedCount;

    int currentViewCount;
    int totalSteps;
    int currentStep;

    bool endEffectPlayed;

    void OnEnable()
    {
        totalSpawned = 0;
        aliveIcons = 0;
        destroyedCount = 0;
        endEffectPlayed = false;

        currentViewCount = startViewCount;

        totalSteps = maxSpawn / 10;
        currentStep = 0;

        UpdateLuotXemText();

        if (whiteOverlay != null)
        {
            whiteOverlay.alpha = 1f;
            whiteOverlay.gameObject.SetActive(false);
        }

        InvokeRepeating(nameof(Spawn), 0f, spawnInterval);
    }

    void OnDisable()
    {
        CancelInvoke();
    }

    void Spawn()
    {
        if (totalSpawned >= maxSpawn)
        {
            CancelInvoke();
            CheckWinCondition();
            return;
        }

        int wave = totalSpawned / increaseEvery;
        int currentSpawnCount = Mathf.Clamp(
            spawnPerWave + wave,
            1,
            maxSpawnPerWave
        );

        for (int i = 0; i < currentSpawnCount; i++)
        {
            if (totalSpawned >= maxSpawn) break;

            GameObject prefab = iconPrefabs[Random.Range(0, iconPrefabs.Length)];
            GameObject icon = Instantiate(prefab, transform);

            RectTransform rt = icon.GetComponent<RectTransform>();

            float halfWidth = canvasRect.rect.width / 2f;
            float iconHalfWidth = rt.rect.width / 2f;

            float x = Random.Range(
                -halfWidth + horizontalPadding + iconHalfWidth,
                 halfWidth - horizontalPadding - iconHalfWidth
            );

            rt.anchoredPosition = new Vector2(
                x,
                -canvasRect.rect.height / 2f - 100f
            );

            totalSpawned++;
            aliveIcons++;
        }
    }

    // ĐƯỢC GỌI TỪ FlyingIcon
    public void OnIconDestroyed()
    {
        aliveIcons--;
        destroyedCount++;

        if (destroyedCount % 10 == 0)
        {
            currentStep++;
            DecreaseViewCountControlled();
        }

        CheckWinCondition();
    }

    // GIẢM LƯỢT XEM CÓ KIỂM SOÁT
    void DecreaseViewCountControlled()
    {
        if (currentStep >= totalSteps)
        {
            currentViewCount = 0;
            UpdateLuotXemText();

            if (!endEffectPlayed)
            {
                endEffectPlayed = true;
                PlayEndViewEffect();
            }
            return;
        }

        int stepsLeft = totalSteps - currentStep + 1;
        float averageLoss = (float)currentViewCount / stepsLeft;

        float randomFactor = Random.Range(0.7f, 1.3f);
        int loss = Mathf.RoundToInt(averageLoss * randomFactor);

        loss = Mathf.Clamp(loss, 1, currentViewCount);
        currentViewCount -= loss;

        UpdateLuotXemText();
    }

    void UpdateLuotXemText()
    {
        if (luotXemText == null) return;
        luotXemText.text = currentViewCount.ToString();
    }

    // 🔥 CHỈ NỀN TRẮNG CHẠY TỪ TRÁI VÀO
    void PlayEndViewEffect()
    {
        if (whiteOverlay == null || canvasRect == null)
            return;

        RectTransform overlayRect = whiteOverlay.GetComponent<RectTransform>();

        whiteOverlay.gameObject.SetActive(true);

        float canvasWidth = canvasRect.rect.width;

        // đặt nền trắng ngoài màn hình bên trái
        overlayRect.anchoredPosition = new Vector2(-canvasWidth, 0);

        // chạy chậm vào giữa
        overlayRect.DOAnchorPos(Vector2.zero, overlaySlideDuration)
                   .SetEase(Ease.InOutSine);
    }

    void CheckWinCondition()
    {
        if (totalSpawned >= maxSpawn && aliveIcons <= 0)
        {
            if (HPManager.Instance != null &&
                HPManager.Instance.currentHP > 0)
            {
                YouWin();
            }
        }
    }

    void YouWin()
    {
        Debug.Log("YOU WIN 🎉");
    }
}
