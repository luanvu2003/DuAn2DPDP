using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Rigidbody2D rb;
    public Animator animator;

    // Biến này để các script khác điều khiển việc khóa di chuyển
    [HideInInspector] public bool isLocked = false;

    private Vector2 movement;

    void Update()
    {
        // 1. Kiểm tra khóa: Nếu bị khóa thì không nhận Input nữa
        if (isLocked)
        {
            movement = Vector2.zero;      // Xóa vector di chuyển
            rb.linearVelocity = Vector2.zero;   // Dừng ngay vật lý (Quan trọng!)
            animator.SetFloat("Speed", 0f); // Ép Animation về đứng yên
            return; // Thoát luôn, không chạy code bên dưới nữa
        }
        // ----------------

        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        if (movement.x != 0 || movement.y != 0)
        {
            animator.SetFloat("Speed", 1f);
        }
        else
        {
            animator.SetFloat("Speed", 0f);
        }

        if (movement != Vector2.zero)
        {
            animator.SetFloat("Horizontal", movement.x);
            animator.SetFloat("Vertical", movement.y);
        }
    }

    void FixedUpdate()
    {
        if (!isLocked)
        {
            rb.MovePosition(rb.position + movement.normalized * moveSpeed * Time.fixedDeltaTime);
        }
        else
        {
            // Khi bị khóa, giữ nguyên vị trí hiện tại để tránh bị đẩy trôi
            rb.MovePosition(rb.position);
        }
    }
}