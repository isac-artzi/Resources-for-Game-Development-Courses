// ActivityMenu.cs
// Adds the "Activity" menu to the Unity menu bar with two helpers:
//   Activity > Open README            — opens this activity's README.md in your default editor
//   Activity > Check Setup            — reports whether XRI samples and OpenXR are ready
// The activity-specific "Activity > Build Starter Scene" lives in ActivitySceneBuilder.cs.
//
// Created by Isac Artzi

using System.IO;
using UnityEditor;
using UnityEngine;

namespace ActivityTools
{
    public static class ActivityMenu
    {
        [MenuItem("Activity/Open README", priority = 0)]
        public static void OpenReadme()
        {
            string projectRoot = Directory.GetParent(Application.dataPath).FullName;
            string readme = Path.Combine(projectRoot, "README.md");
            if (File.Exists(readme)) EditorUtility.OpenWithDefaultApp(readme);
            else Debug.LogWarning("[Activity] README.md not found at project root.");
        }

        [MenuItem("Activity/Check Setup", priority = 1)]
        public static void CheckSetup()
        {
            bool rig = HasPrefab("XR Origin (XR Rig)");
            bool sim = HasPrefab("XR Interaction Simulator") || HasPrefab("XR Device Simulator");

            string report = "[Activity] Setup check\n" +
                            (rig ? "  OK   " : "  MISSING  ") + "XRI Starter Assets sample (XR Origin (XR Rig) prefab)\n" +
                            (sim ? "  OK   " : "  MISSING  ") + "XRI XR Interaction Simulator sample (desktop testing)\n" +
                            "  Import samples from: Window > Package Manager > XR Interaction Toolkit > Samples\n" +
                            "  Enable OpenXR from:  Edit > Project Settings > XR Plug-in Management (PC tab and Android tab)\n" +
                            "  Then run:            Activity > Build Starter Scene";
            if (rig && sim) Debug.Log(report); else Debug.LogWarning(report);
        }

        static bool HasPrefab(string prefabName)
        {
            foreach (string guid in AssetDatabase.FindAssets("\"" + prefabName + "\" t:Prefab"))
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (Path.GetFileNameWithoutExtension(path) == prefabName) return true;
            }
            return false;
        }
    }
}
