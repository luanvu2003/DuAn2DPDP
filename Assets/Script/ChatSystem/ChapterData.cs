using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Chapter", menuName = "TwoWorlds/ChapterData")]
public class ChapterData : ScriptableObject
{
    public string chapterName; // Tên chương (VD: Cái Hố Rác)
    
    [Header("Chuỗi 5 Lượt Chat")]
    public List<DialogueTurn> chatSequence; // Kéo thả 5 file Turn vào đây

    [Header("Cấu hình Minigame")]
    public string minigameName; // Tên Minigame (để hiển thị)
    public int maxMinigameScore = 20; // Điểm tối đa của minigame này
}