using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // Đừng quên import cái này để load Scene Ending
public class GameController : MonoBehaviour
{
    public static GameController Instance;

    [Header("--- DỮ LIỆU GAME ---")]
    public List<ChapterData> allChapters; // Kéo các ChapterData vào đây

    // [SerializeField] private int currentChapterIndex = 0;
    private int currentTurnIndex = 0;
    private int totalScore = 0;

    [Header("--- UI CHAT ---")]
    public ScrollRect chatScrollRect; // Kéo Scroll View vào
    public Transform chatContent; // Kéo object Content trong Viewport vào
    public GameObject choicePanel; // Panel chứa 2 nút chọn

    [Header("--- UI BUTTONS ---")]
    public Button btnOptionA;
    public TextMeshProUGUI txtOptionA;
    public Button btnOptionB;
    public TextMeshProUGUI txtOptionB;

    [Header("--- PREFABS ---")]
    public GameObject npcBubblePrefab; // Prefab tin nhắn NPC (có tên ở trên)
    public GameObject playerBubblePrefab; // Prefab tin nhắn Player (không cần tên)

    [Header("--- UI THOUGHT (MỚI) ---")]
    public GameObject thoughtPanel; // Kéo cái Panel chứa suy nghĩ vào
    public TextMeshProUGUI thoughtText; // Kéo cái Text hiển thị suy nghĩ vào

    [Header("--- CẤU HÌNH ---")]
    public float typingSpeed = 0.05f; // Tốc độ chạy chữ (càng nhỏ càng nhanh)

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    // Hàm này được gọi từ OpenPhone.cs
    public void StartChapter()
    {
        int chapterIdx = StoryData.CurrentChapterIndex;

        // Kiểm tra xem có còn chapter nào không
        if (chapterIdx >= allChapters.Count)
            return;

        ChapterData currentChapter = allChapters[chapterIdx];
        int turnIdx = StoryData.CurrentTurnIndex;

        // KIỂM TRA: Nếu đã chat hết lượt của chương này rồi
        if (turnIdx >= currentChapter.chatSequence.Count)
        {
            Debug.Log("Đã hết tin nhắn hôm nay rồi!");
            // Bạn có thể hiện một dòng text "Không có tin nhắn mới" ở đây nếu muốn
            return; // Dừng lại, không load gì cả
        }

        // Nếu chưa hết thì load bình thường
        LoadTurn(currentChapter.chatSequence[turnIdx]);
    }

    // Tải nội dung của lượt chat hiện tại
    // Thay đổi hàm LoadTurn thành Coroutine để xử lý việc chờ đợi
    void LoadTurn(DialogueTurn turn)
    {
        // Gọi Coroutine chạy tuần tự
        StartCoroutine(PlayTurnSequence(turn));
    }

    IEnumerator PlayTurnSequence(DialogueTurn turn)
    {
        // 1. Ẩn nút và suy nghĩ
        choicePanel.SetActive(false);
        if (thoughtPanel != null)
            thoughtPanel.SetActive(false);

        // 2. CHẠY LIST TIN NHẮN
        foreach (NPCMessage msg in turn.conversation)
        {
            // LOGIC MỚI: Phải thỏa mãn 2 điều kiện:
            // - Có nội dung (content không rỗng)
            // - Dấu tích showBubble được BẬT
            if (!string.IsNullOrEmpty(msg.content) && msg.showBubble)
            {
                SpawnBubble(npcBubblePrefab, msg.content, msg.speakerName);
                StartCoroutine(ScrollToBottom());
            }
            else
            {
                // Nếu bỏ tích -> Không hiện bong bóng
                // Nhưng vẫn có thể Log ra để biết game đang chạy ngầm
                Debug.Log($"[ẨN] {msg.speakerName}: {msg.content}");
            }

            // Dù có hiện bong bóng hay không thì VẪN PHẢI CHỜ (Delay)
            // Để tạo nhịp độ cho game (ví dụ chờ 2s cho người chơi đọc dòng dẫn chuyện)
            yield return new WaitForSeconds(msg.delayDuration);
        }

        // 3. Hiện Suy Nghĩ
        yield return StartCoroutine(RunThoughtSequence(turn));
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
        OptionData selectedOption = (choiceIndex == 0) ? turn.optionA : turn.optionB;

        // 1. CẬP NHẬT ĐIỂM SỐ (Chung)
        StoryData.TotalScore += selectedOption.scoreImpact;

        // --- SỬA LẠI: ĐỒNG BỘ MOOD (Đưa lên đây để luôn chạy) ---
        if (UIThongSo.Instance != null)
        {
            UIThongSo.Instance.AddMood(selectedOption.scoreImpact);
            Debug.Log($"Đã cập nhật Mood: {selectedOption.scoreImpact} điểm");
        }
        // ---------------------------------------------------------

        // 2. HIỂN THỊ BONG BÓNG CHAT (Nếu có)
        if (selectedOption.showBubble && !string.IsNullOrEmpty(selectedOption.responseText))
        {
            SpawnBubble(playerBubblePrefab, selectedOption.responseText, "Me");
        }

        // 3. ẨN UI LỰA CHỌN
        choicePanel.SetActive(false);
        if (thoughtPanel != null)
            thoughtPanel.SetActive(false);

        // 4. KIỂM TRA: NẾU LÀ LƯỢT CUỐI (FINAL TURN)
        if (turn.isFinalTurn)
        {
            StoryData.CurrentTurnIndex++;

            // Nếu chọn A: Kết thúc hội thoại, không làm gì thêm
            if (choiceIndex == 0)
            {
                // Có thể thêm hàm đóng hội thoại ở đây nếu cần, VD: EndDialogue();
                return;
            }

            // Nếu chọn B: Nhận nhiệm vụ
            QuestData.HasActiveQuest = true;
            QuestData.IsQuestCompleted = false;
            QuestData.ShouldShowQuestUI = true;

            QuestData.QuestText = selectedOption.questText;
            QuestData.TargetTag = selectedOption.targetTag;
            QuestData.QuestScene = selectedOption.questScene;
            QuestData.OriginScene = selectedOption.originScene;

            UIQuest.Instance?.Refresh();
        }
        // 5. NẾU KHÔNG PHẢI LƯỢT CUỐI -> CHUYỂN TIẾP (QUAN TRỌNG)
        else
        {
            NextTurn();
        }
    }

