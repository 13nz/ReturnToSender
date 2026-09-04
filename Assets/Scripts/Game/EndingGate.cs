using UnityEngine;

public class EndingGate : MonoBehaviour
{
    [Header("gate settings")]
    [SerializeField] private string requiredNpcId = "lighthouse_keeper";

    [Header("gate objects")]
    [SerializeField] private GameObject closed;
    [SerializeField] private GameObject closedDown;

    private Collider2D[] gateColliders;
    private Renderer[] gateRenderers;

    private void Awake()
    {
        // gets colliders and renderers on the gate itself,
        // but does not include the sign children
        gateColliders = GetComponents<Collider2D>();
        gateRenderers = GetComponents<Renderer>();

        // finds the sign children automatically if they were not assigned
        if (closed == null)
        {
            Transform closedTransform = transform.Find("closed");

            if (closedTransform != null)
                closed = closedTransform.gameObject;
        }

        if (closedDown == null)
        {
            Transform closedDownTransform = transform.Find("closed_down");

            if (closedDownTransform != null)
                closedDown = closedDownTransform.gameObject;
        }
    }

    private void Start()
    {
        UpdateGate();
    }

    public void UpdateGate()
    {
        if (GameManager.Instance == null)
            return;

        bool unlocked = GameManager.Instance.HasSpokenToNPC(requiredNpcId);

        // the actual gate becomes disabled after the lighthouse keeper
        foreach (Collider2D gateCollider in gateColliders)
        {
            if (gateCollider != null)
                gateCollider.enabled = !unlocked;
        }

        foreach (Renderer gateRenderer in gateRenderers)
        {
            if (gateRenderer != null)
                gateRenderer.enabled = !unlocked;
        }

        // the original standing sign is visible only while closed
        if (closed != null)
            closed.SetActive(!unlocked);

        // the fallen sign is visible only after the gate opens
        if (closedDown != null)
            closedDown.SetActive(unlocked);
    }
}