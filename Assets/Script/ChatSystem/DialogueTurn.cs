using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Turn", menuName = "TwoWorlds/DialogueTurn")]
public class DialogueTurn : ScriptableObject
{
    [Header("--- HỘI THOẠI GROUP CHAT (MỚI) ---")]
    public List<NPCMessage> conversation;

    [Header("--- SUY NGHĨ CỦA NAM (MỚI) ---")]
    [TextArea(2, 4)] public string internalThought;

    [Header("--- LỰA CHỌN A ---")]
    public OptionData optionA;

    [Header("--- LỰA CHỌN B ---")]
    public OptionData optionB;

    [Header("--- CẤU HÌNH ĐẶC BIỆT ---")]
    public bool isFinalTurn = false; // Đánh dấu lượt cuối
}

[System.Serializable]
public class NPCMessage
{
    public string speakerName;
    [TextArea(2, 5)] public string content;

    [Tooltip("Tích vào để hiện bong bóng. Bỏ tích để làm lời dẫn/hành động.")]
    public bool showBubble = true;

    public float delayDuration = 1.0f;
}

[System.Serializable]
public class OptionData
{
    public string optionText;
    public string responseText;
    public int scoreImpact;

    public float minigameBonusTime = 0;
    public bool showBubble = true;

    [Header("--- NHIỆM VỤ (Chỉ dùng cho Option B khi isFinalTurn) ---")]
    public string questText;     // Nội dung nhiệm vụ
    public string targetTag;     // Tag của GameObject mà Player phải đến
    public string questScene;    // Scene nhiệm vụ
    public string originScene;   // Scene gốc để quay lại
}
