using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public AudioSource musicSource;
    public void SetVolume(float value)
    {
        if (musicSource != null) musicSource.volume = value;
    }
    public void SetMute(bool isMuted)
    {
        if (musicSource != null) musicSource.mute = !isMuted;
    }

    private void Awake()
    {
        Time.timeScale = 1f;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void LoadLevel(string sceneName)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }

    public void QuitGame()
    {
        Debug.Log("Quit the game...");
        Application.Quit();
    }
}
