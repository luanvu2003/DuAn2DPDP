using System.Collections;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class VideoHPFillController : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public Image hpFillImage;

    public GameObject youWinCanvas;   // canvas YOU WIN
    public float winDelay = 2f;        // đợi mấy giây rồi chuyển scene
    public string nextSceneName;

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

        if (youWinCanvas)
            youWinCanvas.SetActive(false);

        if (videoPlayer)
            videoPlayer.loopPointReached += OnVideoEnd;
    }

    void Update()
    {
        if (win) return;
        if (!videoPlayer || !videoPlayer.isPlaying) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            hpFillImage.fillAmount = Mathf.Clamp01(
                hpFillImage.fillAmount + addPerPress
            );
        }

        if (hpFillImage.fillAmount >= 1f)
        {
            WinGame();
            return;
        }

        hpFillImage.fillAmount -= drainSpeed * Time.deltaTime;
        hpFillImage.fillAmount = Mathf.Clamp01(hpFillImage.fillAmount);
    }

    void WinGame()
    {
        win = true;

        // 🛑 DỪNG VIDEO
        if (videoPlayer)
        {
            videoPlayer.Pause();
            videoPlayer.enabled = false;
        }

        // 👀 HIỆN YOU WIN
        if (youWinCanvas)
            youWinCanvas.SetActive(true);

        Debug.Log("YOU WIN");

        // ⏳ ĐỢI → LOAD SCENE
        StartCoroutine(LoadNextScene());
    }

    IEnumerator LoadNextScene()
    {
        yield return new WaitForSeconds(winDelay);
        SceneManager.LoadScene(nextSceneName);
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        if (!win)
        {
            Debug.Log("BẠN ĐÃ THUA");
        }
    }
}
