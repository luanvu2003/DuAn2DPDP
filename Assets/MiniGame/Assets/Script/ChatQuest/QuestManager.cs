using UnityEngine;
using UnityEngine.SceneManagement;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

    private string currentQuestText;
    private string targetTag;
    private string questScene;
    private string originScene;
    private bool questActive = false;
    private bool questCompleted = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void StartQuest(string questText, string targetTag, string questSceneName, string originSceneName)
    {
        currentQuestText = questText;
        this.targetTag = targetTag;
        questScene = questSceneName;
        originScene = originSceneName;
        questActive = true;
        questCompleted = false;

        Debug.Log("Nhiệm vụ bắt đầu: " + questText);
        UIQuest.Instance.ShowQuest(currentQuestText);
    }

    public void CompleteQuest()
    {
        questCompleted = true;
        questActive = false;
        Debug.Log("Nhiệm vụ hoàn thành: " + currentQuestText);

        UIQuest.Instance.CompleteQuest();

        // Quay lại scene gốc
        if (!string.IsNullOrEmpty(originScene))
        {
            SceneManager.LoadScene(originScene);
        }
        else
        {
            Debug.LogWarning("Origin scene chưa được thiết lập!");
        }
    }

    public bool IsQuestActive() => questActive;
    public bool IsQuestCompleted() => questCompleted;
}
