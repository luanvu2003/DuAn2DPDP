using UnityEngine;

public class GameProgressManager : MonoBehaviour
{
    public static GameProgressManager Instance;

    [Header("--- TIẾN TRÌNH GAME ---")]
    public int currentChapterIndex = 0;
    public int currentTurnIndex = 0;
    public int totalScore = 0;

    [Header("--- TRẠNG THÁI NHIỆM VỤ ---")]
    public bool questActive = false;
    public bool questCompleted = false;
    public string currentQuestText = "";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // giữ lại khi đổi scene
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartQuest(string questText)
    {
        questActive = true;
        questCompleted = false;
        currentQuestText = questText;
    }

    public void CompleteQuest()
    {
        questActive = false;
        questCompleted = true;
    }
}
