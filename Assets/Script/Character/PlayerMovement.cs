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
        if (isLocked)
        {
            movement = Vector2.zero; 
            rb.linearVelocity = Vector2.zero;   
            animator.SetFloat("Speed", 0f); 
            return; 
        }

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
            rb.MovePosition(rb.position);
        }
    }
}