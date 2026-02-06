using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider2D))]
public class PotInteraction : MonoBehaviour
{
    public Text interactText;

    private bool playerInRange;
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

    void OnInteract(InputAction.CallbackContext ctx)
    {
        if (!playerInRange) return;
        if (!QuestData.HasActiveQuest) return;

        SceneManager.LoadScene(QuestData.QuestScene);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (!QuestData.HasActiveQuest) return;

        playerInRange = true;
        interactText.text = "Nhấn F để làm nhiệm vụ";
        interactText.gameObject.SetActive(true);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerInRange = false;
        interactText.gameObject.SetActive(false);
    }
}
