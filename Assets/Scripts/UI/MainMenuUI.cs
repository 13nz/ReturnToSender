using UnityEngine;
using UnityEngine.InputSystem;

public class MainMenuUI : MonoBehaviour
{
    [Header("menu panels")]
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject settingsPanel;

    [Header("menu state")]
    private bool menuOpen;

    private void Awake()
    {
        // keeps the menu canvas alive when changing scenes
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        // starts with the menu closed
        SetMenuOpen(false);
    }

    private void Update()
    {
        if (Keyboard.current == null)
            return;

        // opens or closes the pause menu with esc
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            ToggleMenu();
        }
    }

    private void ToggleMenu()
    {
        SetMenuOpen(!menuOpen);
    }

    private void SetMenuOpen(bool open)
    {
        // stores whether the menu is currently open
        menuOpen = open;

        // pauses or resumes the game.
        Time.timeScale = menuOpen ? 0f : 1f;

        if (menuOpen)
        {
            // starts on the main menu whenever the pause menu is opened
            ShowMainMenu();
        }
        else
        {
            // hides both menu pages when the menu is closed
            menuPanel.SetActive(false);
            settingsPanel.SetActive(false);
        }
    }

    public void ResumeGame()
    {
        // closes the menu and resumes gameplay
        SetMenuOpen(false);
    }

    public void OpenSettings()
    {
        // hides the main menu and displays the settings page
        menuPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        // returns from the settings page to the main menu
        ShowMainMenu();
    }
    

    private void ShowMainMenu()
    {
        // displays the main menu page
        menuPanel.SetActive(true);

        // hides the settings page
        settingsPanel.SetActive(false);
    }

    public void ExitGame()
    {
        // restores normal game speed before leaving the game
        Time.timeScale = 1f;

        // quits the built game.
        Application.Quit();

#if UNITY_EDITOR
        // stops play mode when testing inside the editor
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}