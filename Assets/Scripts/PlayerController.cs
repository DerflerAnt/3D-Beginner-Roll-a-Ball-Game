using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public float speed = 0;
    public TextMeshProUGUI countText;
    public GameObject collectEffectPrefab;
    public GameObject playerExplosionPrefab;
    public GameObject enemyDissolvePrefab;

    public GameObject playerLight;

    public AudioSource pickupSound;
    public AudioClip playerDeathClip;
    public AudioSource enemyWinSoundSource;

    public float jumpForce = 5.0f;
    public float sprintMultiplier = 2.0f;

    [Header("End Game UI")]
    public GameObject endGamePanel;
    public TextMeshProUGUI endGameTitle;
    public TextMeshProUGUI finalScoreText;

    private float currentSprint = 1.0f;
    private bool isGrounded;
    private Rigidbody rb;
    private int count;
    private float movementX;
    private float movementY;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        count = 0;

        SetCountText();
    }

    void OnMove(InputValue movementValue)
    {
        Vector2 movementVector = movementValue.Get<Vector2>();

        movementX = movementVector.x;
        movementY = movementVector.y;

    }

    void SetCountText()
    {
        countText.text = "Count: " + count.ToString();

        if (count >= 12)
        {
            ShowEndScreen("MISSION ACCOMPLISHED", Color.green);

            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

            if (enemies.Length > 0)
            {
                if (enemyWinSoundSource != null) enemyWinSoundSource.Play();

                foreach (GameObject e in enemies)
                {
                    if (enemyDissolvePrefab != null)
                    {
                        Instantiate(enemyDissolvePrefab, e.transform.position, Quaternion.identity);
                    }
                    Destroy(e);
                }
            }
        }
    }

    void FixedUpdate()
    {
        Vector3 movement = new Vector3(movementX, 0.0f, movementY);

        rb.AddForce(movement * speed * currentSprint);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PickUp"))
        {
            if (pickupSound != null) pickupSound.Play();

            if (collectEffectPrefab != null)
            {
                Instantiate(collectEffectPrefab, other.transform.position, Quaternion.identity);
            }

            other.gameObject.SetActive(false);
            count = count + 1;

            SetCountText();
        }
    }

    void OnJump()
    {
        if (isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }
    }

    void OnSprint(InputValue value)
    {
        currentSprint = value.isPressed ? sprintMultiplier : 1.0f;
    }

    void ShowEndScreen(string message, Color textColor)
    {
        StartCoroutine(DelayEndScreen(message, textColor));
    }

    IEnumerator DelayEndScreen(string message, Color textColor)
    {
        yield return new WaitForSecondsRealtime(2f);

        if (countText != null) countText.gameObject.SetActive(false);

        endGamePanel.SetActive(true);
        endGameTitle.text = message;
        endGameTitle.color = textColor;
        finalScoreText.text = "Final Score: " + count.ToString();

        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            if (playerDeathClip != null)
            {
                AudioSource.PlayClipAtPoint(playerDeathClip, transform.position);
            }

            if (playerExplosionPrefab != null)
            {
                Instantiate(playerExplosionPrefab, transform.position, Quaternion.identity);
            }

            if (playerLight != null)
            {
                playerLight.SetActive(false);
            }

            GetComponent<MeshRenderer>().enabled = false;

            rb.linearVelocity = Vector3.zero;
            rb.isKinematic = true;

            ShowEndScreen("YOU DIED", Color.red);

            this.enabled = false;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            if (contact.normal.y > 0.7f)
            {
                isGrounded = true;
                return;
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        isGrounded = false;
    }
}
