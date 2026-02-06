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
        else Destroy(gameObject);
    }

    public void StartQuest(string questText, string targetTag, string questScene, string origin)
    {
        originScene = origin;
        SceneManager.LoadScene(questScene);
    }
}
