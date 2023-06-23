using UnityEngine;

public class FixedRotationCamera : MonoBehaviour
{
    public Transform target;  // The parent game object to follow
    public Vector3 offset;    // The offset from the target's position

    void LateUpdate()
    {
        if (target == null)
            return;

        // Calculate the desired position of the camera
        Vector3 desiredPosition = target.position + offset;

        // Set the camera's position to the desired position
        transform.position = desiredPosition;

        // Set the camera's rotation to match the target's rotation
        //transform.rotation = target.rotation;
    }
}