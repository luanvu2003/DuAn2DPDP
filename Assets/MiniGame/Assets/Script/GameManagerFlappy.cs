using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManagerFlappy : MonoBehaviour
{
    public Text scoreText;
    public int targetScore = 10;
    private int score = 0;

    public void AddScore()
    {
        score++;
        scoreText.text = "Nhiệm vụ kiếm 10 điểm: " + score;

        if (score >= targetScore)
        {
            // Quay lại scene gốc
            SceneManager.LoadScene("BedRoom");
        }
    }

    public void ResetGame()
    {
        score = 0;
        scoreText.text = "Thua cuộc!!";
        // Load lại chính scene minigame
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
