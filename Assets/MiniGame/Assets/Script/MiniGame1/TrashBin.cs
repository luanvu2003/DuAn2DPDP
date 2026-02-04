using UnityEngine;

public class TrashBin : MonoBehaviour
{
    public GameObject leafPrefab;
    public int leafCount = 5;
    public Vector2 spawnAreaMin;
    public Vector2 spawnAreaMax;
    private bool spawned = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!spawned && other.CompareTag("Player"))
        {
            other.GetComponent<PlayerMovement>().isLocked = true;
            SpawnLeaves();
            spawned = true;
        }
    }

    void SpawnLeaves()
    {
        for (int i = 0; i < leafCount; i++)
        {
            Vector2 pos = new Vector2(
                Random.Range(spawnAreaMin.x, spawnAreaMax.x),
                Random.Range(spawnAreaMin.y, spawnAreaMax.y)
            );
            Instantiate(leafPrefab, pos, Quaternion.identity);
        }
    }
}
