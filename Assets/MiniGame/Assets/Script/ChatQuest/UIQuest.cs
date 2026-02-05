using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class UIQuest : MonoBehaviour
{

    private TextMeshProUGUI questText;

    void Awake()
    {
        FindQuestText();
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindQuestText();
        Refresh();
    }

    void FindQuestText()
    {
        GameObject textObj = GameObject.Find("QuestText");
        if (textObj == null) return;

        questText = textObj.GetComponent<TextMeshProUGUI>();
    }

    public void Refresh()
    {
        if (questText == null) return;

        // ❌ Không được hiện nếu chưa tới turn 5
        if (!QuestData.ShouldShowQuestUI || !QuestData.HasActiveQuest)
        {
            questText.gameObject.SetActive(false);
            return;
        }

        questText.gameObject.SetActive(true);

        if (QuestData.IsQuestCompleted)
        {
            questText.text = "✅ Hoàn thành nhiệm vụ:\n" + QuestData.QuestText;
        }
        else
        {
            questText.text = "🎯 Nhiệm vụ:\n" + QuestData.QuestText;
        }
    }

}
