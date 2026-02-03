using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public void ChooseOptionA(DialogueTurn turn)
    {
        Debug.Log("Option A được chọn: " + turn.optionA.responseText);
        // Logic bình thường
    }

    public void ChooseOptionB(DialogueTurn turn)
    {
        if (turn.isFinalTurn)
        {
            QuestManager questManager = FindObjectOfType<QuestManager>();
            if (questManager != null)
            {
                questManager.StartQuest(
                    turn.optionB.questText,
                    turn.optionB.targetTag,
                    turn.optionB.questScene,
                    turn.optionB.originScene
                );
            }
        }
        else
        {
            Debug.Log("Option B được chọn: " + turn.optionB.responseText);
        }
    }
}
