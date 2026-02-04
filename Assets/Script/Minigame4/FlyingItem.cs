using UnityEngine;

public class FlyingItem : MonoBehaviour
{
    public float speed = 200f;
    private HealthManager healthManager;

    void Start()
    {
        healthManager = FindObjectOfType<HealthManager>();
    }

    void Update()
    {
        transform.Translate(Vector3.up * speed * Time.deltaTime);

        // Bay khỏi màn hình
        if (transform.position.y > Screen.height + 100)
        {
            healthManager.TakeDamage(1); // 💔 Trừ 1 máu
            Destroy(gameObject);
        }
    }

    void OnMouseDown()
    {
        // Click trúng
        Destroy(gameObject);
    }
}


