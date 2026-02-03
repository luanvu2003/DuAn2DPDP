using UnityEngine;

public class PlantPot : MonoBehaviour
{
    public enum PlantState { Empty, Sprout, Flower }
    public PlantState state = PlantState.Empty;

    [Header("Prefab cho mầm và hoa")]
    public GameObject sproutPrefab;
    public GameObject flowerPrefab;

    [Header("Vị trí spawn")]
    public Transform sproutPosition; // Empty GameObject đặt ở vị trí muốn mầm xuất hiện
    public Transform flowerPosition; // Empty GameObject đặt ở vị trí muốn hoa xuất hiện

    [Header("Thời gian chờ (giây)")]
    public float flowerDelay = 120f; // 2 phút

    private float sproutTime;
    private GameObject currentPlant;

    void Start()
    {
        if (GameData.potsSprouted && state == PlantState.Empty)
        {
            SetSprout();
        }
    }

    void Update()
    {
        if (state == PlantState.Sprout)
        {
            if (Time.time - sproutTime >= flowerDelay)
            {
                SetFlower();
            }
        }
    }

    public void SetSprout()
    {
        state = PlantState.Sprout;
        sproutTime = Time.time;

        if (currentPlant != null) Destroy(currentPlant);
        currentPlant = Instantiate(sproutPrefab, sproutPosition.position, Quaternion.identity, transform);

        Debug.Log("Cây đã mọc mầm!");
    }

    private void SetFlower()
    {
        state = PlantState.Flower;

        if (currentPlant != null) Destroy(currentPlant);
        currentPlant = Instantiate(flowerPrefab, flowerPosition.position, Quaternion.identity, transform);

        Debug.Log("Cây đã nở hoa!");
    }
}
