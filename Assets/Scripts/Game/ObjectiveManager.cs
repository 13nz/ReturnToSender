using TMPro;
using UnityEngine;

public class ObjectiveManager : MonoBehaviour
{
    [Header("ui")]
    [SerializeField] private TMP_Text objectiveText;

    [Header("objectives")]
    [TextArea]
    [SerializeField] private string checkpoint0 = "talk to the postmaster";

    [TextArea]
    [SerializeField] private string checkpoint1 = "visit the bookseller";

    [TextArea]
    [SerializeField] private string checkpoint2 = "visit the record store";

    [TextArea]
    [SerializeField] private string checkpoint3 = "visit the bakery";

    [TextArea]
    [SerializeField] private string checkpoint4 = "visit the café";

    [TextArea]
    [SerializeField] private string checkpoint5 = "visit the apothecary";

    [TextArea]
    [SerializeField] private string checkpoint6 = "visit the florist";

    [TextArea]
    [SerializeField] private string checkpoint7 = "visit the tavern";

    [TextArea]
    [SerializeField] private string checkpoint8 = "visit the motel";

    [TextArea]
    [SerializeField] private string checkpoint9 = "visit the lighthouse";

    [TextArea]
    [SerializeField] private string checkpoint10 = "visit the observatory";

    private void Start()
    {
        // waits until the game manager has initialized
        if (GameManager.Instance == null)
        {
            Debug.LogWarning("no game manager was found for the objective manager.");
            return;
        }

        // subscribes after the game manager has initialized
        GameManager.Instance.OnCheckpointChanged += UpdateObjective;

        // displays the correct objective immediately when the scene loads
        UpdateObjective(GameManager.Instance.CurrentCheckpoint);
    }

    private void OnDestroy()
    {
        // removes the event subscription when this object is destroyed
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnCheckpointChanged -= UpdateObjective;
        }
    }

    private void UpdateObjective(int checkpoint)
    {
        if (objectiveText == null)
        {
            //Debug.LogWarning("objective text has not been assigned.");
            return;
        }

        objectiveText.text = GetObjectiveText(checkpoint);

        //Debug.Log("objective updated to checkpoint: " + checkpoint);
    }

    private string GetObjectiveText(int checkpoint)
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
                return "investigation complete";
        }
    }
}