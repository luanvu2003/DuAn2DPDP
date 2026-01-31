using UnityEngine;

public class OpenPhone : MonoBehaviour
{
    public GameObject OpenPhoneText;
    public GameObject Phone;
    public GameObject PhoneUI;

    [Header("Kết nối Player")]
    public PlayerMovement playerMovement; // Kéo script PlayerMovement vào đây
    public Animator playerAnimator;       // Kéo Animator của Player vào đây

    private bool isPlayerNearby = false;
    private bool isPhoneActive = false; // Biến kiểm tra xem đang mở điện thoại hay chưa

    void Update()
    {
        // TRƯỜNG HỢP 1: Mở điện thoại (Khi ở gần + Bấm E + Chưa mở)
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E) && !isPhoneActive)
        {
            OpenThePhone();
        }

        // TRƯỜNG HỢP 2: Đóng điện thoại (Khi đang mở + Bấm E hoặc ESC)
        else if (isPhoneActive && (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Escape)))
        {
            CloseThePhone();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            OpenPhoneText.SetActive(true);
            isPlayerNearby = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            OpenPhoneText.SetActive(false);
            isPlayerNearby = false;
        }
    }

    // Hàm Mở Điện Thoại
    // Hàm Mở Điện Thoại
    void OpenThePhone()
    {
        Debug.Log("📱 [OpenPhone] Bắt đầu mở điện thoại..."); // Kiểm tra xem hàm có chạy không

        isPhoneActive = true;

        // 1. Xử lý UI
        Phone.SetActive(false);       // Ẩn cái điện thoại dưới đất
        PhoneUI.SetActive(true);      // Hiện UI
        OpenPhoneText.SetActive(false);

        // 2. Khóa di chuyển Player
        if (playerMovement != null)
        {
            playerMovement.isLocked = true;
        }

        // 3. Chạy Animation mở điện thoại
        if (playerAnimator != null)
        {
            playerAnimator.SetBool("IsUsingPhone", true);
        }

        // --- QUAN TRỌNG NHẤT: GỌI GAME CONTROLLER ---
        if (GameController.Instance != null)
        {
            Debug.Log("✅ [OpenPhone] Tìm thấy GameController -> Gọi StartChapter");
            GameController.Instance.StartChapter(); // <--- THÊM DÒNG NÀY VÀO
        }
        else
        {
            Debug.LogError("❌ [OpenPhone] LỖI: Không tìm thấy GameController! (Instance bị Null)");
        }
    }

    // Hàm Đóng Điện Thoại
    void CloseThePhone()
    {
        isPhoneActive = false;

        Phone.SetActive(true);
        // 1. Tắt UI
        PhoneUI.SetActive(false);

        // 2. Mở khóa di chuyển
        if (playerMovement != null)
        {
            playerMovement.isLocked = false;
        }

        // 3. Tắt Animation
        if (playerAnimator != null)
        {
            playerAnimator.SetBool("IsUsingPhone", false);
        }
    }
}