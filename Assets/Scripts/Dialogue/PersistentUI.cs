using UnityEngine;

public class PersistentUI : MonoBehaviour
{
    private void Awake()
    {
        // keeps the dialogue interface alive when changing scenes
        DontDestroyOnLoad(gameObject);
    }
}