// SceneBuilderUtil.cs
// Shared helper used by every activity's ActivitySceneBuilder.
// It creates a fresh scene, drops in the XR rig + simulator, and offers
// small helpers for primitives, labels, lights, materials and triggers.
//
// You do NOT need to edit this file for the activity. Read it if you are
// curious how the starter scene is assembled — everything here is plain
// Unity Editor scripting.
//
// Created by Isac Artzi

using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

namespace ActivityTools
{
    public static class SceneBuilderUtil
    {
        public const string ScenesFolder = "Assets/Scenes";
        public const string MaterialsFolder = "Assets/Materials";

        // ------------------------------------------------------------------
        // Scene lifecycle
        // ------------------------------------------------------------------

        /// <summary>Creates a new empty scene (with a directional light) and removes the default camera,
        /// because the XR Origin brings its own camera.</summary>
        public static Scene NewScene()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
            var cam = GameObject.Find("Main Camera");
            if (cam != null) Object.DestroyImmediate(cam);
            return scene;
        }

        /// <summary>Saves the scene under Assets/Scenes and registers it in Build Settings.</summary>
        public static string SaveScene(Scene scene, string sceneName)
        {
            EnsureFolder(ScenesFolder);
            string path = ScenesFolder + "/" + sceneName + ".unity";
            EditorSceneManager.SaveScene(scene, path);

            var list = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
            if (!list.Exists(s => s.path == path))
                list.Add(new EditorBuildSettingsScene(path, true));
            EditorBuildSettings.scenes = list.ToArray();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[Activity] Starter scene saved to " + path + ". Open README.md and start on the TODOs.");
            return path;
        }

        /// <summary>Creates nested folders under Assets/ if they do not exist.</summary>
        public static void EnsureFolder(string assetPath)
        {
            if (AssetDatabase.IsValidFolder(assetPath)) return;
            string[] parts = assetPath.Split('/');
            string current = parts[0]; // "Assets"
            for (int i = 1; i < parts.Length; i++)
            {
                string next = current + "/" + parts[i];
                if (!AssetDatabase.IsValidFolder(next))
                    AssetDatabase.CreateFolder(current, parts[i]);
                current = next;
            }
        }

        // ------------------------------------------------------------------
        // XR rig and simulator
        // ------------------------------------------------------------------

        /// <summary>
        /// Adds an XR Origin. Prefers the "XR Origin (XR Rig)" prefab from the XRI Starter Assets sample.
        /// Falls back to the GameObject > XR > XR Origin (VR) menu, and finally to a plain camera rig
        /// so the scene still opens even if XRI samples were not imported yet.
        /// Also drops in the XR Interaction Simulator so you can test with mouse + keyboard.
        /// </summary>
        public static GameObject AddXrRig(Vector3 position)
        {
            GameObject rig = InstantiatePrefabByName("XR Origin (XR Rig)");

            if (rig == null)
            {
                Selection.activeGameObject = null;
                if (EditorApplication.ExecuteMenuItem("GameObject/XR/XR Origin (VR)"))
                    rig = Selection.activeGameObject;
            }

            if (rig == null)
            {
                Debug.LogWarning("[Activity] XR Interaction Toolkit rig not found. " +
                                 "Import Window > Package Manager > XR Interaction Toolkit > Samples > Starter Assets, " +
                                 "then run Activity > Build Starter Scene again. A plain camera rig was added for now.");
                rig = new GameObject("XR Origin (PLACEHOLDER - import XRI Starter Assets)");
                var camGo = new GameObject("Main Camera");
                camGo.tag = "MainCamera";
                camGo.AddComponent<Camera>();
                camGo.AddComponent<AudioListener>();
                camGo.transform.SetParent(rig.transform, false);
                camGo.transform.localPosition = new Vector3(0f, 1.6f, 0f);
            }

            rig.name = "XR Origin (XR Rig)";
            rig.transform.position = position;
            AddSimulator();
            return rig;
        }

        /// <summary>Adds the XRI simulator prefab (new "XR Interaction Simulator" or legacy "XR Device Simulator").</summary>
        public static GameObject AddSimulator()
        {
            GameObject sim = InstantiatePrefabByName("XR Interaction Simulator");
            if (sim == null) sim = InstantiatePrefabByName("XR Device Simulator");
            if (sim == null)
            {
                Debug.LogWarning("[Activity] No XR simulator prefab found. Import Package Manager > XR Interaction Toolkit > " +
                                 "Samples > XR Interaction Simulator to test on desktop without a headset.");
            }
            return sim;
        }

        /// <summary>Finds a prefab by exact file name anywhere in the project (including imported samples).</summary>
        public static GameObject InstantiatePrefabByName(string prefabName)
        {
            string[] guids = AssetDatabase.FindAssets("\"" + prefabName + "\" t:Prefab");
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (Path.GetFileNameWithoutExtension(path) != prefabName) continue;
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (prefab == null) continue;
                return (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            }
            return null;
        }

