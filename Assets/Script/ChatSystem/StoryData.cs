using UnityEngine;

// Không cần : MonoBehaviour vì ta không gắn vào object
public static class StoryData
{
    public static int CurrentChapterIndex = 0; // Đang ở ngày mấy (Chương mấy)
    public static int CurrentTurnIndex = 0;    // Đang chat tới câu nào
    public static int TotalScore = 0;

    // Biến này để kiểm tra xem có phải lần đầu mở game không
    public static bool HasStarted = false;
}