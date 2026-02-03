using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider2D))]
public class InteractablePortal : MonoBehaviour
{
    [Header("Tên scene minigame")]
    public string miniGameSceneName;

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
        controls.Gameplay.Interact.performed += OnInteract;
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
            Debug.Log("Nhấn F để chơi minigame: " + miniGameSceneName);
            SceneManager.LoadScene(miniGameSceneName);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            if (interactText != null)
            {
                interactText.text = "Nhấn F để chơi " ;
                interactText.gameObject.SetActive(true);
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
                interactText.gameObject.SetActive(false);
            }
        }
    }
}
