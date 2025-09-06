using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour
{
    public void LoadGame()
    {
        SceneManager.LoadScene("GameScene");
    }
    public void LoadEndScreen()
    {
        SceneManager.LoadScene("EndScreen");
    }
    public void LoadMenu() {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene("MainMenu");
    }
    public void LoadPause()
    {   
        // Load without unloading the current one
        SceneManager.LoadScene("Pause", LoadSceneMode.Additive);
        // Unload when done
        

    }
    public void PauseEnded()
    {
        Time.timeScale=1.0f;
        SceneManager.UnloadSceneAsync("Pause");
    }
    public void QuitGame()
    {
        Debug.Log("Quitting Game...");
        Application.Quit();
        UnityEditor.EditorApplication.isPlaying= false;

    }
    public void RestartGame()
    {
        // Reset time in case it was paused
        Time.timeScale = 1f;

        // Reload the active scene
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }
}
