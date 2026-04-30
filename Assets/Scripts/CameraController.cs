using UnityEngine;

/// <summary>
/// Follows the assigned player object by maintaining a fixed positional offset.
/// Attach to the Main Camera and assign the Player reference in the Inspector.
/// </summary>
public class CameraController : MonoBehaviour
{
    [Tooltip("The player GameObject the camera should follow")]
    public GameObject player;

    private Vector3 positionOffset;

    private void Start()
    {
        // Calculate the initial distance between camera and player
        positionOffset = transform.position - player.transform.position;
    }

    private void LateUpdate()
    {
        // Keep the camera at a constant offset from the player
        transform.position = player.transform.position + positionOffset;
    }
}
