using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class ItemController : MonoBehaviour
{
    public enum ItemType { Seed, WateringCan }
    public ItemType itemType;

    [Header("Vị trí ban đầu của item")]
    public Transform startPosition;

    private bool isFollowingMouse = false;
    private bool isInsidePot = false;
    private bool hasInteractedInPot = false; // cờ chống spam
    private MiniGamePot currentPot;

    private PlayerControlsHoa controls;

    void Awake()
    {
        controls = new PlayerControlsHoa();
    }

    void OnEnable()
    {
        controls.Gameplay.Enable();
        controls.Gameplay.Click.performed += OnClick;
        controls.Gameplay.RightClick.performed += OnRightClick;
    }

    void OnDisable()
    {
        controls.Gameplay.Click.performed -= OnClick;
        controls.Gameplay.RightClick.performed -= OnRightClick;
        controls.Gameplay.Disable();
    }

    void Update()
    {
        if (isFollowingMouse && !isInsidePot)
        {
            Vector2 mousePos = controls.Gameplay.MousePosition.ReadValue<Vector2>();
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
            worldPos.z = 0;
            transform.position = worldPos;
        }
    }

    private void OnClick(InputAction.CallbackContext ctx)
    {
        Vector2 mousePos = controls.Gameplay.MousePosition.ReadValue<Vector2>();
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
        worldPos.z = 0;

        RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);

        if (hit.collider != null)
        {
            // Nếu click trúng chính item này → bật theo chuột
            if (hit.collider.gameObject == gameObject && !isFollowingMouse && !isInsidePot)
            {
                isFollowingMouse = true;
                return;
            }

            // Nếu item đang trong chậu và click → gieo hạt/tưới nước + xoay sprite
            if (isInsidePot && currentPot != null && !hasInteractedInPot)
            {
                hasInteractedInPot = true; // đánh dấu đã tương tác

                if (itemType == ItemType.Seed)
                    currentPot.AddSeed();
                else if (itemType == ItemType.WateringCan)
                    currentPot.Water();

                StartCoroutine(RotateAtPotThenReset());
                return;
            }
        }
    }

    private IEnumerator RotateAtPotThenReset()
    {
        // Xoay từ 0 → 40
        float duration = 0.5f;
        float elapsed = 0f;
        float startZ = 0f;
        float endZ = 40f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float z = Mathf.Lerp(startZ, endZ, elapsed / duration);
            transform.rotation = Quaternion.Euler(0, 0, z);
            yield return null;
        }

        // Xoay ngược từ 40 → 0 ngay tại chậu
        duration = 0.5f;
        elapsed = 0f;
        startZ = 40f;
        endZ = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float z = Mathf.Lerp(startZ, endZ, elapsed / duration);
            transform.rotation = Quaternion.Euler(0, 0, z);
            yield return null;
        }

        transform.rotation = Quaternion.identity;

        // Sau khi xoay ngược xong → mới reset về vị trí ban đầu
        ResetPosition();
    }

    private void OnRightClick(InputAction.CallbackContext ctx)
    {
        if (isFollowingMouse || isInsidePot)
        {
            ResetPosition();
        }
    }

    private void ResetPosition()
    {
        transform.position = startPosition.position;
        transform.rotation = Quaternion.identity;
        isFollowingMouse = false;
        isInsidePot = false;
        hasInteractedInPot = false; // reset cờ chống spam
        currentPot = null;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        MiniGamePot pot = other.GetComponent<MiniGamePot>();
        if (pot != null)
        {
            isInsidePot = true;
            currentPot = pot;
            isFollowingMouse = false;

            if (pot.holdPosition != null)
                transform.position = pot.holdPosition.position;
            else
                transform.position = other.transform.position;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        MiniGamePot pot = other.GetComponent<MiniGamePot>();
        if (pot != null && pot == currentPot)
        {
            isInsidePot = false;
            currentPot = null;
            isFollowingMouse = true;
        }
    }
}
