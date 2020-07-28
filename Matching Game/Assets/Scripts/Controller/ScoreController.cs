using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
public class ScoreController : MonoBehaviour
{
    public ScoreData scoreData = new ScoreData();
    // Start is called before the first frame update
    public async void UpdateScoreFromDatabase()
    {
        await FirebaseInit.LoadSpecificUser();
        if (FirebaseInit.playerInfo.userScore > scoreData.HighScore)
        {
            scoreData.HighScore = FirebaseInit.playerInfo.userScore;
        }
        FirebaseInit.playerInfo.userScore = scoreData.Score;
        
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
