using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// Controls the player ball — movement, pickups, collisions,
/// jump / sprint mechanics, and end-game flow.
/// </summary>
public class PlayerController : MonoBehaviour
{
    // ── Movement ──────────────────────────────────
    [Header("Movement")]
    public float speed = 0;
    public float jumpForce = 5.0f;
    public float sprintMultiplier = 2.0f;

    // ── UI references ────────────────────────────
    [Header("HUD")]
    public TextMeshProUGUI countText;

    [Header("End Game UI")]
    public GameObject endGamePanel;
    public TextMeshProUGUI endGameTitle;
    public TextMeshProUGUI finalScoreText;

    // ── VFX / SFX prefabs ────────────────────────
    [Header("Effects")]
    public GameObject collectEffectPrefab;
    public GameObject playerExplosionPrefab;
    public GameObject enemyDissolvePrefab;
    public GameObject playerLight;

    [Header("Audio")]
    public AudioSource pickupSound;
    public AudioClip playerDeathClip;
    public AudioSource enemyWinSoundSource;

    // ── Private state ────────────────────────────
    private Rigidbody rb;
    private int pickupCount;
    private float inputX;
    private float inputZ;
    private float currentSprintFactor = 1f;
    private bool grounded;

    private const int TotalPickups = 12;
    private const float EndScreenDelay = 2f;

    // ─────────────────────────────────────────────
    //  Unity lifecycle
    // ─────────────────────────────────────────────

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        pickupCount = 0;
        RefreshHUD();
    }

    private void FixedUpdate()
    {
        Vector3 direction = new Vector3(inputX, 0f, inputZ);
        rb.AddForce(direction * speed * currentSprintFactor);
    }

    // ─────────────────────────────────────────────
    //  Input callbacks (New Input System)
    // ─────────────────────────────────────────────

    private void OnMove(InputValue movementValue)
    {
        Vector2 axis = movementValue.Get<Vector2>();
        inputX = axis.x;
        inputZ = axis.y;
    }

    private void OnJump()
    {
        if (!grounded) return;
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        grounded = false;
    }

    private void OnSprint(InputValue value)
    {
        currentSprintFactor = value.isPressed ? sprintMultiplier : 1f;
    }

    // ─────────────────────────────────────────────
    //  Collision & Trigger logic
    // ─────────────────────────────────────────────

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("PickUp")) return;

        PlayPickupFeedback(other.transform.position);
        other.gameObject.SetActive(false);

        pickupCount++;
        RefreshHUD();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Enemy")) return;

        HandlePlayerDeath();
    }

    private void OnCollisionStay(Collision collision)
    {
        foreach (ContactPoint cp in collision.contacts)
        {
            if (cp.normal.y > 0.7f)
            {
                grounded = true;
                return;
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        grounded = false;
    }

    // ─────────────────────────────────────────────
    //  HUD & end-game flow
    // ─────────────────────────────────────────────

    private void RefreshHUD()
    {
        countText.text = "Count: " + pickupCount;

        if (pickupCount >= TotalPickups)
            OnAllPickupsCollected();
    }

    private void OnAllPickupsCollected()
    {
        ShowEndScreen("MISSION ACCOMPLISHED", Color.green);
        DestroyRemainingEnemies();
    }

    private void DestroyRemainingEnemies()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemies.Length == 0) return;

        if (enemyWinSoundSource != null)
            enemyWinSoundSource.Play();

        foreach (GameObject e in enemies)
        {
            if (enemyDissolvePrefab != null)
                Instantiate(enemyDissolvePrefab, e.transform.position, Quaternion.identity);

            Destroy(e);
        }
    }

    private void HandlePlayerDeath()
    {
        if (playerDeathClip != null)
            AudioSource.PlayClipAtPoint(playerDeathClip, transform.position);

        if (playerExplosionPrefab != null)
            Instantiate(playerExplosionPrefab, transform.position, Quaternion.identity);

        if (playerLight != null)
            playerLight.SetActive(false);

        GetComponent<MeshRenderer>().enabled = false;

        rb.linearVelocity = Vector3.zero;
        rb.isKinematic = true;

        ShowEndScreen("YOU DIED", Color.red);
        GetComponent<UnityEngine.InputSystem.PlayerInput>().enabled = false;
    }

    private void ShowEndScreen(string message, Color color)
    {
        StartCoroutine(PresentEndScreen(message, color));
    }

    private IEnumerator PresentEndScreen(string message, Color color)
    {
        yield return new WaitForSecondsRealtime(EndScreenDelay);

        if (countText != null)
            countText.gameObject.SetActive(false);

        endGamePanel.SetActive(true);
        endGameTitle.text = message;
        endGameTitle.color = color;
        finalScoreText.text = "Final Score: " + pickupCount;

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // ─────────────────────────────────────────────
    //  Scene navigation (called from end-game UI)
    // ─────────────────────────────────────────────

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    // ─────────────────────────────────────────────
    //  Helpers
    // ─────────────────────────────────────────────

    private void PlayPickupFeedback(Vector3 position)
    {
        if (pickupSound != null)
            pickupSound.Play();

        if (collectEffectPrefab != null)
            Instantiate(collectEffectPrefab, position, Quaternion.identity);
    }
}
