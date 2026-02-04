using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TrashCollector : MonoBehaviour
{
    public int collected = 0;
    public int target = 5;
    public Text missionText;

    void Start()
    {
        UpdateMissionText();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Leaf"))
        {
            collected++;
            Destroy(other.gameObject);
            UpdateMissionText();

            if (collected >= target)
            {
                missionText.text = "Hoàn thành nhiệm vụ!";
                // Quay lại scene gốc
                SceneManager.LoadScene("MainScene");
            }
        }
    }

    void UpdateMissionText()
    {
        missionText.text = $"Bỏ 5 lá vào thùng rác: {collected}/{target}";
    }
}
