// --------------------------------------------------------------
// Copyright 2025 CyberAgent, Inc.
// --------------------------------------------------------------

using UnityEditor;
using UnityEngine;

namespace Nova.Editor.Foundation.Scripts
{
    internal static class UnusedReferencesRemove
    {
        [MenuItem("Tools/NOVA Shader/RemoveUnusedReferences")]
        private static void RemoveUnusedReferences()
        {
            Debug.Log("[NOVA] Start remove unused references.");

            // 選択されたオブジェクトを取得
            var selectedObjects = Selection.objects;

            foreach (var obj in selectedObjects)
                if (obj is Material material)
                    RemoveUnusedReferences(material);

            // アセットデータベースを保存
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("[NOVA] Finished remove unused references.");
        }

        private static bool MaterialPropertyIsUnused(Material material, string propertyName)
        {
            // この関数はプロジェクトに応じて拡張する必要があります。
            // 現在のシェーダコード、カスタムロジックなどを使ってプロパティの使用状況を判定するべきです。
            // ここでは単純にテクスチャが割り当てられているかを確認します。
            var texture = material.GetTexture(propertyName);
            return texture == null;
        }

        private static void RemoveUnusedReferences(Material material)
        {
            var shader = material.shader;
            var shaderName = shader.name;

            switch (shaderName)
            {
                case ShaderNames.ParticlesUberLit:
                    RemoveUnusedReferencesFromParticlesUberLit(material);
                    break;
                case ShaderNames.ParticlesUberUnlit:
                    RemoveUnusedReferencesFromParticlesUberUnlit(material);
                    break;
                case ShaderNames.UIParticlesUberLit:
                    RemoveUnusedReferencesFromUIParticlesUberLit(material);
                    break;
                case ShaderNames.UIParticlesUberUnlit:
                    RemoveUnusedReferencesFromUIParticlesUberUnlit(material);
                    break;
                default:
                    Debug.LogWarning($"[NOVA] {material.name} is not a target shader.");
                    break;
            }
        }

        private static void RemoveUnusedReferencesFromParticlesUberLit(Material material)
        {
            var shader = material.shader;
            var propertyCount = ShaderUtil.GetPropertyCount(shader);

            for (var i = 0; i < propertyCount; i++)
            {
                var propertyName = ShaderUtil.GetPropertyName(shader, i);
                var propertyType = ShaderUtil.GetPropertyType(shader, i);

                // テクスチャプロパティのみを対象とする
                if (propertyType != ShaderUtil.ShaderPropertyType.TexEnv)
                    continue;
                // 現在Materialのプロパティにテクスチャが設定されているかチェック
                if (!material.HasProperty(propertyName) || !MaterialPropertyIsUnused(material, propertyName))
                    continue;
                // 使用されていないテクスチャを削除する
                material.SetTexture(propertyName, null);
                Debug.Log($"[NOVA] {material.name}: Removed unused texture from property: {propertyName}");
            }

            // マテリアルを保存する
            EditorUtility.SetDirty(material);
        }

        private static void RemoveUnusedReferencesFromParticlesUberUnlit(Material material)
        {
        }

        private static void RemoveUnusedReferencesFromUIParticlesUberLit(Material material)
        {
        }

        private static void RemoveUnusedReferencesFromUIParticlesUberUnlit(Material material)
        {
        }

        private static class ShaderNames
        {
            public const string ParticlesUberLit = "Nova/Particles/UberLit";
            public const string ParticlesUberUnlit = "Nova/Particles/UberUnlit";
            public const string UIParticlesUberLit = "Nova/UIParticles/UberLit";
            public const string UIParticlesUberUnlit = "Nova/UIParticles/UberUnlit";
        }
    }
}
