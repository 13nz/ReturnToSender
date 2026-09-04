using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("follow settings")]
    [SerializeField] private Transform target;

    [Header("camera settings")]
    [SerializeField] private float smoothSpeed = 8f;

    private static CameraFollow instance;

    private void Awake()
    {
        // destroys duplicate cameras created when a scene containing a camera is loaded
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // stores this camera as the single persistent camera instance
        instance = this;

        // keeps the camera alive when changing between outdoor and interior scenes
        DontDestroyOnLoad(gameObject);
    }

    private void LateUpdate()
    {
        // stops the camera from trying to follow if no target has been assigned
        if (target == null)
            return;

        // keeps the camera's existing z position so it remains in front of the 2d scene
        Vector3 targetPosition = new Vector3(
            target.position.x,
            target.position.y,
            transform.position.z
        );

        // smoothly moves the camera after player movement has finished for the frame
        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            smoothSpeed * Time.deltaTime
        );
    }
}