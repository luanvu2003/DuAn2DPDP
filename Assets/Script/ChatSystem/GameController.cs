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

    [Header("--- UI THOUGHT (MỚI) ---")]
    public GameObject thoughtPanel;      // Kéo cái Panel chứa suy nghĩ vào
    public TextMeshProUGUI thoughtText;  // Kéo cái Text hiển thị suy nghĩ vào
    [Header("--- CẤU HÌNH ---")]
    public float typingSpeed = 0.05f; // Tốc độ chạy chữ (càng nhỏ càng nhanh)
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
        // --- SỬA ĐOẠN NÀY ---
        // Chỉ sinh bong bóng NPC nếu có nội dung thoại
        if (!string.IsNullOrEmpty(turn.npcDialogue))
        {
            SpawnBubble(npcBubblePrefab, turn.npcDialogue, turn.speakerName);
        }
        // --------------------

        // 2. Ẩn nút chọn và khung suy nghĩ
        choicePanel.SetActive(false);
        if (thoughtPanel != null) thoughtPanel.SetActive(false);

        // 3. Chạy hiệu ứng suy nghĩ -> Rồi mới hiện nút
        StartCoroutine(RunThoughtSequence(turn));

        // 4. Cuộn xuống
        StartCoroutine(ScrollToBottom());
    }
    IEnumerator RunThoughtSequence(DialogueTurn turn)
    {
        // Kiểm tra xem có suy nghĩ không
        if (!string.IsNullOrEmpty(turn.internalThought))
        {
            // Bật khung suy nghĩ lên
            thoughtPanel.SetActive(true);
            thoughtText.text = ""; // Xóa trắng nội dung cũ

            // --- HIỆU ỨNG ĐÁNH MÁY (Typewriter) ---
            foreach (char letter in turn.internalThought.ToCharArray())
            {
                // KIỂM TRA AN TOÀN: Nếu bảng suy nghĩ hoặc text bị hủy thì dừng ngay
                if (thoughtPanel == null || thoughtText == null || !thoughtPanel.activeSelf)
                    yield break; // Thoát khỏi Coroutine ngay lập tức

                thoughtText.text += letter;
                yield return new WaitForSeconds(typingSpeed);
            }

            // Đợi thêm 1 chút sau khi chạy xong chữ cho người chơi kịp đọc
            yield return new WaitForSeconds(0.2f);
        }
        else
        {
            // Nếu không có suy nghĩ thì tắt bảng đi
            thoughtPanel.SetActive(false);
        }

        // --- CHẠY CHỮ XONG MỚI HIỆN NÚT CHỌN ---
        SetupChoices(turn);

        // Cuộn xuống lần nữa để chắc chắn nút chọn không bị che
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
        // Lấy dữ liệu của lựa chọn vừa bấm (A hoặc B)
        OptionData selectedOption = (choiceIndex == 0) ? turn.optionA : turn.optionB;

        // 1. CỘNG ĐIỂM
        StoryData.TotalScore += selectedOption.scoreImpact;

        // 2. XỬ LÝ HIỆN TIN NHẮN (Dựa trên dấu tích showBubble)
        // Nếu dấu tích ĐƯỢC BẬT -> Hiện bong bóng
        if (selectedOption.showBubble)
        {
            // Kiểm tra thêm cho chắc: Phải có chữ mới hiện
            if (!string.IsNullOrEmpty(selectedOption.responseText))
            {
                SpawnBubble(playerBubblePrefab, selectedOption.responseText, "Me");
            }
        }
        else
        {
            // Nếu dấu tích BỊ TẮT -> Không làm gì cả (Im lặng/Hành động)
            Debug.Log("Người chơi chọn hành động ẩn (Không hiện chat).");
        }

        // 3. TẮT UI & CHUYỂN TIẾP
        choicePanel.SetActive(false);
        if (thoughtPanel != null) thoughtPanel.SetActive(false);

        if (turn.isFinalTurn)
        {
            StartCoroutine(EndChapterAndStartMinigame(selectedOption.minigameBonusTime));
        }
        else
        {
            NextTurn();
        }
    }

    IEnumerator EndChapterAndStartMinigame(float bonusTime)
    {
        yield return new WaitForSeconds(1f); // Đợi 1 xíu cho mượt

        Debug.Log("🚀 CHUYỂN SANG DỌN RÁC! Bonus Time: " + bonusTime);

        // Lưu thời gian bonus vào StoryData để Minigame đọc được
        // StoryData.BonusTime = bonusTime; 

        // Load Scene Minigame (Ví dụ tên scene là "MiniGame_DonRac")
        // UnityEngine.SceneManagement.SceneManager.LoadScene("MiniGame_DonRac");
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