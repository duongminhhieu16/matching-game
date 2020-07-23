
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuPresenter : MonoBehaviour
{
    public void PlayGame()
    {
        PlayerPrefs.SetInt("score", 0);
        PlayerPrefs.SetInt("win", 1);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitGame()
    {
        Debug.Log("QUIT!!!");
        Application.Quit();
    }
}
