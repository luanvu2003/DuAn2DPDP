using UnityEngine;

public class StoryController : MonoBehaviour
{
    [Header("Turn OFF at Start")]
    public GameObject baCuImage;        // ảnh bà cụ
    public GameObject miniGameCanvas1;  // canvas mini game
    public GameObject miniGameCanvas2;  // canvas mini game
    public GameObject phoneObject;      // điện thoại

    [Header("Cutscene")]
    public GameObject cutsceneVideo;    // GameObject có VideoPlayer

    public void PlayStory()
    {
        Debug.Log("PLAY STORY CALLED");

        if (baCuImage) baCuImage.SetActive(false);
        if (miniGameCanvas1) miniGameCanvas1.SetActive(false);
        if (miniGameCanvas2) miniGameCanvas2.SetActive(false);
        if (phoneObject) phoneObject.SetActive(false);

        // BẬT VIDEO CUTSCENE
        if (cutsceneVideo) cutsceneVideo.SetActive(true);
    }
}
