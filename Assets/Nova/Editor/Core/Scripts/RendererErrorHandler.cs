// --------------------------------------------------------------
// Copyright 2025 CyberAgent, Inc.
// --------------------------------------------------------------

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Nova.Editor.Core.Scripts
{
    /// <summary>
    ///     Provides methods for handling ParticleSystemRenderer errors when using NovaShader.
    /// </summary>
    public static class RendererErrorHandler
    {
        /// <summary>
        ///     Checks for errors in renderers using the specified material.
        /// </summary>
        /// <returns>Returns true if an error has occurred.</returns>
        public static bool CheckErrorWithMaterial(Material material)
        {
            if (material == null)
            {
                Debug.LogWarning(
                    "Nova Shader: Particle System Renderer error check skipped because the material was null.");
                return true;
            }

            Object[] materials = { material };
            var materialProperties = MaterialEditor.GetMaterialProperties(materials);
            ParticlesUberCommonMaterialProperties commonMaterialProperties = new(materialProperties);
            SetupCorrectVertexStreams(material, out var correctVertexStreams, out var correctVertexStreamsInstanced,
                out var correctTrailVertexStreams, out var correctTrailVertexStreamsInstanced, commonMaterialProperties);
            var allRenderersWithMaterial = FindAllRenderersWithMaterial(material);
            // If there is no renderer using this material, there is no error.
            if (allRenderersWithMaterial == null || allRenderersWithMaterial.Count == 0) return false;

            return CheckError(allRenderersWithMaterial, material, correctVertexStreams, correctVertexStreamsInstanced,
                correctTrailVertexStreams, correctTrailVertexStreamsInstanced);
        }

        /// <summary>
        ///     Corrects errors in renderers using the specified material.
        /// </summary>
        public static void FixErrorWithMaterial(Material material)
        {
            if (material == null)
            {
                Debug.LogWarning(
                    "Nova Shader: Skipped fixing the Particle System Renderer error because the material was null.");
                return;
            }

            Object[] materials = { material };
            var materialProperties = MaterialEditor.GetMaterialProperties(materials);
            ParticlesUberCommonMaterialProperties commonMaterialProperties = new(materialProperties);
            SetupCorrectVertexStreams(material, out var correctVertexStreams, out var correctVertexStreamsInstanced,
                out var correctTrailVertexStreams, out var correctTrailVertexStreamsInstanced, commonMaterialProperties);
            var allRenderersWithMaterial = FindAllRenderersWithMaterial(material);
            // If there is no renderer using this material, there is no error.
            if (allRenderersWithMaterial == null || allRenderersWithMaterial.Count == 0) return;

            FixError(allRenderersWithMaterial, material, correctVertexStreams, correctVertexStreamsInstanced,
                correctTrailVertexStreams, correctTrailVertexStreamsInstanced);
        }

        private static bool IsCustomCoordUsed(ParticlesGUI.Property prop)
        {
            return (CustomCoord)prop.Value.floatValue !=
                   CustomCoord.Unused;
        }

        private static bool IsCustomCoordUsedInVertexDeformation(
            ParticlesUberCommonMaterialProperties commonMaterialProperties)
        {
            var isCustomCoordUsed = IsCustomCoordUsed(commonMaterialProperties.VertexDeformationMapOffsetXCoordProp)
                                    || IsCustomCoordUsed(commonMaterialProperties.VertexDeformationMapOffsetYCoordProp)
                                    || IsCustomCoordUsed(commonMaterialProperties.VertexDeformationIntensityCoordProp);
            return isCustomCoordUsed;
        }

        private static bool IsCustomCoordUsedInBaseMap(ParticlesUberCommonMaterialProperties commonMaterialProperties)
        {
            var isCustomCoordUsed = IsCustomCoordUsed(commonMaterialProperties.BaseMapOffsetXCoordProp)
                                    || IsCustomCoordUsed(commonMaterialProperties.BaseMapOffsetYCoordProp)
                                    || IsCustomCoordUsed(commonMaterialProperties.BaseMapRotationCoordProp)
                                    || IsCustomCoordUsed(commonMaterialProperties.BaseMapRotationCoordProp);
            isCustomCoordUsed |= (BaseMapMode)commonMaterialProperties.BaseMapModeProp.Value.floatValue !=
                                 BaseMapMode.SingleTexture
                                 && IsCustomCoordUsed(commonMaterialProperties.BaseMapProgressProp);

            isCustomCoordUsed |= (BaseMapMode)commonMaterialProperties.BaseMapModeProp.Value.floatValue !=
                                 BaseMapMode.SingleTexture
                                 && IsCustomCoordUsed(commonMaterialProperties.BaseMapProgressCoordProp);

            return isCustomCoordUsed;
        }

        private static bool IsCustomCoordUsedInTintColor(ParticlesUberCommonMaterialProperties commonMaterialProperties)
        {
            var isCustomCoordUsed = false;
            var tintAreaMode = (TintAreaMode)commonMaterialProperties.TintAreaModeProp.Value.floatValue;
            if (tintAreaMode != TintAreaMode.None)
            {
                isCustomCoordUsed |= IsCustomCoordUsed(commonMaterialProperties.TintMapBlendRateCoordProp);
                if (tintAreaMode == TintAreaMode.Rim)
                    isCustomCoordUsed |= IsCustomCoordUsed(commonMaterialProperties.TintRimProgressCoordProp)
                                         || IsCustomCoordUsed(commonMaterialProperties.TintRimSharpnessCoordProp);

                var tintMapMode = (TintColorMode)commonMaterialProperties.TintColorModeProp.Value.floatValue;
                if (tintMapMode == TintColorMode.Texture2D || tintMapMode == TintColorMode.Texture3D)
                {
                    isCustomCoordUsed |= IsCustomCoordUsed(commonMaterialProperties.TintMapOffsetXCoordProp)
                                         || IsCustomCoordUsed(commonMaterialProperties.TintMapOffsetYCoordProp);

                    if (tintMapMode == TintColorMode.Texture3D)
                        isCustomCoordUsed |= IsCustomCoordUsed(commonMaterialProperties.TintMap3DProgressCoordProp);
                }
            }

            return isCustomCoordUsed;
        }

        private static bool IsCustomCoordUsedInFlowMap(ParticlesUberCommonMaterialProperties commonMaterialProperties)
        {
            return IsCustomCoordUsed(commonMaterialProperties.FlowMapOffsetXCoordProp)
                   || IsCustomCoordUsed(commonMaterialProperties.FlowMapOffsetYCoordProp)
                   || IsCustomCoordUsed(commonMaterialProperties.FlowIntensityCoordProp);
        }

        private static bool IsCustomCoordUsedInParallax(ParticlesUberCommonMaterialProperties commonMaterialProperties)
        {
            var isCustomCoordUsed = IsCustomCoordUsed(commonMaterialProperties.ParallaxMapOffsetXCoordProp)
                                    || IsCustomCoordUsed(commonMaterialProperties.ParallaxMapOffsetYCoordProp);
            isCustomCoordUsed |= (ParallaxMapMode)commonMaterialProperties.ParallaxMapModeProp.Value.floatValue !=
                                 ParallaxMapMode.SingleTexture
                                 && (IsCustomCoordUsed(commonMaterialProperties.ParallaxMapProgressProp)
                                     || IsCustomCoordUsed(commonMaterialProperties.ParallaxMapProgressCoordProp));

            return isCustomCoordUsed;
        }

        private static bool IsCustomCoordUsedInAlphaTransition(
            ParticlesUberCommonMaterialProperties commonMaterialProperties)
        {
            var props = commonMaterialProperties;
            var mode = (AlphaTransitionMode)props.AlphaTransitionModeProp.Value.floatValue;
            if (mode == AlphaTransitionMode.None) return false;

            bool isCustomCoordUsed;
            var isFlipBook = (AlphaTransitionMapMode)props.AlphaTransitionMapModeProp.Value.floatValue !=
                             AlphaTransitionMapMode.SingleTexture;
            // 1st texture
            {
                isCustomCoordUsed = IsCustomCoordUsed(props.AlphaTransitionProgressCoordProp);
                isCustomCoordUsed |= IsCustomCoordUsed(props.AlphaTransitionMapOffsetXCoordProp)
                                     || IsCustomCoordUsed(props.AlphaTransitionMapOffsetYCoordProp);
                isCustomCoordUsed |= isFlipBook && IsCustomCoordUsed(props.AlphaTransitionMapProgressCoordProp);
            }
            // 2nd texture
            {
                if ((AlphaTransitionBlendMode)props.AlphaTransitionSecondTextureBlendModeProp.Value.floatValue !=
                    AlphaTransitionBlendMode.None)
                {
                    isCustomCoordUsed |= IsCustomCoordUsed(props.AlphaTransitionProgressCoordSecondTextureProp);
                    isCustomCoordUsed |= IsCustomCoordUsed(props.AlphaTransitionMapSecondTextureOffsetXCoordProp) ||
                                         IsCustomCoordUsed(props.AlphaTransitionMapSecondTextureOffsetYCoordProp);
                    isCustomCoordUsed |= isFlipBook &&
                                         IsCustomCoordUsed(props.AlphaTransitionMapSecondTextureProgressCoordProp);
                }
            }
            return isCustomCoordUsed;
        }

        private static bool IsCustomCoordUsedInEmission(ParticlesUberCommonMaterialProperties commonMaterialProperties)
        {
            var mode = (EmissionAreaType)commonMaterialProperties.EmissionAreaTypeProp.Value.floatValue;
            if (mode == EmissionAreaType.None) return false;
            var isCustomCoordUsed = false;
            isCustomCoordUsed = IsCustomCoordUsed(commonMaterialProperties.EmissionIntensityCoordProp);
            if (mode == EmissionAreaType.ByTexture)
            {
                isCustomCoordUsed |= IsCustomCoordUsed(commonMaterialProperties.EmissionMapOffsetXCoordProp)
                                     || IsCustomCoordUsed(commonMaterialProperties.EmissionMapOffsetYCoordProp);
                isCustomCoordUsed |= (EmissionMapMode)commonMaterialProperties.EmissionMapModeProp.Value.floatValue !=
                                     EmissionMapMode.SingleTexture
                                     && IsCustomCoordUsed(commonMaterialProperties.EmissionMapProgressCoordProp);
            }

            return isCustomCoordUsed;
        }

        private static bool IsCustomCoordUsedInTransparency(
            ParticlesUberCommonMaterialProperties commonMaterialProperties)
        {
            var isCustomCoordUsed = false;
            var enabledRim = commonMaterialProperties.RimTransparencyEnabledProp.Value.floatValue > 0.5f;
            if (enabledRim)
            {
                isCustomCoordUsed |= IsCustomCoordUsed(commonMaterialProperties.RimTransparencyProgressCoordProp);
                isCustomCoordUsed |= IsCustomCoordUsed(commonMaterialProperties.RimTransparencySharpnessCoordProp);
            }

            var enabledLuminance = commonMaterialProperties.LuminanceTransparencyEnabledProp.Value.floatValue > 0.5f;
            if (enabledLuminance)
            {
                isCustomCoordUsed |=
                    IsCustomCoordUsed(commonMaterialProperties.LuminanceTransparencyProgressCoordProp);
                isCustomCoordUsed |=
                    IsCustomCoordUsed(commonMaterialProperties.LuminanceTransparencySharpnessCoordProp);
            }

            return isCustomCoordUsed;
        }

        private static bool IsCustomCoordUsed(ParticlesUberCommonMaterialProperties commonMaterialProperties)
        {
            if (commonMaterialProperties == null) return false;

            return IsCustomCoordUsedInVertexDeformation(commonMaterialProperties)
                   || IsCustomCoordUsedInBaseMap(commonMaterialProperties)
                   || IsCustomCoordUsedInTintColor(commonMaterialProperties)
                   || IsCustomCoordUsedInFlowMap(commonMaterialProperties)
                   || IsCustomCoordUsedInParallax(commonMaterialProperties)
                   || IsCustomCoordUsedInAlphaTransition(commonMaterialProperties)
                   || IsCustomCoordUsedInEmission(commonMaterialProperties)
                   || IsCustomCoordUsedInTransparency(commonMaterialProperties);
        }

        internal static void SetupCorrectVertexStreams(Material material,
            out List<ParticleSystemVertexStream> correctVertexStreams,
            out List<ParticleSystemVertexStream> correctVertexStreamsInstanced,
            out List<ParticleSystemVertexStream> correctTrailVertexStreams,
            out List<ParticleSystemVertexStream> correctTrailVertexStreamsInstanced,
            ParticlesUberCommonMaterialProperties commonMaterialProperties)
        {
            // Correct vertex streams when enabled GPU Instance.
            correctVertexStreamsInstanced = new List<ParticleSystemVertexStream>();
            correctVertexStreamsInstanced.Add(ParticleSystemVertexStream.Position);
            correctVertexStreamsInstanced.Add(ParticleSystemVertexStream.Normal);
            correctVertexStreamsInstanced.Add(ParticleSystemVertexStream.Color);
            correctVertexStreamsInstanced.Add(ParticleSystemVertexStream.UV);
            correctVertexStreamsInstanced.Add(ParticleSystemVertexStream.UV2);
            correctVertexStreamsInstanced.Add(ParticleSystemVertexStream.Custom1XYZW);
            correctVertexStreamsInstanced.Add(ParticleSystemVertexStream.Custom2XYZW);

            // Correct vertex streams when disabled GPU Instance.
            correctVertexStreams = new List<ParticleSystemVertexStream>();
            correctVertexStreams.Add(ParticleSystemVertexStream.Position);
            correctVertexStreams.Add(ParticleSystemVertexStream.Normal);
            correctVertexStreams.Add(ParticleSystemVertexStream.Color);
            correctVertexStreams.Add(ParticleSystemVertexStream.UV);
            correctVertexStreams.Add(ParticleSystemVertexStream.UV2);

            // Is custom coord Used ?
            if (IsCustomCoordUsed(commonMaterialProperties))
            {
                correctVertexStreams.Add(ParticleSystemVertexStream.Custom1XYZW);
                correctVertexStreams.Add(ParticleSystemVertexStream.Custom2XYZW);
            }

            // Tangent
            {
                var useParallaxMap = material.IsKeywordEnabled(ShaderKeywords.ParallaxMapTargetBase) ||
                                     material.IsKeywordEnabled(ShaderKeywords.ParallaxMapTargetTint) ||
                                     material.IsKeywordEnabled(ShaderKeywords.ParallaxMapTargetEmission);
                if (useParallaxMap || material.shader.name == "Nova/Particles/UberLit" &&
                    material.IsKeywordEnabled(ShaderKeywords.NormalMapEnabled))
                {
                    correctVertexStreamsInstanced.Add(ParticleSystemVertexStream.Tangent);
                    correctVertexStreams.Add(ParticleSystemVertexStream.Tangent);
                }
            }

            // Trail Vertex Streams - same logic as regular vertex streams
            correctTrailVertexStreamsInstanced = new List<ParticleSystemVertexStream>(correctVertexStreamsInstanced);
            correctTrailVertexStreams = new List<ParticleSystemVertexStream>(correctVertexStreams);
        }

        private static bool IsEnabledGPUInstancing(ParticleSystemRenderer particleSystem)
        {
            return particleSystem.enableGPUInstancing && particleSystem.renderMode == ParticleSystemRenderMode.Mesh;
        }

        private static bool CompareVertexStreams(List<ParticleSystemVertexStream> a,
            List<ParticleSystemVertexStream> b)
        {
            if (a.Count != b.Count) return false;
            for (var i = 0; i < a.Count; i++)
                if (a[i] != b[i])
                    return false;
            return true;
        }

        internal static List<ParticleSystemRenderer> FindRendererWithMaterial(Material material)
        {
            var renderersWithMaterial = new List<ParticleSystemRenderer>();
#if UNITY_2023_3_OR_NEWER
            var renderers = Object.FindObjectsByType<ParticleSystemRenderer>(FindObjectsSortMode.None);
#else
            var renderers = Object.FindObjectsOfType(typeof(ParticleSystemRenderer)) as ParticleSystemRenderer[];
#endif
            if (renderers == null) return null;
            foreach (var renderer in renderers)
                if (renderer.sharedMaterial == material)
                    renderersWithMaterial.Add(renderer);
            return renderersWithMaterial;
        }

        internal static List<ParticleSystemRenderer> FindRendererWithTrailMaterial(Material material)
        {
            var renderersWithMaterial = new List<ParticleSystemRenderer>();
#if UNITY_2023_3_OR_NEWER
            var renderers = Object.FindObjectsByType<ParticleSystemRenderer>(FindObjectsSortMode.None);
#else
            var renderers = Object.FindObjectsOfType(typeof(ParticleSystemRenderer)) as ParticleSystemRenderer[];
#endif
            if (renderers == null) return null;
            foreach (var renderer in renderers)
                if (renderer.trailMaterial == material)
                    renderersWithMaterial.Add(renderer);
            return renderersWithMaterial;
        }

        internal static List<ParticleSystemRenderer> FindAllRenderersWithMaterial(Material material)
        {
            var renderersWithMaterial = new HashSet<ParticleSystemRenderer>();
#if UNITY_2023_3_OR_NEWER
            var renderers = Object.FindObjectsByType<ParticleSystemRenderer>(FindObjectsSortMode.None);
#else
            var renderers = Object.FindObjectsOfType(typeof(ParticleSystemRenderer)) as ParticleSystemRenderer[];
#endif
            if (renderers == null) return new List<ParticleSystemRenderer>();
            
            foreach (var renderer in renderers)
            {
                // Add if shared material matches
                if (renderer.sharedMaterial == material)
                    renderersWithMaterial.Add(renderer);
                
                // Add if trail material matches
                if (renderer.trailMaterial == material)
                    renderersWithMaterial.Add(renderer);
            }
            
            return new List<ParticleSystemRenderer>(renderersWithMaterial);
        }


        internal static bool CheckError(
            List<ParticleSystemRenderer> renderers,
            Material targetMaterial,
            List<ParticleSystemVertexStream> correctVertexStreams,
            List<ParticleSystemVertexStream> correctVertexStreamsInstanced,
            List<ParticleSystemVertexStream> correctTrailVertexStreams,
            List<ParticleSystemVertexStream> correctTrailVertexStreamsInstanced)
        {
            var hasError = false;
            var rendererStreams = new List<ParticleSystemVertexStream>();
            foreach (var renderer in renderers)
            {
                // Check regular vertex streams if this renderer uses the target material as sharedMaterial
                if (renderer.sharedMaterial == targetMaterial)
                {
                    renderer.GetActiveVertexStreams(rendererStreams);
                    var streamsValid = false;
                    if (IsEnabledGPUInstancing(renderer))
                        streamsValid = CompareVertexStreams(rendererStreams, correctVertexStreamsInstanced);
                    else
                        streamsValid = CompareVertexStreams(rendererStreams, correctVertexStreams);
                    if (!streamsValid)
                    {
                        hasError = true;
                        break;
                    }
                }

                // Check trail vertex streams if this renderer uses the target material as trailMaterial
                if (renderer.trailMaterial == targetMaterial)
                {
                    renderer.GetActiveTrailVertexStreams(rendererStreams);
                    var trailStreamsValid = false;
                    if (IsEnabledGPUInstancing(renderer))
                        trailStreamsValid = CompareVertexStreams(rendererStreams, correctTrailVertexStreamsInstanced);
                    else
                        trailStreamsValid = CompareVertexStreams(rendererStreams, correctTrailVertexStreams);
                    if (!trailStreamsValid)
                    {
                        hasError = true;
                        break;
                    }
                }
            }

            return hasError;
        }

        internal static void FixError(List<ParticleSystemRenderer> renderers,
            Material targetMaterial,
            List<ParticleSystemVertexStream> correctVertexStreams,
            List<ParticleSystemVertexStream> correctVertexStreamsInstanced,
            List<ParticleSystemVertexStream> correctTrailVertexStreams,
            List<ParticleSystemVertexStream> correctTrailVertexStreamsInstanced)
        {
            foreach (var renderer in renderers)
            {
                // Fix regular vertex streams if this renderer uses the target material as sharedMaterial
                if (renderer.sharedMaterial == targetMaterial)
                {
                    renderer.SetActiveVertexStreams(IsEnabledGPUInstancing(renderer)
                        ? correctVertexStreamsInstanced
                        : correctVertexStreams);
                }

                // Fix trail vertex streams if this renderer uses the target material as trailMaterial
                if (renderer.trailMaterial == targetMaterial)
                {
                    renderer.SetActiveTrailVertexStreams(IsEnabledGPUInstancing(renderer)
                        ? correctTrailVertexStreamsInstanced
                        : correctTrailVertexStreams);
                }
            }
        }
    }
}
