using System.Collections;
using UnityEngine;

public class EndingSequence : MonoBehaviour
{
    [Header("ending objects")]
    [SerializeField] private Transform chipalope;
    [SerializeField] private SpriteRenderer chipalopeRenderer;
    [SerializeField] private Transform letter;
    [SerializeField] private SpriteRenderer letterRenderer;

    [Header("ending movement")]
    [SerializeField] private float letterHeightAbovePlayer = 0.45f;
    [SerializeField] private float letterTravelDuration = 1.2f;
    [SerializeField] private float downwardDistance = 0.3f;
    [SerializeField] private float letterHoldHeightOffset = -0.2f;

    [Header("ending timing")]
    [SerializeField] private float chipalopeAppearDelay = 0.3f;
    [SerializeField] private float chipalopeFadeInDuration = 0.8f;
    [SerializeField] private float letterAppearDelay = 0.25f;
    [SerializeField] private float letterHoldDelay = 0.2f;
    [SerializeField] private float downwardDuration = 0.8f;
    [SerializeField] private float fadeDuration = 1.2f;

    [Header("ending sounds")]
    [SerializeField] private float soundVolume = 1f;

    [Header("ending camera")]
    [SerializeField] private float cameraMoveUpAmount = 1.5f;
    [SerializeField] private float cameraMoveDuration = 1f;
    [SerializeField] private float cameraReturnDuration = 1f;

    private Camera mainCamera;

    // this is captured when the ending actually begins
    private Vector3 endingStartCameraPosition;
    private MonoBehaviour cameraFollow;

    // audio
    private AudioSource audioSource;
    private AudioClip whooshSound;
    private AudioClip chipalopeSound;

    private bool endingStarted;

    private Vector3 chipalopeStartPosition;
    private Color chipalopeStartColor;
    private Color letterStartColor;

    private void Awake()
    {
        // gets the existing AudioSource if one is already attached
        audioSource = GetComponent<AudioSource>();

        // adds an AudioSource if one is missing
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // loads the encounter sounds from Resources
        whooshSound = Resources.Load<AudioClip>("Audio/Sounds/whoosh");
        chipalopeSound = Resources.Load<AudioClip>("Audio/Sounds/chipalope_sound");

        // AudioSource settings
        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.spatialBlend = 0f;
        audioSource.volume = soundVolume;
        audioSource.pitch = 1f;
        audioSource.panStereo = 0f;
        audioSource.reverbZoneMix = 1f;

        // stores the original Chipalope position and colors
        chipalopeStartPosition = chipalope.position;
        chipalopeStartColor = chipalopeRenderer.color;
        letterStartColor = letterRenderer.color;

        // keeps the Chipalope hidden until the ending begins
        chipalopeRenderer.enabled = false;

        // keeps the letter hidden until the ending begins
        letterRenderer.enabled = false;

        // finds the active camera
        mainCamera = Camera.main;
    }

    public void StartEnding()
    {
        // prevents the ending from starting more than once
        if (endingStarted)
        {
            return;
        }

        endingStarted = true;

        // finds the camera-follow script on the active camera
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        if (mainCamera != null)
        {
            endingStartCameraPosition = mainCamera.transform.position;

            // replace "CameraFollow" with the actual name of your camera-follow script
            cameraFollow = mainCamera.GetComponent<CameraFollow>();

            if (cameraFollow != null)
            {
                cameraFollow.enabled = false;
            }
        }

        // // refreshes the camera reference in case the camera changed
        // if (mainCamera == null)
        // {
        //     mainCamera = Camera.main;
        // }

        // // captures the camera's actual position at the moment
        // // the ending starts, rather than its scene-spawn position
        // if (mainCamera != null)
        // {
        //     endingStartCameraPosition = mainCamera.transform.position;
        // }

        // disables player movement during the ending
        PlayerMovement playerMovement =
            FindFirstObjectByType<PlayerMovement>();

        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }

