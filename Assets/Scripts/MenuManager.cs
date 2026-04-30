using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// Handles all main menu interactions including scene transitions,
/// audio configuration and application lifecycle.
/// Attached to the MenuLogic object in the MainMenu scene.
/// </summary>
public class MenuManager : MonoBehaviour
{
    [Header("Audio")]
    [Tooltip("Reference to the background music AudioSource")]
    public AudioSource musicSource;

    [Header("Transition")]
    [Tooltip("Delay before scene actually loads (seconds)")]
    [Range(0f, 2f)]
    [SerializeField] private float sceneLoadDelay = 0f;

    private bool isTransitioning = false;

    // ──────────────────────────────────────────────
    //  Unity lifecycle
    // ──────────────────────────────────────────────

    private void Awake()
    {
        // Make sure the game runs at normal speed when returning from gameplay
        ResetTimeScale();

        // Show the mouse cursor so the player can interact with the UI
        SetCursorVisible(true);
    }

    // ──────────────────────────────────────────────
    //  Public API — called by UI elements
    // ──────────────────────────────────────────────

    /// <summary>
    /// Starts loading the specified scene. Called from the level-select buttons.
    /// </summary>
    public void LoadLevel(string sceneName)
    {
        if (isTransitioning) return;

        if (sceneLoadDelay > 0f)
        {
            StartCoroutine(TransitionToScene(sceneName));
        }
        else
        {
            ResetTimeScale();
            SceneManager.LoadScene(sceneName);
        }
    }

    /// <summary>
    /// Closes the application. Called from the Exit button.
    /// </summary>
    public void QuitGame()
    {
#if UNITY_EDITOR
        Debug.Log("[MenuManager] QuitGame() — would quit in a build.");
#else
        Application.Quit();
#endif
    }

    /// <summary>
    /// Adjusts the music volume. Connected to the VolumeSlider's OnValueChanged.
    /// </summary>
    public void SetVolume(float value)
    {
        if (musicSource != null)
            musicSource.volume = Mathf.Clamp01(value);
    }

    /// <summary>
    /// Toggles mute state. Connected to the MuteToggle's OnValueChanged.
    /// </summary>
    public void SetMute(bool isMuted)
    {
        if (musicSource != null)
            musicSource.mute = !isMuted;
    }

    // ──────────────────────────────────────────────
    //  Internal helpers
    // ──────────────────────────────────────────────

    private void ResetTimeScale()
    {
        Time.timeScale = 1f;
    }

    private void SetCursorVisible(bool visible)
    {
        Cursor.visible = visible;
        Cursor.lockState = visible ? CursorLockMode.None : CursorLockMode.Locked;
    }

    private IEnumerator TransitionToScene(string target)
    {
        isTransitioning = true;
        yield return new WaitForSeconds(sceneLoadDelay);
        ResetTimeScale();
        SceneManager.LoadScene(target);
    }
}
