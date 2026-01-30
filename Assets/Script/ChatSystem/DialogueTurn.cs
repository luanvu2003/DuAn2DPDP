using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class DialogueOption
{
    [TextArea] public string optionText; // Lời thoại của Nam
    public int scoreImpact; // Điểm cộng/trừ (VD: +4 hoặc 0)
}

[CreateAssetMenu(fileName = "New Turn", menuName = "TwoWorlds/DialogueTurn")]
public class DialogueTurn : ScriptableObject
{
    [Header("Lời thoại NPC / Tình huống")]
    public string speakerName; // Tên người nói (Hùng, Lan, Spammer...)
    [TextArea(3, 10)] public string npcDialogue; // Nội dung chat của họ

    [Header("Lựa chọn của Người chơi")]
    public DialogueOption optionA; // Lựa chọn Tiêu cực
    public DialogueOption optionB; // Lựa chọn Tích cực
}