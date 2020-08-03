
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuPresenter : MonoBehaviour
{
    
    public void PlayGame()
    {
        FacebookController.cnt++;
        PlayerPrefs.SetInt("score", 0);
        PlayerPrefs.SetInt("win", 1);
        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        Debug.Log("QUIT!!!");
        PlayerPrefs.SetInt("FBInit", 0);
        Application.Quit();
    }
}