        StartCoroutine(PlayEnding());
    }

    private IEnumerator PlayEnding()
    {
        // moves the camera upward from its actual starting position
        if (mainCamera != null)
        {
            Vector3 cameraTargetPosition =
                endingStartCameraPosition +
                Vector3.up * cameraMoveUpAmount;

            yield return MoveCameraTo(
                cameraTargetPosition,
                cameraMoveDuration
            );
        }

        // waits before the creature appears
        yield return new WaitForSeconds(chipalopeAppearDelay);

        // enables the Chipalope renderer while keeping it fully transparent
        chipalopeRenderer.enabled = true;

        Color invisibleChipalopeColor = chipalopeStartColor;
        invisibleChipalopeColor.a = 0f;
        chipalopeRenderer.color = invisibleChipalopeColor;

        // plays the whoosh as the Chipalope begins appearing
        if (whooshSound != null)
        {
            audioSource.PlayOneShot(whooshSound);
        }

        // fades the Chipalope into view
        yield return FadeChipalopeIn();

        // plays the sound immediately after it finishes appearing
        if (chipalopeSound != null)
        {
            audioSource.PlayOneShot(chipalopeSound);
        }

        // waits briefly before the letter appears
        yield return new WaitForSeconds(letterAppearDelay);

        // finds the player and places the letter above them
        GameObject playerObject =
            GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            letter.position =
                playerObject.transform.position +
                Vector3.up * letterHeightAbovePlayer;
        }

        // makes the letter visible
        letterRenderer.enabled = true;
        letterRenderer.color = letterStartColor;

        // places the letter slightly below its transform position
        Vector3 letterTargetPosition =
            chipalope.position +
            Vector3.up * letterHoldHeightOffset;

        yield return MoveTransform(
            letter,
            letterTargetPosition,
            letterTravelDuration
        );

        // pauses briefly to show the handoff
        yield return new WaitForSeconds(letterHoldDelay);

        // moves both the Chipalope and letter slightly downward together
        Vector3 chipalopeTargetPosition =
            chipalope.position +
            Vector3.down * downwardDistance;

        Vector3 letterDownwardTargetPosition =
            letter.position +
            Vector3.down * downwardDistance;

        yield return MoveBothObjectsDown(
            chipalopeTargetPosition,
            letterDownwardTargetPosition,
            downwardDuration
        );

        // hides the letter after the downward movement finishes
        letterRenderer.enabled = false;

        // fades the Chipalope out
        yield return FadeChipalopeOut();

        // disables the objects after the ending finishes
        chipalope.gameObject.SetActive(false);
        letter.gameObject.SetActive(false);

        // returns the camera to the position it had
        // when the ending originally started
        if (mainCamera != null)
        {
            yield return MoveCameraTo(
                endingStartCameraPosition,
                cameraReturnDuration
            );
        }

        // allows the camera to follow the player again after the ending
        if (cameraFollow != null)
        {
            cameraFollow.enabled = true;
        }

        Debug.Log("ending complete.");
    }

    private IEnumerator FadeChipalopeIn()
    {
        float elapsedTime = 0f;
        Color chipalopeColor = chipalopeRenderer.color;

        while (elapsedTime < chipalopeFadeInDuration)
        {
            elapsedTime += Time.deltaTime;

            float progress =
                elapsedTime / chipalopeFadeInDuration;

            chipalopeColor.a = Mathf.Lerp(0f, 1f, progress);
            chipalopeRenderer.color = chipalopeColor;

            yield return null;
        }

        chipalopeColor.a = 1f;
        chipalopeRenderer.color = chipalopeColor;
    }

    private IEnumerator FadeChipalopeOut()
    {
        float elapsedTime = 0f;
        Color chipalopeColor = chipalopeRenderer.color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;

            float progress =
                elapsedTime / fadeDuration;

            chipalopeColor.a = Mathf.Lerp(1f, 0f, progress);
            chipalopeRenderer.color = chipalopeColor;

            yield return null;
        }

        chipalopeColor.a = 0f;
        chipalopeRenderer.color = chipalopeColor;
    }

    private IEnumerator MoveTransform(
        Transform target,
        Vector3 destination,
        float duration
    )
    {
        Vector3 startingPosition = target.position;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            float progress =
                elapsedTime / duration;

            progress = Mathf.SmoothStep(0f, 1f, progress);

            target.position = Vector3.Lerp(
                startingPosition,
                destination,
                progress
            );

            yield return null;
        }

        target.position = destination;
    }

    private IEnumerator MoveBothObjectsDown(
        Vector3 chipalopeDestination,
        Vector3 letterDestination,
        float duration
    )
    {
        Vector3 chipalopeStartingPosition = chipalope.position;
        Vector3 letterStartingPosition = letter.position;

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            float progress =
                elapsedTime / duration;

            progress = Mathf.SmoothStep(0f, 1f, progress);

            chipalope.position = Vector3.Lerp(
                chipalopeStartingPosition,
                chipalopeDestination,
                progress
            );

            letter.position = Vector3.Lerp(
                letterStartingPosition,
                letterDestination,
                progress
            );

            yield return null;
        }

        chipalope.position = chipalopeDestination;
        letter.position = letterDestination;
    }

    private IEnumerator MoveCameraTo(
        Vector3 destination,
        float duration
    )
    {
        if (mainCamera == null)
        {
            yield break;
        }

        Vector3 startingPosition = mainCamera.transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            float progress =
                elapsedTime / duration;

            progress = Mathf.SmoothStep(0f, 1f, progress);

            mainCamera.transform.position = Vector3.Lerp(
                startingPosition,
                destination,
                progress
            );

            yield return null;
        }

        mainCamera.transform.position = destination;
    }
}