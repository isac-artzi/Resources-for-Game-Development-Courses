// DemoMenu.cs — Activity 7.6: Build, Record, Present
// A world-space list of buttons, one per scene, so that during the live demo (or while recording) you can jump
// straight to any environment or feature instead of playing through. The same menu exists in every scene, so
// you can always get back. Keyboard 1–9 does the same on desktop.
// Attached to: "Demo Menu" canvas in every scene the builder creates (buttons and names pre-wired).
// Created by Isac Artzi

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DemoMenu : MonoBehaviour
{
    [Header("Entries (same length, same order)")]
    [Tooltip("Exact scene names, all present in File > Build Profiles > Scene List.")]
    public string[] sceneNames = new string[0];

    [Tooltip("One Button per scene name. The builder creates them.")]
    public Button[] buttons = new Button[0];

    [Header("Look")]
    [Tooltip("Button color for the scene you are currently in.")]
    public Color currentColor = new Color(0.2f, 0.7f, 0.35f);

    [Tooltip("Button color for every other scene.")]
    public Color otherColor = new Color(0.25f, 0.45f, 0.85f);

    [Header("Read-only")]
    public bool isLoading;

    void Start()
    {
        // TODO 1: Wire the buttons. for (int i = 0; i < buttons.Length && i < sceneNames.Length; i++) {
        //             string target = sceneNames[i];               // <- copy into a LOCAL before the lambda
        //             buttons[i].onClick.AddListener(() => Load(target)); }
        //         Why the local copy: a lambda captures the VARIABLE, not the value. Capturing 'i' or 'sceneNames[i]'
        //         directly would make every button load the LAST scene once the loop has finished.
        //         Look at: Button.onClick.AddListener(UnityAction), C# closures.
        //         Check: pointing the controller ray at "Demo_Forest" and pressing the trigger loads that scene.

        HighlightCurrent();
    }

    void Update()
    {
        // TODO 3: Keyboard fallback. var kb = Keyboard.current; if (kb == null) return;
        //         for i in 0..min(9, sceneNames.Length)-1: var key = (Key)((int)Key.Digit1 + i);
        //             if (kb[key].wasPressedThisFrame) Load(sceneNames[i]);
        //         Look at: the Key enum — Digit1..Digit9 are consecutive, so Digit1 + i is the (i+1)th number key.
        //         Check: pressing 2 loads the second scene in the list.
    }

    /// <summary>Loads the named scene asynchronously (ignored if already there or already loading).</summary>
    public void Load(string sceneName)
    {
        if (isLoading || string.IsNullOrEmpty(sceneName)) return;
        if (SceneManager.GetActiveScene().name == sceneName) return;

        // TODO 2: isLoading = true; Debug.Log("[DemoMenu] -> " + sceneName);
        //         var op = SceneManager.LoadSceneAsync(sceneName);
        //         if (op == null) { isLoading = false; Debug.LogError("[DemoMenu] not in Scene List: " + sceneName); }
        //         (No fade here — Activity 7.1's SceneLoader has one; add it to your project if you want the blink.)
        //         Look at: SceneManager.LoadSceneAsync returns null when the scene is not in the Scene List.
        //         Check: the new scene appears and its own Demo Menu shows the new current scene highlighted.
        Debug.Log("[DemoMenu] Load(" + sceneName + ") — implement TODO 2");
    }

    /// <summary>Colors the button of the active scene differently.</summary>
    public void HighlightCurrent()
    {
        // TODO 4: string current = SceneManager.GetActiveScene().name;
        //         for each i: var img = buttons[i].GetComponent<Image>(); if (img != null) img.color = sceneNames[i] == current ? currentColor : otherColor;
        //         Look at: Image.color. Why: during a demo you glance at the menu to know where you are without reading.
        //         Check: the current scene's button is green, the others blue, in every scene.
    }
}
