using UnityEngine;

public class EndingManager : MonoBehaviour
{
    [Header("--- KÉO 5 GAME OBJECT ENDING VÀO ĐÂY ---")]
    public GameObject panelGameOver; // ID 0: Mood về 0
    public GameObject panelBad;      // ID 1: Dưới 25
    public GameObject panelNormal;   // ID 2: 26 -> 50
    public GameObject panelGood;     // ID 3: 51 -> 75
    public GameObject panelBest;     // ID 4: Trên 76

    void Start()
    {
        // 1. Tắt hết tất cả trước
        if(panelGameOver) panelGameOver.SetActive(false);
        if(panelBad) panelBad.SetActive(false);
        if(panelNormal) panelNormal.SetActive(false);
        if(panelGood) panelGood.SetActive(false);
        if(panelBest) panelBest.SetActive(false);

        // 2. Lấy ID từ StoryData
        int id = StoryData.EndingID;
        Debug.Log("Đang load Ending ID: " + id);

        // 3. Bật cái tương ứng
        switch (id)
        {
            case 0: if(panelGameOver) panelGameOver.SetActive(true); break;
            case 1: if(panelBad) panelBad.SetActive(true); break;
            case 2: if(panelNormal) panelNormal.SetActive(true); break;
            case 3: if(panelGood) panelGood.SetActive(true); break;
            case 4: if(panelBest) panelBest.SetActive(true); break;
            default: Debug.Log("Chưa có Ending ID hợp lệ!"); break;
        }
    }

    public void PlayAgain()
{
    // --- BƯỚC 1: RESET DỮ LIỆU CỐT TRUYỆN ---
    StoryData.CurrentChapterIndex = 0;
    StoryData.CurrentTurnIndex = 0;
    StoryData.TotalScore = 0;
    StoryData.HasStarted = false;
    StoryData.EndingID = -1;

    // --- BƯỚC 2 (QUAN TRỌNG): RESET INTRO ---
    // Phải set về 0 thì đoạn code trong IntroCutscene mới chạy lại
    PlayerPrefs.SetInt("IntroPlayed", 0); 
    PlayerPrefs.Save(); // Lưu xuống ổ cứng ngay lập tức để chắc chắn

    // --- BƯỚC 3: RESET UI THÔNG SỐ (MOOD/TIME) ---
    // Vì UIThongSo dùng DontDestroyOnLoad, nó vẫn sống từ game trước với Mood = 0
    // Ta phải hủy nó đi để khi vào Scene mới, nó tự tạo lại cái mới tinh (Mood = 50)
    if (UIThongSo.Instance != null)
    {
        Destroy(UIThongSo.Instance.gameObject);
    }

    // --- BƯỚC 4: LOAD LẠI GAME ---
    // Load đúng cái Scene có chứa Intro (thường là MainScene hoặc Gameplay)
    // Đừng load MenuStart trừ khi Intro của bạn nằm ở Menu
    UnityEngine.SceneManagement.SceneManager.LoadScene("BedRoom");
}
}