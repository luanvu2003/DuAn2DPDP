using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject[] items; // Comment, Like
    public Transform parent;
    public float spawnRate = 1f;

    void Start()
    {
        InvokeRepeating(nameof(Spawn), 1f, spawnRate);
    }

    void Spawn()
    {
        int rand = Random.Range(0, items.Length);

        Vector3 spawnPos = new Vector3(
            Random.Range(100, Screen.width - 100),
            -100,
            0
        );

        GameObject item = Instantiate(items[rand], spawnPos, Quaternion.identity, parent);
    }
}

