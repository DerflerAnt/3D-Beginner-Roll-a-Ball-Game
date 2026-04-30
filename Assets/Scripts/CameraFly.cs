using UnityEngine;

/// <summary>
/// Smoothly interpolates the camera towards a target transform.
/// Used in the MainMenu scene for cinematic camera transitions
/// between menu panels (main → level select → back).
/// </summary>
public class CameraFly : MonoBehaviour
{
    [Tooltip("Transform the camera should fly towards")]
    public Transform target;

    [Tooltip("Interpolation speed (higher = faster transitions)")]
    public float speed = 4f;

    private void Start()
    {
        if (target == null)
            target = transform;
    }

    private void Update()
    {
        float step = speed * Time.deltaTime;

        transform.position = Vector3.Lerp(transform.position, target.position, step);
        transform.rotation = Quaternion.Slerp(transform.rotation, target.rotation, step);
    }

    /// <summary>
    /// Switches the fly-to destination. Called by UI buttons via UnityEvent.
    /// </summary>
    public void ChangeTarget(Transform newTarget)
    {
        target = newTarget;
    }
}
