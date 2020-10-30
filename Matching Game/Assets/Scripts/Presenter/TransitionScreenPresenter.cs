using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionScreenPresenter : MonoBehaviour
{
    
    public void BackToMenu()
    {
        SceneManager.LoadScene(0);
    }
    public void LoadLevel()
    {
        SceneManager.LoadScene(1);
    }
    public void NextLevel()
    {
        ScoreData.level++;
        ScoreData.startingMoves += ScoreData.level - 1;
        PlayerPrefs.SetInt("numMoves", ScoreData.startingMoves);
    }
}
