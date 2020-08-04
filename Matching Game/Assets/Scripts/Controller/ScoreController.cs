using UnityEngine;
using UnityEngine.SceneManagement;
public class ScoreController : MonoBehaviour
{
    public ScoreData scoreData = new ScoreData();
    // Start is called before the first frame update
    public async void UpdateScoreToDatabase()
    {
        await FirebaseInit.LoadScoreOfCurrentPlayer();
        if (FirebaseInit.highscoreOfUser > scoreData.HighScore)
        {
            scoreData.HighScore = FirebaseInit.highscoreOfUser;
        }
        FirebaseInit.UpdateScore(scoreData.HighScore);
        PlayerPrefs.SetInt("highScore", FirebaseInit.highscoreOfUser);
    }
    public void CheckIfGameEnd()
    {
        bool win = PlayerPrefs.GetInt("win") > 1;
        if (win)
        {
            Debug.Log("YOU WIN!!!");
            PlayerPrefs.SetInt("score", scoreData.Score);
            SceneManager.LoadScene(2);
        }
        else
        {
            Debug.Log("YOU LOSE!!!!!!!!!!!!!!!!!!!!!!");
            PlayerPrefs.SetInt("score", 0);
            SceneManager.LoadScene(3);
        }
    }
    
}
