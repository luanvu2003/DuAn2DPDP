using UnityEngine;

public class MiniGamePot : MonoBehaviour
{
    public enum PotState { Empty, Seeded, Sprout }
    public PotState state = PotState.Empty;

    [Header("Prefab mầm cây")]
    public GameObject sproutPrefab;

    [Header("Vị trí spawn mầm")]
    public Transform sproutPosition;

    [Header("Vị trí giữ item (bình tưới/hạt)")]
    public Transform holdPosition; // Empty GameObject đặt ngay miệng chậu

    [Header("ID của chậu (0,1,2...)")]
    public int potID; // gán trong Inspector

    private MiniGameMMG manager;
    private GameObject currentSprout;

    void Awake()
    {
        manager = FindObjectOfType<MiniGameMMG>();
    }

    public void AddSeed()
    {
        if (state == PotState.Empty)
        {
            state = PotState.Seeded;
            Debug.Log("Đã gieo hạt vào chậu " + potID);

            if (manager != null)
            {
                manager.SeedPot(potID); // truyền ID vào GameManager
            }
        }
    }

    public void Water()
    {
        if (state == PotState.Seeded)
        {
            state = PotState.Sprout;

            if (currentSprout != null) Destroy(currentSprout);

            if (sproutPrefab != null && sproutPosition != null)
            {
                currentSprout = Instantiate(sproutPrefab, sproutPosition.position, Quaternion.identity, transform);
            }

            Debug.Log("Đã tưới nước chậu " + potID);

            if (manager != null)
            {
                manager.WaterPot(potID); // truyền ID vào GameManager
            }
        }
    }
}
