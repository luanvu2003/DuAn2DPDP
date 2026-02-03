using UnityEngine;

public class ObstacleMovement : MonoBehaviour
{
    [Header("Tốc độ di chuyển sang trái")]
    public float moveSpeed = 2f;

    [Header("Giới hạn bên trái màn hình (x)")]
    public float leftBound = -10f;

    void Update()
    {
        // Di chuyển cột sang trái
        transform.Translate(Vector2.left * moveSpeed * Time.deltaTime);

        // Nếu cột đi quá khỏi màn hình bên trái thì hủy
        if (transform.position.x < leftBound)
        {
            Destroy(gameObject);
        }
    }
}
