using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BedInteract : MonoBehaviour
{
    [Header("UI")]
    public GameObject sleepText;
    private bool isPlayerNearby = false;
    private bool isSleeping = false;

    void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E) && !isSleeping)
        {
            if (StoryData.IsEndOfDay)
            {
                StartCoroutine(GoToSleepProcess());
            }
            else
            {
                Debug.Log("Chưa buồn ngủ đâu!");
            }
        }
    }

    IEnumerator GoToSleepProcess()
    {
        isSleeping = true;
        if (sleepText) sleepText.SetActive(false);
        
        // Reset Quest
        QuestData.HasActiveQuest = false;
        QuestData.ShouldShowQuestUI = false;

        Debug.Log("💤 Đang đi ngủ...");
        yield return new WaitForSeconds(1f); // Hiệu ứng ngủ

        // --- KIỂM TRA: NẾU ĐÂY LÀ CHƯƠNG 5 (Index = 4) ---
        if (StoryData.CurrentChapterIndex >= 4)
        {
            Debug.Log("🏁 Ngủ xong chương 5 -> END GAME");
            
            // Gọi hàm tính điểm bên GameController
            if (GameController.Instance != null)
            {
                GameController.Instance.CalculateFinalEnding();
            }
            else
            {
                // Dự phòng nếu không tìm thấy GameController
                SceneManager.LoadScene("Ending"); 
            }
        }
        else
        {
            // --- CÁC CHƯƠNG BÌNH THƯỜNG -> SANG NGÀY MỚI ---
            StoryData.CurrentChapterIndex++; // Tăng chương
            StoryData.CurrentTurnIndex = 0;  // Reset tin nhắn
            StoryData.IsEndOfDay = false;    // Tắt cờ ngủ
            
            // Lưu game
            PlayerPrefs.SetInt("Save_Chapter", StoryData.CurrentChapterIndex);
            PlayerPrefs.Save();

            // Load lại Scene
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (StoryData.IsEndOfDay && sleepText) sleepText.SetActive(true);
            isPlayerNearby = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (sleepText) sleepText.SetActive(false);
            isPlayerNearby = false;
        }
    }
}