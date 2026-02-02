using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Turn", menuName = "TwoWorlds/DialogueTurn")]
public class DialogueTurn : ScriptableObject
{
    [Header("--- HỘI THOẠI GROUP CHAT (MỚI) ---")]
    // Thay thế 2 biến cũ bằng List này
    public List<NPCMessage> conversation;

    [Header("--- SUY NGHĨ CỦA NAM (MỚI) ---")]
    [TextArea(2, 4)] public string internalThought; // "Nam (Nghĩ): Lại là nó..."

    [Header("--- LỰA CHỌN A ---")]
    public OptionData optionA;

    [Header("--- LỰA CHỌN B ---")]
    public OptionData optionB;

    [Header("--- CẤU HÌNH ĐẶC BIỆT ---")]
    public bool isFinalTurn = false; // Đánh dấu đây là lượt cuối (Lượt 5) để chuyển game
}

[System.Serializable]
public class NPCMessage
{
    public string speakerName; // Tên người nói
    [TextArea(2, 5)] public string content; // Nội dung

    [Tooltip("Tích vào để hiện bong bóng. Bỏ tích để làm lời dẫn/hành động.")]
    public bool showBubble = true; // <--- THÊM CÁI NÀY VÀO (Mặc định True)

    public float delayDuration = 1.0f; // Thời gian chờ
}
[System.Serializable]
public class OptionData
{
    public string optionText;   // Chữ hiện trên nút
    public string responseText; // Tin nhắn sẽ gửi đi (Nếu để trống = Im lặng/Hành động)
    public int scoreImpact;     // Điểm cộng/trừ

    // Thêm biến này để quy định thời gian cho Minigame (Dùng cho lượt 5)
    public float minigameBonusTime = 0;
    [Tooltip("Tích vào để hiện bong bóng chat. Bỏ tích nếu đây là hành động/suy nghĩ.")]
    public bool showBubble = true;  // <--- THÊM DÒNG NÀY (Mặc định là True)
}