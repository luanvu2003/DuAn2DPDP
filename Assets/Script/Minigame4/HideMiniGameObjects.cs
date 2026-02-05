using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class StoryController : MonoBehaviour
{
    [Header("Turn OFF when PlayStory")]
    public GameObject baCuImage;
    public GameObject miniGameCanvas1;
    public GameObject miniGameCanvas2;
    public GameObject phoneObject;

    [Header("Cutscene")]
    public GameObject cutsceneVideo;     // BAN ĐẦU TẮT
    public VideoPlayer videoPlayer;

    [Header("UI")]
    public GameObject startText;         // CHA - LUÔN BẬT
    public TMP_Text startTMPText;        // CON - TẮT SẴN
    public Button tapCatcher;            // BUTTON FULL MÀN HÌNH

    [Header("Blink Setting")]
    public float blinkDuration = 0.45f;

    private Tween blinkTween;

    // =======================
    // GỌI KHI NHẤN NÚT PLAY
    // =======================
    public void PlayStory()
    {
        Debug.Log("PLAY STORY");

        // 1. TẮT CÁC CANVAS / OBJECT
        if (baCuImage) baCuImage.SetActive(false);
        if (miniGameCanvas1) miniGameCanvas1.SetActive(false);
        if (miniGameCanvas2) miniGameCanvas2.SetActive(false);
        if (phoneObject) phoneObject.SetActive(false);

        // 2. CHƯA BẬT VIDEO
        if (cutsceneVideo) cutsceneVideo.SetActive(false);

        // 3. BẬT TEXT CON
        if (startTMPText != null)
        {
            startTMPText.gameObject.SetActive(true);
            startTMPText.color = Color.white;
        }

        // 4. DOTWEEN NHẤP NHÁY
        blinkTween = startTMPText
            .DOColor(Color.yellow, blinkDuration)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);

        // 5. BẬT BUTTON BẮT TAP
        if (tapCatcher) tapCatcher.gameObject.SetActive(true);
    }

    // =======================
    // ĐƯỢC GỌI KHI TAP MÀN HÌNH
    // =======================
    public void StartCutscene()
    {
        // TẮT TWEEN
        if (blinkTween != null) blinkTween.Kill();

        // TẮT TEXT
        if (startTMPText != null)
            startTMPText.gameObject.SetActive(false);

        // TẮT BUTTON TAP
        if (tapCatcher) tapCatcher.gameObject.SetActive(false);

        // BẬT + PLAY VIDEO
        if (cutsceneVideo) cutsceneVideo.SetActive(true);
        if (videoPlayer) videoPlayer.Play();
    }
}