    IEnumerator EndChapterAndStartMinigame(float bonusTime)
    {
        yield return new WaitForSeconds(1f);

        Debug.Log("🚀 CHUYỂN SANG NHIỆM VỤ! Bonus Time: " + bonusTime);

        DialogueTurn currentTurn = allChapters[StoryData.CurrentChapterIndex].chatSequence[
            StoryData.CurrentTurnIndex - 1
        ];
        QuestManager questManager = FindObjectOfType<QuestManager>();
        if (questManager != null)
        {
            questManager.StartQuest(
                currentTurn.optionB.questText,
                currentTurn.optionB.targetTag,
                currentTurn.optionB.questScene,
                currentTurn.optionB.originScene
            );
        }
    }

    void NextTurn()
    {
        StoryData.CurrentTurnIndex++;

        int chapterIdx = StoryData.CurrentChapterIndex;
        int turnIdx = StoryData.CurrentTurnIndex;

        ChapterData currentChapter = allChapters[chapterIdx];

        // Nếu vẫn còn tin nhắn trong chương -> Load tiếp
        if (turnIdx < currentChapter.chatSequence.Count)
        {
            StartCoroutine(WaitAndLoadNext(currentChapter.chatSequence[turnIdx]));
        }
        else
        {
            Debug.Log("--- HẾT CHƯƠNG " + (chapterIdx + 1) + " ---");

            // --- KIỂM TRA HẾT GAME ---
            // Nếu đây là chương 5 (Index là 4 vì đếm từ 0, 1, 2, 3, 4)
            // Hoặc đơn giản là kiểm tra xem còn chương nào phía sau không
            if (chapterIdx >= 4) // Giả sử chương 5 là chương cuối
            {
                CalculateFinalEnding(); // Tính điểm kết thúc
            }
            else
            {
                // Chưa hết game -> Sang chương mới
                StoryData.CurrentChapterIndex++;
                StoryData.CurrentTurnIndex = 0;

                // Load lại Scene để refresh UI cho ngày mới
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
    }

    void CalculateFinalEnding()
    {
        float finalMood = 0;
        if (UIThongSo.Instance != null)
        {
            finalMood = UIThongSo.Instance.currentMood;
        }

        Debug.Log("🏁 TỔNG KẾT GAME! Mood cuối cùng: " + finalMood);

        // Logic chia ending như bạn yêu cầu
        if (finalMood < 26) // Dưới 25 (tức là từ 1 đến 25, vì 0 đã chết rồi)
        {
            StoryData.EndingID = 1; // Bad
        }
        else if (finalMood >= 26 && finalMood <= 50)
        {
            StoryData.EndingID = 2; // Normal
        }
        else if (finalMood >= 51 && finalMood <= 75)
        {
            StoryData.EndingID = 3; // Good
        }
        else // >= 76
        {
            StoryData.EndingID = 4; // Best
        }

        // Chuyển sang Scene Ending
        // Thay "EndingScene" bằng tên thật scene của bạn
        SceneManager.LoadScene("Ending");
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

        if (prefab == null)
            Debug.LogError("❌ LỖI: Prefab bị NULL!");
        if (chatContent == null)
            Debug.LogError("❌ LỖI: ChatContent bị NULL!");

        GameObject bubble = Instantiate(prefab, chatContent);

        // Tự động tìm các TextMeshPro bên trong Prefab
        // QUY ƯỚC: Text[0] là Tên (nếu có), Text[1] là Nội dung
        TextMeshProUGUI[] texts = bubble.GetComponentsInChildren<TextMeshProUGUI>();

        if (texts.Length == 2) // Dành cho NPC (Có tên + Nội dung)
        {
            texts[0].text = senderName; // Cái text nằm trên
            texts[1].text = message; // Cái text nằm trong bong bóng
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

    private void Update()
    {
        // --- CHEAT CODE: NHẤN R ĐỂ RESET GAME ---
        // Chỉ dùng khi test, sau này build game nhớ xóa hoặc ẩn đi
        if (Input.GetKeyDown(KeyCode.R))
        {
            // 1. Reset dữ liệu trong RAM về 0
            StoryData.CurrentChapterIndex = 0;
            StoryData.CurrentTurnIndex = 0;
            StoryData.TotalScore = 0;

            // 2. Xóa dữ liệu trong ổ cứng (nếu bạn có dùng PlayerPrefs)
            PlayerPrefs.DeleteAll();

            // 3. Load lại màn chơi hiện tại
            UnityEngine.SceneManagement.SceneManager.LoadScene(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
            );

            Debug.Log("🔄 ĐÃ RESET GAME VỀ TỪ ĐẦU (NGÀY 0 - TURN 0)!");
        }
    }
}
