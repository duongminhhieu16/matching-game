﻿using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionScreenPresenter : MonoBehaviour
{
    bool isPaused = false;
    
    public void PauseGame()
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

    public void BackToMenu()
    {
        SceneManager.LoadScene(0);
    }
    public void LoadLevel()
    {
        SceneManager.LoadScene(1);
    }
}