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
}
