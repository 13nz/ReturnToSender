using UnityEngine;

public class Building : MonoBehaviour
{
    [Header("building settings")]
    [SerializeField] private string npcId;

    [Header("closed sign")]
    [SerializeField] private GameObject closedSign;

    [Header("door")]
    [SerializeField] private Door door;

    private void Start()
    {
        // updates the building state when the main scene loads
        UpdateBuildingState();
    }

    public void UpdateBuildingState()
    {
        // prevents the building from failing if the game manager is unavailable
        if (GameManager.Instance == null)
            return;

        // checks whether this npc has already been completed
        bool alreadyCompleted = GameManager.Instance.HasSpokenToNPC(npcId);

        // checks whether this building belongs to the current checkpoint
        NPCCheckpoint currentCheckpoint =
            GameManager.Instance.GetCurrentNPCCheckpoint();

        bool isCurrentBuilding =
            currentCheckpoint != null &&
            currentCheckpoint.npcId == npcId;

        // the building is available if it has already been completed
        // or if it is the current building in the progression
        bool canEnter = alreadyCompleted || isCurrentBuilding;

        // shows the closed sign only while the building is unavailable
        if (closedSign != null)
        {
            closedSign.SetActive(!canEnter);
        }

        // enables the door only while the building is available
        if (door != null)
        {
            door.SetInteractable(canEnter);
        }
    }
}