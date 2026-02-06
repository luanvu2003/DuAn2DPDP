using UnityEngine;
using UnityEngine.SceneManagement;
public class MenuManager : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("BedRoom");
    }
    public void QuitGame()
    {
        Application.Quit();
    }

}