        // ------------------------------------------------------------------
        // Materials
        // ------------------------------------------------------------------

        /// <summary>Creates (or reuses) a flat-colored material asset that works in both Built-in and URP.</summary>
        public static Material MakeMaterial(string name, Color color)
        {
            EnsureFolder(MaterialsFolder);
            string path = MaterialsFolder + "/" + name + ".mat";
            var existing = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (existing != null)
            {
                existing.color = color;
                EditorUtility.SetDirty(existing);
                return existing;
            }
            var mat = new Material(PickLitShader());
            mat.color = color;
            AssetDatabase.CreateAsset(mat, path);
            return mat;
        }

        public static Material MakeEmissiveMaterial(string name, Color color, float emission)
        {
            var mat = MakeMaterial(name, color);
            if (mat.HasProperty("_EmissionColor"))
            {
                mat.EnableKeyword("_EMISSION");
                mat.SetColor("_EmissionColor", color * emission);
                EditorUtility.SetDirty(mat);
            }
            return mat;
        }

        static Shader PickLitShader()
        {
            if (GraphicsSettings.currentRenderPipeline != null)
            {
                var urp = Shader.Find("Universal Render Pipeline/Lit");
                if (urp != null) return urp;
            }
            return Shader.Find("Standard");
        }

        // ------------------------------------------------------------------
        // Geometry helpers  (1 Unity unit = 1 meter; keep it that way in VR)
        // ------------------------------------------------------------------

        public static GameObject AddEmpty(string name, Vector3 position, Transform parent = null)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.transform.position = position;
            return go;
        }

        public static GameObject AddPrimitive(PrimitiveType type, string name, Vector3 position, Vector3 scale,
                                              Material material, Transform parent = null)
        {
            var go = GameObject.CreatePrimitive(type);
            go.name = name;
            go.transform.SetParent(parent, false);
            go.transform.position = position;
            go.transform.localScale = scale;
            if (material != null) go.GetComponent<Renderer>().sharedMaterial = material;
            return go;
        }

        /// <summary>A flat floor. Unity's Plane primitive is 10 x 10 m at scale 1.</summary>
        public static GameObject AddFloor(float sizeMeters, Material material, Transform parent = null)
        {
            var floor = AddPrimitive(PrimitiveType.Plane, "Floor", Vector3.zero,
                                     new Vector3(sizeMeters / 10f, 1f, sizeMeters / 10f), material, parent);
            floor.isStatic = true;
            return floor;
        }

        /// <summary>World-space 3D text using the legacy TextMesh (no extra package needed).</summary>
        public static GameObject AddLabel(string name, string text, Vector3 position, float characterSize = 0.05f,
                                          Color? color = null, Transform parent = null)
        {
            var go = AddEmpty(name, position, parent);
            var tm = go.AddComponent<TextMesh>();
            tm.text = text;
            tm.fontSize = 64;
            tm.characterSize = characterSize;
            tm.anchor = TextAnchor.MiddleCenter;
            tm.alignment = TextAlignment.Center;
            tm.color = color ?? Color.white;
            return go;
        }

        public static GameObject AddPointLight(string name, Vector3 position, Color color, float range = 6f,
                                               float intensity = 1.5f, Transform parent = null)
        {
            var go = AddEmpty(name, position, parent);
            var light = go.AddComponent<Light>();
            light.type = LightType.Point;
            light.color = color;
            light.range = range;
            light.intensity = intensity;
            return go;
        }

        /// <summary>An invisible box trigger volume (BoxCollider with isTrigger = true).</summary>
        public static GameObject AddTriggerVolume(string name, Vector3 center, Vector3 size, Transform parent = null)
        {
            var go = AddEmpty(name, center, parent);
            var box = go.AddComponent<BoxCollider>();
            box.isTrigger = true;
            box.size = size;
            return go;
        }

        public static void SetFog(Color color, float density)
        {
            RenderSettings.fog = true;
            RenderSettings.fogMode = FogMode.Exponential;
            RenderSettings.fogColor = color;
            RenderSettings.fogDensity = density;
        }

        public static void SetSun(Color color, float intensity, Vector3 eulerAngles)
        {
            var sun = Object.FindFirstObjectByType<Light>();
            if (sun == null) return;
            sun.color = color;
            sun.intensity = intensity;
            sun.transform.rotation = Quaternion.Euler(eulerAngles);
        }

        /// <summary>Marks a hierarchy static (useful for lightmapping / occlusion activities).</summary>
        public static void MarkStaticRecursive(GameObject root)
        {
            foreach (var t in root.GetComponentsInChildren<Transform>(true))
                t.gameObject.isStatic = true;
        }
    }
}
