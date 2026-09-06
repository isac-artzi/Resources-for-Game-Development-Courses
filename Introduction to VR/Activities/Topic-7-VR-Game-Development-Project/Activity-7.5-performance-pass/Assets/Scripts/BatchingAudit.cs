// BatchingAudit.cs — Activity 7.5: Performance Pass for Quest 3
// EDITOR-ONLY audit (Menu: Activity > Audit > Batching Audit). Walks every renderer in the open scene and
// reports what stops Unity from batching draw calls: materials used by a single renderer, non-static objects
// that share a mesh but whose material has GPU instancing off, and real-time lights that cast shadows.
// Lives in Scripts/ but is wrapped in #if UNITY_EDITOR so it is stripped from the Quest build.
// Created by Isac Artzi

using UnityEngine;

#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;

public static class BatchingAudit
{
    /// <summary>Renderers sharing a mesh at or above this count are worth instancing.</summary>
    const int InstancingThreshold = 5;

    [MenuItem("Activity/Audit/Batching Audit", priority = 100)]
    public static void Run()
    {
        var sb = new StringBuilder();
        sb.AppendLine("BATCHING AUDIT — scene: " + UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);

        var renderers = Object.FindObjectsByType<MeshRenderer>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        sb.AppendLine("Mesh renderers: " + renderers.Length);

        // TODO 1: Group renderers by material. var byMaterial = new Dictionary<Material, List<MeshRenderer>>();
        //         For each renderer r and each material m in r.sharedMaterials (skip null): add r to byMaterial[m]
        //         (create the list on first sight). Then append "Unique materials: " + byMaterial.Count.
        //         Look at: Renderer.sharedMaterials (the asset, not a per-object copy — .materials would CREATE copies and
        //         make everything look unique). Why: one draw call per material per batch is the floor; fewer materials = fewer calls.
        //         Check: the report shows about 33 unique materials for the starter courtyard (3 shared + 30 "Unique_xx").

        // TODO 2: Materials used by exactly ONE renderer: for each pair in byMaterial with list.Count == 1, append
        //             "  single-use material: " + mat.name + " on " + renderer.name
        //         and count them. Why they hurt: a single-use material can never batch with anything; 30 of them is 30
        //         draw calls that a shared material (or a texture atlas) would fold into one or two.
        //         Check: the 30 "Unique_xx" rubble blocks are listed.

        // TODO 3: Instancing candidates. Group by sharedMesh instead (MeshFilter.sharedMesh): var byMesh = new Dictionary<Mesh, List<MeshRenderer>>().
        //         For groups with Count >= InstancingThreshold where the objects are NOT static (!r.gameObject.isStatic) and the
        //         first material has enableInstancing == false, append "  instancing candidate: " + mesh.name + " x" + count + " (" + mat.name + ")".
        //         Look at: Material.enableInstancing, GameObject.isStatic. Why: identical non-static meshes with the same material
        //         collapse into one instanced call if the material allows it; static ones get static batching instead.
        //         Check: the 40 "Pillar" objects appear as one candidate line.

        // TODO 4: Lights. var lights = Object.FindObjectsByType<Light>(FindObjectsSortMode.None); for each with
        //         light.shadows != LightShadows.None append "  shadow-casting light: " + light.name + " (" + light.type + ")",
        //         and append the total number of real-time lights. Why: every shadow-casting light re-renders the shadow casters,
        //         and every real-time point light adds a pass in the Built-in forward renderer. Quest budgets allow ~1 shadow light.
        //         Check: six "Torch" lights are listed.

        Finish(sb, "batching_report.txt");
    }

    /// <summary>Prints the report and writes it next to the Assets folder (Audit/&lt;file&gt;).</summary>
    static void Finish(StringBuilder sb, string fileName)
    {
        string dir = Path.Combine(Directory.GetParent(Application.dataPath).FullName, "Audit");
        Directory.CreateDirectory(dir);
        string path = Path.Combine(dir, fileName);
        File.WriteAllText(path, sb.ToString());
        Debug.Log(sb.ToString() + "\n(saved to " + path + ")");
    }
}
#endif
