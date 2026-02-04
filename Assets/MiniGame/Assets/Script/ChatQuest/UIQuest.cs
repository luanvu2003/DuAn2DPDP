using UnityEngine;
using TMPro;

public class UIQuest : MonoBehaviour
{
    public static UIQuest Instance;
    public TextMeshProUGUI questText;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void ShowQuest(string text)
    {
        questText.text = "Nhiệm vụ:  "+ text ;
    }

    public void CompleteQuest()
    {
        questText.text = "Nhiệm vụ: Hoàn thành!";
    }
}
