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
        isSleeping = true;
        sleepText.SetActive(false);

        Debug.Log("💤 Đang đi ngủ... Kết thúc ngày " + (StoryData.CurrentChapterIndex + 1));

        // ===============================
        // 🔥 RESET QUEST (QUAN TRỌNG)
        // ===============================
        QuestData.HasActiveQuest = false;
        QuestData.IsQuestCompleted = false;
        QuestData.ShouldShowQuestUI = false;
        QuestData.QuestText = "";
        QuestData.TargetTag = "";
        QuestData.QuestScene = "";
        QuestData.OriginScene = "";
        // ===============================

        // 3. XỬ LÝ DỮ LIỆU NGÀY MỚI
        StoryData.CurrentChapterIndex++;
        StoryData.CurrentTurnIndex = 0;
        SaveGameData();

        yield return new WaitForSeconds(0.5f);

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