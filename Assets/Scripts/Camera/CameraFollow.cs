using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("follow settings")]
    [SerializeField] private Transform target;

    [Header("camera settings")]
    [SerializeField] private float smoothSpeed = 8f;

    private void LateUpdate()
    {
        // stops the camera from trying to follow if no target has been assigned.
        if (target == null)
            return;

        // keeps the camera's existing z position so it remains in front of the 2d scene.
        Vector3 targetPosition = new Vector3(
            target.position.x,
            target.position.y,
            transform.position.z
        );

        // smoothly moves the camera toward the player's position after all player movement is complete.
        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            smoothSpeed * Time.deltaTime
        );
    }
}