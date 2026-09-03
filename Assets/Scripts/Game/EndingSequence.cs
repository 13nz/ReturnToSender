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
    [Header("letter placement")]
    [SerializeField] private float letterHoldHeightOffset = -0.2f;

    [Header("ending timing")]
    [SerializeField] private float chipalopeAppearDelay = 0.3f;
    [SerializeField] private float chipalopeFadeInDuration = 0.8f;
    [SerializeField] private float letterAppearDelay = 0.25f;
    [SerializeField] private float letterHoldDelay = 0.2f;
    [SerializeField] private float downwardDuration = 0.8f;
    [SerializeField] private float fadeDuration = 1.2f;

    private bool endingStarted;

    private Vector3 chipalopeStartPosition;
    private Color chipalopeStartColor;
    private Color letterStartColor;

    private void Awake()
    {
        // stores the original Chipalope position and colors.
        chipalopeStartPosition = chipalope.position;
        chipalopeStartColor = chipalopeRenderer.color;
        letterStartColor = letterRenderer.color;

        // keeps the Chipalope hidden until the ending begins.
        chipalopeRenderer.enabled = false;

        // keeps the letter hidden until the ending begins.
        letterRenderer.enabled = false;
    }

    public void StartEnding()
    {
        // prevents the ending from starting more than once.
        if (endingStarted)
            return;

        endingStarted = true;

        // prevents the player from moving during the ending.
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
        // waits before the Chipalope appears.
        yield return new WaitForSeconds(chipalopeAppearDelay);

        // enables the Chipalope renderer while keeping it fully transparent.
        chipalopeRenderer.enabled = true;

        Color invisibleChipalopeColor = chipalopeStartColor;
        invisibleChipalopeColor.a = 0f;
        chipalopeRenderer.color = invisibleChipalopeColor;

        // fades the Chipalope into view.
        yield return FadeChipalopeIn();

        // waits briefly before the letter appears.
        yield return new WaitForSeconds(letterAppearDelay);

        // finds the player and places the letter above them.
        GameObject playerObject =
            GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            letter.position =
                playerObject.transform.position +
                Vector3.up * letterHeightAbovePlayer;
        }

        // makes the letter visible.
        letterRenderer.enabled = true;
        letterRenderer.color = letterStartColor;

        // places the letter slightly below the Chipalope's transform position.
        Vector3 letterTargetPosition =
            chipalope.position +
            Vector3.up * letterHoldHeightOffset;

        yield return MoveTransform(
            letter,
            letterTargetPosition,
            letterTravelDuration
        );

        // pauses briefly to show the handoff.
        yield return new WaitForSeconds(letterHoldDelay);

        // moves both the Chipalope and letter slightly downward together.
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

        // hides the letter after the downward movement finishes.
        letterRenderer.enabled = false;

        // fades the Chipalope out.
        yield return FadeChipalopeOut();

        // disables the objects after the ending finishes.
        chipalope.gameObject.SetActive(false);
        letter.gameObject.SetActive(false);

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

            float progress = elapsedTime / fadeDuration;

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

            float progress = elapsedTime / duration;
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

            float progress = elapsedTime / duration;
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
}