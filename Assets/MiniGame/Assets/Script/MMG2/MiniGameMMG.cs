using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class MiniGameMMG : MonoBehaviour
{
    [Header("UI nhiệm vụ")]
    public Text missionText;

    private HashSet<int> seededPots = new HashSet<int>();
    private HashSet<int> wateredPots = new HashSet<int>();

    private int target = 3;
    private bool missionComplete = false;

    void Start()
    {
        UpdateMissionText();
    }

    // gọi khi gieo hạt vào chậu
    public void SeedPot(int potID)
    {
        if (!seededPots.Contains(potID))
        {
            seededPots.Add(potID);
            Debug.Log("Đã gieo hạt vào chậu " + potID);
            UpdateMissionText();
            CheckMissionComplete();
        }
    }

    // gọi khi tưới nước vào chậu
    public void WaterPot(int potID)
    {
        if (!wateredPots.Contains(potID))
        {
            wateredPots.Add(potID);
            Debug.Log("Đã tưới nước chậu " + potID);
            UpdateMissionText();
            CheckMissionComplete();
        }
    }

    private void UpdateMissionText()
    {
        missionText.text =
            "Nhiệm vụ 1: Gieo 3 hạt giống (" + seededPots.Count + "/" + target + ")\n" +
            "Nhiệm vụ 2: Tưới 3 chậu (" + wateredPots.Count + "/" + target + ")";

        if (missionComplete)
        {
            missionText.text += "\n🎉 Hoàn thành nhiệm vụ!";
        }
    }


    private void CheckMissionComplete()
    {
        if (!missionComplete && seededPots.Count >= target && wateredPots.Count >= target)
        {
            missionComplete = true;
            Debug.Log("Hoàn thành minigame trồng cây!");
            GameData.potsSprouted = true; // đánh dấu đã mọc mầm

            // Xóa 2 dòng nhiệm vụ, chỉ hiển thị dòng hoàn thành
            missionText.text = "🎉 Hoàn thành nhiệm vụ!";

            StartCoroutine(LoadSceneAfterDelay(4f));
        }
    }



    private IEnumerator LoadSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("BedRoom");
    }
}
