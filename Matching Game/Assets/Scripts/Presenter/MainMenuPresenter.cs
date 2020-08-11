
using Facebook.Unity;
using Google;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuPresenter : MonoBehaviour
{
    public GameObject loading;
    public async void PlayGame()
    {
        FacebookController.cnt++;
        PlayerPrefs.SetInt("score", 0);
        PlayerPrefs.SetInt("win", 1);
        loading.SetActive(true);
        await Task.Delay(500);
        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        GoogleController.google.GoogleSignOut();
        FacebookController.facebookController.FBLogOut();
        Debug.Log("QUIT!!!");
        PlayerPrefs.SetInt("FBInit", 0);
        Application.Quit();
    }
}
