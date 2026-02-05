using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class QuestUI : MonoBehaviour
{
    private TextMeshProUGUI questText;

    void Awake()
    {
        FindQuestText();
    }

    void OnEnable()
    {
        // Khi load scene mới
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
            Debug.LogWarning("❌ Không tìm thấy GameObject tên 'QuestText' trong scene!");
            return;
        }

        questText = textObj.GetComponent<TextMeshProUGUI>();

        if (questText == null)
        {
            Debug.LogError("❌ 'QuestText' không có TextMeshProUGUI!");
        }
    }

    public void Refresh()
    {
        if (questText == null) return;

        // ❌ KHÔNG ĐƯỢC HIỆN nếu chưa bật cờ
        if (!QuestData.ShouldShowQuestUI)
        {
            questText.gameObject.SetActive(false);
            return;
        }

        // ❌ Chưa có quest
        if (!QuestData.HasActiveQuest)
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
