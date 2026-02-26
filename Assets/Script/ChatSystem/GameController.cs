using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public static GameController Instance;

    [Header("--- DỮ LIỆU GAME ---")]
    public List<ChapterData> allChapters;

    [Header("--- UI CHAT ---")]
    public ScrollRect chatScrollRect;
    public Transform chatContent;
    public GameObject choicePanel;

    [Header("--- UI BUTTONS ---")]
    public Button btnOptionA;
    public TextMeshProUGUI txtOptionA;
    public Button btnOptionB;
    public TextMeshProUGUI txtOptionB;

    [Header("--- PREFABS ---")]
    public GameObject npcBubblePrefab;
    public GameObject playerBubblePrefab;

    [Header("--- UI THOUGHT (MỚI) ---")]
    public GameObject thoughtPanel;
    public TextMeshProUGUI thoughtText;

    [Header("--- CẤU HÌNH ---")]
    public float typingSpeed = 0.05f;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    // Hàm được gọi từ OpenPhone.cs hoặc Scene Start
    public void StartChapter()
    {
        // Nếu đã hết ngày (đang đợi ngủ) thì không hiện tin nhắn mới
        if (StoryData.IsEndOfDay)
        {
            Debug.Log("🌙 Đã hết ngày (IsEndOfDay = true). Hãy đi ngủ.");
            return;
        }

        string currentSceneName = SceneManager.GetActiveScene().name;
        ChapterData currentChapterData = null;

        // LOGIC LẤY DATA THEO SCENE
        if (currentSceneName == "city" || currentSceneName == "Ending")
        {
            // Ở City chỉ có 1 đoạn hội thoại (Chap 4b) -> Lấy cái đầu tiên
            if (allChapters.Count > 0)
            {
                currentChapterData = allChapters[0];
            }
        }
        else // Ở Nhà
        {
            int globalIndex = StoryData.CurrentChapterIndex;
            if (globalIndex < allChapters.Count)
            {
                currentChapterData = allChapters[globalIndex];
            }
        }

        if (currentChapterData == null)
        {
            Debug.LogWarning("⚠️ Không tìm thấy Chapter Data cho Scene: " + currentSceneName);
            return;
        }

        int turnIdx = StoryData.CurrentTurnIndex;
        if (turnIdx >= currentChapterData.chatSequence.Count)
        {
            Debug.Log("Đã hết tin nhắn trong Chapter này!");
            return;
        }

        LoadTurn(currentChapterData.chatSequence[turnIdx]);
    }

    void LoadTurn(DialogueTurn turn)
    {
        StartCoroutine(PlayTurnSequence(turn));
    }

    IEnumerator PlayTurnSequence(DialogueTurn turn)
    {
        choicePanel.SetActive(false);
        if (thoughtPanel != null)
            thoughtPanel.SetActive(false);

        foreach (NPCMessage msg in turn.conversation)
        {
            if (!string.IsNullOrEmpty(msg.content) && msg.showBubble)
            {
                SpawnBubble(npcBubblePrefab, msg.content, msg.speakerName);
                StartCoroutine(ScrollToBottom());
            }
            yield return new WaitForSeconds(msg.delayDuration);
        }

        yield return StartCoroutine(RunThoughtSequence(turn));
    }

    IEnumerator RunThoughtSequence(DialogueTurn turn)
    {
        if (!string.IsNullOrEmpty(turn.internalThought))
        {
            thoughtPanel.SetActive(true);
            thoughtText.text = "";

            foreach (char letter in turn.internalThought.ToCharArray())
            {
                if (thoughtPanel == null || thoughtText == null || !thoughtPanel.activeSelf)
                    yield break;
                thoughtText.text += letter;
                yield return new WaitForSeconds(typingSpeed);
            }
            yield return new WaitForSeconds(0.2f);
        }
        else
        {
            thoughtPanel.SetActive(false);
        }

        SetupChoices(turn);
        StartCoroutine(ScrollToBottom());
    }

    void SetupChoices(DialogueTurn turn)
    {
        choicePanel.SetActive(true);

        txtOptionA.text = turn.optionA.optionText;
        btnOptionA.onClick.RemoveAllListeners();
        btnOptionA.onClick.AddListener(() => OnOptionSelected(turn, 0));

        txtOptionB.text = turn.optionB.optionText;
        btnOptionB.onClick.RemoveAllListeners();
        btnOptionB.onClick.AddListener(() => OnOptionSelected(turn, 1));
    }

    void OnOptionSelected(DialogueTurn turn, int choiceIndex)
    {
        OptionData selectedOption = (choiceIndex == 0) ? turn.optionA : turn.optionB;

        // 1. CỘNG ĐIỂM
        StoryData.TotalScore += selectedOption.scoreImpact;
        if (UIThongSo.Instance != null)
        {
            UIThongSo.Instance.AddMood(selectedOption.scoreImpact);
        }

        choicePanel.SetActive(false);
        if (thoughtPanel != null)
            thoughtPanel.SetActive(false);

        // 2. XỬ LÝ CHUYỂN SCENE (VÍ DỤ: TỪ NHÀ -> CITY HOẶC CITY -> NHÀ)
        if (!string.IsNullOrEmpty(selectedOption.nextSceneName))
        {
            Debug.Log("🚕 Đang chuyển scene sang: " + selectedOption.nextSceneName);
            string currentScene = SceneManager.GetActiveScene().name;

            // Nếu đang ở City -> Về Nhà (Hết Chap 4b)
            if (currentScene == "city")
            {
                // Về nhà là phải ngủ luôn
                StoryData.IsEndOfDay = true;
                if (UIThongSo.Instance != null)
                    UIThongSo.Instance.ForceEndOfDay();
            }

            // Reset tin nhắn về 0 để qua scene mới đọc từ đầu
            StoryData.CurrentTurnIndex = 0;
            SceneManager.LoadScene(selectedOption.nextSceneName);
            return;
        }

        // 3. HIỂN THỊ BONG BÓNG PLAYER
        if (selectedOption.showBubble && !string.IsNullOrEmpty(selectedOption.responseText))
        {
            SpawnBubble(playerBubblePrefab, selectedOption.responseText, "Me");
        }

        // 4. KIỂM TRA LƯỢT CUỐI (FINAL TURN)
        if (turn.isFinalTurn)
        {
            StoryData.CurrentTurnIndex++; // Tăng lên để đánh dấu đã xong câu cuối
            CheckEndOfChapterLogic(); // GỌI HÀM KIỂM TRA LOGIC NGỦ/NHIỆM VỤ

            if (choiceIndex == 0)
                return;

            // Nếu chọn B có nhiệm vụ (Đi City)
            QuestData.HasActiveQuest = true;
            QuestData.IsQuestCompleted = false;
            QuestData.ShouldShowQuestUI = true;
            QuestData.QuestText = selectedOption.questText;
            QuestData.TargetTag = selectedOption.targetTag;
            QuestData.QuestScene = selectedOption.questScene;
            QuestData.OriginScene = selectedOption.originScene;
            UIQuest.Instance?.Refresh();
        }
        else
        {
            NextTurn();
        }
    }

    void NextTurn()
    {
        StoryData.CurrentTurnIndex++;
        int turnIdx = StoryData.CurrentTurnIndex;

        ChapterData currentChapter = null;
        string currentScene = SceneManager.GetActiveScene().name;

        if (currentScene == "city")
        {
            if (allChapters.Count > 0)
                currentChapter = allChapters[0];
        }
        else
        {
            int chapterIdx = StoryData.CurrentChapterIndex;
            if (chapterIdx < allChapters.Count)
                currentChapter = allChapters[chapterIdx];
        }

        if (currentChapter == null)
            return;

        // NẾU CÒN TIN NHẮN -> CHẠY TIẾP
        if (turnIdx < currentChapter.chatSequence.Count)
        {
            StartCoroutine(WaitAndLoadNext(currentChapter.chatSequence[turnIdx]));
        }
        else
        {
            // NẾU HẾT TIN NHẮN -> KIỂM TRA LOGIC KẾT THÚC CHƯƠNG
            Debug.Log("✅ Hết danh sách tin nhắn (NextTurn)");
            CheckEndOfChapterLogic();
        }
    }

    // --- HÀM LOGIC QUAN TRỌNG NHẤT: QUYẾT ĐỊNH CÓ ĐƯỢC NGỦ HAY KHÔNG ---
    // Tìm hàm này trong GameController và sửa lại
    void CheckEndOfChapterLogic()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        int currentChapIdx = StoryData.CurrentChapterIndex;

        // --- TRƯỜNG HỢP 1: ĐANG Ở CITY (Chap 4b) ---
        if (currentScene == "city")
        {
            Debug.Log("✅ Đã nói chuyện xong ở City. Hãy tự đi bộ về nhà!");

            // 1. BẬT TRẠNG THÁI "HẾT NGÀY" (Để các scene sau biết là Nam đang mệt)
            StoryData.IsEndOfDay = true;

            // 2. Tua đồng hồ thành đêm (cho hợp lý)
            if (UIThongSo.Instance != null)
                UIThongSo.Instance.ForceEndOfDay();

            // 3. QUAN TRỌNG: KHÔNG CHUYỂN SCENE TỰ ĐỘNG
            // Để người chơi tự đi bộ đến cổng dịch chuyển
            return;
        }

        // --- TRƯỜNG HỢP 2: ĐANG Ở NHÀ (Giữ nguyên logic cũ) ---
        bool isChapter4AtHome = (currentChapIdx == 3);
        if (isChapter4AtHome)
        {
            // Chap 4a ở nhà -> Chưa được ngủ -> Phải đi City
            Debug.Log("⚠️ Chap 4a ở nhà. Chưa được ngủ. Đi City!");
            StoryData.IsEndOfDay = false;
        }
        else
        {
            // Chap 1, 2, 3, 5 -> Xong việc -> Được ngủ
            Debug.Log("✅ Xong việc ở nhà. Mở khóa giường!");
            StoryData.IsEndOfDay = true;
            if (UIThongSo.Instance != null)
                UIThongSo.Instance.ForceEndOfDay();
        }
    }

    // CÁC HÀM PHỤ TRỢ GIỮ NGUYÊN
    IEnumerator WaitAndLoadNext(DialogueTurn turn)
    {
        yield return new WaitForSeconds(0.5f);
        LoadTurn(turn);
    }

    void SpawnBubble(GameObject prefab, string message, string senderName)
    {
        if (prefab == null || chatContent == null)
            return;
        GameObject bubble = Instantiate(prefab, chatContent);
        TextMeshProUGUI[] texts = bubble.GetComponentsInChildren<TextMeshProUGUI>();
        if (texts.Length == 2)
        {
            texts[0].text = senderName;
            texts[1].text = message;
        }
        else if (texts.Length == 1)
        {
            texts[0].text = message;
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(chatContent.GetComponent<RectTransform>());
        StartCoroutine(ScrollToBottom());
    }

    IEnumerator ScrollToBottom()
    {
        yield return new WaitForEndOfFrame();
        Canvas.ForceUpdateCanvases();
        chatScrollRect.verticalNormalizedPosition = 0f;
        chatScrollRect.velocity = Vector2.zero;
    }

    // Hàm tính Ending được gọi từ Script Giường (BedInteract)
    public void CalculateFinalEnding()
    {
        float finalMood = 0;
        if (UIThongSo.Instance != null)
        {
            finalMood = UIThongSo.Instance.currentMood;
        }
        Debug.Log("🏁 TỔNG KẾT GAME! Mood: " + finalMood);

        if (finalMood < 26)
            StoryData.EndingID = 1; // Bad
        else if (finalMood <= 50)
            StoryData.EndingID = 2; // Normal
        else if (finalMood <= 75)
            StoryData.EndingID = 3; // Good
        else
            StoryData.EndingID = 4; // Best

        SceneManager.LoadScene("Ending");
    }
}
