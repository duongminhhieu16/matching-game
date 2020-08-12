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
    }
}
