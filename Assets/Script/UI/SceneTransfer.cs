using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransfer : MonoBehaviour
{
    [Header("Cấu hình Cổng")]
    public string nextSceneName; // Điền tên scene: "city"
    
    [Tooltip("Tích vào nếu đây là cổng YÊU CẦU phải xong việc mới được đi (VD: Cổng về nhà)")]
    public bool requireEndOfDay = false; 

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Kiểm tra điều kiện (nếu cần)
            if (requireEndOfDay && !StoryData.IsEndOfDay)
            {
                Debug.Log("🚫 Chưa xong việc, chưa được đi!");
                return;
            }

            // --- QUAN TRỌNG: RESET TIN NHẮN VỀ 0 KHI CHUYỂN CẢNH ---
            StoryData.CurrentTurnIndex = 0; 
            
            Debug.Log("🚪 Đang chuyển sang: " + nextSceneName);
            SceneManager.LoadScene(nextSceneName);
        }
    }
}