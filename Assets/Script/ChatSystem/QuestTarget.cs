using UnityEngine;

public class QuestTarget : MonoBehaviour
{
    private bool started = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (started) return;
        if (!other.CompareTag("Player")) return;
        if (!QuestData.HasActiveQuest) return;
        if (!CompareTag(QuestData.TargetTag)) return;

        started = true;

        Debug.Log("🎯 ĐÃ ĐẾN ĐỊA ĐIỂM NHIỆM VỤ");

        // 👉 Ẩn UI khi bắt đầu làm nhiệm vụ
        QuestData.ShouldShowQuestUI = false;
        UIQuest.Instance?.Refresh();

        // 👉 Ở ĐÂY bạn gọi script nhiệm vụ thật
        // ví dụ:
        // MinigameManager.Instance.StartMinigame();
        // hoặc mở UI tương tác
    }
}