using UnityEngine;
using UnityEngine.InputSystem; // dùng namespace mới

public class Leaf : MonoBehaviour
{
    private bool isDragging = false;
    private Vector3 offset;

    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            mousePos.z = transform.position.z;

            // kiểm tra raycast trúng lá
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
            if (hit.collider != null && hit.collider.gameObject == gameObject)
            {
                isDragging = true;
                offset = transform.position - mousePos;
                Debug.Log("Click on Leaf!");
            }
        }

        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            isDragging = false;
        }

        if (isDragging)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            mousePos.z = transform.position.z;
            transform.position = mousePos + offset;
            Debug.Log("Dragging Leaf...");
        }
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("TrashBin"))
        {
            // Gọi MiniGameManager để cộng điểm
            FindObjectOfType<MiniGameManager>().CollectLeaf(gameObject);
        }
    }
}


