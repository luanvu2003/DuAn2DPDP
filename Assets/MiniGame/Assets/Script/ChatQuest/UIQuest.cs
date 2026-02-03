using UnityEngine;
using UnityEngine.UI;

public class UIQuest : MonoBehaviour
{
    public static UIQuest Instance;
    public Text questText;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void ShowQuest(string text)
    {
        questText.text = "Nhiệm vụ: " + text;
    }

    public void CompleteQuest()
    {
        questText.text = "Nhiệm vụ: Hoàn thành!";
    }
}
