using UnityEngine;

public class CameraFly : MonoBehaviour
{
    public Transform target;
    public float speed = 4f;

    void Start()
    {
        if (target == null) target = transform;
    }

    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, target.position, speed * Time.deltaTime);

        transform.rotation = Quaternion.Slerp(transform.rotation, target.rotation, speed * Time.deltaTime);
    }

    public void ChangeTarget(Transform newTarget)
    {
        target = newTarget;
    }
}
