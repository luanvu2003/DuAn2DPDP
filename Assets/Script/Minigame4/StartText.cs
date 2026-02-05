using UnityEngine;
using TMPro;
using DG.Tweening;

public class StartText : MonoBehaviour
{
    public float blinkSpeed = 0.5f;
    public GameObject objectToEnable;

    private TextMeshProUGUI text;
    private bool gameStarted = false;
    private Tween colorTween;

    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
        Time.timeScale = 0f;

        if (objectToEnable != null)
            objectToEnable.SetActive(false);

        // 🔥 DOTWEEN đổi màu trắng <-> vàng
        colorTween = text
            .DOColor(Color.yellow, blinkSpeed)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine)
            .SetUpdate(true); // chạy dù Time.timeScale = 0
    }

    void Update()
    {
        if (gameStarted) return;

        if (Input.GetMouseButtonDown(0))
        {
            StartGame();
        }
    }

    void StartGame()
    {
        gameStarted = true;
        Time.timeScale = 1f;

        // Dừng tween cho gọn
        if (colorTween != null)
            colorTween.Kill();

        if (objectToEnable != null)
            objectToEnable.SetActive(true);

        gameObject.SetActive(false);
    }
}
