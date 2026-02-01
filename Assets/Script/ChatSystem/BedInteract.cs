using UnityEngine;
using UnityEngine.SceneManagement; // Để chuyển cảnh

public class BedInteract : MonoBehaviour
{
    public GameObject sleepText; // Dòng chữ "Nhấn E để đi ngủ"
    private bool isPlayerNearby = false;

    void Update()
    {
        // Nếu đang đứng cạnh giường và bấm E
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E))
        {
            GoToSleep();
        }
    }

    void GoToSleep()
    {
        Debug.Log("💤 Đang đi ngủ... Kết thúc ngày " + (StoryData.CurrentChapterIndex + 1));

        // 1. TĂNG CHƯƠNG LÊN (Quan trọng nhất)
        StoryData.CurrentChapterIndex++; 
        
        // 2. RESET LẠI TIN NHẮN (Để ngày mai chat từ đầu)
        StoryData.CurrentTurnIndex = 0;

        // 3. (Tùy chọn) LƯU GAME VÀO Ổ CỨNG LUÔN
        SaveGameData();

        // 4. CHUYỂN CẢNH (Sang ngày mới)
        // Nếu game bạn chỉ có 1 map Phòng Ngủ thì reload lại chính nó
        // SceneManager.LoadScene(SceneManager.GetActiveScene().name); 
        
        // Hoặc nếu bạn có scene riêng cho từng ngày
        // SceneManager.LoadScene("Day" + StoryData.CurrentChapterIndex);
        
        // Ở đây mình ví dụ reload lại scene hiện tại để test:
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void SaveGameData()
    {
        PlayerPrefs.SetInt("Save_Chapter", StoryData.CurrentChapterIndex);
        PlayerPrefs.SetInt("Save_Score", StoryData.TotalScore);
        PlayerPrefs.Save();
    }

    // --- PHẦN XỬ LÝ VA CHẠM (Trigger) ---
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            sleepText.SetActive(true);
            isPlayerNearby = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            sleepText.SetActive(false);
            isPlayerNearby = false;
        }
    }
}