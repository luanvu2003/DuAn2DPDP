using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TrashBinPortal : MonoBehaviour
{
    [Header("UI thông báo")]
    public Text interactText; // Kéo thả Text UI từ Canvas vào đây

    private bool playerInRange = false;
    private PlayerControlsHoa controls;

    void Awake()
    {
        controls = new PlayerControlsHoa();
    }

    void OnEnable()
    {
        controls.Gameplay.Enable();
        controls.Gameplay.Interact.performed += OnInteract; // Action "Interact" gán phím F
    }

    void OnDisable()
    {
        controls.Gameplay.Interact.performed -= OnInteract;
        controls.Gameplay.Disable();
    }

    private void OnInteract(InputAction.CallbackContext ctx)
    {
        if (playerInRange)
        {
            SceneManager.LoadScene("MiniGameScene");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            if (interactText != null)
            {
                interactText.text = "Nhấn F để dọn rác";   // gán nội dung chữ
                interactText.gameObject.SetActive(true); // bật hiển thị
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            if (interactText != null)
            {
                interactText.text = "";                  // xoá nội dung khi ra ngoài
                interactText.gameObject.SetActive(false); // tắt hiển thị
            }
        }
    }

}
