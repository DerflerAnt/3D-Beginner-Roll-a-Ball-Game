using UnityEngine;
using System.Collections;

/// <summary>
/// Creates a realistic flickering effect on a Light component.
/// The light stays stable for a random interval, then rapidly
/// toggles on/off several times to simulate electrical instability.
/// </summary>
public class FlickerLight : MonoBehaviour
{
    [Header("Stable Phase")]
    [Tooltip("Minimum seconds the light stays on before flickering")]
    public float minStableTime = 2.0f;
    [Tooltip("Maximum seconds the light stays on before flickering")]
    public float maxStableTime = 5.0f;

    [Header("Flicker Phase")]
    [Tooltip("Minimum number of on/off toggles per flicker burst")]
    public int minFlickers = 3;
    [Tooltip("Maximum number of on/off toggles per flicker burst")]
    public int maxFlickers = 6;
    [Tooltip("Duration of each individual toggle (seconds)")]
    public float flickerSpeed = 0.1f;

    private Light attachedLight;

    private void Start()
    {
        attachedLight = GetComponent<Light>();
        StartCoroutine(FlickerRoutine());
    }

    /// <summary>
    /// Infinite coroutine: stable → flicker burst → repeat.
    /// </summary>
    private IEnumerator FlickerRoutine()
    {
        while (true)
        {
            // ── Stable phase ──
            attachedLight.enabled = true;
            float stableDuration = Random.Range(minStableTime, maxStableTime);
            yield return new WaitForSeconds(stableDuration);

            // ── Flicker burst ──
            int toggles = Random.Range(minFlickers, maxFlickers) * 2;
            for (int i = 0; i < toggles; i++)
            {
                attachedLight.enabled = !attachedLight.enabled;
                yield return new WaitForSeconds(flickerSpeed);
            }

            // Guarantee light is back on after the burst
            attachedLight.enabled = true;
        }
    }
}
