using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class GameController : MonoBehaviour
{
    [Header("Dữ liệu Game")]
    public List<ChapterData> allChapters; // Kéo 5 Chapter vào đây
    
    [Header("UI Chat")]
    public GameObject chatPanel;
    public TextMeshProUGUI npcNameText;
    public TextMeshProUGUI npcDialogueText;
    public Button buttonOptionA;
    public Button buttonOptionB;
    public TextMeshProUGUI textOptionA;
    public TextMeshProUGUI textOptionB;

    [Header("UI Minigame (Giả lập)")]
    public GameObject minigamePanel;
    public TextMeshProUGUI minigameTitle;
    public TextMeshProUGUI currentTotalScoreText;

    [Header("UI Ending")]
    public GameObject endingPanel;
    public TextMeshProUGUI endingTitleText;
    public TextMeshProUGUI endingDescriptionText;

    // Biến nội bộ
    private int currentChapterIndex = 0;
    private int currentTurnIndex = 0;
    private int totalHumanityScore = 0;

    void Start()
    {
        // Bắt đầu game
        totalHumanityScore = 0;
        currentChapterIndex = 0;
        LoadChapter(currentChapterIndex);
    }

    // --- LOGIC CHAT ---
    void LoadChapter(int index)
    {
        if (index >= allChapters.Count)
        {
            ShowEnding();
            return;
        }

        currentTurnIndex = 0;
        chatPanel.SetActive(true);
        minigamePanel.SetActive(false);
        endingPanel.SetActive(false);
        
        LoadTurn(allChapters[index].chatSequence[0]);
    }

    void LoadTurn(DialogueTurn turn)
    {
        npcNameText.text = turn.speakerName;
        npcDialogueText.text = turn.npcDialogue;

        // Setup nút bấm A
        textOptionA.text = turn.optionA.optionText;
        buttonOptionA.onClick.RemoveAllListeners();
        buttonOptionA.onClick.AddListener(() => OnOptionSelected(turn.optionA.scoreImpact));

        // Setup nút bấm B
        textOptionB.text = turn.optionB.optionText;
        buttonOptionB.onClick.RemoveAllListeners();
        buttonOptionB.onClick.AddListener(() => OnOptionSelected(turn.optionB.scoreImpact));
    }

    void OnOptionSelected(int score)
    {
        totalHumanityScore += score;
        Debug.Log("Điểm hiện tại: " + totalHumanityScore);

        // Chuyển sang lượt chat tiếp theo
        currentTurnIndex++;
        ChapterData currentChapter = allChapters[currentChapterIndex];

        if (currentTurnIndex < currentChapter.chatSequence.Count)
        {
            // Vẫn còn chat trong chương này
            LoadTurn(currentChapter.chatSequence[currentTurnIndex]);
        }
        else
        {
            // Hết 5 lượt chat -> Chuyển sang Minigame
            StartMinigame(currentChapter);
        }
    }

    // --- LOGIC MINIGAME ---
    void StartMinigame(ChapterData chapter)
    {
        chatPanel.SetActive(false);
        minigamePanel.SetActive(true);
        minigameTitle.text = "NHIỆM VỤ: " + chapter.minigameName;
        
        // Ở đây bạn sẽ load Scene Minigame thật.
        // Tạm thời tôi làm nút bấm giả lập "Thắng/Thua" minigame nhé.
    }

    // Hàm này được gọi khi chơi xong Minigame (Bạn sẽ gọi hàm này từ script Minigame của bạn)
    public void CompleteMinigame(int scoreFromGame)
    {
        totalHumanityScore += scoreFromGame;
        currentTotalScoreText.text = "Tổng điểm Nhân Tính: " + totalHumanityScore;
        
        // Qua chương tiếp theo
        currentChapterIndex++;
        
        // Tạm dừng 2s để người chơi nhìn điểm rồi qua chương mới (Dùng Coroutine nếu muốn)
        LoadChapter(currentChapterIndex); 
    }
    
    // Hàm giả lập nút bấm cho Minigame Panel (Dùng để test)
    public void SimulateMinigameWin()
    {
        CompleteMinigame(20); // Giả sử thắng tuyệt đối được 20 điểm
    }
    
    public void SimulateMinigameFail()
    {
        CompleteMinigame(10); // Giả sử chơi tệ được 10 điểm
    }

    // --- LOGIC ENDING ---
    void ShowEnding()
    {
        chatPanel.SetActive(false);
        minigamePanel.SetActive(false);
        endingPanel.SetActive(true);

        string title = "";
        string desc = "";

        if (totalHumanityScore <= 20)
        {
            title = "BAD ENDING 1: BÓNG MA";
            desc = "Bạn tan biến vào hư vô. Thế giới ảo đã nuốt chửng bạn.";
        }
        else if (totalHumanityScore <= 40)
        {
            title = "BAD ENDING 2: KẺ TRẮNG TAY";
            desc = "Bạn nghèo khổ và cô độc. Lòng tham đã hại bạn.";
        }
        else if (totalHumanityScore <= 60)
        {
            title = "BAD ENDING 3: CHIẾC LỒNG SON";
            desc = "Bạn giàu có nhưng vô cảm. Bạn mất đi những người thân yêu nhất.";
        }
        else if (totalHumanityScore <= 80)
        {
            title = "ENDING 4: NGƯỜI VÔ HÌNH";
            desc = "Bạn sống cuộc đời bình lặng, nhưng thiếu đi ngọn lửa đam mê.";
        }
        else
        {
            title = "HAPPY ENDING: HAI THẾ GIỚI";
            desc = "Cái cây cổ thụ tỏa bóng mát. Bạn đã tìm thấy sự cân bằng và hạnh phúc thực sự.";
        }

        endingTitleText.text = title;
        endingDescriptionText.text = desc;
    }
}