using UnityEngine;

public static class QuestData
{
    public static bool HasActiveQuest = false;
    public static bool IsQuestCompleted = false;

    public static string QuestText = "";
    public static string TargetTag = "";
    public static string QuestScene = "";
    public static string OriginScene = "";

    // 🔥 Chỉ bật khi turn 5 chọn B
    public static bool ShouldShowQuestUI = false;
}

