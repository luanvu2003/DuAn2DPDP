using UnityEngine;
using UnityEngine.SceneManagement;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

    public string originScene;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Được gọi khi chọn Option B trong chat
    public void StartQuest(string questText, string targetTag, string questScene, string origin)
    {
        Debug.Log("🚀 BẮT ĐẦU NHIỆM VỤ");

        QuestData.HasActiveQuest = true;
        QuestData.IsQuestCompleted = false;
        QuestData.ShouldShowQuestUI = true;

        QuestData.QuestText = questText;
        QuestData.TargetTag = targetTag;
        QuestData.QuestScene = questScene;
        QuestData.OriginScene = origin;

        originScene = origin;

        SceneManager.LoadScene(questScene);
    }

    // Được gọi khi player hoàn thành nhiệm vụ
    public void CompleteQuest()
    {
        Debug.Log("✅ HOÀN THÀNH NHIỆM VỤ");

        QuestData.IsQuestCompleted = true;
        QuestData.ShouldShowQuestUI = true;

        UIQuest.Instance?.Refresh();

        Invoke(nameof(ReturnToOrigin), 1f);
    }

    void ReturnToOrigin()
    {
        if (!string.IsNullOrEmpty(originScene))
        {
            Debug.Log("↩️ Quay về scene gốc: " + originScene);
            SceneManager.LoadScene(originScene);
        }
        else
        {
            Debug.LogWarning("⚠ originScene rỗng, không thể quay về");
        }
    }
}