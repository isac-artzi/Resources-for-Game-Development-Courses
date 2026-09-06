// PauseMenuController.cs — Activity 2.3: Menus in Space
// Owns the game state (NotStarted -> Playing <-> Paused), wires the Start / Resume / Quit buttons in code, shows and
// hides the world-space menu, and pauses the world with Time.timeScale. The menu itself keeps working while paused
// because the UI and XRI run on unscaled time.
// Attached to: Menu Controller (an empty GameObject). Buttons, canvas and labels are pre-wired by the builder.
// Created by Isac Artzi

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class PauseMenuController : MonoBehaviour
{
    /// <summary>The three states the prototype can be in.</summary>
    public enum GameState { NotStarted, Playing, Paused }

    [Header("Menu")]
    [Tooltip("The Menu Canvas GameObject. Shown in NotStarted and Paused, hidden while Playing.")]
    public GameObject menuRoot;
    public Button startButton;
    public Button resumeButton;
    public Button quitButton;

    [Tooltip("A Text on the canvas we can rewrite to explain the current state.")]
    public Text titleText;

    [Header("Input")]
    [Tooltip("Desktop key that toggles pause while playing (Input System Key enum).")]
    public Key toggleKey = Key.P;

    [Tooltip("Optional controller button for the headset, e.g. the XRI Menu action. Leave empty on desktop.")]
    public InputActionReference toggleAction;

    [Header("Debug")]
    [Tooltip("A 3D label in the scene that shows the state and Time.timeScale.")]
    public TextMesh statusLabel;

    /// <summary>The current state.</summary>
    public GameState State { get; private set; }

    void Start()
    {
        if (toggleAction != null && toggleAction.action != null) toggleAction.action.Enable();

        // TODO 1: Wire the buttons in code so the scene does not depend on hidden Inspector links:
        //             if (startButton  != null) startButton.onClick.AddListener(OnStartPressed);
        //             if (resumeButton != null) resumeButton.onClick.AddListener(OnResumePressed);
        //             if (quitButton   != null) quitButton.onClick.AddListener(OnQuitPressed);
        //         Look at: Button.onClick (a UnityEvent), UnityEvent.AddListener.
        //         Why code: when you later build the menu from a prefab or spawn it, the wiring travels with the script.
        //         Check: aim the ray at Start and press Trigger -> Console prints "Start pressed" (from OnStartPressed).

        SetState(GameState.NotStarted);
    }

    void OnDestroy()
    {
        if (startButton  != null) startButton.onClick.RemoveListener(OnStartPressed);
        if (resumeButton != null) resumeButton.onClick.RemoveListener(OnResumePressed);
        if (quitButton   != null) quitButton.onClick.RemoveListener(OnQuitPressed);
    }

    void Update()
    {
        // TODO 3: Toggle with the keyboard (desktop) or the optional action (headset). Only while the game has started:
        //             bool pressed = (Keyboard.current != null && Keyboard.current[toggleKey].wasPressedThisFrame)
        //                         || (toggleAction != null && toggleAction.action != null && toggleAction.action.WasPressedThisFrame());
        //             if (pressed) { if (State == GameState.Playing) SetState(GameState.Paused);
        //                            else if (State == GameState.Paused) SetState(GameState.Playing); }
        //         Look at: Keyboard.current, KeyControl.wasPressedThisFrame, InputAction.WasPressedThisFrame.
        //         Check: press P while playing -> fireflies freeze and the menu appears; press P again -> they resume.

        if (statusLabel != null)
            statusLabel.text = "State: " + State + "   timeScale: " + Time.timeScale.ToString("F1") + "   [" + toggleKey + "]";
    }

    /// <summary>Moves to a new state and applies its side effects (menu visibility, time scale, button availability).</summary>
    public void SetState(GameState next)
    {
        State = next;

        // TODO 2: Apply the state.
        //         - Menu visible when NOT Playing:            if (menuRoot != null) menuRoot.SetActive(State != GameState.Playing);
        //         - Pause the world when Paused:              Time.timeScale = (State == GameState.Paused) ? 0f : 1f;
        //                                                     AudioListener.pause = (State == GameState.Paused);
        //         - Start button only before the first start; Resume only while paused:
        //                                                     startButton.gameObject.SetActive(State == GameState.NotStarted);
        //                                                     resumeButton.gameObject.SetActive(State == GameState.Paused);
        //         - titleText.text = a one-line hint for the state (e.g. "Paused - the fireflies wait for you").
        //         Look at: Time.timeScale (0 stops Update-driven motion that uses Time.deltaTime, physics, particles, animation),
        //         AudioListener.pause, GameObject.SetActive.
        //         Why timeScale and not "disable everything": one number pauses the whole simulation, and anything that
        //         must keep moving (this menu!) reads Time.unscaledDeltaTime instead.
        //         Check: Paused shows timeScale 0.0 on the status label and the fireflies stop mid-air; Playing shows 1.0.
        Debug.Log("State -> " + State);
    }

    /// <summary>Start button: begin the game.</summary>
    public void OnStartPressed()
    {
        Debug.Log("Start pressed");
        SetState(GameState.Playing);
    }

    /// <summary>Resume button: back to the game.</summary>
    public void OnResumePressed()
    {
        Debug.Log("Resume pressed");
        SetState(GameState.Playing);
    }

    /// <summary>Quit button: stop Play mode in the Editor, quit the app on a device.</summary>
    public void OnQuitPressed()
    {
        Debug.Log("Quit pressed");
        Time.timeScale = 1f;   // never leave the editor paused
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
