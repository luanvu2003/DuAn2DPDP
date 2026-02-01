using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour
{
    public static GameController Instance;

    [Header("--- DỮ LIỆU GAME ---")]
    public List<ChapterData> allChapters; // Kéo các ChapterData vào đây
    // [SerializeField] private int currentChapterIndex = 0;
    private int currentTurnIndex = 0;
    private int totalScore = 0;

    [Header("--- UI CHAT ---")]
    public ScrollRect chatScrollRect;       // Kéo Scroll View vào
    public Transform chatContent;           // Kéo object Content trong Viewport vào
    public GameObject choicePanel;          // Panel chứa 2 nút chọn

    [Header("--- UI BUTTONS ---")]
    public Button btnOptionA;
    public TextMeshProUGUI txtOptionA;
    public Button btnOptionB;
    public TextMeshProUGUI txtOptionB;

    [Header("--- PREFABS ---")]
    public GameObject npcBubblePrefab;      // Prefab tin nhắn NPC (có tên ở trên)
    public GameObject playerBubblePrefab;   // Prefab tin nhắn Player (không cần tên)

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // --- THÊM ĐOẠN NÀY ĐỂ TEST ---
        // Mỗi lần bấm Play là reset về Ngày 0, Lượt 0 để test cho dễ
        // Sau này làm xong tính năng Save/Load thì xóa 2 dòng này đi
        StoryData.CurrentChapterIndex = 0;
        StoryData.CurrentTurnIndex = 0;
        StoryData.TotalScore = 0;
        // -----------------------------
    }

    // Hàm này được gọi từ OpenPhone.cs
    public void StartChapter()
    {
        int chapterIdx = StoryData.CurrentChapterIndex;

        // KIỂM TRA HẾT GAME
        if (chapterIdx >= allChapters.Count)
        {
            Debug.Log("🎉 CHÚC MỪNG! BẠN ĐÃ PHÁ ĐẢO GAME!");
            // Gọi UI End Game hoặc Credit tại đây
            return; // Dừng lại, không load chat nữa
        }

        // Nếu chưa hết game thì load bình thường
        int turnIdx = StoryData.CurrentTurnIndex;
        LoadTurn(allChapters[chapterIdx].chatSequence[turnIdx]);
    }

    // Tải nội dung của lượt chat hiện tại
    void LoadTurn(DialogueTurn turn)
    {
        // 1. Sinh bong bóng chat của NPC
        // Tham số: Prefab, Nội dung chat, Tên người nói
        SpawnBubble(npcBubblePrefab, turn.npcDialogue, turn.speakerName);

        // 2. Setup lựa chọn cho người chơi
        SetupChoices(turn);

        // 3. Cuộn xuống dưới cùng
        StartCoroutine(ScrollToBottom());
    }

    void SetupChoices(DialogueTurn turn)
    {
        choicePanel.SetActive(true);

        // Setup Nút A
        txtOptionA.text = turn.optionA.optionText;
        btnOptionA.onClick.RemoveAllListeners();
        btnOptionA.onClick.AddListener(() => OnOptionSelected(turn, 0));

        // Setup Nút B
        txtOptionB.text = turn.optionB.optionText;
        btnOptionB.onClick.RemoveAllListeners();
        btnOptionB.onClick.AddListener(() => OnOptionSelected(turn, 1));
    }

    void OnOptionSelected(DialogueTurn turn, int choiceIndex)
    {
        // 1. Xác định người chơi chọn gì
        string playerText = (choiceIndex == 0) ? turn.optionA.optionText : turn.optionB.optionText;
        int score = (choiceIndex == 0) ? turn.optionA.scoreImpact : turn.optionB.scoreImpact;

        // 2. Cộng điểm
        StoryData.TotalScore += score;
        Debug.Log("Tổng điểm Nhân tính: " + totalScore);

        // 3. Sinh bong bóng chat của Player (Bên phải)
        SpawnBubble(playerBubblePrefab, playerText, "Me");

        // 4. Ẩn bảng chọn
        choicePanel.SetActive(false);

        // 5. Chuyển sang lượt tiếp theo
        NextTurn();
    }

    void NextTurn()
    {
        // TĂNG LƯỢT TRONG SỔ TAY
        StoryData.CurrentTurnIndex++;

        int chapterIdx = StoryData.CurrentChapterIndex;
        int turnIdx = StoryData.CurrentTurnIndex;

        ChapterData currentChapter = allChapters[chapterIdx];

        if (turnIdx < currentChapter.chatSequence.Count)
        {
            StartCoroutine(WaitAndLoadNext(currentChapter.chatSequence[turnIdx]));
        }
        else
        {
            Debug.Log("--- HẾT CHƯƠNG ---");
            // Tăng Chapter lên để lần sau vào game là qua chương mới
            StoryData.CurrentChapterIndex++;
            StoryData.CurrentTurnIndex = 0; // Reset turn về 0 cho chương mới
        }
    }

    IEnumerator WaitAndLoadNext(DialogueTurn turn)
    {
        yield return new WaitForSeconds(0.5f);
        LoadTurn(turn);
    }

    // --- HÀM QUAN TRỌNG: SINH BONG BÓNG CHAT ---
    void SpawnBubble(GameObject prefab, string message, string senderName)
    {
        Debug.LogError("🔴 [BƯỚC 3] Code đã chạy tới SpawnBubble! Đang tạo Clone..."); // <--- Thêm dòng này

        if (prefab == null) Debug.LogError("❌ LỖI: Prefab bị NULL!");
        if (chatContent == null) Debug.LogError("❌ LỖI: ChatContent bị NULL!");

        GameObject bubble = Instantiate(prefab, chatContent);

        // Tự động tìm các TextMeshPro bên trong Prefab
        // QUY ƯỚC: Text[0] là Tên (nếu có), Text[1] là Nội dung
        TextMeshProUGUI[] texts = bubble.GetComponentsInChildren<TextMeshProUGUI>();

        if (texts.Length == 2) // Dành cho NPC (Có tên + Nội dung)
        {
            texts[0].text = senderName; // Cái text nằm trên
            texts[1].text = message;    // Cái text nằm trong bong bóng
        }
        else if (texts.Length == 1) // Dành cho Player (Chỉ có nội dung)
        {
            texts[0].text = message;
        }

        // Bắt buộc UI cập nhật lại kích thước ngay lập tức
        LayoutRebuilder.ForceRebuildLayoutImmediate(chatContent.GetComponent<RectTransform>());
        StartCoroutine(ScrollToBottom());
    }

    // Tự động cuộn xuống đáy
    IEnumerator ScrollToBottom()
    {
        yield return new WaitForEndOfFrame();
        // Cập nhật lại layout lần nữa cho chắc
        Canvas.ForceUpdateCanvases();
        chatScrollRect.verticalNormalizedPosition = 0f; // 0 = Dưới cùng
        chatScrollRect.velocity = Vector2.zero;
    }
}