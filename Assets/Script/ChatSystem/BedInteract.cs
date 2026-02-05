using UnityEngine;
using UnityEngine.SceneManagement; 
using System.Collections; // Cần thêm cái này để dùng Coroutine

public class BedInteract : MonoBehaviour
{
    [Header("UI")]
    public GameObject sleepText; // Dòng chữ "Nhấn E để đi ngủ"

    private bool isPlayerNearby = false;
    private bool isSleeping = false; // Biến cờ quan trọng: để chặn spam nút E

    void Update()
    {
        // LOGIC: Phải thỏa mãn 3 điều kiện:
        // 1. Đang đứng gần (isPlayerNearby)
        // 2. Bấm nút E
        // 3. Chưa bấm ngủ trước đó (!isSleeping) -> Để tránh bấm 2 lần
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E) && !isSleeping)
        {
            StartCoroutine(GoToSleepProcess());
        }
    }

    // Dùng Coroutine để xử lý tuần tự (Tránh việc Scene reload quá nhanh khi chưa kịp save)
    IEnumerator GoToSleepProcess()
    {
        // 🔥 RESET QUEST KHI QUA NGÀY MỚI
        QuestData.HasActiveQuest = false;
        QuestData.IsQuestCompleted = false;
        QuestData.ShouldShowQuestUI = false;
        QuestData.QuestText = "";
        QuestData.TargetTag = "";
        QuestData.QuestScene = "";
        QuestData.OriginScene = "";


        isSleeping = true;          // 1. Khóa ngay nút E lại (không cho bấm nữa)
        sleepText.SetActive(false); // 2. Tắt dòng chữ "Nhấn E..." đi ngay cho đỡ vướng mắt

        Debug.Log("💤 Đang đi ngủ... Kết thúc ngày " + (StoryData.CurrentChapterIndex + 1));

        // 3. XỬ LÝ DỮ LIỆU
        StoryData.CurrentChapterIndex++; // Tăng ngày
        StoryData.CurrentTurnIndex = 0;  // Reset tin nhắn về 0
        SaveGameData();                  // Lưu lại

        // (Tùy chọn) Bạn có thể delay 0.5s - 1s ở đây để làm hiệu ứng màn hình đen nếu muốn
        yield return new WaitForSeconds(0.5f); 

        // 4. CHUYỂN CẢNH / RELOAD
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void SaveGameData()
    {
        PlayerPrefs.SetInt("Save_Chapter", StoryData.CurrentChapterIndex);
        PlayerPrefs.SetInt("Save_Score", StoryData.TotalScore);
        PlayerPrefs.Save();
    }

    // --- PHẦN TRIGGER: CHỈ DÙNG ĐỂ BẬT/TẮT TEXT ---
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Đi vào -> Chỉ hiện Text lên thôi, KHÔNG làm gì khác
            if (sleepText != null) sleepText.SetActive(true);
            isPlayerNearby = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Đi ra -> Tắt Text
            if (sleepText != null) sleepText.SetActive(false);
            isPlayerNearby = false;
        }
    }
}