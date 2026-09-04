using UnityEngine;

public class EndingGate : MonoBehaviour
{
    [Header("gate settings")]
    [SerializeField] private string requiredNpcId = "lighthouse";

    private Collider2D[] colliders;
    private Renderer[] renderers;

    private void Awake()
    {
        // gets every collider attached to this fence and its children
        colliders = GetComponentsInChildren<Collider2D>(true);

        // gets every renderer attached to this fence and its children
        renderers = GetComponentsInChildren<Renderer>(true);
    }

    private void Start()
    {
        // updates the gate when the main scene loads
        UpdateGate();
    }

    public void UpdateGate()
    {
        // makes sure the game manager is available before checking progression
        if (GameManager.Instance == null)
            return;

        // checks whether the lighthouse keeper has been spoken to
        bool unlocked = GameManager.Instance.HasSpokenToNPC(requiredNpcId);

        // enables or disables every fence collider
        foreach (Collider2D collider in colliders)
        {
            if (collider != null)
                collider.enabled = !unlocked;
        }

        // shows or hides every fence renderer
        foreach (Renderer renderer in renderers)
        {
            if (renderer != null)
                renderer.enabled = !unlocked;
        }
    }
}