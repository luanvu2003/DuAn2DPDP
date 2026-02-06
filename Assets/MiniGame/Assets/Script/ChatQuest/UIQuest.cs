using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class UIQuest : MonoBehaviour
{
    public static UIQuest Instance;

    private TextMeshProUGUI questText;

    void Awake()
    {
        // Singleton
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        FindQuestText();
        Refresh(); // 🔥 refresh ngay khi scene load
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
        if (textObj == null)
        {
            questText = null;
            return;
        }

        questText = textObj.GetComponent<TextMeshProUGUI>();
    }

    public void Refresh()
    {
        if (questText == null) return;

        // ❌ Chưa được phép hiện quest
        if (!QuestData.HasActiveQuest || !QuestData.ShouldShowQuestUI)
        {
            questText.gameObject.SetActive(false);
            return;
        }

        questText.gameObject.SetActive(true);

        if (QuestData.IsQuestCompleted)
        {
            questText.text =
                "✅ Hoàn thành nhiệm vụ:\n" +
                QuestData.QuestText;
        }
        else
        {
            questText.text =
                "🎯 Nhiệm vụ:\n" +
                QuestData.QuestText;
        }
    }
}
