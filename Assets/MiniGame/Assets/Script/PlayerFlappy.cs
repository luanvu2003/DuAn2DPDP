using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerFlappy : MonoBehaviour
{
    public float jumpForce = 5f;
    private Rigidbody2D rb;
    private GameManagerFlappy gameManager;

    private ControlsFlap controls;

    void Awake()
    {
        controls = new ControlsFlap();
    }

    void OnEnable()
    {
        controls.Flap.Enable();
        controls.Flap.Jump.performed += OnJump;
    }

    void OnDisable()
    {
        controls.Flap.Jump.performed -= OnJump;
        controls.Flap.Disable();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        gameManager = FindObjectOfType<GameManagerFlappy>();
    }

    private void OnJump(InputAction.CallbackContext ctx)
    {
        rb.linearVelocity = Vector2.up * jumpForce; // ✅ dùng velocity
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Obstacle") || collision.collider.CompareTag("Ground"))
        {
            Debug.Log("Player thua, reset game!");
            gameManager.ResetGame();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("ScoreZone"))
        {
            gameManager.AddScore();
        }
    }
}
