using UnityEngine;

public class CameraFollowZOnly : MonoBehaviour
{
    public Transform target;
    private Vector3 initialOffset;

    void Start()
    {
        if (target != null)
            initialOffset = transform.position - target.position;
    }

    void LateUpdate()
    {
        if (target != null)
        {
            Vector3 newPos = transform.position;
            newPos.z = target.position.z + initialOffset.z;
            newPos.x = initialOffset.x; 
            transform.position = newPos;
        }
    }
}
