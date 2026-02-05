using UnityEngine;

public static class QuestData
{
    public static bool HasActiveQuest = false;
    public static bool IsQuestCompleted = false;

    public static string QuestText = "";
    public static string TargetTag = "";
    public static string QuestScene = "";
    public static string OriginScene = "";

    // 🔥 NEW: chỉ dùng để quyết định có hiện QuestUI hay không
    public static bool ShouldShowQuestUI = false;
}
