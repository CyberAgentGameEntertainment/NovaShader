using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using Nova.Editor.Core.Scripts;  // Add this for RenderType enum

public class AutoShaderVariantsGenerator : MonoBehaviour
{
    private const int TargetVariantCount = 1000;
    private const string MaterialOutputPath = "Assets/Demo/Demo_OptimizedShader/GeneratedMaterials";
    private static readonly string ShaderPath = "Nova/Particles/UberLit"; // Replace with your actual shader path.

    // キーワードリスト
    private static readonly string[] Keywords = new string[]
    {
        "_VERTEX_ALPHA_AS_TRANSITION_PROGRESS",
        "_ALPHAMODULATE_ENABLED",
        "_ALPHATEST_ENABLED",
        "_RECEIVE_SHADOWS_ENABLED",
        "_SPECULAR_HIGHLIGHTS_ENABLED",
        "_ENVIRONMENT_REFLECTIONS_ENABLED",
        "_SPECULAR_SETUP",
        "_NORMAL_MAP_ENABLED",
        "_METALLIC_MAP_ENABLED",
        "_SMOOTHNESS_MAP_ENABLED",
        "_SPECULAR_MAP_ENABLED",
        "_BASE_MAP_MODE_2D",
        "_BASE_MAP_MODE_2D_ARRAY",
        "_BASE_MAP_MODE_3D",
        "_BASE_MAP_ROTATION_ENABLED",
        "_BASE_SAMPLER_STATE_POINT_MIRROR",
        "_BASE_SAMPLER_STATE_LINEAR_MIRROR",
        "_BASE_SAMPLER_STATE_TRILINEAR_MIRROR",
        "_TINT_AREA_ALL",
        "_TINT_AREA_RIM",
        "_TINT_COLOR_ENABLED",
        "_TINT_MAP_ENABLED",
        "_TINT_MAP_3D_ENABLED",
        "_FLOW_MAP_ENABLED",
        "_FLOW_MAP_TARGET_BASE",
        "_FLOW_MAP_TARGET_TINT",
        "_FLOW_MAP_TARGET_EMISSION",
        "_FLOW_MAP_TARGET_ALPHA_TRANSITION",
        "_PARALLAX_MAP_TARGET_BASE",
        "_PARALLAX_MAP_TARGET_TINT",
        "_PARALLAX_MAP_TARGET_EMISSION",
        "_PARALLAX_MAP_MODE_2D",
        "_PARALLAX_MAP_MODE_2D_ARRAY",
        "_PARALLAX_MAP_MODE_3D",
        "_GREYSCALE_ENABLED",
        "_GRADIENT_MAP_ENABLED",
        "_FADE_TRANSITION_ENABLED",
        "_DISSOLVE_TRANSITION_ENABLED",
        "_ALPHA_TRANSITION_MAP_MODE_2D",
        "_ALPHA_TRANSITION_MAP_MODE_2D_ARRAY",
        "_ALPHA_TRANSITION_MAP_MODE_3D",
        "_ALPHA_TRANSITION_BLEND_SECOND_TEX_AVERAGE",
        "_ALPHA_TRANSITION_BLEND_SECOND_TEX_MULTIPLY",
        "_EMISSION_AREA_ALL",
        "_EMISSION_AREA_MAP",
        "_EMISSION_AREA_ALPHA",
        "_EMISSION_MAP_MODE_2D",
        "_EMISSION_MAP_MODE_2D_ARRAY",
        "_EMISSION_MAP_MODE_3D",
        "_EMISSION_COLOR_COLOR",
        "_EMISSION_COLOR_BASECOLOR",
        "_EMISSION_COLOR_MAP",
        "_TRANSPARENCY_BY_LUMINANCE",
        "_TRANSPARENCY_BY_RIM",
        "_SOFT_PARTICLES_ENABLED",
        "_DEPTH_FADE_ENABLED",
        "_VERTEX_DEFORMATION_ENABLED",
    };

    [MenuItem("Tools/Generate Shader Variants Scene")]
    public static void GenerateShaderVariantsScene()
    {
        var shader = Shader.Find(ShaderPath);
        if (shader == null)
        {
            Debug.LogError("Target shader not found! Make sure the path is correct.");
            return;
        }

        // Create a parent object for cleanliness
        var parent = new GameObject("GeneratedVariants");

        for (int i = 0; i < TargetVariantCount; i++)
        {
            // Create a new material
            Material mat = new Material(shader);

            // Select a random subset of keywords
            HashSet<string> selectedKeywords = GetRandomKeywords();

            mat.shaderKeywords = new List<string>(selectedKeywords).ToArray();

            // Randomly set render type
            var randomRenderType = (RenderType)Random.Range(0, (int)RenderType.Num);
            mat.SetFloat("_RenderType", (float)randomRenderType);

            // Save material
            string matPath = $"{MaterialOutputPath}/GeneratedMat_{i:D4}.mat";
            AssetDatabase.CreateAsset(mat, matPath);

            // Apply to GameObject in scene
            GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            obj.transform.position = new Vector3(i % 50, 0, i / 50); // Grid layout
            obj.name = $"Variant_{i:D4}";
            obj.GetComponent<Renderer>().sharedMaterial = mat;
            obj.transform.parent = parent.transform;
        }

        Debug.Log($"Generated {TargetVariantCount} variant materials and scene objects.");
        AssetDatabase.SaveAssets();
    }

    private static HashSet<string> GetRandomKeywords()
    {
        HashSet<string> result = new HashSet<string>();
        int count = Random.Range(4, 10); // 適度な数のキーワードを割り当て（必要に応じて調整）
        while (result.Count < count)
        {
            string keyword = Keywords[Random.Range(0, Keywords.Length)];
            result.Add(keyword);
        }
        return result;
    }
}
