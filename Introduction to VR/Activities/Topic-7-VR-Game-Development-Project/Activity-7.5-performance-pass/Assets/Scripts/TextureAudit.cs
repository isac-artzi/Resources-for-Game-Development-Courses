// TextureAudit.cs — Activity 7.5: Performance Pass for Quest 3
// EDITOR-ONLY audit (Menu: Activity > Audit > Texture Audit). Lists every imported texture under Assets/ with
// its import size limit and its Android compression format, flags anything over 1024 px or not ASTC, and
// estimates GPU memory so you can see where the megabytes go.
// Lives in Scripts/ but is wrapped in #if UNITY_EDITOR so it is stripped from the Quest build.
// Created by Isac Artzi

using UnityEngine;

#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;

public static class TextureAudit
{
    /// <summary>Textures above this import size are flagged for a VR mobile target.</summary>
    const int MaxRecommendedSize = 1024;

    /// <summary>Bits per pixel for the formats you will meet. RGBA32 = 32; ASTC 4x4 = 8; ASTC 6x6 = 3.56; ASTC 8x8 = 2.</summary>
    public static float BitsPerPixel(string formatName)
    {
        // TODO 3: Return the bpp for the format name (TextureImporterFormat.ToString()):
        //         "ASTC_4x4" -> 8f, "ASTC_5x5" -> 5.12f, "ASTC_6x6" -> 3.56f, "ASTC_8x8" -> 2f, "ASTC_10x10" -> 1.28f, "ASTC_12x12" -> 0.89f,
        //         "ETC2_RGBA8" -> 8f, "ETC2_RGB4"/"ETC_RGB4" -> 4f, anything else (RGBA32, Automatic on desktop) -> 32f.
        //         Look at: string.StartsWith / a switch. Why: ASTC packs a block of NxN pixels into 128 bits, so bpp = 128 / (N*N).
        //         Check: a 2048x2048 texture reports ~16 MB at 32 bpp and ~1.8 MB at ASTC 6x6 (see README math).
        return 32f;
    }

    [MenuItem("Activity/Audit/Texture Audit", priority = 101)]
    public static void Run()
    {
        var sb = new StringBuilder();
        sb.AppendLine("TEXTURE AUDIT — project textures under Assets/");
        var rows = new List<string>();
        float totalMb = 0f;
        int flagged = 0;

        // TODO 1: Find them. string[] guids = AssetDatabase.FindAssets("t:Texture2D", new[] { "Assets" });
        //         For each guid: string path = AssetDatabase.GUIDToAssetPath(guid);
        //             var importer = AssetImporter.GetAtPath(path) as TextureImporter; if (importer == null) continue;  (render textures, fonts...)
        //             var tex = AssetDatabase.LoadAssetAtPath<Texture2D>(path); if (tex == null) continue;
        //         Look at: AssetDatabase.FindAssets(filter, searchInFolders), AssetImporter.GetAtPath, TextureImporter.
        //         Check: the report lists at least "BigMural" and "SmallTile" (the builder generated them).

        // TODO 2: Inspect each. int maxSize = importer.maxTextureSize;
        //             var android = importer.GetPlatformTextureSettings("Android");
        //             string format = android.overridden ? android.format.ToString() : "project default";
        //             bool isAstc = android.overridden && android.format.ToString().StartsWith("ASTC");
        //             bool tooBig = maxSize > MaxRecommendedSize;
        //         Flag the texture if tooBig or !isAstc, and count 'flagged'.
        //         Look at: TextureImporter.maxTextureSize, TextureImporter.GetPlatformTextureSettings(string), TextureImporterPlatformSettings.overridden/.format.
        //         Why "project default" is a flag: it means the format follows Player Settings > Texture compression format — fine IF that is
        //         set to ASTC for Android (Docs/PortingToQuest3.md says so), but you should know rather than hope.

        // TODO 4: Estimate memory. int w = Mathf.Min(tex.width, maxSize), h = Mathf.Min(tex.height, maxSize);
        //             float mb = w * h * BitsPerPixel(android.overridden ? android.format.ToString() : "RGBA32") / 8f / (1024f * 1024f);
        //             if (importer.mipmapEnabled) mb *= 1.333f;
        //         Add a row: (flag ? "!! " : "   ") + tex.name + "  " + w + "x" + h + "  max " + maxSize + "  Android " + format + "  ~" + mb.ToString("F2") + " MB";
        //         totalMb += mb. After the loop, sort rows (largest first — put the MB first in a parallel list, or use rows.Sort with a
        //         comparison), append them, then "Flagged: " + flagged + "   Estimated total: " + totalMb.ToString("F1") + " MB".
        //         Why x1.33: a full mip chain adds 1/4 + 1/16 + ... = 1/3 of the base level.
        //         Check: BigMural shows ~21 MB (2048^2, 32 bpp, mips) before you fix its import settings, ~2.4 MB after ASTC 6x6.

        foreach (var row in rows) sb.AppendLine(row);
        sb.AppendLine("Flagged: " + flagged + "   Estimated total: " + totalMb.ToString("F1") + " MB");
        Finish(sb, "texture_report.txt");
    }

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
