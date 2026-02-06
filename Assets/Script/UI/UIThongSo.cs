using UnityEngine;
using UnityEngine.UI; 
using TMPro;

public class UIThongSo : MonoBehaviour
{
    public static UIThongSo Instance;

    [Header("--- UI HIỂN THỊ ---")]
    public GameObject hudPanel;       
    public TextMeshProUGUI timeText;  
    public TextMeshProUGUI foodText;  
    public TextMeshProUGUI moodText;  

    [Header("--- CẤU HÌNH TỰ GIẢM (ĐÓI BỤNG) ---")]
    [Tooltip("Số điểm thức ăn bị trừ mỗi giây")]
    public float foodDropRate = 1f; // Chỉnh số này: 1 = mất 1 điểm/giây (Nhanh), 0.1 = Chậm

    [Header("--- ICON THỨC ĂN (FOOD) ---")]
    public Image foodIconUI;          
    public Sprite foodFull;           // No
    public Sprite foodNormal;         // Bình thường
    public Sprite foodHungry;         // Đói

    [Header("--- ICON THỜI GIAN ---")]
    public Image timeIconUI;          
    public Sprite[] timeIcons;        

    [Header("--- THỜI GIAN ---")]
    [Range(0, 24)] public float currentHour = 6f; 
    public float timeSpeed = 1f;      
    public bool isTimeRunning = false;
    
    [Header("--- DEBUG ---")]
    public bool autoStart = false; // Tích vào để test nhanh ko cần Intro

    [Header("--- CHỈ SỐ PLAYER ---")]
    public float maxStat = 100f;
    public float currentFood = 80f;
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
            if (hudPanel) hudPanel.SetActive(false);
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

            // 2. GIẢM ĐỘ ĂN THEO THỜI GIAN (MỚI)
            if (currentFood > 0)
            {
                // Trừ dần thức ăn theo thời gian thực
                currentFood -= foodDropRate * Time.deltaTime;
            }
            else
            {
                currentFood = 0;
                // (Tùy chọn) Nếu đói quá (Food = 0) thì trừ luôn Mood?
                // currentMood -= 1f * Time.deltaTime; 
            }

            // Cập nhật giao diện liên tục
            UpdateClockUI();
            UpdateUI();
        }
    }

    public void StartDay()
    {
        if (hudPanel) hudPanel.SetActive(true); 
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
    
    // Gọi hàm này khi ăn: UIThongSo.Instance.AddFood(20);
    public void AddFood(float amount)
    {
        currentFood += amount;
        currentFood = Mathf.Clamp(currentFood, 0, maxStat);
        UpdateUI();
    }

    // Gọi hàm này khi chơi Minigame: UIThongSo.Instance.AddMood(10);
    public void AddMood(float amount)
    {
        currentMood += amount;
        currentMood = Mathf.Clamp(currentMood, 0, maxStat);
        UpdateUI(); 
    }

    // --- CẬP NHẬT GIAO DIỆN ---

    void UpdateClockUI()
    {
        int hour = Mathf.FloorToInt(currentHour);
        int minute = Mathf.FloorToInt((currentHour - hour) * 60);
        if (timeText) timeText.text = $"{hour:00}:{minute:00}";

        if (timeIconUI != null && timeIcons.Length > 0)
        {
            int index = Mathf.FloorToInt(currentHour / 3f);
            index = Mathf.Clamp(index, 0, timeIcons.Length - 1);
            timeIconUI.sprite = timeIcons[index];
        }
    }

    void UpdateUI()
    {
        // Làm tròn số (int) để không hiện số lẻ xấu xí (VD: 79.5 -> 79)
        if (foodText) foodText.text = $"{(int)currentFood}/{(int)maxStat}";
        if (moodText) moodText.text = $"{(int)currentMood}/{(int)maxStat}";

        // Đổi màu cảnh báo
        if (foodText) foodText.color = currentFood < 20 ? Color.red : Color.white;
        
        // Đổi Icon Food
        if (foodIconUI != null)
        {
            if (currentFood >= 70) foodIconUI.sprite = foodFull;
            else if (currentFood >= 30) foodIconUI.sprite = foodNormal;
            else foodIconUI.sprite = foodHungry;
        }
    }
}