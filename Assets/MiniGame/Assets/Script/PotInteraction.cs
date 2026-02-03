using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider2D))]
public class PotInteraction : MonoBehaviour
{
    public string miniGameSceneName = "MiniGameScene2";
    public Text interactText; // Kéo thả Text UI từ Canvas vào đây

    private bool playerInRange = false;
    private PlayerControlsHoa controls; // class sinh ra từ Input Actions

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
            Debug.Log("Đã nhấn F để chơi");
            SceneManager.LoadScene(miniGameSceneName);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            if (interactText != null)
            {
                interactText.text = "Nhấn F để trồng cây";
                interactText.gameObject.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
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
