using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class HPManager : MonoBehaviour
{
    public static HPManager Instance;

    [Header("HP")]
    public int maxHP = 5;
    public int currentHP;

    [Header("UI")]
    public Image hpFill;
    public GameObject gameOverUI;   // Image / Panel YOU LOSE

    [Header("Tween")]
    public float loseHpDuration = 0.5f;

    Tween hpTween;
    bool isGameOver = false;

    void Awake()
    {
        Instance = this;
    }

    void OnEnable()
    {
        isGameOver = false;

        currentHP = maxHP;
        hpFill.fillAmount = 1f;

        if (gameOverUI)
            gameOverUI.SetActive(false);

        // đảm bảo game chạy lại nếu reload scene
        Time.timeScale = 1f;
    }

    public void TakeDamage(int dmg)
    {
        if (isGameOver) return;

        currentHP -= dmg;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);

        UpdateHPBar();

        if (currentHP <= 0)
        {
            isGameOver = true;
            GameOver();
        }
    }

    void UpdateHPBar()
    {
        float target = (float)currentHP / maxHP;

        if (hpTween != null && hpTween.IsActive())
            hpTween.Kill();

        hpTween = hpFill
            .DOFillAmount(target, loseHpDuration)
            .SetEase(Ease.OutQuad);
    }

    void GameOver()
    {
        Debug.Log("GAME OVER");
        StartCoroutine(GameOverSequence());
    }

    IEnumerator GameOverSequence()
    {
        // ⏳ ĐỢI 1 GIÂY (game vẫn chạy)
        yield return new WaitForSeconds(1f);

        // 🛑 DỪNG GAME
        Time.timeScale = 0f;

        // 👀 HIỆN YOU LOSE
        if (gameOverUI)
            gameOverUI.SetActive(true);
    }

    // ======================
    // TOOL TEST TRONG EDITOR
    // ======================

    [ContextMenu("Test - Mat 1 Mau")]
    void TestLoseHP()
    {
        TakeDamage(1);
    }

    [ContextMenu("Test - Hoi Day Mau")]
    void TestFullHP()
    {
        currentHP = maxHP;
        UpdateHPBar();
    }
}
