using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;

public class ScoreController : MonoBehaviour
{
    public ScoreData scoreData = new ScoreData();
    // Start is called before the first frame update
    public void UpdateScoreToDatabase()
    {
        if (scoreData.Score > FirebaseInit.highscoreOfUser)
        {
            FirebaseInit.highscoreOfUser = scoreData.Score;
            FirebaseInit.UpdateScore(scoreData.Score);
        }
        
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
