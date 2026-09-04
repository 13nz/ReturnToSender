using TMPro;
using UnityEngine;

public class ObjectiveManager : MonoBehaviour
{
    [Header("ui")]
    [SerializeField] private TMP_Text objectiveText;

    [Header("checkpoint objectives")]
    [TextArea(2, 4)]
    [SerializeField] private string checkpoint0 = "Investigate the letter.";

    [TextArea(2, 4)]
    [SerializeField] private string checkpoint1 = "Ask the bookseller about the seal.";

    [TextArea(2, 4)]
    [SerializeField] private string checkpoint2 = "Find out where the record came from.";

    [TextArea(2, 4)]
    [SerializeField] private string checkpoint3 = "Ask the baker about the cupcakes.";

    [TextArea(2, 4)]
    [SerializeField] private string checkpoint4 = "Find out who attended the café event.";

    [TextArea(2, 4)]
    [SerializeField] private string checkpoint5 = "Ask the apothecary about the oils.";

    [TextArea(2, 4)]
    [SerializeField] private string checkpoint6 = "Ask the florist about the flowers.";

    [TextArea(2, 4)]
    [SerializeField] private string checkpoint7 = "Ask the bartender about the wine.";

    [TextArea(2, 4)]
    [SerializeField] private string checkpoint8 = "Ask the motel receptionist about the tourist.";

    [TextArea(2, 4)]
    [SerializeField] private string checkpoint9 = "Ask the lighthouse keeper about the observatory.";

    [TextArea(2, 4)]
    [SerializeField] private string checkpoint10 = "Follow the path to the observatory.";

    private void OnEnable()
    {
        // subscribes to checkpoint changes when the objective canvas is enabled
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnCheckpointChanged += UpdateObjective;
        }
    }

    private void Start()
    {
        // displays the correct objective when the canvas first starts
        if (GameManager.Instance != null)
        {
            UpdateObjective(GameManager.Instance.CurrentCheckpoint);
        }
        else
        {
            Debug.LogWarning("ObjectiveManager could not find the GameManager.");
        }
    }

    private void OnDisable()
    {
        // unsubscribes to prevent missing-reference errors and duplicate listeners
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnCheckpointChanged -= UpdateObjective;
        }
    }

    private void UpdateObjective(int checkpoint)
    {
        if (objectiveText == null)
        {
            Debug.LogWarning("ObjectiveManager is missing its ObjectiveText reference.");
            return;
        }

        objectiveText.text = GetObjectiveForCheckpoint(checkpoint);
    }

    private string GetObjectiveForCheckpoint(int checkpoint)
    {
        switch (checkpoint)
        {
            case 0:
                return checkpoint0;

            case 1:
                return checkpoint1;

            case 2:
                return checkpoint2;

            case 3:
                return checkpoint3;

            case 4:
                return checkpoint4;

            case 5:
                return checkpoint5;

            case 6:
                return checkpoint6;

            case 7:
                return checkpoint7;

            case 8:
                return checkpoint8;

            case 9:
                return checkpoint9;

            case 10:
                return checkpoint10;

            default:
                return string.Empty;
        }
    }
}