using UnityEngine;
using TMPro;

public class StartText : MonoBehaviour
{
    public float blinkSpeed = 0.5f;
    public GameObject objectToEnable; // 👈 object sẽ bật khi click

    private TextMeshProUGUI text;
    private bool gameStarted = false;

    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
        Time.timeScale = 0f;

        if (objectToEnable != null)
            objectToEnable.SetActive(false); // Tắt sẵn trước
    }

    void Update()
    {
        if (gameStarted) return;

        // Nhấp nháy chữ
        text.enabled = Mathf.FloorToInt(Time.unscaledTime / blinkSpeed) % 2 == 0;

        if (Input.GetMouseButtonDown(0))
        {
            StartGame();
        }
    }

    void StartGame()
    {
        gameStarted = true;
        Time.timeScale = 1f;

        // 🔥 BẬT OBJECT KHÁC
        if (objectToEnable != null)
            objectToEnable.SetActive(true);

        // 🔥 TẮT TEXT
        gameObject.SetActive(false);
    }
}

