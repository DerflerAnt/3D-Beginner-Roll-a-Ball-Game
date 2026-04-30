using UnityEngine;

/// <summary>
/// Continuously rotates the attached GameObject.
/// Used on pickup items so they spin and attract the player's attention.
/// </summary>
public class Rotator : MonoBehaviour
{
    [Tooltip("Rotation speed in degrees per second for each axis")]
    [SerializeField] private Vector3 rotationSpeed = new Vector3(15f, 30f, 45f);

    private void Update()
    {
        transform.Rotate(rotationSpeed * Time.deltaTime);
    }
}
