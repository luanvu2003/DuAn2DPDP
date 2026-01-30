using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Cài đặt tốc độ")]
    public float moveSpeed = 5f;

    [Header("Thành phần")]
    public Rigidbody2D rb;
    public Animator animator;

    private Vector2 movement;

    // Hàm Update dùng để xử lý Input (Bàn phím)
    void Update()
{
    movement.x = Input.GetAxisRaw("Horizontal");
    movement.y = Input.GetAxisRaw("Vertical");

    // SỬA ĐOẠN NÀY:
    // Thay vì dùng sqrMagnitude trực tiếp, chúng ta kiểm tra thủ công
    // Nếu có input thì Speed = 1, không thì Speed = 0
    // Cách này giúp Animator bắt tín hiệu dứt khoát hơn
    if (movement.x != 0 || movement.y != 0)
    {
        animator.SetFloat("Speed", 1f);
    }
    else
    {
        animator.SetFloat("Speed", 0f);
    }

    // Đoạn giữ hướng mặt vẫn giữ nguyên
    if (movement != Vector2.zero)
    {
        animator.SetFloat("Horizontal", movement.x);
        animator.SetFloat("Vertical", movement.y);
    }
}

    // Hàm FixedUpdate dùng để xử lý Vật lý (Di chuyển)
    void FixedUpdate()
    {
        // Di chuyển nhân vật
        // normalized giúp đi chéo không bị nhanh hơn đi thẳng
        rb.MovePosition(rb.position + movement.normalized * moveSpeed * Time.fixedDeltaTime);
    }
}