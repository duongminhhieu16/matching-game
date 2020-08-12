
using Facebook.Unity;
using Google;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuPresenter : MonoBehaviour
{
    public GameObject loading;
    public GameObject newGame;
    public GameObject loadGame;
    public async void NewGame()
    {
        FacebookController.cnt++;
        PlayerPrefs.SetInt("win", 1);
        PlayerPrefs.SetInt("score", 0);
        ScoreData.level = 1;
        loading.SetActive(true);
        await Task.Delay(500);
        SceneManager.LoadScene(1);
    }
    public async void LoadGame()
    {
        FacebookController.cnt++;
        PlayerPrefs.SetInt("win", 1);
        loading.SetActive(true);
        await Task.Delay(500);
        PlayerPrefs.SetInt("score", FirebaseInit.playerInfo.currentScore);
        ScoreData.level = FirebaseInit.playerInfo.currentLevel;
        Debug.Log(PlayerPrefs.GetInt("score"));
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
