using UnityEngine;

public static class StoryData
{
    public static int CurrentChapterIndex = 0; // Đang ở ngày mấy (Chương mấy)
    public static int CurrentTurnIndex = 0;    // Đang chat tới câu nào
    public static int TotalScore = 0;

    public static bool HasStarted = false;

    // --- THÊM DÒNG NÀY ---
    // 0: Game Over (Về 0 điểm mood)
    // 1: Bad Ending (< 25)
    // 2: Normal Ending (26 - 50)
    // 3: Good Ending (51 - 75)
    // 4: Best Ending (> 76)
    public static int EndingID = -1; 
    
}