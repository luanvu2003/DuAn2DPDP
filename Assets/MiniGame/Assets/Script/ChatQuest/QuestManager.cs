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

        UIQuest.Instance.ShowQuest(currentQuestText);
    }

    private void Update()
    {
        // Khi đang có nhiệm vụ và người chơi nhấn F gần object có tag
        if (questActive && Input.GetKeyDown(KeyCode.F))
        {
            GameObject target = GameObject.FindGameObjectWithTag(targetTag);
            if (target != null)
            {
                SceneManager.LoadScene(questScene);
            }
        }
    }

    public void CompleteQuest()
    {
        questActive = false;
        UIQuest.Instance.CompleteQuest();

        // Quay lại scene gốc
        if (!string.IsNullOrEmpty(originScene))
        {
            SceneManager.LoadScene(originScene);
        }
    }
}
