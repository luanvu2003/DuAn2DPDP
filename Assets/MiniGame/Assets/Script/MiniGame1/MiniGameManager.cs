using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class MiniGameManager : MonoBehaviour
{
    public GameObject leafPrefab;
    public int leafCount = 5;
    public Vector2 spawnAreaMin;
    public Vector2 spawnAreaMax;
    public Text missionText;

    private int collected = 0;

    void Start()
    {
        SpawnLeaves();
        UpdateMissionText();
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

    public void CollectLeaf(GameObject leaf)
    {
        collected++;
        Destroy(leaf);
        UpdateMissionText();

        if (collected >= leafCount)
        {
            missionText.text = "Hoàn thành nhiệm vụ!";
            StartCoroutine(LoadSceneAfterDelay(4f)); // chỉ dùng coroutine
        }
    }
    private IEnumerator LoadSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("BedRoom");
    }


    void UpdateMissionText()
    {
        missionText.text = $"Bỏ {leafCount} lá vào thùng rác: {collected}/{leafCount}";
    }
}
