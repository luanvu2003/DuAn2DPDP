using UnityEngine;

[CreateAssetMenu(fileName = "New Turn", menuName = "TwoWorlds/DialogueTurn")]
public class DialogueTurn : ScriptableObject
{
    [Header("--- THÔNG TIN NPC ---")]
    public string speakerName;
    [TextArea(3, 5)] public string npcDialogue; // Tin nhắn của Hùng/Lan/Ẩn danh

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