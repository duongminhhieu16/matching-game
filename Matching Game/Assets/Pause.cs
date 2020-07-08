using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Pause : MonoBehaviour
{
    bool isPaused = false;
    
    public void pauseGame()
    {
        if (isPaused)
        {
            Time.timeScale = 1;
            isPaused = false;
        }
        else
        {
            Time.timeScale = 0;
            isPaused = true;
        }
    }

    public void backToMenu()
    {
        SceneManager.LoadScene(0);
    }
    public void nextLevel()
    {
        SceneManager.LoadScene(1);
    }
}
