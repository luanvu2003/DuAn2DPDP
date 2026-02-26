using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement; // Đừng quên import cái này để load Scene Ending
using UnityEngine.UI;

public class UIThongSo : MonoBehaviour
{
    public static UIThongSo Instance;

    [Header("--- UI HIỂN THỊ ---")]
    public GameObject hudPanel;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI foodText;
    public TextMeshProUGUI moodText;

    [Header("--- ICON THỜI GIAN ---")]
    public Image timeIconUI;
    public Sprite[] timeIcons;

    [Header("--- THỜI GIAN ---")]
    [Range(0, 24)]
    public float currentHour = 6f;
    public float timeSpeed = 1f;
    public bool isTimeRunning = false;

    [Header("--- DEBUG ---")]
    public bool autoStart = false; // Tích vào để test nhanh ko cần Intro

    [Header("--- CHỈ SỐ PLAYER ---")]
    public float maxStat = 100f;
    public float currentMood = 50f;

    void Awake()
    {
        // Kiểm tra xem đã có bản sao nào tồn tại chưa
        if (Instance == null)
        {
            Instance = this;
            // Dòng lệnh thần thánh: Giữ object này sống sót qua các Scene
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // Nếu đã có một cái UIThongSo từ Scene trước tồn tại rồi,
            // thì cái mới sinh ra ở Scene này là đồ thừa -> Hủy nó ngay!
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (autoStart)
        {
            StartDay();
        }
        else
        {
            if (hudPanel)
                hudPanel.SetActive(false);
        }
        UpdateUI();
    }

    void Update()
    {
        if (isTimeRunning)
        {
            // 1. CHẠY THỜI GIAN
            currentHour += Time.deltaTime * timeSpeed;
            if (currentHour >= 22f)
            {
                currentHour = 22f;
                EndDay();
            }

            // Cập nhật giao diện liên tục
            UpdateClockUI();
            UpdateUI();
        }
    }

    public void StartDay()
    {
        if (hudPanel)
            hudPanel.SetActive(true);
        isTimeRunning = true;
        currentHour = 6f;
        UpdateUI();
    }

    void EndDay()
    {
        isTimeRunning = false;
        Debug.Log("🌙 Hết ngày!");
    }

    // --- CÁC HÀM CỘNG TRỪ ĐIỂM ---

    // Gọi hàm này khi chơi Minigame: UIThongSo.Instance.AddMood(10);
    // Cập nhật lại hàm này trong UIThongSo.cs
    public void AddMood(float amount)
    {
        currentMood += amount;
        currentMood = Mathf.Clamp(currentMood, 0, maxStat);

        // Thêm log để bạn dễ test
        if (amount > 0)
            Debug.Log($"<color=green>Vui lên! +{amount} Mood</color>");
        else if (amount < 0)
            Debug.Log($"<color=red>Buồn quá! {amount} Mood</color>");

        UpdateUI();
        if (currentMood <= 0)
        {
            Debug.Log("💀 GAME OVER! Mood đã về 0.");
            StoryData.EndingID = 0; // Set ID là Game Over

            // Thay "TenSceneEndingCuaBan" bằng tên thật Scene Ending của bạn
            SceneManager.LoadScene("Ending");
        }
    }

    // --- CẬP NHẬT GIAO DIỆN ---

    void UpdateClockUI()
    {
        int hour = Mathf.FloorToInt(currentHour);
        int minute = Mathf.FloorToInt((currentHour - hour) * 60);
        if (timeText)
            timeText.text = $"{hour:00}:{minute:00}";

        if (timeIconUI != null && timeIcons.Length > 0)
        {
            int index = Mathf.FloorToInt(currentHour / 3f);
            index = Mathf.Clamp(index, 0, timeIcons.Length - 1);
            timeIconUI.sprite = timeIcons[index];
        }
    }

    void UpdateUI()
    {
        if (moodText)
            moodText.text = $"{(int)currentMood}/{(int)maxStat}";
    }

    public void ForceEndOfDay()
    {
        // Đặt hẳn thành 22h (10 giờ tối)
        currentHour = 22f;

        // Dừng thời gian lại để nó không chạy lố sang ngày sau
        isTimeRunning = false;

        UpdateClockUI(); // Cập nhật hình ảnh đồng hồ ngay
    }
}
