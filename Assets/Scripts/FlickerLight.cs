using UnityEngine;
using System.Collections;

public class FlickerLight : MonoBehaviour
{
    private Light localLight;

    [Header("Stability Settings")]
    public float minStableTime = 2.0f;
    public float maxStableTime = 5.0f;

    [Header("Flicker Settings")]
    public int minFlickers = 3;
    public int maxFlickers = 6;
    public float flickerSpeed = 0.1f;

    void Start()
    {
        localLight = GetComponent<Light>();
        StartCoroutine(Flashing());
    }

    IEnumerator Flashing()
    {
        while (true)
        {
            localLight.enabled = true;
            yield return new WaitForSeconds(Random.Range(minStableTime, maxStableTime));

            int flickerCount = Random.Range(minFlickers, maxFlickers) * 2; 
            
            for (int i = 0; i < flickerCount; i++)
            {
                localLight.enabled = !localLight.enabled;
                yield return new WaitForSeconds(flickerSpeed);
            }

            localLight.enabled = true;
        }
    }
}
