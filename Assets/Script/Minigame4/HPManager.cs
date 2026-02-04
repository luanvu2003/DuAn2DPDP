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

    [Header("Tween")]
    public float loseHpDuration = 0.5f;

    Tween hpTween;

    void Awake()
    {
        Instance = this;
    }

    void OnEnable()
    {
        currentHP = maxHP;
        hpFill.fillAmount = 1f;
    }

    public void TakeDamage(int dmg)
    {
        currentHP -= dmg;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);

        UpdateHPBar();

        if (currentHP <= 0)
        {
            GameOver();
        }
    }

    void UpdateHPBar()
    {
        float target = (float)currentHP / maxHP;

        // Kill tween cũ để tránh giật
        if (hpTween != null && hpTween.IsActive())
            hpTween.Kill();

        hpTween = hpFill
            .DOFillAmount(target, loseHpDuration)
            .SetEase(Ease.OutQuad);
    }

    void GameOver()
    {
        Debug.Log("GAME OVER");
        // tắt minigame / hiện lose panel
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
