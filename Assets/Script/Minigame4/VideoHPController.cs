using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

public class VideoHPFillController : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public Image hpFillImage;

    public float drainSpeed = 0.2f;
    public float addPerPress = 0.1f;

    private bool win = false;

    void Start()
    {
        if (hpFillImage)
        {
            hpFillImage.fillAmount = 0;
            hpFillImage.gameObject.SetActive(true);
        }

        if (videoPlayer)
            videoPlayer.loopPointReached += OnVideoEnd;
    }

    void Update()
    {
        if (win) return;
        if (!videoPlayer || !videoPlayer.isPlaying) return;

        // ===== NHẤN SPACE → TĂNG =====
        if (Input.GetKeyDown(KeyCode.Space))
        {
            hpFillImage.fillAmount += addPerPress;
        }

        // ===== CHECK THẮNG NGAY =====
        if (hpFillImage.fillAmount >= 1f)
        {
            win = true;
            hpFillImage.fillAmount = 1f;
            Debug.Log("YOU WIN");
            return; // ⛔ KHÔNG CHO TỤT NỮA
        }

        // ===== TỤT =====
        hpFillImage.fillAmount -= drainSpeed * Time.deltaTime;
        hpFillImage.fillAmount = Mathf.Clamp01(hpFillImage.fillAmount);
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        if (!win)
        {
            Debug.Log("BẠN ĐÃ THUA");
        }
    }
}
